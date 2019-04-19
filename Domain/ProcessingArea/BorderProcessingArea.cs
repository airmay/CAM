using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;

namespace CAM.Domain
{
    [Serializable]
    public class BorderProcessingArea : ProcessingArea
    {
        const double AngleTolerance = 0.000001;

        /// <summary>
        /// Пограничная обрабатываемая область
        /// </summary>
        /// <param name="curve"></param>
        public BorderProcessingArea(Curve curve) : base(curve)
        {
        }

        public Side OuterSide { get; set; }

        private readonly bool[] _isExactly = new bool[2];

        /// <summary>
        /// Точно начало
        /// </summary>
        public bool IsExactlyBegin
        {
            get => _isExactly[(int) Corner.Start];
            set
            {
                IsAutoExactlyBegin = false;
                _isExactly[(int) Corner.Start] = value;
            }
        }

        /// <summary>
        /// Точно конец
        /// </summary>
        public bool IsExactlyEnd
        {
            get => _isExactly[(int) Corner.End];
            set
            {
                IsAutoExactlyEnd = false;
                _isExactly[(int) Corner.End] = value;
            }
        }

        private ref bool IsExactly(Corner corner) => ref _isExactly[(int) corner];

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
                CalcBordersChain(Corner.Start);
                if (borders.Contains(startBorder))
                    CalcBordersChain(Corner.End);
                borders.Remove(startBorder);
            }

            void CalcBordersChain(Corner corner)
            {
                var border = startBorder;
                var point = border.Curve.GetPoint(corner);
                BorderProcessingArea nextBorder;
                while ((nextBorder = borders.SingleOrDefault(p => p != border && (p.Curve.StartPoint == point || p.Curve.EndPoint == point))) != null)
                {
                    borders.Remove(nextBorder);
                    var nextCorner = nextBorder.Curve.GetCorner(point);

                    var isExactly = !border.IsAutoExactly(corner)
                        ? border.IsExactly(corner)
                        : (!nextBorder.IsAutoExactly(nextCorner)
                            ? nextBorder.IsExactly(nextCorner)
                            : CalcIsExactly(border, corner, nextBorder, nextCorner));
                    border.IsExactly(corner) = nextBorder.IsExactly(nextCorner) = isExactly;

                    if (nextBorder == startBorder) // цикл
                        return;

                    nextBorder.OuterSide = nextCorner != corner ? border.OuterSide : border.OuterSide.Swap();
                    border = nextBorder;
                    corner = nextCorner.Swap();
                    point = border.Curve.GetPoint(corner);
                }

                // свободный конец цепочки
                if (!borders.Contains(border) && border.IsAutoExactly(corner))
                    border.IsExactly(corner) = false;
            }

            bool CalcIsExactly(BorderProcessingArea border, Corner corner, BorderProcessingArea nextBorder, Corner nextCorner)
            {
                switch (border.Curve)
                {
                    case Line line:
                        bool isLeftTurn;
                        switch (nextBorder.Curve)
                        {
                            case Line nextLine:
                                var angleDiff = nextLine.Angle - line.Angle;
                                if (Math.Abs(angleDiff) < AngleTolerance)
                                    return false;
                                isLeftTurn = Math.Sin(angleDiff) > 0;
                                var isLeftOuterSide = border.OuterSide == Side.Left;
                                var isNextStartPoint = nextCorner == Corner.Start;
                                return isLeftTurn ^ isLeftOuterSide ^ isNextStartPoint;

                            case Arc nextArc:
                                var angleTan = nextCorner == Corner.Start
                                    ? nextArc.StartAngle + Math.PI / 2
                                    : nextArc.EndAngle - Math.PI / 2;
                                angleDiff = angleTan - line.Angle;
                                isLeftTurn = Math.Abs(angleDiff) > AngleTolerance
                                    ? Math.Sin(angleDiff) > 0
                                    : nextCorner == Corner.Start;
                                var isRightProcessSide = border.OuterSide == Side.Right;
                                return isLeftTurn ^ isRightProcessSide;
                        }

                        break;
                    case Arc arc:
                        if (nextBorder.Curve is Line)
                            return CalcIsExactly(nextBorder, nextCorner, border, corner);
                        break;
                }

                throw new InvalidOperationException("CalcIsExactly throw Error");
            }

            void CalcOuterSide(BorderProcessingArea border)
            {
                border.OuterSide = Side.Right;
            }
        }
    }
}