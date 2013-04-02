using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FamilyGen {
    public partial class PersonPanel : UserControl {
        public Person person;
        
        public PersonPanel(Person p) {
            InitializeComponent();
            Dock = DockStyle.None;
            Anchor = AnchorStyles.Left | AnchorStyles.Top;

            person = p;

            UpdateData();
        }

        protected override Point ScrollToControl(Control activeControl) {
            return this.DisplayRectangle.Location;
        }

        public void UpdateData() {
            nameLabel.Text = person.fullName;
        }

        private void PersonPanel_Click(object sender, EventArgs e) {
            // Generate missing data
            person.FillData(this);
            
            // Open full data panel

            // Center on clicked panel
            MainForm.mainForm.CenterOn(this);
        }

        private Nullable<Point> prevPos = null;

        private void PersonPanel_MouseDown(object sender, MouseEventArgs e) {
            prevPos = new Point(e.X, e.Y);
        }

        private void PersonPanel_MouseUp(object sender, MouseEventArgs e) {
            if (prevPos == null)
                return;

            while (!(sender is PersonPanel))
                sender = (sender as Control).Parent;

            MainForm.mainForm.FinishMovePerson(this);

            prevPos = null;
        }

        private void PersonPanel_MouseMove(object sender, MouseEventArgs e) {
            if (prevPos == null)
                return;

            while (!(sender is PersonPanel))
                sender = (sender as Control).Parent;

            Point pos = new Point(e.X, e.Y);

            MainForm.mainForm.MovePerson(sender as PersonPanel, e.X - prevPos.Value.X, e.Y - prevPos.Value.Y);

            prevPos = pos;
        }
    }
}
