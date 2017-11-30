using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAM.Domain
{
    /// <summary>
    /// Генератор действий процесса обработки
    /// </summary>
    public class ProcessBuilder
    {
        private string _groupName;
        private Point3d _currentPoint = Point3d.Origin;
        private TechProcessParams _techProcessParams;

        public List<Curve> Entities { get; } = new List<Curve>();

        public List<ProcessAction> ProcessActions { get; set; }

        public ProcessBuilder(TechProcessParams techProcessParams)
        {
            _techProcessParams = techProcessParams;
        }

        public void SetGroup(string groupName) => _groupName = groupName;

        private void MoveXYZ(double x, double y, double z, string name, int feed = 0)
        {
            var destPoint = new Point3d(x, y, z);
            var line = new Line(_currentPoint, destPoint);
            line.ObjectId = new ObjectId("перемещение");
            ProcessActions.Add(new ProcessAction(name, _groupName, line, feed));
            _currentPoint = destPoint;
            Entities.Add(line);
        }

        private void MoveZ(double z, string name, int feed = 0) => MoveXYZ(_currentPoint.X, _currentPoint.Y, z, name, feed);

        /// <summary>
        /// Быстрая подача
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void Move(double x, double y)
        {
            if (_currentPoint == Point3d.Origin)
            {
                _currentPoint = new Point3d(x, y, _techProcessParams.ZSafety);
                ProcessActions.Add(new ProcessAction(ProcessActionNames.InitialMove, _groupName));
            }
            else
                MoveXYZ(x, y, _currentPoint.Z, ProcessActionNames.Fast);
        }

        /// <summary>
        /// Опускание
        /// </summary>
        /// <param name="z"></param>
        public void Descent(double z) => MoveZ(z, ProcessActionNames.Descent);

        /// <summary>
        /// Поднятие
        /// </summary>
        public void Uplifting() => MoveZ(_techProcessParams.ZSafety, ProcessActionNames.Uplifting);

        /// <summary>
        /// Заглубление
        /// </summary>
        /// <param name="z"></param>
        public void Penetration(double z) => MoveZ(z, $"{ProcessActionNames.Penetration} F{_techProcessParams.PenetrationRate}");

        /// <summary>
        /// Рабочий ход
        /// </summary>
        /// <param name="curve"></param>
        /// <param name="dz"></param>
        /// <param name="offset"></param>
        /// <param name="startIndent"></param>
        /// <param name="endIndent"></param>
        /// <param name="feed"></param>
        public void Cutting(Curve curve, double dz = 0, double offset = 0, double startIndent = 0, double endIndent = 0, int feed = 0)
        {
            var toolpathCurve = curve.GetOffsetCurves(dz);
            toolpathCurve.ObjectId = new ObjectId($"{curve.ObjectId.Key} обработка");
            if (toolpathCurve.StartPoint == _currentPoint)
                _currentPoint = toolpathCurve.EndPoint;
            else if (toolpathCurve.EndPoint == _currentPoint)
                _currentPoint = toolpathCurve.StartPoint;
            else
                throw new Exception("Несоответствие текущей позиции и рассчитанной траектории");

            ProcessActions.Add(new ProcessAction($"{ProcessActionNames.Cutting} F{feed}", _groupName, toolpathCurve, feed));
            Entities.Add(toolpathCurve);
        }
    }
}
