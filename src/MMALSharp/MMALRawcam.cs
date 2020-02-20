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
        private const int I2C_SLAVE = 0x0703;
        private const int I2C_SLAVE_FORCE = 0x0706;
        private const int I2C_RDWR = 0x0707;

        private MMALRawcamComponent _rawcamComponent;
        private MMALIspComponent _ispComponent;
        private SensorDef _sensorDef;
        private ModeDef _modeDef;
        private int _mode;
        private string _i2cDeviceName;
        private int _activeFileDescriptor;

        /// <summary>
        /// Gets the singleton instance of the MMAL Raw Camera. Call to initialise the camera for first use.
        /// </summary>
        public static MMALRawcam Instance => Lazy.Value;

        /// <summary>
        /// Reference to the camera component.
        /// </summary>
        public MMALRawcamComponent Camera { get; }

        private static readonly Lazy<MMALRawcam> Lazy = new Lazy<MMALRawcam>(() => new MMALRawcam());

        private static List<SensorDef> SensorDefs => new List<SensorDef>
        {
            Ov5647SensorDefs.Ov5647SensorDef,
            Imx219SensorDefs.Imx219SensorDef
        };
        
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
        /// <param name="mode">The selected sensor mode.</param>
        /// <param name="i2cDeviceName">The I2C device name.</param>
        public void ConfigureRawcamPipeline(MMALRawcamComponent rawcamComponent, MMALIspComponent ispComponent, int mode, string i2cDeviceName)
        {
            _rawcamComponent = rawcamComponent;
            _ispComponent = ispComponent;
            _i2cDeviceName = i2cDeviceName;
            _mode = mode;
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

            MMALLog.Logger.LogInformation("Attempting to enable rawcam components...");

            var sensor = this.ProbeSensor(_i2cDeviceName);

            if (sensor == null)
            {
                throw new MMALIOException($"Could not probe sensor {_i2cDeviceName}");
            }
            
            _ispComponent.EnableComponent();
            _rawcamComponent.EnableComponent();

            _ispComponent.Inputs[0].Start();
            _ispComponent.Outputs[0].Start();
            _rawcamComponent.Outputs[0].Start();

            tasks.Add(_ispComponent.Outputs[0].Trigger.Task);
            tasks.Add(_rawcamComponent.Outputs[0].Trigger.Task);

            MMALLog.Logger.LogDebug("Attempting to start rawcam streaming...");

            await this.StartCapture(sensor, sensor.Modes[_mode], _i2cDeviceName);

            if (cancellationToken == CancellationToken.None)
            {
                await Task.WhenAll(tasks).ConfigureAwait(false);

                await this.StopCapture(sensor, _i2cDeviceName);
            }
            else
            {
                await Task.WhenAny(Task.WhenAll(tasks), cancellationToken.AsTask()).ConfigureAwait(false);

                _ispComponent.ForceStopProcessing = true;
                _rawcamComponent.ForceStopProcessing = true;

                await this.StopCapture(sensor, _i2cDeviceName);

                await Task.WhenAll(tasks).ConfigureAwait(false);
            }

            foreach (var port in _ispComponent.ProcessingPorts.Values)
            {
                if (port.ConnectedReference == null)
                {
                    port.DisablePort();
                }
            }

            _ispComponent.CleanPortPools();
            _ispComponent.DisableConnections();

            foreach (var port in _rawcamComponent.ProcessingPorts.Values)
            {
                if (port.ConnectedReference == null)
                {
                    port.DisablePort();
                }
            }

            _rawcamComponent.CleanPortPools();
            _rawcamComponent.DisableConnections();
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
                if (MMALI2C.IoctlByte(_activeFileDescriptor, I2C_SLAVE_FORCE, sensorDef.I2CAddr) < 0)
                {
                    var lastError = Marshal.GetLastWin32Error();
                    throw new MMALIOException($"Failed to set I2C address. Win32Err {lastError}. Fd {_activeFileDescriptor}. I2CAddr {sensorDef.I2CAddr}.");
                }

                await this.SendRegs(_activeFileDescriptor, sensorDef, modeDef.Regs);

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
                if (MMALI2C.IoctlByte(_activeFileDescriptor, I2C_SLAVE_FORCE, sensorDef.I2CAddr) < 0)
                {
                    var lastError = Marshal.GetLastWin32Error();
                    throw new MMALIOException($"Failed to set I2C address. Win32Err {lastError}. Fd {_activeFileDescriptor}. I2CAddr {sensorDef.I2CAddr}.");
                }

                await this.SendRegs(_activeFileDescriptor, sensorDef, sensorDef.StopReg);

                MMALI2C.Close(_activeFileDescriptor);

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

            var err = Marshal.GetLastWin32Error();

            if (fd == 0)
            {
                throw new MMALIOException("Couldn't open I2C device.");
            }

            _activeFileDescriptor = fd;

            MMALLog.Logger.LogInformation($"I2C Device Name: {i2cDeviceName} | Fd err: {err}");
            
            foreach (var sensorDef in SensorDefs)
            {
                MMALLog.Logger.LogInformation($"Probing sensor {sensorDef.Name} on addr {sensorDef.I2CAddr}");

                if (sensorDef.I2CIdentLength <= 2)
                {
                    var rd = this.I2CRead(fd, sensorDef.I2CAddr, sensorDef.I2CIdentReg, sensorDef.I2CIdentLength, sensorDef.I2CAddressing, sensorDef.I2CIdentValue);
                    
                    if (rd != null)
                    {
                        var i2cIdentValueReturned = BitConverter.ToUInt16(rd, 0);

                        if (i2cIdentValueReturned != sensorDef.I2CIdentValue)
                        {
                            MMALLog.Logger.LogInformation($"Sensor probe successful but returned incorrect I2CIdentValue.");
                            return null;
                        }

                        MMALLog.Logger.LogInformation($"Found sensor {sensorDef.Name} at address {sensorDef.I2CAddr}.");

                        return sensorDef;
                    }
                    
                    MMALLog.Logger.LogInformation($"Unable to probe sensor {sensorDef.Name} at address {sensorDef.I2CAddr}.");                    
                }
            }

            return null;
        }

        private byte[] I2CRead(int fd, byte i2cAddr, int reg, ushort n, int addressing, ushort i2cIdentValue)
        {
            MMALLog.Logger.LogInformation($"I2C Addr: {i2cAddr} | Reg: {reg} | Addressing: {addressing} | Fd: {fd}");

            var len = addressing == 1 ? 1 : 2;
            var buf = new byte[2] { (byte)(reg >> 8), (byte)(reg & 0xff) };
            var err = 0;
            var win32Err = 0;

            var ptr1 = Marshal.AllocHGlobal(len);
            var ptr2 = Marshal.AllocHGlobal(n);

            Marshal.Copy(buf, 0, ptr1, buf.Length);

            var msg1 = new I2CMsg(i2cAddr, 0, (ushort)len, ptr1);
            var msg2 = new I2CMsg(i2cAddr, I2C_M_RD, n, ptr2);

            var msgPtr1 = Marshal.AllocHGlobal(Marshal.SizeOf<I2CMsg>() + len);
            var msgPtr2 = Marshal.AllocHGlobal(Marshal.SizeOf<I2CMsg>() + n);

            Marshal.StructureToPtr(msg1, msgPtr1, false);
            Marshal.StructureToPtr(msg2, msgPtr2, false);

            var msgSet1 = new I2CRdwrIoctlData(msgPtr1, 1);
            var msgSet2 = new I2CRdwrIoctlData(msgPtr2, 1);

            var msgSetPtr1 = Marshal.AllocHGlobal(Marshal.SizeOf<I2CRdwrIoctlData>() + len);
            var msgSetPtr2 = Marshal.AllocHGlobal(Marshal.SizeOf<I2CRdwrIoctlData>() + n);

            Marshal.StructureToPtr(msgSet1, msgSetPtr1, false);
            Marshal.StructureToPtr(msgSet2, msgSetPtr2, false);

            err = MMALI2C.Ioctl(fd, I2C_RDWR, msgSetPtr1);

            win32Err = Marshal.GetLastWin32Error();
            Console.WriteLine($"Read I2C err value: {err} | win32Err: {win32Err}");
            
            err = MMALI2C.Ioctl(fd, I2C_RDWR, msgSetPtr2);

            win32Err = Marshal.GetLastWin32Error();
            Console.WriteLine($"Write I2C err value: {err} | win32Err: {win32Err}");

            var arr = new byte[n];

            Marshal.Copy(ptr2, arr, 0, n);

            Marshal.DestroyStructure(msgSetPtr1, typeof(I2CRdwrIoctlData));
            Marshal.FreeHGlobal(msgSetPtr1);

            Marshal.DestroyStructure(msgSetPtr2, typeof(I2CRdwrIoctlData));
            Marshal.FreeHGlobal(msgSetPtr2);

            Marshal.DestroyStructure(msgPtr1, typeof(I2CMsg));
            Marshal.FreeHGlobal(msgPtr1);

            Marshal.DestroyStructure(msgPtr2, typeof(I2CMsg));
            Marshal.FreeHGlobal(msgPtr2);
            
            Marshal.FreeHGlobal(ptr1);
            Marshal.FreeHGlobal(ptr2);

            if (err != 1)
            {
                MMALLog.Logger.LogWarning($"Unable to probe sensor on address {i2cAddr}.");
                return null;
            }

            var i2cIdentValueReturned = BitConverter.ToUInt16(arr, 0);

            MMALLog.Logger.LogInformation($"Probe successful on address {i2cAddr}. Sensor I2C Ident Value: {i2cIdentValue}. I2C Ident Value Returned: {i2cIdentValueReturned}.");
            
            return arr;
        }

        private async Task SendRegs(int fd, SensorDef sensorDef, List<SensorReg> sensorRegs)
        {
            for (var i = 0; i < sensorRegs.Count; i++)
            {
                if (sensorRegs[i].Reg == 0xFFFF)
                {
                    var ptr = Marshal.AllocHGlobal(Marshal.SizeOf(sensorRegs[i].Data));
                    Marshal.WriteInt32(ptr, sensorRegs[i].Data);

                    if (MMALI2C.Ioctl(fd, I2C_SLAVE_FORCE, ptr) < 0)
                    {
                        Marshal.FreeHGlobal(ptr);
                        throw new MMALIOException($"Failed to set I2C address to {sensorRegs[i].Data}");
                    }

                    Marshal.FreeHGlobal(ptr);

                    MMALLog.Logger.LogInformation($"Successfully called Ioctl on file descriptor {fd}.");
                }
                else if (sensorRegs[i].Reg == 0xFFFE)
                {
                    // Sleep...?
                    MMALLog.Logger.LogDebug($"Delaying for {sensorRegs[i].Data}ms.");
                    await Task.Delay(sensorRegs[i].Data).ConfigureAwait(false);
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

                        MMALLog.Logger.LogInformation($"Wrote {msg.Length} bytes to file descriptor {fd}.");
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
                        
                        MMALLog.Logger.LogInformation($"Wrote {msg.Length} bytes to file descriptor {fd}.");
                    }
                }
            }
        }
    }
}
