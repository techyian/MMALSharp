using SharPicam.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharPicam
{
    public unsafe class MMALComponentBase : MMALObject
    {
        public MMAL_COMPONENT_T* Ptr { get; set; }
        public List<MMALPortImpl> Inputs { get; set; }
        public List<MMALPortImpl> Outputs { get; set; }
        public List<MMALPortImpl> Clocks { get; set; }
        public List<MMALPortImpl> Ports { get; set; }


        private MMALComponentBase(MMAL_COMPONENT_T* ptr)
        {
            var comp = new MMAL_COMPONENT_T((*ptr).priv, (*ptr).userData, (*ptr).name,
                                            (*ptr).isEnabled, (*ptr).control, (*ptr).inputNum,
                                            (*ptr).input, (*ptr).outputNum, (*ptr).output, (*ptr).clockNum,
                                            (*ptr).clock, (*ptr).portNum, (*ptr).port, (*ptr).id);
            this.Ptr = ptr;

            var inputs = *comp.input;            
            for(int i = 0; i < comp.inputNum; i++)
            {
                MMAL_PORT_T* t = &inputs[i];
                Inputs.Add(new MMALPortImpl(t));
            }

            /*var outputs = *comp.output;
            for (int i = 0; i < comp.outputNum; i++)
            {
                MMAL_PORT_T* t = &outputs[i];
                Outputs.Add(new MMALPortImpl(t));
            }

            var clocks = *comp.clock;
            for (int i = 0; i < comp.clockNum; i++)
            {
                MMAL_PORT_T* t = &clocks[i];
                Clocks.Add(new MMALPortImpl(t));
            }

            var ports = *comp.port;
            for (int i = 0; i < comp.portNum; i++)
            {
                MMAL_PORT_T* t = &ports[i];
                Ports.Add(new MMALPortImpl(t));
            }*/

        }

        public static MMALComponentBase CreateComponent(string name)
        {
            IntPtr ptr = IntPtr.Zero;
            MMALCheck(MMALComponent.mmal_component_create(name, &ptr));

            System.Console.WriteLine("Ptr address " + ptr.ToString());

            var compPtr = (MMAL_COMPONENT_T*)ptr.ToPointer();

            System.Console.WriteLine("Ptr address " + ptr.ToString());

            return new MMALComponentBase(compPtr);


        }

    }
}
