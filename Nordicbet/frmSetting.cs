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
    public partial class frmSetting : Form
    {
        NumberStyles style = NumberStyles.Number | NumberStyles.AllowCurrencySymbol | NumberStyles.AllowDecimalPoint;
        CultureInfo culture = CultureInfo.CreateSpecificCulture("en-GB");
        public frmSetting()
        {
            InitializeComponent();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (!canSetting())
                return;

            setSetting();
            
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private bool canSetting()
        {
            if(string.IsNullOrEmpty(txtUsername.Text))
            {
                Messagebox.show("Please enter the username!");
                txtUsername.Focus();
                return false;
            }

            if(string.IsNullOrEmpty(txtPassword.Text))
            {
                Messagebox.show("Please enter the password!");
                txtPassword.Focus();
                return false;
            }

//            if(chkSideOddsMin.Checked && string.IsNullOrEmpty(txtSideOddsMin.Text))
//            {
//                Messagebox.show("Please enter the side odds min!");
//                txtSideOddsMin.Focus();
//                return false;
//            }
//
//            if(chkSideOddsMax.Checked && string.IsNullOrEmpty(txtSideOddsMax.Text))
//            {
//                Messagebox.show("Please enter the side odds max!");
//                txtSideOddsMax.Focus();
//                return false;
//            }

            if(string.IsNullOrEmpty(txtSideDelta.Text))
            {
                Messagebox.show("Please enter the side delta!");
                txtSideDelta.Focus();
                return false;
            }

//            if(cbCurrency.SelectedIndex < 1)
//            {
//                Messagebox.show("Please select the currency!");
//                cbCurrency.Focus();
//                return false;
//            }

            if(chkProgression.Checked && numStep.Value <= 1 && numStep.Value > Settings.Instance.stakes.Count)
            {
                Messagebox.show("Please enter the correct stop step!");
                numStep.Focus();
                return false;
            }

            if(numBetMax.Value < 1)
            {
                Messagebox.show("Please enter the betting max number!");
                numBetMax.Focus();
                return false;
            }
            
            return true;
        }

        private void frmSetting_Load(object sender, EventArgs e)
        {
            initControls();
            initSetting();
        }

        private void initControls()
        {
            lblTitle.Parent = picTitle;
        }

        private void initSetting()
        {
            txtUsername.Text = Settings.Instance.username;
            txtPassword.Text = Settings.Instance.password;
//            chkSideOddsMin.Checked = Settings.Instance.useSideOddsMin;
//            initSideOddsMin(chkSideOddsMin.Checked);
            txtSideOddsMin.Text = Settings.Instance.sideOddsMin.ToString();
//            chkSideOddsMax.Checked = Settings.Instance.useSideOddsMax;
//            initSideOddsMax(chkSideOddsMax.Checked);
            txtSideOddsMax.Text = Settings.Instance.sideOddsMax.ToString();
            txtSideDelta.Text = Settings.Instance.sideDelta.ToString();
            chkProgression.Checked = Settings.Instance.useProgression;
            chkUseFilter.Checked = Settings.Instance.useFilter;
            numStep.Value = Settings.Instance.stopStep;
            txtFlatStake.Text = Settings.Instance.flatStake.ToString();
            numBetMax.Value = Settings.Instance.betMax;
            initStopStep(chkProgression.Checked);

            txtAppKeyDelay.Text = Settings.Instance.delayKey;
            txtAppKeyPlace.Text = Settings.Instance.activeKey;
            txtPercentOfBalance.Text = Settings.Instance.percentBalance.ToString();

            radioUseFixedStake.Checked = Settings.Instance.useFixedStake;
            radioUsePercentage.Checked = Settings.Instance.usePercent;
            if (!Settings.Instance.useFixedStake && !Settings.Instance.usePercent)
                radioUseFixedStake.Checked = true;

            cbCurrency.SelectedIndex = Constants.getCurrencyIndex(Settings.Instance.currency);
        }

        private void setSetting()
        {
            Settings.Instance.username = txtUsername.Text;
            Settings.Instance.password = txtPassword.Text;

            Settings.Instance.useSideOddsMin = chkSideOddsMin.Checked;

            double sideOddsMin = 0;
            if (double.TryParse(txtSideOddsMin.Text, style, culture, out sideOddsMin))
                Settings.Instance.sideOddsMin = sideOddsMin;

            Settings.Instance.useSideOddsMax = chkSideOddsMax.Checked;

            double sideOddsMax = 0;
            if (double.TryParse(txtSideOddsMax.Text, style, culture, out sideOddsMax))
                Settings.Instance.sideOddsMax = sideOddsMax;

            int sideDelta = 0;
            if (int.TryParse(txtSideDelta.Text, style, culture, out sideDelta))
                Settings.Instance.sideDelta = sideDelta;

            Settings.Instance.useProgression = chkProgression.Checked;
            Settings.Instance.useFilter = chkUseFilter.Checked;
            Settings.Instance.stopStep = (int)numStep.Value;

            double flatStake = 0;
            if (double.TryParse(txtFlatStake.Text, style, culture, out flatStake))
                Settings.Instance.flatStake = flatStake;

            Settings.Instance.betMax = (int)numBetMax.Value;

//            Settings.Instance.currency = Constants.currency[cbCurrency.SelectedIndex];

            Settings.Instance.delayKey = txtAppKeyDelay.Text;
            Settings.Instance.activeKey = txtAppKeyPlace.Text;
            double percentOfBalance = 0;
            if (double.TryParse(txtPercentOfBalance.Text, style, culture, out percentOfBalance))
                Settings.Instance.percentBalance = percentOfBalance;
            Settings.Instance.useFixedStake = radioUseFixedStake.Checked;
            Settings.Instance.usePercent = radioUsePercentage.Checked;


        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        private void txtSideOdds_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsDigit(e.KeyChar) || e.KeyChar == Convert.ToChar(Keys.Back) || e.KeyChar == 46 || e.KeyChar == 44))
                e.Handled = true;
        }

        private void txtSideDelta_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsDigit(e.KeyChar) || e.KeyChar == Convert.ToChar(Keys.Back) || e.KeyChar == 46 || e.KeyChar == 44))
                e.Handled = true;
        }

        private void initStopStep(bool bState)
        {
            numStep.Enabled = bState;
            txtFlatStake.Enabled = !bState;
            numBetMax.Enabled = !bState;
        }

        private void initSideOddsMin(bool bState)
        {
            txtSideOddsMin.Enabled = bState;
        }

        private void initSideOddsMax(bool bState)
        {
            txtSideOddsMax.Enabled = bState;
        }

        private void chkSideOddsMin_CheckedChanged(object sender, EventArgs e)
        {
//            initSideOddsMin(chkSideOddsMin.Checked);
        }

        private void chkSideOddsMax_CheckedChanged(object sender, EventArgs e)
        {
//            initSideOddsMax(chkSideOddsMax.Checked);
        }

        private void chkProgression_CheckedChanged(object sender, EventArgs e)
        {
            initStopStep(chkProgression.Checked);
        }
    }
}
