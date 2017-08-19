using CAM.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CAM.UI.Test
{
    public partial class TestForm : Form
    {
        public TestForm()
        {
            InitializeComponent();

            var segments = new List<Segment>
            {
                new Segment
                {
                    Name = "Отрезок1",
                    IsExactlyBegin = true,
                    Depth = 30,
                    DepthStep = 3,
                    Speed = 50
                },
                new Segment
                {
                    Name = "Дуга1",
                    IsExactlyEnd = true,
                    Depth = 30,
                    DepthStep = 3,
                    Speed = 50
                }
            };
            productView.SetData(segments);
        }
    }
}
