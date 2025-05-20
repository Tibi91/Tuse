using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace sem11
{
    public partial class Form1 : Form
    {
        Graphics g;
        Pen pen = new Pen(Color.Navy);
        const int raza = 3;
        int n = 0;
        List<PointF> p = new List<PointF>();
        bool poligon_inchis = false;
        Tuple<int, int>[] diagonale;
        int nr_diagonale = 0;
        public Form1()
        {
            InitializeComponent();
            g = this.CreateGraphics();
        }

        private void Form1_Click(object sender, EventArgs e)
        {
            p.Add(this.PointToClient(new Point(Form1.MousePosition.X, Form1.MousePosition.Y)));
            g.DrawEllipse(pen, p[n].X, p[n].Y, raza, raza);
            g.DrawString((n + 1).ToString(), new Font(FontFamily.GenericSansSerif, 10), new SolidBrush(Color.Navy), p[n].X + raza, p[n].Y - raza);
            if (n > 0)
                g.DrawLine(pen, p[n - 1], p[n]);
            n++;
        }

        private double Sarrus(PointF p1, PointF p2, PointF p3)
        {
            return p1.X * p2.Y + p2.X * p3.Y + p3.X * p1.Y - p3.X * p2.Y - p2.X * p1.Y - p1.X * p3.Y;
        }

        private bool intoarcere_spre_stanga(int p1, int p2, int p3) => Sarrus(p[p1], p[p2], p[p3]) < 0;
        private bool intoarcere_spre_dreapta(int p1, int p2, int p3) => Sarrus(p[p1], p[p2], p[p3]) > 0;

        private bool este_varf_convex(int pi)
        {
            int ant = (pi > 0) ? pi - 1 : n - 1;
            int urm = (pi < n - 1) ? pi + 1 : 0;
            return intoarcere_spre_dreapta(ant, pi, urm);
        }

        private bool este_varf_reflex(int pi)
        {
            int ant = (pi > 0) ? pi - 1 : n - 1;
            int urm = (pi < n - 1) ? pi + 1 : 0;
            return intoarcere_spre_stanga(ant, pi, urm);
        }

        private bool se_intersecteaza(PointF a, PointF b, PointF c, PointF d)
        {
            return Sarrus(d, c, a) * Sarrus(d, c, b) <= 0 && Sarrus(b, a, c) * Sarrus(b, a, d) <= 0;
        }

        private bool se_afla_in_interiorul_poligonului(int i, int j)
        {
            int ant = (i > 0) ? i - 1 : n - 1;
            int urm = (i < n - 1) ? i + 1 : 0;
            return (este_varf_convex(i) && intoarcere_spre_stanga(i, j, urm) && intoarcere_spre_stanga(i, ant, j)) ||
                   (este_varf_reflex(i) && !(intoarcere_spre_dreapta(i, j, urm) && intoarcere_spre_dreapta(i, ant, j)));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (n < 3) return;
            g.DrawLine(pen, p[n - 1], p[0]);
            poligon_inchis = true;
        }

        private bool FormeazaPatrulaterConvex(Tuple<int, int> diagonala)
        {
            // funcție auxiliară: întoarcerea convexității pentru un patrulater
            // trebuie implementat detaliat pe baza triunghiurilor care au diagonala în comun

            // schemă: determină 4 puncte A, B, C, D (unite două triunghiuri vecine)
            // returnează true dacă toate întoarcerile sunt "spre dreapta" sau toate "spre stânga"
            return true; // placeholder
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (n <= 3) return;
            if (!poligon_inchis) button1_Click(sender, e);

            diagonale = new Tuple<int, int>[n - 3];
            nr_diagonale = 0;
            pen = new Pen(Color.Red);
            pen.DashPattern = new float[] { 1, 2, 3, 4 };

            for (int i = 0; i < n - 2; i++)
                for (int j = i + 2; j < n; j++)
                {
                    if (i == 0 && j == n - 1) continue;
                    bool inter = false;
                    for (int k = 0; k < n - 1; k++)
                        if (i != k && i != (k + 1) && j != k && j != (k + 1) && se_intersecteaza(p[i], p[j], p[k], p[k + 1]))
                        { inter = true; break; }
                    if (!inter && se_intersecteaza(p[i], p[j], p[n - 1], p[0]) && i != n - 1 && i != 0 && j != n - 1 && j != 0)
                        inter = true;
                    if (!inter)
                    {
                        for (int k = 0; k < nr_diagonale; k++)
                            if (i != diagonale[k].Item1 && i != diagonale[k].Item2 && j != diagonale[k].Item1 && j != diagonale[k].Item2 &&
                                se_intersecteaza(p[i], p[j], p[diagonale[k].Item1], p[diagonale[k].Item2]))
                            { inter = true; break; }
                        if (!inter && se_afla_in_interiorul_poligonului(i, j))
                        {
                            Thread.Sleep(100);
                            g.DrawLine(pen, p[i], p[j]);
                            diagonale[nr_diagonale++] = Tuple.Create(i, j);
                        }
                    }
                    if (nr_diagonale == n - 3) return;
                }
        }

        private void button3_Click(object sender, EventArgs e) => Application.Exit();

        private bool EsteConvex(List<PointF> poligon)
        {
            bool sens = false;
            int nr = poligon.Count;
            for (int i = 0; i < nr; i++)
            {
                PointF a = poligon[i], b = poligon[(i + 1) % nr], c = poligon[(i + 2) % nr];
                double det = Sarrus(a, b, c);
                if (i == 0) sens = det > 0;
                else if ((det > 0) != sens) return false;
            }
            return true;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (diagonale == null || diagonale.Length == 0)
            {
                MessageBox.Show("Triangulează mai întâi poligonul.");
                return;
            }

            Pen penVerde = new Pen(Color.Green, 2);
            List<Tuple<int, int>> esentiale = new List<Tuple<int, int>>();

            for (int i = 0; i < nr_diagonale; i++)
            {
                var d = diagonale[i];
                int a = d.Item1, b = d.Item2;
                int a_ant = (a - 1 + n) % n;
                int b_urm = (b + 1) % n;

                List<PointF> patrulater = new List<PointF> { p[a_ant], p[a], p[b], p[b_urm] };

                if (!EsteConvex(patrulater))
                    esentiale.Add(d);
            }

            g.Clear(this.BackColor);
            Pen penPol = new Pen(Color.Navy);
            for (int i = 0; i < n; i++)
            {
                g.DrawEllipse(penPol, p[i].X, p[i].Y, 3, 3);
                g.DrawString((i + 1).ToString(), new Font(FontFamily.GenericSansSerif, 10), new SolidBrush(Color.Navy), p[i].X + 3, p[i].Y - 3);
                g.DrawLine(penPol, p[i], p[(i + 1) % n]);
            }

            foreach (var diag in esentiale)
                g.DrawLine(penVerde, p[diag.Item1], p[diag.Item2]);
        }
    }
}
