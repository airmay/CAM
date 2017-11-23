using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
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
                _listBox.Items.Add((i % 2 == 0 ? "Прямая" : "Дуга") + i);
        }

        public Curve[] GetSelectedCurves()
        {
            var curvies = new List<Curve>();
            foreach (var item in _listBox.SelectedItems)
                curvies.Add(item.ToString().StartsWith("Прямая") ? new Line(item.ToString()) as Curve : new Arc(item.ToString()));
            return curvies.ToArray();
        }

        public void SelectCurvies(List<ObjectId> list)
        {
            var keys = list.ConvertAll(p => p.Key);
            for (int i = 0; i < _listBox.Items.Count; i++)
                _listBox.SetSelected(i, keys.Contains(_listBox.Items[i]));
        }
    }
}
