using Autodesk.AutoCAD.Geometry;
using CAM.CncWorkCenter;
using System.Collections.Generic;

namespace CAM;

public class ChampionPostProcessor : PostProcessorCnc
{
    public override string[] StartMachine()
    {
        return
        [
            "G90",
            "#MCS G0Z0.0",
            "C90 A0",
            "V.A.ORGT[1].Z=190",
            "#FLUSH",
            "G54",
            "M22",
        ];
    }

    public override string[] StopMachine()
    {
        return
        [
            "#MCS G0Z0.0",
            "M23",
            "D0",
            "#MCS G0Z0.0",
            "#MCS G0Y-1",
            "M30"
        ];
    }

    public override string[] StartEngine(int frequency, bool hasTool)
    {
        return
        [
            $"S{frequency}",
            "M3",
            "#HSC ON[CONTERROR 0.1]"
        ];
    }
        
    public override string[] StopEngine()
    {
        return
        [
            "#HSC OFF",
            "M05"
        ];
    }

    public override string[] SetTool(int toolNo, double angleA, double angleC, int originCellNumber)
    {
        return
        [
            $"T{toolNo}D{(originCellNumber == 10 ? 1 : originCellNumber)}",
            "M6", 
        ];
    }

    //public double AC => 165 + Tool.Thickness.GetValueOrDefault();
    //public double AC_V => 169 + Tool.Thickness.GetValueOrDefault();

    protected override List<CommandParam> GetParams(Point3d? point, double? angleC, double? angleA, int? feed, Point2d? arcCenter)
    {
        return base.GetParams(point, angleC, angleA, feed, arcCenter);
        
        //var newParams = base.CreateParams(position, feed, center);

        //newParams["A"] = position.AngleC < 180 ? -position.AngleC : (360 - position.AngleC);
        //newParams["C"] = 90 - position.AngleA;

        //if (position.X.HasValue && position.Y.HasValue && position.Z.HasValue)
        //{
        //    var angleC = position.AngleC.ToRad();
        //    var angleA = position.AngleA.ToRad();
        //    var dl = AC * (1 - Math.Cos(angleA)) + Tool.Diameter / 2 * Math.Sin(angleA);
        //    var angle = Math.PI * 3 / 2 - angleC;
        //    if (position.X.HasValue)
        //        newParams["X"] = position.X.Value + dl * Math.Cos(angle) + AC_V * Math.Sin(angleC);
        //    if (position.Y.HasValue)
        //        newParams["Y"] = position.Y.Value + dl * Math.Sin(angle) + AC_V * Math.Cos(angleC); ;
        //    if (position.Z.HasValue)
        //        newParams["Z"] = position.Z.Value + AC * Math.Sin(angleA) - Tool.Diameter / 2 * (1 - Math.Cos(angleA)) + DZ;
        //}

        //return newParams;
    }
}