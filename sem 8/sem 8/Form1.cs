using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace sem_8
{
    public partial class Form1 : Form
    {
        Graphics g;
        Pen pen = new Pen(Color.Navy);
        List<PointF> points = new List<PointF>();
        bool polygonClosed = false;

        public Form1()
        {
            InitializeComponent(); 
            this.DoubleBuffered = true;
            this.Text = "Triangulare prin octetomie";
            this.Width = 800;
            this.Height = 600;
            g = this.CreateGraphics();
            this.MouseClick += Form1_MouseClick;
        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            if (polygonClosed)
                return;

            points.Add(e.Location);
            int n = points.Count;

            DrawPoint(points[n - 1], n);

            if (n > 1)
                g.DrawLine(pen, points[n - 2], points[n - 1]);

            if (n >= 3 && Distance(points[0], points[n - 1]) < 10)
            {
                g.DrawLine(pen, points[n - 1], points[0]);
                polygonClosed = true;
                Triangulate();
            }
        }

        private void DrawPoint(PointF p, int index)
        {
            g.FillEllipse(Brushes.Navy, p.X - 3, p.Y - 3, 6, 6);
            g.DrawString(index.ToString(), new Font("Arial", 9), Brushes.Black, p.X + 4, p.Y - 12);
        }

        private float Distance(PointF a, PointF b)
        {
            return (float)Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
        }

        private double Sarrus(PointF p1, PointF p2, PointF p3)
        {
            return p1.X * p2.Y + p2.X * p3.Y + p3.X * p1.Y
                 - p1.Y * p2.X - p2.Y * p3.X - p3.Y * p1.X;
        }

        private bool IsDiagonal(List<PointF> poly, int i)
        {
            int n = poly.Count;
            PointF a = poly[i % n];
            PointF b = poly[(i + 2) % n];

            // Check if diagonal intersects any other edge of the polygon
            for (int j = 0; j < n; j++)
            {
                int k = (j + 1) % n;
                if (j == i || k == i || j == (i + 1) % n || k == (i + 1) % n)
                    continue;
                if (DoIntersect(a, b, poly[j], poly[k]))
                    return false;
            }

            // Check orientation to ensure it's an ear
            PointF prev = poly[i % n];
            PointF mid = poly[(i + 1) % n];
            PointF next = poly[(i + 2) % n];

            return Sarrus(prev, mid, next) < 0 && IsInsidePolygon(a, b, poly);
        }

        private bool DoIntersect(PointF a, PointF b, PointF c, PointF d)
        {
            float o1 = Orientation(a, b, c);
            float o2 = Orientation(a, b, d);
            float o3 = Orientation(c, d, a);
            float o4 = Orientation(c, d, b);

            return o1 * o2 < 0 && o3 * o4 < 0;
        }

        private float Orientation(PointF a, PointF b, PointF c)
        {
            return (b.X - a.X) * (c.Y - a.Y) - (b.Y - a.Y) * (c.X - a.X);
        }

        private bool IsInsidePolygon(PointF a, PointF b, List<PointF> poly)
        {
            // Check if the diagonal (a, b) is inside the polygon
            int n = poly.Count;
            int intersections = 0;

            for (int i = 0; i < n; i++)
            {
                PointF p1 = poly[i];
                PointF p2 = poly[(i + 1) % n];
                if (DoIntersect(a, b, p1, p2))
                {
                    intersections++;
                }
            }

            // The diagonal is inside if it doesn't cross any edges
            return intersections == 0;
        }

        private void Triangulate()
        {
            List<PointF> poly = new List<PointF>(points);
            Pen redPen = new Pen(Color.Red) { DashPattern = new float[] { 5, 3 } };

            while (poly.Count > 3)
            {
                int n = poly.Count;
                bool earFound = false;

                for (int i = 0; i < n; i++)
                {
                    if (IsDiagonal(poly, i))
                    {
                        PointF a = poly[i % n];
                        PointF b = poly[(i + 2) % n];
                        g.DrawLine(redPen, a, b);
                        poly.RemoveAt((i + 1) % n);
                        earFound = true;
                        break;
                    }
                }

                if (!earFound)
                    break; // No ears found, possibly non-simple polygon
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}