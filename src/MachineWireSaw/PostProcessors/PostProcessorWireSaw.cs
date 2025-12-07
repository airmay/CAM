namespace CAM.MachineWireSaw.PostProcessors;

public class PostProcessorWireSaw : PostProcessorWireSawBase
{
    public override string[] StartMachine()
    {
        return
        [
            "G92",
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

    public string[] StartEngine()
    {
        return
        [
            "M03",
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

    public override string Pause(double duration) => $"(DLY,{duration})";
}