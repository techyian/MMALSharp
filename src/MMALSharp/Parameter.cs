using System;

namespace MMALSharp
{
    public class Parameter
    {
        public Parameter(int paramVal, Type paramType, string paramName)
        {
            this.ParamValue = paramVal;
            this.ParamType = paramType;
            this.ParamName = paramName;
        }

        public int ParamValue { get; set; }

        public Type ParamType { get; set; }

        public string ParamName { get; set; }
    }
}
