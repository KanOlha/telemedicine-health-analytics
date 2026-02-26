using System;
using System.Drawing;
using System.Windows.Forms;

namespace Telemedicine
{
    public partial class First_screen : Form
    {
        public First_screen()
        {
            InitializeComponent();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            //Blue frame design
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            int thickness = 2;
            int radius = 40;
            Color borderColor = Color.Blue;

            using (var path = new System.Drawing.Drawing2D.GraphicsPath())
            {
                path.AddArc(0, 0, radius, radius, 180, 90);
                path.AddArc(panel1.Width - radius - 1, 0, radius, radius, 270, 90);
                path.AddArc(panel1.Width - radius - 1, panel1.Height - radius - 1, radius, radius, 0, 90);
                path.AddArc(0, panel1.Height - radius - 1, radius, radius, 90, 90);
                path.CloseAllFigures();

                using (var pen = new Pen(borderColor, thickness))
                {
                    e.Graphics.DrawPath(pen, path);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            LogIN_Doctor fr = new LogIN_Doctor();
            fr.Show(); 
            Hide(); 
        }

        private void button2_Click(object sender, EventArgs e)
        {
            LogIN_Patient fr = new LogIN_Patient();
            fr.Show(); Hide();
        }
    }
}
