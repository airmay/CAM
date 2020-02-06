using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Dreambuild.AutoCAD;

namespace CAM
{
    [Serializable]
    public class BorderProcessingArea : ProcessingArea
    {
        /// <summary>
        /// Пограничная обрабатываемая область
        /// </summary>
        /// <param name="curve"></param>
        public BorderProcessingArea(Curve curve) : base(new Curve[] { curve })
        {
        }

        public Curve Curve => Curves[0];

        public Side OuterSide { get; set; }

        private readonly bool[] _isExactly = new bool[2];

        /// <summary>
        /// Точно начало
        /// </summary>
        public bool IsExactlyBegin
        {
            get => _isExactly[(int)Corner.Start];
            set
            {
                IsAutoExactlyBegin = false;
                _isExactly[(int)Corner.Start] = value;
            }
        }

        /// <summary>
        /// Точно конец
        /// </summary>
        public bool IsExactlyEnd
        {
            get => _isExactly[(int)Corner.End];
            set
            {
                IsAutoExactlyEnd = false;
                _isExactly[(int)Corner.End] = value;
            }
        }

        public ref bool IsExactly(Corner corner) => ref _isExactly[(int)corner];

        /// <summary>
        /// Автоформирование свойства точно начало
        /// </summary>
        public bool IsAutoExactlyBegin { get; set; } = true;

        /// <summary>
        /// Автоформирование свойства точно конец
        /// </summary>
        public bool IsAutoExactlyEnd { get; set; } = true;

        public bool IsAutoExactly(Corner corner) => corner == Corner.Start ? IsAutoExactlyBegin : IsAutoExactlyEnd;

        public static void ProcessBorders(List<BorderProcessingArea> borders, BorderProcessingArea fixedSideBorder = null)
        {
            BorderProcessingArea startBorder;
            while ((startBorder = fixedSideBorder ?? borders.Find(p => p.OuterSide != Side.None) ?? borders.FirstOrDefault()) != null)
            {
                fixedSideBorder = null;
                if (startBorder.OuterSide == Side.None)
                    CalcOuterSide(startBorder);
                var contour = CalcBordersChain(Corner.End);
                if (borders.Contains(startBorder))
                {
                    var contourBack = CalcBordersChain(Corner.Start);
                    contourBack.Reverse();
                    contourBack.Add(startBorder.Curve);
                    contourBack.AddRange(contour);
                    contour = contourBack;
                    borders.Remove(startBorder);
                }
                var sign = startBorder.OuterSide == Side.Left ? 1 : -1;
                Graph.CreateHatch(contour, sign);
            }

            List<Curve> CalcBordersChain(Corner corner)
            {
                var border = startBorder;
                var point = border.Curve.GetPoint(corner);
                BorderProcessingArea nextBorder;
                var contour = new List<Curve>();
                while ((nextBorder = borders.SingleOrDefault(p => p != border && p.Curve.HasPoint(point))) != null)
                {
                    contour.Add(nextBorder.Curve);
                    borders.Remove(nextBorder);
                    var nextCorner = nextBorder.Curve.GetCorner(point);
                    nextBorder.OuterSide = nextCorner != corner ? border.OuterSide : border.OuterSide.Swap();

                    var isExactly = !border.IsAutoExactly(corner)
                        ? border.IsExactly(corner)
                        : (!nextBorder.IsAutoExactly(nextCorner)
                            ? nextBorder.IsExactly(nextCorner)
                            : CalcIsExactly(border, corner, nextBorder, nextCorner, point));
                    border.IsExactly(corner) = nextBorder.IsExactly(nextCorner) = isExactly;

                    if (nextBorder == startBorder) // цикл
                        return contour;

                    border = nextBorder;
                    corner = nextCorner.Swap();
                    point = border.Curve.GetPoint(corner);
                }

                // свободный конец цепочки
                if (border.IsAutoExactly(corner))
                    border.IsExactly(corner) = false;
                return contour;
            }

            bool CalcIsExactly(BorderProcessingArea border, Corner corner, BorderProcessingArea nextBorder, Corner nextCorner, Point3d point)
            {
                var v1 = border.Curve.GetTangent(corner);
                var v2 = nextBorder.Curve.GetTangent(nextCorner);
                var isLeftTurn = v1.MinusPiToPiAngleTo(v2) > Consts.Epsilon;
                var isLeftOuterSide = border.OuterSide == Side.Left;
                var isNextStartPoint = nextCorner == Corner.Start;
                return isLeftTurn ^ isLeftOuterSide ^ isNextStartPoint;
            }

            void CalcOuterSide(BorderProcessingArea border)
            {
                //border.OuterSide = Side.Right;
                //var center = borders.Select(p => p.Curve).GetCenter();
                //var v1 = border.EndPoint.ToPoint2d() - border.StartPoint.ToPoint2d();
                //var v2 = center.ToPoint2d() - border.StartPoint.ToPoint2d();
                //border.OuterSide = v1.MinusPiToPiAngleTo(v2) > 0 ? Side.Right : Side.Left;
            }
        }
    }
}