using Microsoft.Extensions.Logging;
using MMALSharp.Common.Utility;
using MMALSharp.Components;
using MMALSharp.Config;
using MMALSharp.Config.SensorDefs;
using MMALSharp.Config.SensorRegs;
using MMALSharp.Native;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using static MMALSharp.Native.MMALI2C;

namespace MMALSharp
{
    public sealed class MMALRawcam
    {        
        private const int OPEN_READ_WRITE = 2;

        private const int I2C_M_RD = 0x0001;
        private const int I2C_SLAVE_FORCE = 0x0706;
        private const int I2C_RDWR = 0x0707;

        /// <summary>
        /// Gets the singleton instance of the MMAL Raw Camera. Call to initialise the camera for first use.
        /// </summary>
        public static MMALRawcam Instance => Lazy.Value;

        /// <summary>
        /// Reference to the camera component.
        /// </summary>
        public MMALRawcamComponent Camera { get; }

        private static readonly Lazy<MMALRawcam> Lazy = new Lazy<MMALRawcam>(() => new MMALRawcam());

        private static List<SensorDef> SensorDefs 
        { 
            get 
            {
                return new List<SensorDef>
                {
                    Ov5647SensorDefs.Ov5647SensorDef,
                    Imx219SensorDefs.Imx219SensorDef
                };
            } 
        }

        private MMALRawcamComponent RawcamComponent { get; set; }

        private MMALIspComponent IspComponent { get; set; }

        private SensorDef SensorDef { get; set; }

        private ModeDef ModeDef { get; set; }

        private string I2CDeviceName { get; set; }

        private MMALRawcam()
        {
            BcmHost.bcm_host_init();

            this.Camera = new MMALRawcamComponent();
        }

        /// <summary>
        /// Configures the raw camera pipeline.
        /// </summary>
        /// <param name="rawcamComponent">The <see cref="MMALRawcamComponent"/> component reference.</param>
        /// <param name="ispComponent">The <see cref="MMALIspComponent"/> component reference.</param>
        /// <param name="sensorDef">The sensor definition config object.</param>
        /// <param name="modeDef">The mode definition config object.</param>
        /// <param name="i2cDeviceName">The I2C device name.</param>
        public void ConfigureRawcamPipeline(MMALRawcamComponent rawcamComponent, MMALIspComponent ispComponent, SensorDef sensorDef, ModeDef modeDef, string i2cDeviceName)
        {
            this.RawcamComponent = rawcamComponent;
            this.IspComponent = ispComponent;
            this.SensorDef = sensorDef;
            this.ModeDef = modeDef;
            this.I2CDeviceName = i2cDeviceName;
        }

        /// <summary>
        /// Helper method to begin processing image data. Starts the Raw Camera component and awaits until processing is complete.
        /// Cleans up resources upon finish.
        /// </summary>            
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for a task to complete.</param>
        /// <returns>The awaitable Task.</returns>
        public async Task ProcessAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var tasks = new List<Task>();

            MMALLog.Logger.LogInformation("Attemping to enable rawcam components...");

            var sensor = this.ProbeSensor(this.I2CDeviceName);

            if (sensor == null)
            {
                throw new MMALIOException($"Could not probe sensor {this.I2CDeviceName}");
            }

            this.IspComponent.EnableComponent();
            this.RawcamComponent.EnableComponent();

            this.IspComponent.Inputs[0].Start();
            this.IspComponent.Outputs[0].Start();
            this.RawcamComponent.Outputs[0].Start();

            tasks.Add(this.IspComponent.Outputs[0].Trigger.Task);
            tasks.Add(this.RawcamComponent.Outputs[0].Trigger.Task);

            MMALLog.Logger.LogDebug("Attemping to start rawcam streaming...");

            await this.StartCapture(this.SensorDef, this.ModeDef, this.I2CDeviceName);

            if (cancellationToken == CancellationToken.None)
            {
                await Task.WhenAll(tasks).ConfigureAwait(false);

                await this.StopCapture(this.SensorDef, this.I2CDeviceName);
            }
            else
            {
                await Task.WhenAny(Task.WhenAll(tasks), cancellationToken.AsTask()).ConfigureAwait(false);

                this.IspComponent.ForceStopProcessing = true;
                this.RawcamComponent.ForceStopProcessing = true;

                await this.StopCapture(this.SensorDef, this.I2CDeviceName);

                await Task.WhenAll(tasks).ConfigureAwait(false);
            }

            foreach (var port in this.IspComponent.ProcessingPorts.Values)
            {
                if (port.ConnectedReference == null)
                {
                    port.DisablePort();
                }
            }

            this.IspComponent.CleanPortPools();
            this.IspComponent.DisableConnections();

