using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace sem10
{
    public partial class Form1 : Form
    {

        private List<Point> polygonPoints = new List<Point>();
        private List<(Point, Point)> triangulationLines = new List<(Point, Point)>();
        private bool polygonClosed = false;

        public Form1()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.Text = "Triangularea unui poligon monoton";
            this.BackColor = Color.White;
            this.KeyPreview = true;

            this.Click += Form1_Click;
            this.KeyDown += Form1_KeyDown;
        }

        private void Form1_Click(object sender, EventArgs e)
        {
            if (polygonClosed) return;

            // Obține poziția mouse-ului în coordonatele formularului
            Point mousePoint = this.PointToClient(Control.MousePosition);
            polygonPoints.Add(mousePoint);
            Invalidate();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && polygonPoints.Count >= 3)
            {
                polygonClosed = true;
                Triangulate();
                Invalidate();
            }
            else if (e.KeyCode == Keys.Escape)
            {
                polygonPoints.Clear();
                triangulationLines.Clear();
                polygonClosed = false;
                Invalidate();
            }
        }

        private void Triangulate()
        {
            // Presupunem că poligonul este monoton după axa Y
            var sorted = polygonPoints.OrderByDescending(p => p.Y).ToList();
            Stack<Point> stack = new Stack<Point>();
            stack.Push(sorted[0]);
            stack.Push(sorted[1]);

            for (int i = 2; i < sorted.Count - 1; i++)
            {
                Point current = sorted[i];

                if (IsOnDifferentChain(sorted, stack.Peek(), current))
                {
                    Point last = stack.Pop();
                    while (stack.Count > 0)
                    {
                        triangulationLines.Add((stack.Peek(), current));
                        stack.Pop();
                    }
                    stack.Push(last);
                    stack.Push(current);
                }
                else
                {
                    Point prev = stack.Pop();
                    while (stack.Count > 0 && IsConvex(current, prev, stack.Peek()))
                    {
                        triangulationLines.Add((stack.Peek(), current));
                        prev = stack.Pop();
                    }
                    stack.Push(prev);
                    stack.Push(current);
                }
            }

            Point lastPoint = sorted.Last();
            stack.Pop();
            while (stack.Count > 1)
            {
                triangulationLines.Add((stack.Peek(), lastPoint));
                stack.Pop();
            }
        }

        private bool IsOnDifferentChain(List<Point> sorted, Point a, Point b)
        {
            // Dacă au coordonata X într-o ordine diferită față de vârful de sus și jos, sunt pe lanțuri diferite
            Point top = sorted.First();
            Point bottom = sorted.Last();

            bool isLeftA = a.X < top.X && a.X < bottom.X;
            bool isLeftB = b.X < top.X && b.X < bottom.X;

            return isLeftA != isLeftB;
        }

        private bool IsConvex(Point a, Point b, Point c)
        {
            // Verificăm dacă unghiul este convex (cross product pozitiv)
            int cross = (b.X - a.X) * (c.Y - a.Y) - (b.Y - a.Y) * (c.X - a.X);
            return cross < 0; // pentru coordonate cu origine în colțul stânga-sus
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;

            if (polygonPoints.Count > 1)
            {
                for (int i = 0; i < polygonPoints.Count - 1; i++)
                    g.DrawLine(Pens.Black, polygonPoints[i], polygonPoints[i + 1]);

                if (polygonClosed)
                    g.DrawLine(Pens.Black, polygonPoints.Last(), polygonPoints[0]);
            }

            foreach (var (p1, p2) in triangulationLines)
            {
                g.DrawLine(Pens.Red, p1, p2);
            }
        }

    }
}
