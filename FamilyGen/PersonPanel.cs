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

        private bool noClick = false;

        private void PersonPanel_MouseDown(object sender, MouseEventArgs e) {
            while (!(sender is PersonPanel))
                sender = (sender as Control).Parent;

            MainForm.mainForm.StartMovePerson(sender as PersonPanel, e.X, e.Y);

            noClick = false;
        }

        private void PersonPanel_MouseUp(object sender, MouseEventArgs e) {
            MainForm.mainForm.FinishMovePerson();
        }

        private void PersonPanel_MouseMove(object sender, MouseEventArgs e) {
            MainForm.mainForm.MovePerson(e.X, e.Y);

            noClick = true;
        }

        private void nameLabel_Click(object sender, EventArgs e) {
            PersonPanel_Click(sender, e);
        }

        private void PersonPanel_Click(object sender, EventArgs e) {
            if (noClick)
                return;

            while (!(sender is PersonPanel))
                sender = (sender as Control).Parent;

            // Generate missing data
            person.FillData(sender as PersonPanel);

            // Open full data panel
            FullInfoForm f = new FullInfoForm(person);
            f.Show();

            // Center on clicked panel
            MainForm.mainForm.CenterOn(sender as PersonPanel);
        }
    }
}