            foreach (var port in this.RawcamComponent.ProcessingPorts.Values)
            {
                if (port.ConnectedReference == null)
                {
                    port.DisablePort();
                }
            }

            this.RawcamComponent.CleanPortPools();
            this.RawcamComponent.DisableConnections();
        }

        /// <summary>
        /// Cleans up any unmanaged resources. It is intended for this method to be run when no more activity is to be done with MMAL.
        /// </summary>
        public void Cleanup()
        {
            MMALLog.Logger.LogDebug("Destroying final components");

            var tempList = new List<MMALDownstreamComponent>(MMALBootstrapper.DownstreamComponents);

            tempList.ForEach(c => c.Dispose());

            BcmHost.bcm_host_deinit();
        }

        private async Task StartCapture(SensorDef sensorDef, ModeDef modeDef, string i2cDeviceName)
        {
            try
            {
                var fd = MMALI2C.Open(i2cDeviceName, OPEN_READ_WRITE);

                if (fd == 0)
                {
                    throw new MMALIOException("Couldn't open I2C device.");
                }

                if (MMALI2C.Ioctl(fd, I2C_SLAVE_FORCE, (IntPtr)sensorDef.I2CAddr) < 0)
                {
                    throw new MMALIOException("Failed to set I2C address.");
                }

                await this.SendRegs(fd, sensorDef, modeDef.Regs);
                MMALI2C.Close(fd);
                MMALLog.Logger.LogInformation("Now streaming...");
            }
            catch (Exception e)
            {
                MMALLog.Logger.LogError($"An error occurred when starting capture: {e.Message}");
            }            
        }
     
        private async Task StopCapture(SensorDef sensorDef, string i2cDeviceName)
        {
            try
            {
                var fd = MMALI2C.Open(i2cDeviceName, OPEN_READ_WRITE);

                if (fd == 0)
                {
                    throw new MMALIOException("Couldn't open I2C device.");
                }

                if (MMALI2C.Ioctl(fd, I2C_SLAVE_FORCE, (IntPtr)sensorDef.I2CAddr) < 0)
                {
                    throw new MMALIOException("Failed to set I2C address.");
                }

                await this.SendRegs(fd, sensorDef, sensorDef.StopReg);
                MMALI2C.Close(fd);
                MMALLog.Logger.LogInformation("Stop streaming...");
            }
            catch (Exception e)
            {
                MMALLog.Logger.LogError($"An error occurred when stopping capture: {e.Message}");
            }
        }

        private SensorDef ProbeSensor(string i2cDeviceName)
        {
            var fd = MMALI2C.Open(i2cDeviceName, OPEN_READ_WRITE);

            if (fd == 0)
            {
                throw new MMALIOException("Couldn't open I2C device.");
            }

            foreach (var sensorDef in SensorDefs)
            {
                MMALLog.Logger.LogDebug($"Probing sensor {sensorDef.Name} on addr {sensorDef.I2CAddr}");

                if (sensorDef.I2CIdentLength <= 2)
                {
                    var rd = this.I2CRead(fd, sensorDef.I2CAddr, sensorDef.I2CIdentReg, sensorDef.I2CIdentLength, sensorDef.I2CAddressing);

                    if (rd != null && rd[0] == sensorDef.I2CIdentValue)
                    {
                        MMALLog.Logger.LogInformation($"Found sensor {sensorDef.Name} at address {sensorDef.I2CAddr}.");

                        return sensorDef;
                    }
                    else
                    {
                        MMALLog.Logger.LogInformation($"Unable to probe sensor {sensorDef.Name} at address {sensorDef.I2CAddr}.");
                    }
                }
            }

            return null;
        }

        private unsafe byte[] I2CRead(int fd, int i2cAddr, int reg, int n, int addressing)
        {
            int len = 0;

            Console.WriteLine($"I2C Addr: {i2cAddr}");
            Console.WriteLine($"Reg: {reg}");
            Console.WriteLine($"N: {n}");
            Console.WriteLine($"Addressing: {addressing}");

            var buf = new byte[2] { (byte)(reg >> 8), (byte)(reg & 0xff) };

            Console.WriteLine($"Buffer value 1 {reg >> 8}. Buffer value 2 {reg & 0xff}");

            var ptr1 = Marshal.AllocHGlobal(2);
            var ptr2 = Marshal.AllocHGlobal(n);

            Marshal.Copy(buf, 0, ptr1, 2);
            
            if (addressing == 1)
            {
                len = 1;
            }
            else
            {
                len = n;
            }
            
            var msg1 = new I2CMsg(i2cAddr, 0, 2, ptr1);
            var msg2 = new I2CMsg(i2cAddr, I2C_M_RD, len, ptr2);

            var msgArr = new I2CMsg[2] { msg1, msg2 };
            var err = 0;
            var win32Err = 0;
                           
            fixed (I2CMsg* pArray = msgArr)
            {                    
                IntPtr msgs = new IntPtr((void*)pArray);

                var msgSet = new I2CRdwrIoctlData(msgs, 2);

                var msgSetPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(I2CRdwrIoctlData)));
                Marshal.StructureToPtr(msgSet, msgSetPtr, true);

