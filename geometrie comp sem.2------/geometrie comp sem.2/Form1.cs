using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace geometrie_comp_sem._2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        

        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Pen p = new Pen(Color.Black, 3);
            Pen p1 = new Pen(Color.Red, 3);
            Pen p2 = new Pen(Color.Green, 3);
            Random random = new Random();
            int n = random.Next(100);
            int d = random.Next(100);
            PointF[] q = new PointF[1];
            q[0].X = random.Next(10, this.ClientSize.Width - 10);
            q[0].Y = random.Next(10, this.ClientSize.Height - 10);
            g.DrawEllipse(p1, q[0].X - 2, q[0].Y - 2, 2, 2);
            PointF[] m = new PointF[n];
            for (int i = 0; i < n; i++)
            {
                m[i].X = random.Next(10, this.ClientSize.Width - 10);
                m[i].Y = random.Next(10, this.ClientSize.Height - 10);
                g.DrawEllipse(p, m[i].X - 2, m[i].Y - 2, 2, 2);
            }
            float dist = 0;
            for (int i = 0; i < n; i++)
            {
                dist = (float)Math.Sqrt(Math.Pow(q[0].X - m[i].X, 2) + Math.Pow(q[0].Y - m[i].Y, 2));
                if(dist<=d)
                g.DrawEllipse(p2, m[i].X - 2, m[i].Y - 2, 2, 2);
            }
        }
    }
}
