using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
// merge aproape perfect
namespace sem_6_pb_1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            //desenam punctele
            Graphics g = e.Graphics;
            Pen linePen = new Pen(Color.Black, 2);
            Pen p1 = new Pen(Color.Red, 2);
            Random random = new Random();
            int n = random.Next(3,100); 
            List<PointF> points = new List<PointF>();

            for (int i = 0; i < n; i++)
            {
                float x = random.Next(10, this.ClientSize.Width - 10);
                float y = random.Next(10, this.ClientSize.Height - 10);
                points.Add(new PointF(x, y));
                g.DrawEllipse (p1, x - 2, y - 2, 4, 4);
            }

            List<(PointF A, PointF B, PointF C)> triangles = PerformDelaunayTriangulation(points);
            AfisareTriunghiuri(g, linePen, triangles);
        }

        private List<(PointF A, PointF B, PointF C)> PerformDelaunayTriangulation(List<PointF> points)
        {
            List<(PointF A, PointF B, PointF C)> triangles = new List<(PointF, PointF, PointF)>();
            //initializeaza lista de triunghiuri
            points = points.OrderBy(p => p.X).ThenBy(p => p.Y).ToList();
            //ordoneaza punctele  

            for (int i = 0; i < points.Count - 2; i++)
            {
                for (int j = i + 1; j < points.Count - 1; j++)
                {
                    for (int k = j + 1; k < points.Count; k++)
                    {
                        if (TriunghiBool(points[i], points[j], points[k], points))
                        {
                            triangles.Add((points[i], points[j], points[k]));
                        }
                    }
                }
            }
            //adauga triunghiurile valide in lista
            return triangles;
        }

        private bool TriunghiBool(PointF a, PointF b, PointF c, List<PointF> points)
        {
            foreach (var p in points)
            {
                if (p != a && p != b && p != c && CercBool(p, a, b, c))
                {
                    return false;
                }
            }
            return true;
        }
        //verifica triunghiurile

        private bool CercBool(PointF p, PointF a, PointF b, PointF c)
        {
            float ax = a.X - p.X, ay = a.Y - p.Y;
            float bx = b.X - p.X, by = b.Y - p.Y;
            float cx = c.X - p.X, cy = c.Y - p.Y;
            float det = (ax * ax + ay * ay) * (bx * cy - by * cx) -
                        (bx * bx + by * by) * (ax * cy - ay * cx) +
                        (cx * cx + cy * cy) * (ax * by - ay * bx);
            return det > 0;
        }
        //verifica daca exista un punct in triunghi

        private void AfisareTriunghiuri(Graphics g, Pen pen, List<(PointF A, PointF B, PointF C)> triangles)
        {
            foreach (var triangle in triangles)
            {
                g.DrawLine(pen, triangle.A, triangle.B);
                g.DrawLine(pen, triangle.B, triangle.C);
                g.DrawLine(pen, triangle.C, triangle.A);
            }
           //nu stiu daca ati observat sau nu dar acolo scrie clar "AfisareTriunghiuri" care ce inseamna?da,exact!!!! afiseaza triunghiurile:)
        }
    }
}
/*
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace sem_6_pb_1
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
            Pen pointPen = new Pen(Color.Magenta, 4);
            Pen linePen = new Pen(Color.Black, 2);
            Random random = new Random();
            int n = random.Next(10, 20);
            List<PointF> points = new List<PointF>();

            for (int i = 0; i < n; i++)
            {
                float x = random.Next(10, this.ClientSize.Width - 10);
                float y = random.Next(10, this.ClientSize.Height - 10);
                points.Add(new PointF(x, y));
                g.FillEllipse(new SolidBrush(Color.Magenta), x - 2, y - 2, 4, 4);
            }

            List<(PointF A, PointF B, PointF C)> triangles = ComputeTriangulation(points);
            DrawTriangles(g, linePen, triangles);
        }

        private List<(PointF A, PointF B, PointF C)> ComputeTriangulation(List<PointF> points)
        {
            List<(PointF A, PointF B, PointF C)> triangles = new List<(PointF, PointF, PointF)>();
            if (points.Count < 3) return triangles;

            List<(PointF A, PointF B)> mstEdges = ComputeMinimumSpanningTree(points);
            HashSet<PointF> visited = new HashSet<PointF>(points);

            foreach (var edge in mstEdges)
            {
                PointF closest = points
                    .Where(p => p != edge.A && p != edge.B)
                    .OrderBy(p => DistanceToSegment(p, edge.A, edge.B))
                    .FirstOrDefault();

                if (closest != PointF.Empty && visited.Contains(closest))
                {
                    triangles.Add((edge.A, edge.B, closest));
                }
            }

            return triangles;
        }

        private float DistanceToSegment(PointF p, PointF a, PointF b)
        {
            float lengthSq = Distance(a, b) * Distance(a, b);
            if (lengthSq == 0) return Distance(p, a);

            float t = Math.Max(0, Math.Min(1, ((p.X - a.X) * (b.X - a.X) + (p.Y - a.Y) * (b.Y - a.Y)) / lengthSq));
            PointF projection = new PointF(a.X + t * (b.X - a.X), a.Y + t * (b.Y - a.Y));
            return Distance(p, projection);
        }

        private List<(PointF A, PointF B)> ComputeMinimumSpanningTree(List<PointF> points)
        {
            List<(PointF A, PointF B)> mstEdges = new List<(PointF, PointF)>();
            if (points.Count < 2) return mstEdges;

            HashSet<PointF> visited = new HashSet<PointF>();
            visited.Add(points[0]);

            while (visited.Count < points.Count)
            {
                (PointF A, PointF B, float Length) minEdge = (new PointF(), new PointF(), float.MaxValue);
                foreach (var p1 in visited)
                {
                    foreach (var p2 in points.Except(visited))
                    {
                        float dist = Distance(p1, p2);
                        if (dist < minEdge.Length)
                        {
                            minEdge = (p1, p2, dist);
                        }
                    }
                }
                mstEdges.Add((minEdge.A, minEdge.B));
                visited.Add(minEdge.B);
            }
            return mstEdges;
        }

        private float Distance(PointF p1, PointF p2)
        {
            return (float)Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
        }

        private void DrawTriangles(Graphics g, Pen pen, List<(PointF A, PointF B, PointF C)> triangles)
        {
            foreach (var triangle in triangles)
            {
                g.DrawLine(pen, triangle.A, triangle.B);
                g.DrawLine(pen, triangle.B, triangle.C);
                g.DrawLine(pen, triangle.C, triangle.A);
            }
        }
    }
}
*/
