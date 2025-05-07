using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp4
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Pen red = new Pen(Color.Red, 3);
            Pen black = new Pen(Color.Black, 1);
            Random r = new Random();
            int n = r.Next(100);
            float raza = 1;
            PointF[] m = new PointF[n];
            for (int i = 0; i < n; i++)
            {
                m[i].X = r.Next(10, this.ClientSize.Width - 10);
                m[i].Y = r.Next(10, this.ClientSize.Height - 10);
                g.DrawEllipse(red, m[i].X - raza, m[i].Y - raza, raza * 2, raza * 2);
            }
            float xmin = m[0].X, xmax = m[0].X, ymin = m[0].Y, ymax = m[0].Y;
            for (int i = 1; i < n; i++)
            {
                if (m[i].X < xmin)
                    xmin = m[i].X;
                if (m[i].Y < ymin)
                    ymin = m[i].Y;
                if (m[i].X > xmax)
                    xmax = m[i].X;
                if (m[i].Y > ymax)
                    ymax = m[i].Y;
            }
            g.DrawLine(black, xmin, ymin, xmin, ymax);
            g.DrawLine(black, xmin, ymin, xmax, ymin);
            g.DrawLine(black, xmin, ymax, xmax, ymax);
            g.DrawLine(black, xmax, ymin, xmax, ymax);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
