using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Nordicbet
{
    public partial class frmAlert : Form
    {
        public string content { get; set; }
        public frmAlert()
        {
            InitializeComponent();
        }

        private void initControls()
        {
            lblTitle.Parent = picTitle;
            btnClose.Parent = picTitle;
        }

        private void initValues()
        {
            if (!string.IsNullOrEmpty(content))
                lblError.Text = content;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmAlert_Load(object sender, EventArgs e)
        {
            initControls();
            initValues();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void lblTitle_Click(object sender, EventArgs e)
        {

        }
    }
}
