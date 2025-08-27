using CAM.CncWorkCenter;

namespace CAM
{
    public class PostProcessorWireSaw : PostProcessorWireSawBase
    {
        public override string[] StartMachine()
        {
            return new[]
            {
                "G92"
            };
        }

        public override string[] StopMachine()
        {
            return new[]
            {
                "G0 G53 Z0 ",
                "G0 G53 A0 C0",
                "G0 G53 X0 Y0",
                "M30",
            };
        }

        public override string[] StartEngine(int frequency, bool hasTool)
        {
            throw new System.NotImplementedException();
        }

        public new string[] StartEngine()
        {
            return new[]
            {
                "M03"
            };
        }

        public override string[] StopEngine()
        {
            return new[]
            {
                "M5",
                "M9",
                "G61",
                "G153",
                "G0 G53 Z0",
                "SETMSP=1",
            };
        }

        public override string Pause(double duration) => $"(DLY,{duration})";
    }
}
