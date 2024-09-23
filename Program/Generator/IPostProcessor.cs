using Autodesk.AutoCAD.Geometry;

namespace CAM
{
    public interface IPostProcessor
    {
        Point2d Origin { get; set; }
        // string GCommand(int gCode, MillToolPosition position, int? feed, Point2d? arcCenter = null);
        string[] StartMachine();
        string[] StopMachine();
        string[] SetTool(int toolNo, double angleA, double angleC, int originCellNumber);
        string[] StartEngine(int frequency, bool hasTool);
        string[] StopEngine();
        string Pause(double duration);
        // void SetParams(MillToolPosition toolPosition);
        string Cycle();
    }
}