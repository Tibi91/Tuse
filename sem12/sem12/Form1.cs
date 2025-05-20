using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace sem12
{
    public partial class Form1 : Form
    {
        private List<PointF> puncte = new List<PointF>();

        public Form1()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.Text = "Diagrama Voronoi – Triunghi din 3 puncte";
            this.ClientSize = new Size(800, 600);
            this.MouseClick += Form1_Click;
        }

        private void Form1_Click(object sender, EventArgs e)
        {
            if (puncte.Count < 3)
            {
                // Transformare din coordonate absolute (ecran) în coordonate client (formă)
                Point p = this.PointToClient(Cursor.Position);
                puncte.Add(p);
                Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            Pen penTriunghi = new Pen(Color.Blue, 2);
            Pen penMediatoare = new Pen(Color.Green, 1) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dash };
            Brush brushCentru = Brushes.Red;

            if (puncte.Count == 3)
            {
                // Desenează triunghiul
                g.DrawPolygon(penTriunghi, puncte.ToArray());

                // Calculează centrul cercului circumscris
                PointF A = puncte[0];
                PointF B = puncte[1];
                PointF C = puncte[2];
                PointF centru = GetCircumcenter(A, B, C);

                // Desenează centrul
                g.FillEllipse(brushCentru, centru.X - 4, centru.Y - 4, 8, 8);

                // Desenează cercul circumscris
                float raza = Distance(centru, A);
                g.DrawEllipse(Pens.Red, centru.X - raza, centru.Y - raza, 2 * raza, 2 * raza);

                // (Opțional) Desenează mediatoarele
                DrawPerpendicularBisector(g, A, B, penMediatoare);
                DrawPerpendicularBisector(g, B, C, penMediatoare);
                DrawPerpendicularBisector(g, C, A, penMediatoare);

                // Desenează punctele
                foreach (var p in puncte)
                {
                    g.FillEllipse(Brushes.Black, p.X - 3, p.Y - 3, 6, 6);
                }
            }
        }

        private PointF GetCircumcenter(PointF A, PointF B, PointF C)
        {
            float D = 2 * (A.X * (B.Y - C.Y) +
                           B.X * (C.Y - A.Y) +
                           C.X * (A.Y - B.Y));

            float Ux = ((A.X * A.X + A.Y * A.Y) * (B.Y - C.Y) +
                        (B.X * B.X + B.Y * B.Y) * (C.Y - A.Y) +
                        (C.X * C.X + C.Y * C.Y) * (A.Y - B.Y)) / D;

            float Uy = ((A.X * A.X + A.Y * A.Y) * (C.X - B.X) +
                        (B.X * B.X + B.Y * B.Y) * (A.X - C.X) +
                        (C.X * C.X + C.Y * C.Y) * (B.X - A.X)) / D;

            return new PointF(Ux, Uy);
        }

        private float Distance(PointF p1, PointF p2)
        {
            float dx = p1.X - p2.X;
            float dy = p1.Y - p2.Y;
            return (float)Math.Sqrt(dx * dx + dy * dy);
        }

        private void DrawPerpendicularBisector(Graphics g, PointF p1, PointF p2, Pen pen)
        {
            PointF mid = new PointF((p1.X + p2.X) / 2, (p1.Y + p2.Y) / 2);
            float dx = p2.X - p1.X;
            float dy = p2.Y - p1.Y;

            if (dx == 0)
            {
                g.DrawLine(pen, mid.X - 1000, mid.Y, mid.X + 1000, mid.Y);
            }
            else if (dy == 0)
            {
                g.DrawLine(pen, mid.X, mid.Y - 1000, mid.X, mid.Y + 1000);
            }
            else
            {
                float slope = -dx / dy;
                float x1 = mid.X - 500;
                float y1 = mid.Y - slope * 500;
                float x2 = mid.X + 500;
                float y2 = mid.Y + slope * 500;
                g.DrawLine(pen, x1, y1, x2, y2);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            puncte.Clear();
            Invalidate(); // redesenează forma fără puncte
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
