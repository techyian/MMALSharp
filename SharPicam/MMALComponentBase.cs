using SharPicam.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SharPicam
{
    public unsafe class MMALComponentBase : MMALObject
    {
        public MMAL_COMPONENT_T* Ptr { get; set; }
        public string Name { get; set; }
        public bool Enabled {
            get {
                return (*this.Ptr).isEnabled == 1;
            }
        }
        public List<MMALPortImpl> Inputs { get; set; }
        public List<MMALPortImpl> Outputs { get; set; }
        public List<MMALPortImpl> Clocks { get; set; }
        public List<MMALPortImpl> Ports { get; set; }
                
        protected MMALComponentBase(string name)
        {
            var ptr = CreateComponent(name);

            this.Ptr = ptr;

            this.Name = Marshal.PtrToStringAnsi((IntPtr)((*ptr).name));

            Inputs = new List<MMALPortImpl>();
            Outputs = new List<MMALPortImpl>();
            Clocks = new List<MMALPortImpl>();
            Ports = new List<MMALPortImpl>();

            if((*ptr).inputNum > 0)
            {
                var inputs = *(*ptr).input;
                for (int i = 0; i < (*ptr).inputNum; i++)
                {
                    MMAL_PORT_T* t = &inputs[i];
                    Inputs.Add(new MMALPortImpl(t));
                }
            }
                
            if((*ptr).outputNum > 0)
            {
                var outputs = *(*ptr).output;
                for (int i = 0; i < (*ptr).outputNum; i++)
                {
                    MMAL_PORT_T* t = &outputs[i];
                    Outputs.Add(new MMALPortImpl(t));
                }
            }        
            
            if((*ptr).clockNum > 0)
            {
                var clocks = *(*ptr).clock;
                for (int i = 0; i < (*ptr).clockNum; i++)
                {
                    MMAL_PORT_T* t = &clocks[i];
                    Clocks.Add(new MMALPortImpl(t));
                }
            }

            if((*ptr).portNum > 0)
            {
                var ports = *(*ptr).port;
                for (int i = 0; i < (*ptr).portNum; i++)
                {
                    MMAL_PORT_T* t = &ports[i];
                    Ports.Add(new MMALPortImpl(t));
                }
            }
        }

        private static MMAL_COMPONENT_T* CreateComponent(string name)
        {
            IntPtr ptr = IntPtr.Zero;
            MMALCheck(MMALComponent.mmal_component_create(name, &ptr), "Unable to create component");

            System.Console.WriteLine("Ptr address " + ptr.ToString());

            var compPtr = (MMAL_COMPONENT_T*)ptr.ToPointer();

            System.Console.WriteLine("Ptr address " + ptr.ToString());

            return compPtr;
        }

        protected void AcquireComponent()
        {
            MMALComponent.mmal_component_acquire(this.Ptr);
        }

        protected void ReleaseComponent()
        {
            MMALCheck(MMALComponent.mmal_component_release(this.Ptr), "Unable to release component");
        }

        protected void DestroyComponent()
        {
            MMALCheck(MMALComponent.mmal_component_destroy(this.Ptr), "Unable to destroy component");
        }

        protected void EnableComponent()
        {
            MMALCheck(MMALComponent.mmal_component_enable(this.Ptr), "Unable to enable component");                        
        }

        protected void DisableComponent()
        {
            MMALCheck(MMALComponent.mmal_component_enable(this.Ptr), "Unable to disable component");
        }

    }
}
