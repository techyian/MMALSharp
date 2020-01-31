namespace MMALSharp.Config.SensorRegs
{
    public struct SensorReg
    {
        private int _reg;
        private int _data;

        public int Reg => _reg;
        public int Data => _data;

        public SensorReg(int reg, int data)
        {
            _reg = reg;
            _data = data;
        }
    }
}
