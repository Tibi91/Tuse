using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace sem11
{
    public partial class Form1 : Form
    {
        private List<PointF> vertices = new List<PointF>();
        private List<Tuple<int, int>> diagonals = new List<Tuple<int, int>>();
        private Graphics g;
        private Pen pen = new Pen(Color.Black);
        private const int RADIUS = 3;
        private bool polygonClosed = false;
        public Form1()
        {
            InitializeComponent();
            g = this.CreateGraphics();
        }

        private void Form1_Click(object sender, EventArgs e)
        {
            if (polygonClosed) return;

            PointF point = this.PointToClient(Cursor.Position);
            vertices.Add(point);
            g.DrawEllipse(pen, point.X - RADIUS, point.Y - RADIUS, RADIUS * 2, RADIUS * 2);
            g.DrawString(vertices.Count.ToString(), new Font(FontFamily.GenericSansSerif, 10), Brushes.Black, point.X + RADIUS, point.Y - RADIUS);

            if (vertices.Count > 1)
            {
                g.DrawLine(pen, vertices[vertices.Count - 2], vertices[vertices.Count - 1]);
            }
        }

        private void btnClosePolygon_Click(object sender, EventArgs e)
        {
            if (vertices.Count < 3) return;
            g.DrawLine(pen, vertices[vertices.Count - 1], vertices[0]);
            polygonClosed = true;
        }

        private void btnTriangulate_Click(object sender, EventArgs e)
        {
            if (vertices.Count < 3 || !polygonClosed) return;
            Triangulate();

            Pen diagPen = new Pen(Color.Red) { DashPattern = new float[] { 5, 3 } };
            foreach (var d in diagonals)
            {
                g.DrawLine(diagPen, vertices[d.Item1], vertices[d.Item2]);
            }
        }

        private void btnEliminaDiagonale_Click(object sender, EventArgs e)
        {
            EliminaDiagonaleNeesentiale();
            Refresh();
            g = this.CreateGraphics();
            Pen diagPen = new Pen(Color.Green, 2);
            foreach (var d in diagonals)
            {
                g.DrawLine(diagPen, vertices[d.Item1], vertices[d.Item2]);
            }
        }

        private double Sarrus(PointF p1, PointF p2, PointF p3)
        {
            return p1.X * p2.Y + p2.X * p3.Y + p3.X * p1.X - p3.X * p2.Y - p2.X * p1.Y - p1.X * p3.Y;
        }

        private bool EsteConvex(PointF a, PointF b, PointF c)
        {
            return Sarrus(a, b, c) < 0;
        }

        private bool SeIntersecteaza(PointF a, PointF b, PointF c, PointF d)
        {
            double o1 = Sarrus(a, b, c);
            double o2 = Sarrus(a, b, d);
            double o3 = Sarrus(c, d, a);
            double o4 = Sarrus(c, d, b);
            return o1 * o2 < 0 && o3 * o4 < 0;
        }
        private void Triangulate()
        {
            int n = vertices.Count;
            diagonals.Clear();

            for (int i = 0; i < n - 2; i++)
            {
                for (int j = i + 2; j < n; j++)
                {
                    if (i == 0 && j == n - 1) continue;

                    bool intersect = false;
                    for (int k = 0; k < n; k++)
                    {
                        int k1 = k;
                        int k2 = (k + 1) % n;
                        if (i == k1 || i == k2 || j == k1 || j == k2) continue;
                        if (SeIntersecteaza(vertices[i], vertices[j], vertices[k1], vertices[k2]))
                        {
                            intersect = true;
                            break;
                        }
                    }
                    if (!intersect)
                    {
                        diagonals.Add(new Tuple<int, int>(i, j));
                        if (diagonals.Count == n - 3) return;
                    }
                }
            }
        }

        private void EliminaDiagonaleNeesentiale()
        {
            List<Tuple<int, int>> esentiale = new List<Tuple<int, int>>();
            foreach (var d in diagonals)
            {
                int i = d.Item1;
                int j = d.Item2;
                int prev = (i - 1 + vertices.Count) % vertices.Count;
                int next = (j + 1) % vertices.Count;

                if (!EsteConvex(vertices[prev], vertices[i], vertices[j]) ||
                    !EsteConvex(vertices[i], vertices[j], vertices[next]))
                {
                    esentiale.Add(d);
                }
            }
            diagonals = esentiale;
        }
    }
}
