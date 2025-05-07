using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp3
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
            Pen pink = new Pen(Color.Pink, 3);
            Pen blue = new Pen(Color.Blue, 3);
            Random r = new Random();
            int n = r.Next(100);
            PointF[] q = new PointF[1];
            PointF[] m = new PointF[n];
            q[0].X = r.Next(10, this.ClientSize.Width - 10);
            q[0].Y = r.Next(10 , this.ClientSize .Height - 10);
            g.DrawEllipse(blue, q[0].X, q[0].Y, 2, 2);
            for (int i = 0; i < n; i++)
            {
                m[i].X = r.Next(10, this.ClientSize.Width - 10);
                m[i].Y = r.Next(this.ClientSize.Width - 10);
                g.DrawEllipse(pink, m[i].X, m[i].Y, 2, 2);
            }
            float dist;
            float dist_min = float.MaxValue;
            for (int i = 0;i < n; i++)
            {
                dist = (float)Math.Sqrt(Math.Pow(q[0].X - m[i].X, 2) + Math.Pow(q[0].Y - m[i].Y, 2));
                if (dist_min > dist)
                    dist_min = dist;
            }
            g.DrawEllipse(red, q[0].X-dist_min, q[0].Y- dist_min, (dist_min - 1)* 2, (dist_min -1)*2);  
        }
    }
}
