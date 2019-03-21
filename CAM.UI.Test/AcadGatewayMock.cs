using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using CAM.Domain;

namespace CAM.UI.Test
{
    /// <summary>
    /// Макет класса осуществляющего взаимодействие с автокадом
    /// </summary>
    public class AcadGatewayMock : IAcadGateway
    {
        private ListBox _listBox;

        public AcadGatewayMock(ListBox listBox)
        {
            _listBox = listBox;
            for (int i = 1; i < 10; i++)
                _listBox.Items.Add((i % 2 == 0 ? "Прямая" : "Дуга") + "_" + i);
        }

        public Curve[] GetSelectedEntities()
        {
            var curvies = new List<Curve>();
            foreach (var item in _listBox.SelectedItems)
            {
                var strings = item.ToString().Split('_');
                var curve = strings[0] == "Прямая"
                    ? new Line() as Curve
                    : new Arc();
                var pos = Int32.Parse(strings[1]) * 1000;
                curve.StartPoint = new Point3d(pos, pos, pos);
                curve.EndPoint = new Point3d(pos + 1000, pos + 1000, pos + 1000);
                curvies.Add(curve);
            }
            return curvies.ToArray();
        }

        public void SelectEntities(List<ObjectId> list)
        {
            var keys = list.ConvertAll(p => p.ToString());
            for (int i = 0; i < _listBox.Items.Count; i++)
                _listBox.SetSelected(i, keys.Contains(_listBox.Items[i]));
        }

        public void CreateEntities(List<Curve> entities)
        {
            entities.ForEach(p => _listBox.Items.Add(p.ObjectId.ToString()));
        }

	    public void DeleteEntities(IEnumerable<Curve> ids)
        {
            ids.ToList().ForEach(p => { if (_listBox.Items.Contains(p.ObjectId.ToString())) _listBox.Items.Remove(p.ObjectId.ToString()); });
        }

	    public void CreateEntities(IEnumerable<Curve> entities)
	    {
	        entities.ToList().ForEach(p => _listBox.Items.Add(p.ObjectId.ToString()));
        }

        public void SelectCurve(Curve curve)
        {
            for (int i = 0; i < _listBox.Items.Count; i++)
                _listBox.SetSelected(i, curve.ObjectId.ToString() == _listBox.Items[i].ToString());
        }
    }
}
