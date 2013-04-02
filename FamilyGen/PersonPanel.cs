using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

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

        private void PersonPanel_MouseDown(object sender, MouseEventArgs e) {
            this.Capture = true;

            while (!(sender is PersonPanel))
                sender = (sender as Control).Parent;

            MainForm.mainForm.StartMovePerson(sender as PersonPanel, e.X, e.Y);
        }

        private void PersonPanel_MouseUp(object sender, MouseEventArgs e) {
            MainForm.mainForm.FinishMovePerson();

            this.Capture = false;
        }

        private void PersonPanel_MouseMove(object sender, MouseEventArgs e) {
            MainForm.mainForm.MovePerson(e.X, e.Y);
        }
    }
}
