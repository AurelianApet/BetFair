using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;

namespace Nordicbet
{
    public partial class frmStake : Form
    {
        NumberStyles style = NumberStyles.Number | NumberStyles.AllowCurrencySymbol | NumberStyles.AllowDecimalPoint;
        CultureInfo culture = CultureInfo.CreateSpecificCulture("en-GB");
        public double stake { get; set; }
        public frmStake()
        {
            InitializeComponent();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            double stakeValue = 0;
            
            if(string.IsNullOrEmpty(txtStake.Text) || !double.TryParse(txtStake.Text, style, culture, out stakeValue))
            {
                Messagebox.show("Please enter the correct stake!");
                txtStake.Focus();
                return;
            }

            stake = stakeValue;

            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        private void frmStake_Load(object sender, EventArgs e)
        {
            txtStake.Text = stake.ToString();
        }

        private void txtStake_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsDigit(e.KeyChar) || e.KeyChar == Convert.ToChar(Keys.Back) || e.KeyChar == 46 || e.KeyChar == 44))
                e.Handled = true;
        }
    }
}
