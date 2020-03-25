using System;
using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace CAM
{
    public interface ICommandGenerator: IDisposable
    {
        void StartTechProcess(string caption, double originX, double originY, int zSafety);

        List<ProcessCommand> FinishTechProcess();

        void StartTechOperation();

        List<ProcessCommand> FinishTechOperation();

        void SetTool(int toolNo, int frequency, double angleA = 0);

        void Uplifting(double? z = null);

        void Move(double x, double y, double angleC, double angleA = 0);

        void Cutting(double x, double y, double z, double angleC, int feed, double angleA = 0);

        void Cutting(Point3d startPoint, Point3d endPoint, int cuttingFeed, int transitionFeed);

        void Cutting(Curve curve, int cuttingFeed, int transitionFeed, Side engineSide = Side.None);

        void Command(string text, string name = null);

        void GCommand(string name, int gCode, string paramsString = null, Point3d? point = null, double? x = null, double? y = null, double? z = null, double? angleC = null, double? angleA = null, Curve curve = null, int? feed = null);

        bool WithThick { get; set; }

        bool IsUpperTool { get; }

        void Uplifting(Vector3d vector3d);

        void Move(Point3d point3d, double v1, double v2);
    }
}