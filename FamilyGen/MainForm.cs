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

            ExpandScrollbars(x, y, x + pp.Width, y + pp.Height);

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
        }

        public void FinishMovePerson() {
            ExpandScrollbars(dragPanel.Location.X, dragPanel.Location.Y, dragPanel.Location.X + dragPanel.Width, dragPanel.Location.Y + dragPanel.Height);
            CenterOn(dragPanel);

            dragPanel = null;
        }
    }
}
