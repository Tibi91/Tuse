using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace sem5._2
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
            Pen p1 = new Pen(Color.Black, 3);  // Pix pentru puncte
            Pen p2 = new Pen(Color.Red, 2);    // Pix pentru contur
            Random random = new Random();

            int n = random.Next(10, 20);  // Se alege un numar aleator de puncte intre 10 si 20
            PointF[] m = new PointF[n];   // Vector care va contine punctele generate

            // Generam puncte aleatoare 
            for (int i = 0; i < n; i++)
            {
                m[i] = new PointF(
                    random.Next(10, this.ClientSize.Width - 10),   
                    random.Next(10, this.ClientSize.Height - 10)   
                );
                g.DrawEllipse(p1, m[i].X - 1, m[i].Y - 1, 2, 2);    // Deseneaza punctul
            }

            // Alegem punctul cel mai jos (sau cel mai la stanga in caz de egalitate) ca punct de start
            PointF start = m[0];
            for (int i = 1; i < n; i++)
            {
                if (m[i].Y < start.Y || (m[i].Y == start.Y && m[i].X < start.X))
                {
                    start = m[i];
                }
            }

            // Implementam algoritmul Jarvis March pentru invelitoarea convexa
            List<PointF> hull = new List<PointF>();  // Lista punctelor care formeaza invelitoarea convexa
            PointF current = start;

            do
            {
                hull.Add(current);  // Adaugam punctul curent in hull
                PointF next = m[0]; // Initial presupunem ca urmatorul punct e primul

                for (int i = 0; i < n; i++)
                {
                    if (m[i] == current) continue;  // Sarim peste punctul curent

                    float cross = CrossProduct(current, next, m[i]);  // Calculam produsul vectorial
                    // Daca m[i] este mai la stanga decât "next", sau este coliniar dar mai departe
                    if (cross > 0 || (cross == 0 && Distance(current, m[i]) > Distance(current, next)))
                    {
                        next = m[i];  // m[i] devine noul candidat pentru următorul punct
                    }
                }

                current = next;  // Trecem la urmatorul punct
            } while (current != start);  // Terminam cand ne intoarcem la punctul de start

            // Desenam liniile dintre punctele din convex hull
            for (int i = 0; i < hull.Count - 1; i++)
            {
                g.DrawLine(p2, hull[i], hull[i + 1]);  // Linia dintre doua puncte consecutive
            }
            g.DrawLine(p2, hull[hull.Count - 1], hull[0]);  // Inchidem conturul
        }

        // Functie pentru a calcula produsul vectorial dintre 3 puncte
        private float CrossProduct(PointF a, PointF b, PointF c)
        {
            return (b.X - a.X) * (c.Y - a.Y) - (b.Y - a.Y) * (c.X - a.X);
        }

        // Functie pentru a calcula patratul distantei dintre doua puncte
        private float Distance(PointF a, PointF b)
        {
            return (b.X - a.X) * (b.X - a.X) + (b.Y - a.Y) * (b.Y - a.Y);
        }
    }
}
