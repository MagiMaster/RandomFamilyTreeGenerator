using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FamilyGen {
    public partial class FullInfoForm : Form {
        public FullInfoForm(Person p) {
            InitializeComponent();

            nameLabel.Text = p.fullName;
            ageLabel.Text = p.age.ToString();
            birthLabel.Text = p.birth.ToString();
            deathLabel.Text = (p.death < MainForm.mainForm.year ? p.death.ToString() : "-");
            hairLabel.Text = p.hair;
            eyesLabel.Text = p.eyes;

            detailLabel.MaximumSize = detailLabel.Size;
            detailLabel.Text = p.hobby;
        }

        private void FullInfoForm_Resize(object sender, EventArgs e) {
            detailLabel.MaximumSize = detailLabel.Size;
        }
    }
}
