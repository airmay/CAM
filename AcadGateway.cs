using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Windows;
using CAM.Domain;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CAM
{
    /// <summary>
    /// Класс осуществляющий взаимодействие с автокадом
    /// </summary>
    public class AcadGateway : IAcadGateway
    {
        public void CreateEntities(List<Curve> entities)
        {
            throw new NotImplementedException();
        }

        public void CreateEntities(IEnumerable<Curve> entities)
        {
            throw new NotImplementedException();
        }

        public void DeleteEntities(IEnumerable<Curve> idList)
        {
            throw new NotImplementedException();
        }

        public Curve[] GetSelectedCurves()
        {
            return new Curve[] { new Line(new Point3d(0, 0, 0), new Point3d(1, 0, 0)), new Arc(new Point3d(0, 0, 0), 1, 0, 1) };
        }

        public Curve[] GetSelectedEntities()
        {
            throw new NotImplementedException();
        }

        public void SelectCurve(Curve curve)
        {
            throw new NotImplementedException();
        }

        public void SelectEntities(List<ObjectId> idList)
        {
            throw new NotImplementedException();
        }


        public static void AddPaletteSet(PaletteSet paletteSet, string name, Control control)
        {
            if (paletteSet == null)
                paletteSet = new PaletteSet("Технология")
                {
                    Style = PaletteSetStyles.NameEditable | PaletteSetStyles.ShowPropertiesMenu | PaletteSetStyles.ShowAutoHideButton | PaletteSetStyles.ShowCloseButton,
                    MinimumSize = new Size(300, 200),
                    KeepFocus = true,
                    Visible = true
                };
            paletteSet.Add(name, control);
        }

        internal PaletteSet CreatePaletteSet()
        {
            return new PaletteSet("Технология")
            {
                Style = PaletteSetStyles.NameEditable | PaletteSetStyles.ShowPropertiesMenu | PaletteSetStyles.ShowAutoHideButton | PaletteSetStyles.ShowCloseButton,
                MinimumSize = new Size(300, 200),
                KeepFocus = true,
                Visible = true
            };
        }
    }
}
