using System;
using System.Drawing;
using System.Windows.Forms;
using Telemedicine.Controller;

namespace Telemedicine
{
    public partial class LogUP_Patient : Form
    {
        Query controller;

        public LogUP_Patient()
        {
            InitializeComponent();
            controller = new Query(ConnectionString.ConnStr);
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
            controller.SignUP_Patient_Account(textBox1.Text, textBox2.Text, textBox3.Text, textBox4.Text, textBox5.Text, textBox6.Text);
        }
    }
}