                Console.WriteLine($"Fd: {fd}");

                err = MMALI2C.Ioctl(fd, I2C_RDWR, msgSetPtr);

                win32Err = Marshal.GetLastWin32Error();

                Marshal.FreeHGlobal(msgSetPtr);
            }

            var arr = new byte[n];

            Marshal.Copy(ptr2, arr, 0, n);

            Marshal.FreeHGlobal(ptr1);
            Marshal.FreeHGlobal(ptr2);

            Console.WriteLine($"Return value: {err}");

            if (err != 2)
            {
                MMALLog.Logger.LogWarning($"Unable to read from I2C. Error {win32Err}.");
                return null;
            }

            return arr;
        }

        private async Task SendRegs(int fd, SensorDef sensorDef, List<SensorReg> sensorRegs)
        {
            for (var i = 0; i < sensorRegs.Count; i++)
            {
                if (sensorRegs[i].Reg == 0xFFFF)
                {
                    var ptr = Marshal.AllocHGlobal(Marshal.SizeOf(sensorRegs[i].Data));

                    if (MMALI2C.Ioctl(fd, I2C_SLAVE_FORCE, ptr) < 0)
                    {
                        Marshal.FreeHGlobal(ptr);
                        throw new MMALIOException($"Failed to set I2C address to {sensorRegs[i].Data}");
                    }

                    Marshal.FreeHGlobal(ptr);
                }
                else if (sensorRegs[i].Reg == 0xFFFE)
                {
                    // Sleep...?
                    MMALLog.Logger.LogDebug($"Delaying for {sensorRegs[i].Data}ms.");
                    await Task.Delay(sensorRegs[i].Data);
                }
                else
                {
                    if (sensorDef.I2CAddressing == 1)
                    {
                        byte[] msg;

                        if (sensorDef.I2CDataSize == 2)
                        {
                            msg = new byte[3]
                            {
                                (byte)sensorRegs[i].Reg,
                                (byte)((sensorRegs[i].Data >> 8) & 0xFF),
                                (byte)(sensorRegs[i].Data & 0xFF)
                            };
                        }
                        else
                        {
                            msg = new byte[2];

                            msg[0] = (byte)sensorRegs[i].Reg;
                            msg[1] = (byte)(sensorRegs[i].Data & 0xFF);
                        }

                        var unmanagedArray = Marshal.AllocHGlobal(msg.Length);

                        Marshal.Copy(msg, 0, unmanagedArray, msg.Length);

                        var write = MMALI2C.Write(fd, unmanagedArray, msg.Length);

                        Marshal.FreeHGlobal(unmanagedArray);

                        if (write != msg.Length)
                        {
                            var errno = Marshal.GetLastWin32Error();

                            throw new MMALIOException($"Failed to write register index {i} ({sensorRegs[i].Reg} val {sensorRegs[i].Data}). Msg length: {msg.Length}. Written: {write}. Errno: {errno}");
                        }
                    }
                    else
                    {
                        byte[] msg;

                        if (sensorDef.I2CDataSize == 2)
                        {
                            msg = new byte[4]
                            {
                                (byte)(sensorRegs[i].Reg >> 8),
                                (byte)sensorRegs[i].Reg,
                                (byte)(sensorRegs[i].Data >> 8),
                                (byte)sensorRegs[i].Data
                            };
                        }
                        else
                        {
                            msg = new byte[3]
                            {
                                (byte)(sensorRegs[i].Reg >> 8),
                                (byte)sensorRegs[i].Reg,
                                (byte)sensorRegs[i].Data
                            };
                        }

                        var unmanagedArray = Marshal.AllocHGlobal(msg.Length);

                        Marshal.Copy(msg, 0, unmanagedArray, msg.Length);

                        var write = MMALI2C.Write(fd, unmanagedArray, msg.Length);

                        Marshal.FreeHGlobal(unmanagedArray);
                                                
                        if (write != msg.Length)
                        {
                            var errno = Marshal.GetLastWin32Error();
                            
                            throw new MMALIOException($"Failed to write register index {i}. Msg length: {msg.Length}. Written: {write}. Errno: {errno}");
                        }
                    }
                }
            }
        }
    }
}
