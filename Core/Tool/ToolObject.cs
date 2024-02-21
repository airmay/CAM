﻿using System;
using System.Collections.Generic;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.GraphicsInterface;

namespace CAM
{
    public class ToolObject
    {
        public Curve[] Curves { get; set; }
        private Tool _tool;
        private Point3d _position;
        private double _angleC;
        private double _angleA;

        public void Set(Tool tool, Point3d _position, double angleC, double angleA, MachineType machineType)
        {
            var l = new List<string>();
            var r = l.Contains("sdf");

            if (Curves != null && (toolPosition == null || tool != _tool))
                DeleteCurves();
            _tool = tool;
            if (toolPosition != null)
            {
                if (Curves == null)
                    CreateCurves(machineType);

                var matrix = toolPosition.GetTransformMatrixFrom(_toolPosition);
                //var matrix = toolPosition.Matrix;
                //if (_toolPosition != null)
                //    matrix = matrix * _toolPosition.InvMatrix;
                TransformCurves(matrix);
            }
            _toolPosition = toolPosition;
        }

        private void CreateCurves(MachineType machineType)
        {
            Curves = _tool.GetModelCurves(machineType);

            using (var doclock = Application.DocumentManager.MdiActiveDocument.LockDocument())
            using (Transaction tr = Acad.Database.TransactionManager.StartTransaction())
            {
                foreach (var item in Curves)
                {
                    item.Color = Color.FromColorIndex(ColorMethod.ByColor, 131);
                    TransientManager.CurrentTransientManager.AddTransient(item, TransientDrawingMode.Main, 128, new IntegerCollection());
                }
                tr.Commit();
            }
        }

        private void TransformCurves(Matrix3d matrix)
        {
            foreach (var item in Curves)
            {
                item.TransformBy(matrix);
                TransientManager.CurrentTransientManager.UpdateTransient(item, new IntegerCollection());
            }
            Acad.Editor.UpdateScreen();
        }

        public void DeleteCurves()
        {
            if (Curves == null)
                return;
            using (var doclock = Application.DocumentManager.MdiActiveDocument.LockDocument())
            using (Transaction tr = Acad.Database.TransactionManager.StartTransaction())
                foreach (var item in Curves)
                {
                    TransientManager.CurrentTransientManager.EraseTransient(item, new IntegerCollection());
                    item.Dispose();
                }
            Curves = null;
            _tool = null;
            _toolPosition = null;
        }

        //public bool HasTool { get; set; }

        //public ToolLocation Location { get; set; }

        //public static ToolObject CreateToolObject(Tool tool, bool isFrontPlaneZero)
        //{
        //    if (tool.Type == ToolType.Cable)
        //        return CreateToolObjectInt(true, new Line(new Point3d(0, -1000, 0), new Point3d(0, 1000, 0)));

        //    var circle0 = new Circle(new Point3d(0, isFrontPlaneZero ? 0 : -tool.Thickness.Value, tool.Diameter / 2), Vector3d.YAxis, tool.Diameter / 2);
        //    var circle1 = new Circle(circle0.Center + Vector3d.YAxis * tool.Thickness.Value, Vector3d.YAxis, tool.Diameter / 2);
        //    var axis = new Line(circle1.Center, circle1.Center + Vector3d.YAxis * tool.Diameter / 4);

        //    return CreateToolObjectInt(true, circle0, circle1, axis);
        //}

        //public static ToolObject CreateToolObject() => 
        //    CreateToolObjectInt(false, new Circle(Point3d.Origin, Vector3d.ZAxis, 20), new Line(Point3d.Origin, Point3d.Origin + Vector3d.ZAxis * 100));


    }
}
