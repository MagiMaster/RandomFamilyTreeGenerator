using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace FamilyGen {
    public partial class MainForm : Form {
        public static MainForm mainForm;

        public int year = DateTime.Now.Year;

        public int minX=0, maxX=0, minY=0, maxY=0;

        public List<PersonPanel> ppl = new List<PersonPanel>();

        public Panel panel;

        public int AddPerson(Person p, int x, int y) {
            int n = ppl.Count;

            PersonPanel pp = new PersonPanel(p);
            ppl.Add(pp);
            pp.Location = new Point(x, y);
            Controls.Add(pp);

            ExpandScrollbars(x - 32, y - 32, x + pp.Width + 32, y + pp.Height + 32);
            Invalidate();

            return n;
        }

        private void ExpandScrollbars(int x1, int y1, int x2, int y2) {
            minX = Math.Min(minX, x1);
            maxX = Math.Max(maxX, x2);
            minY = Math.Min(minY, y1);
            maxY = Math.Max(maxY, y2);

            int dx = Math.Max(-minX, 0);
            int dy = Math.Max(-minY, 0);

            if (dx > 0 || dy > 0) {
                minX += dx;
                minY += dy;
                maxX += dx;
                maxY += dy;
            }

            AutoScrollMinSize = new System.Drawing.Size(maxX, maxY);

            if (dx > 0 || dy > 0) {
                foreach (Control c in Controls)
                    c.Location = new Point(c.Location.X + dx, c.Location.Y + dy);

                AutoScrollPosition = new Point(dx - AutoScrollPosition.X, dy - AutoScrollPosition.Y);
            }
        }

        public void CenterOn(PersonPanel pp) {
            int x = pp.Location.X - AutoScrollPosition.X;
            int y = pp.Location.Y - AutoScrollPosition.Y;

            AutoScrollPosition = new Point(x - (ClientRectangle.Right - pp.Width) / 2, y - (ClientRectangle.Bottom - pp.Height) / 2);
        }

        public MainForm() {
            InitializeComponent();
            mainForm = this;

            ExpandScrollbars(ClientRectangle.Left, ClientRectangle.Top, ClientRectangle.Right, ClientRectangle.Bottom);

            //TODO Don't hardcode PersonPanel sizes.
            AddPerson(Person.GeneratePerson(), (ClientRectangle.Right - 128) / 2, (ClientRectangle.Bottom - 40) / 2);

            Timer t = new Timer();
            t.Interval = 50;
            t.Tick += new EventHandler(timerTick);
            t.Start();
        }

        private Nullable<Point> prevPos = null;

        private void MainForm_MouseDown(object sender, MouseEventArgs e) {
            prevPos = new Point(e.X, e.Y);
        }

        private void MainForm_MouseUp(object sender, MouseEventArgs e) {
            prevPos = null;
        }

        private void MainForm_MouseMove(object sender, MouseEventArgs e) {
            if (prevPos == null)
                return;

            Point pos = new Point(e.X, e.Y);

            Point dif = new Point(prevPos.Value.X - pos.X, prevPos.Value.Y - pos.Y);

            AutoScrollPosition = new Point(dif.X - AutoScrollPosition.X, dif.Y - AutoScrollPosition.Y);

            prevPos = pos;
        }

        private Point dragOrigin;
        private PersonPanel dragPanel;

        public void StartMovePerson(PersonPanel pp, int ox, int oy) {
            dragOrigin = new Point(ox, oy); //(pp.Location.X - ox, pp.Location.Y - oy);
            dragPanel = pp;
        }

        public void MovePerson(int mx, int my) {
            if (dragPanel == null)
                return;

            dragPanel.Location = new Point(dragPanel.Location.X + mx - dragOrigin.X, dragPanel.Location.Y + my - dragOrigin.Y);
            Invalidate();
        }

        public void FinishMovePerson() {
            if (dragPanel == null)
                return;

            ExpandScrollbars(dragPanel.Location.X, dragPanel.Location.Y, dragPanel.Location.X + dragPanel.Width, dragPanel.Location.Y + dragPanel.Height);
            CenterOn(dragPanel);

            dragPanel = null;
        }

        protected override void OnPaint(PaintEventArgs e) {

            Graphics g = e.Graphics;
            g.Clear(MainForm.DefaultBackColor);

            Pen dash = new Pen(Color.Black);
            dash.DashPattern = new float[]{10, 10};

            //TODO Don't hardcode PersonPanel sizes.
            int ox = 128 / 2;
            int oy = 40 / 2;

            foreach (PersonPanel pp in ppl) {
                if (pp.person.isMale && pp.person.spouse != null) {
                    Point pa = pp.Location;
                    Point pb = pp.person.spouse.panel.Location;
                    Point p1 = new Point(pa.X + ox, pa.Y + oy);
                    Point p2 = new Point(pb.X + ox, pb.Y + oy);
                    Point p3 = new Point((p1.X + p2.X) / 2, (p1.Y + p2.Y) / 2);

                    g.DrawLine(dash, p1, p2);

                    foreach (Person c in pp.person.children) {
                        if (c == null)
                            continue;

                        Point pc = c.panel.Location;
                        Point p4 = new Point(pc.X + ox, pc.Y + oy);

                        g.DrawLine(Pens.Black, p3, p4);
                    }
                }
            }

            base.OnPaint(e);
        }

        private int slice = 0;
        public void timerTick(object sender, EventArgs e) {
            if(ppl.Count == 0 || dragPanel != null)
                return;

            Point pa = ppl[slice].Location;

            bool moved = false;
            for (int i = 0; i < ppl.Count; ++i) {
                if (i == slice)
                    continue;

                Point pb = ppl[i].Location;

                int dx = pa.X - pb.X;
                int dy = 4 * (pa.Y - pb.Y);
                double dd = 300.0 / (dx * dx + dy * dy + 0.01);

                Debug.Write(dx.ToString() + " " + dy.ToString() + "   " + dd.ToString() + "\n");

                dx = (int)(dx * dd);
                dy = (int)(dy * dd);

                if (dx != 0 || dy != 0) {
                    moved = true;
                    ppl[i].Location = new Point(pb.X - dx, pb.Y - dy);
                }
            }

            slice = (slice + 1) % ppl.Count;
            if(moved)
                Invalidate();
        }
    }
}
