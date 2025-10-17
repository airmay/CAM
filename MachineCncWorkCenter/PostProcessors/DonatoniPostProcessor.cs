namespace CAM.MachineCncWorkCenter.PostProcessors;

public class DonatoniPostProcessor : PostProcessorCnc
{
    public override string[] StartMachine()
    {
        return
        [
            "%300",
            "RTCP=1",
            "G600 X0 Y-2500 Z-370 U3800 V0 W0 N0",
            "G601",
        ];
    }

    public override string[] StopMachine()
    {
        return
        [
            "G0 G53 Z0 ",
            "G0 G53 A0 C0",
            "G0 G53 X0 Y0",
            "M30",
        ];
    }

    public override string[] SetTool(int toolNo, double angleA, double angleC, int originCellNumber)
    {
        return
        [
            "G0 G53 Z0",
            $"G0 G53 C{angleC} A{angleA}",
            "G64",
            $"G154O{originCellNumber}",
            $"T{toolNo}",
            "M6",
            "G172 T1 H1 D1",
            "M300",
        ];
    }

    public override string[] StartEngine(int frequency, bool hasTool)
    {
        return
        [
            hasTool ? "M7" : "M8", // todo
            $"S{frequency}",
            "M3",
        ];
    }

    public override string[] StopEngine()
    {
        return
        [
            "M5",
            "M9",
            "G61",
            "G153",
            "G0 G53 Z0",
            "SETMSP=1",
        ];
    }
}