using System;
using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace CAM
{
    public interface ICommandGenerator: IDisposable
    {
        void StartTechProcess(string caption, double originX, double originY, double zSafety);

        List<ProcessCommand> FinishTechProcess();

        void StartTechOperation();

        List<ProcessCommand> FinishTechOperation();

        void SetTool(int toolNo, int frequency, double angleA = 0, bool hasTool = true);

        void SetZSafety(double zSafety, double zMax = 0);

        void Uplifting(double? z = null);

        void Move(double? x = null, double? y = null, double? z = null, double? angleC = null, double angleA = 0);

        void Cutting(double x, double y, double z, int feed);

        void Transition(double? x = null, double? y = null, double? z = null, int? feed = null);

        void Cutting(Point3d startPoint, Point3d endPoint, int cuttingFeed, int transitionFeed, double angleA = 0);

        void Cutting(Point3d startPoint, Vector3d delta, int cuttingFeed, int transitionFeed);

        void Cutting(Curve curve, int cuttingFeed, int transitionFeed, Side engineSide = Side.None, double? angleC = null, double angleA = 0);

        void Cutting(Point3d startPoint, Vector3d delta, double[] zArray, int cuttingFeed, int smallFeed, Side engineSide = Side.None, double? angleC = null, double angleA = 0);

        void Cutting(Curve curve, double[] zArray, int cuttingFeed, int smallFeed, Side engineSide = Side.None, double? angleC = null, double angleA = 0);

        void Command(string text, string name = null, double duration = 0);

        void GCommand(string name, int gCode, string paramsString = null, Point3d? point = null, double? x = null, double? y = null, double? z = null, 
            double? angleC = null, double? angleA = null, Curve curve = null, int? feed = null, Point2d? center = null);

        bool WithThick { get; set; }

        bool IsUpperTool { get; }

        void Uplifting(Vector3d vector3d);

        double ZSafety { get; set; }

        Location ToolLocation { get; set; }
        string ThickCommand { get; set; }

        void Pause(double duration);
    }
}