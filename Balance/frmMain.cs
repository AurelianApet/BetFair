using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using Microsoft.Win32;

namespace Balance
{
    public partial class frmMain : Form
    {
        private bool _bRun = false;
        private Thread thread = null;
        private Point _ptPrevPoint = new Point(0, 0);
        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            initControls();

            loadSettingInfo();
        }

        private void initControls()
        {
            lblTitle.Parent = picTitle;
            btnClose.Parent = picTitle;
        }

        private bool canStart()
        {
            if(_bRun)
            {
                MessageBox.Show("Please stop the software first!");
                return false;
            }

            if(string.IsNullOrEmpty(txtUsername.Text))
            {
                MessageBox.Show("Please enter the username!");
                txtUsername.Focus();
                return false;
            }

            Settings.Instance.username = txtUsername.Text;

            if(string.IsNullOrEmpty(txtPassword.Text))
            {
                MessageBox.Show("Please enter the password!");
                txtPassword.Focus();
                return false;
            }

            Settings.Instance.password = txtPassword.Text;
            
            Settings.Instance.domain = "www.betfair.com";
            Settings.Instance.delay = (int)numDelay.Value;

            return true;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (!canStart())
                return;

            _bRun = true;

            saveSettingInfo();
            refreshControls(false);

            thread = new Thread(funcThread);
            thread.Start();
        }

        private void funcThread()
        {
            while(_bRun)
            {
                Sbobet task = new Sbobet();

                bool loginResult = task.doLogin().Result;
                if (!loginResult)
                    continue;

//                setLoginResult(loginResult);

                Thread.Sleep(Settings.Instance.delay * 1000 * 60);
            }
        }

        private void setLoginResult(LoginResult loginResult)
        {
            this.BeginInvoke(new Action(() =>
            {
                lblBalance.Text = string.Format("{0} {1:N2}", loginResult.currency, loginResult.balance);
            }));
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            progressStop();
        }

        private void progressStop()
        {
            _bRun = false;
            refreshControls(true);

            try
            {
                if (thread != null)
                    thread.Abort();
            }
            catch (Exception)
            {

            }
        }

        private string ReadRegistry(string KeyName)
        {
            return Registry.CurrentUser.CreateSubKey("SoftWare").CreateSubKey("Balance").GetValue(KeyName, (object)"").ToString();
        }

        private void WriteRegistry(string KeyName, string KeyValue)
        {
            Registry.CurrentUser.CreateSubKey("SoftWare").CreateSubKey("Balance").SetValue(KeyName, (object)KeyValue);
        }

        private void saveSettingInfo()
        {
            WriteRegistry("username", Settings.Instance.username);
            WriteRegistry("password", Settings.Instance.password);
            WriteRegistry("domain", Settings.Instance.domain);
            WriteRegistry("delay", Settings.Instance.delay.ToString());
        }

        private void loadSettingInfo()
        {
            Settings.Instance.username = ReadRegistry("username");
            Settings.Instance.password = ReadRegistry("password");
            Settings.Instance.domain = ReadRegistry("domain");

            int delay = 0;
            if (int.TryParse(ReadRegistry("delay"), out delay))
                Settings.Instance.delay = delay;

            txtUsername.Text = Settings.Instance.username;
            txtPassword.Text = Settings.Instance.password;
            numDelay.Value = Settings.Instance.delay;
        }

        private void refreshControls(bool bState)
        {
            groupSetting.Enabled = bState;
            btnStart.Visible = bState;
            btnStop.Visible = !bState;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            progressStop();
            this.Close();
        }

        private void picTitle_MouseDown(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Default;
            this._ptPrevPoint.X = e.X;
            this._ptPrevPoint.Y = e.Y;
        }

        private void picTitle_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;

            Point point = new Point(e.X - this._ptPrevPoint.X, e.Y - this._ptPrevPoint.Y);
            point.X = this.Location.X + point.X;
            point.Y = this.Location.Y + point.Y;
            this.Location = point;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            progressStop();
            this.Close();
        }
    }
}
