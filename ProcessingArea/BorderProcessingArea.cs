using System;
using Autodesk.AutoCAD.DatabaseServices;

namespace CAM.Domain
{
    [Serializable]
    public class BorderProcessingArea : ProcessingArea
    {
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
    }
}