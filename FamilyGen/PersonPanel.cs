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

        private void personPanel_Click(object sender, EventArgs e) {
            // Generate missing data
            person.FillData(this);
            
            // Open full data panel

            // Center on clicked panel
            MainForm.mainForm.CenterOn(this);
        }
    }
}
