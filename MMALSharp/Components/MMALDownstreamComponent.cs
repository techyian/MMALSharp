using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMALSharp.Components
{
    public abstract unsafe class MMALDownstreamComponent : MMALComponentBase
    {
        //Storage for downstream components. Stored here until we've finished processing. 
        public byte[] Storage { get; set; }

        public MMALConnectionImpl Connection { get; set; }

        public MMALDownstreamComponent(string name) : base(name)
        {
        }

        public void CreateConnection(MMALPortBase output)
        {
            MMALConnectionImpl.CreateConnection(output, this.Inputs.ElementAt(0));
        }


    }
}
