using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.Win32;
using System.IO;
using System.Globalization;
using System.Web.Compilation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Nordicbet.Json;

using Api_ng_sample_code;
using Api_ng_sample_code.TO;
using Api_ng_sample_code.Json;

using HtmlAgilityPack;
using Exception = System.Exception;

namespace Nordicbet
{
    public delegate void onWriteStatusEvent(string status);

    public partial class frmMain : Form
    {
        private Point _ptPrevPoint = new Point(0, 0);

        private BackgroundWorker _slave;
        private Dictionary<int, string> _allSports;
        private CookieContainer container = null;
        private string token = string.Empty;

        private Nordicbet task = null;

        private List<JsonEl> preList = new List<JsonEl>();

        NumberStyles style = NumberStyles.Number | NumberStyles.AllowCurrencySymbol | NumberStyles.AllowDecimalPoint;
        CultureInfo culture = CultureInfo.CreateSpecificCulture("en-GB");

        public event onWriteStatusEvent onWriteStatus;

        string Url = "https://api.betfair.com/exchange/betting";
            //"hhttps://developers.betfair.com/api.betfair.com/exchange/account";

//        string Settings.Instance.delayKey = "DMF16VoWXnoADo0z";
        string sessionToken = "T9q5t5M5MGLwpDPbOA1LrZ+fX5GVTrvHK8t8bcXRfBc=";
        double balance = 0;

        List<LiveMatchInfo> _currentMatchInfos = new List<LiveMatchInfo>();
        List<MarketCatalogue> _allMarketCatalogues = new List<MarketCatalogue>();
        List<SoccerVistaInfo> _svList = new List<SoccerVistaInfo>();
        List<MarketCatalogue> _listToWait = new List<MarketCatalogue>();

        private IList<MarketBook> _marketBooks = new List<MarketBook>();

        Thread forSvList = null;
        Thread forLiveInfo = null;
        Thread forInplayGames = null;
        Thread forExtra = null;

        private Dictionary<string, string> _countryCode = new Dictionary<string, string>()
        {
            {"Afghanistan", "AF"},
            {"Albania", "AL"},
            {"Algeria", "DZ"},
            {"Andorra", "AD"},
            {"Angola", "AO"},
            {"Anguilla", "AI"},
            {"Antarctica", "AQ"},
            {"Antigua and Barbuda", "AG"},
            {"Argentina", "AR"},
            {"Armenia", "AM"},
            {"Aruba", "AW"},
            {"Australia", "AU"},
            {"Austria", "AT"},
            {"Azerbaijan", "AZ"},
            {"Bahamas", "BS"},
            {"Bahrain", "BH"},
            {"Bangladesh", "BD"},
            {"Barbados", "BB"},
            {"Belarus", "BY"},
            {"Belgium", "BE"},
            {"Belize", "BZ"},
            {"Benin", "BJ"},
            {"Bermuda", "BM"},
            {"Bhutan", "BT"},
            {"Bolivia", "BO"},
            {"Bonaire", "BQ"},
            {"Bosnia-Herzegovina", "BA"},
            {"Botswana", "BW"},
            {"Brazil", "BR"},
            {"Brunei Darussalam", "BN"},
            {"Bulgaria", "BG"},
            {"Burkina Faso", "BF"},
            {"Burundi", "BI"},
            {"Cambodia", "KH"},
            {"Cameroon", "CM"},
            {"Canada", "CA"},
            {"Cape Verde", "CV"},
            {"Cayman Islands", "KY"},
            {"Central African Republic", "CF"},
            {"Chad", "TD"},
            {"Chile", "CL"},
            {"China", "CN"},
            {"Christmas Island", "CX"},
            {"Cocos (Keeling) Islands", "CC"},
            {"Colombia", "CO"},
            {"Comoros", "KM"},
            {"Congo", "CG"},
            {"Democratic Republic of the Congo", "CD"},
            {"Cook Islands", "CK"},
            {"Costa Rica", "CR"},
            {"Croatia", "HR"},
            {"Cuba", "CU"},
            {"CuraÃ§ao", "CW"},
            {"Cyprus", "CY"},
            {"Czech Republic", "CZ"},
            {"CÃ´te d'Ivoire", "CI"},
            {"Denmark", "DK"},
            {"Djibouti", "DJ"},
            {"Dominica", "DM"},
            {"Dominican Republic", "DO"},
            {"Ecuador", "EC"},
            {"Egypt", "EG"},
            {"El Salvador", "SV"},
            {"England", "GB"},
            {"Equatorial Guinea", "GQ"},
            {"Eritrea", "ER"},
            {"Estonia", "EE"},
            {"Ethiopia", "ET"},
            {"Falkland Islands (Malvinas)", "FK"},
            {"Faroe Islands", "FO"},
            {"Fiji", "FJ"},
            {"Finland", "FI"},
            {"France", "FR"},
            {"French Guiana", "GF"},
            {"French Polynesia", "PF"},
            {"French Southern Territories", "TF"},
            {"Gabon", "GA"},
            {"Gambia", "GM"},
            {"Georgia", "GE"},
            {"Germany", "DE"},
            {"Ghana", "GH"},
            {"Gibraltar", "GI"},
            {"Greece", "GR"},
            {"Greenland", "GL"},
            {"Grenada", "GD"},
            {"Guadeloupe", "GP"},
            {"Guam", "GU"},
            {"Guatemala", "GT"},
            {"Guernsey", "GG"},
            {"Guinea", "GN"},
            {"Guinea-Bissau", "GW"},
            {"Guyana", "GY"},
            {"Haiti", "HT"},
            {"Heard Island and McDonald Mcdonald Islands", "HM"},
            {"Holy See (Vatican City State)", "VA"},
            {"Honduras", "HN"},
            {"Hong Kong", "HK"},
            {"Hungary", "HU"},
            {"Iceland", "IS"},
            {"India", "IN"},
            {"Indonesia", "ID"},
            {"Iran", "IR"},
            {"Iraq", "IQ"},
            {"Ireland", "IE"},
            {"Isle of Man", "IM"},
            {"Israel", "IL"},
            {"Italy", "IT"},
            {"Jamaica", "JM"},
            {"Japan", "JP"},
            {"Jersey", "JE"},
            {"Jordan", "JO"},
            {"Kazakhstan", "KZ"},
            {"Kenya", "KE"},
            {"Kiribati", "KI"},
            {"Korea, Democratic People's Republic of", "KP"},
            {"South Korea", "KR"},
            {"Kuwait", "KW"},
            {"Kyrgyzstan", "KG"},
            {"Lao People's Democratic Republic", "LA"},
            {"Latvia", "LV"},
            {"Lebanon", "LB"},
            {"Lesotho", "LS"},
            {"Liberia", "LR"},
            {"Libya", "LY"},
            {"Liechtenstein", "LI"},
            {"Lithuania", "LT"},
            {"Luxembourg", "LU"},
            {"Macao", "MO"},
            {"Macedonia", "MK"},
            {"Madagascar", "MG"},
            {"Malawi", "MW"},
            {"Malaysia", "MY"},
            {"Maldives", "MV"},
            {"Mali", "ML"},
            {"Malta", "MT"},
            {"Marshall Islands", "MH"},
            {"Martinique", "MQ"},
            {"Mauritania", "MR"},
            {"Mauritius", "MU"},
            {"Mayotte", "YT"},
            {"Mexico", "MX"},
            {"Micronesia, Federated States of", "FM"},
            {"Moldova, Republic of", "MD"},
            {"Monaco", "MC"},
            {"Mongolia", "MN"},
            {"Montenegro", "ME"},
            {"Montserrat", "MS"},
            {"Morocco", "MA"},
            {"Mozambique", "MZ"},
            {"Myanmar", "MM"},
            {"Namibia", "NA"},
            {"Nauru", "NR"},
            {"Nepal", "NP"},
            {"Netherlands", "NL"},
            {"New Caledonia", "NC"},
            {"New Zealand", "NZ"},
            {"Nicaragua", "NI"},
            {"Niger", "NE"},
            {"Nigeria", "NG"},
            {"Niue", "NU"},
            {"Norfolk Island", "NF"},
            {"Northern Mariana Islands", "MP"},
            {"Norway", "NO"},
            {"Oman", "OM"},
            {"Pakistan", "PK"},
            {"Palau", "PW"},
            {"Palestine, State of", "PS"},
            {"Panama", "PA"},
            {"Papua New Guinea", "PG"},
            {"Paraguay", "PY"},
            {"Peru", "PE"},
            {"Philippines", "PH"},
            {"Pitcairn", "PN"},
            {"Poland", "PL"},
            {"Portugal", "PT"},
            {"Puerto Rico", "PR"},
            {"Qatar", "QA"},
            {"Romania", "RO"},
            {"Russia", "RU"},
            {"Rwanda", "RW"},
            {"Reunion", "RE"},
            {"Saint Barthelemy", "BL"},
            {"Saint Helena", "SH"},
            {"Saint Kitts and Nevis", "KN"},
            {"Saint Lucia", "LC"},
            {"Saint Martin (French part)", "MF"},
            {"Saint Pierre and Miquelon", "PM"},
            {"Saint Vincent and the Grenadines", "VC"},
            {"Samoa", "WS"},
            {"San Marino", "SM"},
            {"Sao Tome and Principe", "ST"},
            {"Saudi Arabia", "SA"},
            {"Scotland", "GM"},
            {"Senegal", "SN"},
            {"Serbia", "RS"},
            {"Seychelles", "SC"},
            {"Sierra Leone", "SL"},
            {"Singapore", "SG"},
            {"Sint Maarten (Dutch part)", "SX"},
            {"Slovakia", "SK"},
            {"Slovenia", "SI"},
            {"Solomon Islands", "SB"},
            {"Somalia", "SO"},
            {"South Africa", "ZA"},
            {"South Georgia and the South Sandwich Islands", "GS"},
            {"South Sudan", "SS"},
            {"Spain", "ES"},
            {"Sri Lanka", "LK"},
            {"Sudan", "SD"},
            {"Suriname", "SR"},
            {"Svalbard and Jan Mayen", "SJ"},
            {"Swaziland", "SZ"},
            {"Sweden", "SE"},
            {"Switzerland", "CH"},
            {"Syrian Arab Republic", "SY"},
            {"Taiwan, Province of China", "TW"},
            {"Tajikistan", "TJ"},
            {"United Republic of Tanzania", "TZ"},
            {"Thailand", "TH"},
            {"Timor-Leste", "TL"},
            {"Togo", "TG"},
            {"Tokelau", "TK"},
            {"Tonga", "TO"},
            {"Trinidad and Tobago", "TT"},
            {"Tunisia", "TN"},
            {"Turkey", "TR"},
            {"Turkmenistan", "TM"},
            {"Turks and Caicos Islands", "TC"},
            {"Tuvalu", "TV"},
            {"Uganda", "UG"},
            {"Ukraine", "UA"},
            {"U.A.E.", "AE"},
            {"United Kingdom", "GB"},
            {"USA", "US"},
            {"United States Minor Outlying Islands", "UM"},
            {"Uruguay", "UY"},
            {"Uzbekistan", "UZ"},
            {"Vanuatu", "VU"},
            {"Venezuela", "VE"},
            {"Viet Nam", "VN"},
            {"British Virgin Islands", "VG"},
            {"US Virgin Islands", "VI"},
            {"Wallis and Futuna", "WF"},
            {"Western Sahara", "EH"},
            {"Yemen", "YE"},
            {"Zambia", "ZM"},
            {"Zimbabwe", "ZW"},
            {"Aland Islands", "AX"},
        };
        private IList<MarketProfitAndLoss> _marketProfits;


        public frmMain()
        {
            InitializeComponent();
            _slave = new BackgroundWorker();
            _slave.WorkerSupportsCancellation = true;
            _slave.DoWork += new DoWorkEventHandler(this.Slave_DoWork);
            this._slave.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.Slave_RunWorkerCompleted);
            initSports();

            container = new CookieContainer();
            System.Net.ServicePointManager.Expect100Continue = false;
        }

        private HttpClient initHttpClient()
        {
            HttpClientHandler handler = new HttpClientHandler();
            handler.CookieContainer = container;
            HttpClient httpClient = new HttpClient(handler);

            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept",
                "application/json, text/javascript, */*; q=0.01");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent",
                "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/49.0.2623.110 Safari/537.36");
            ;
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("X-Requested-With", "XMLHttpRequest");

            httpClient.DefaultRequestHeaders.Referrer = new Uri("https://pyckio.com/i/");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Bearer " + token);
            
            return httpClient;
        }

        private HttpClient initHttpClientOption()
        {
            HttpClientHandler handler = new HttpClientHandler();
            handler.CookieContainer = container;
            HttpClient httpClientOption = new HttpClient(handler);

            httpClientOption.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "*/*");
            httpClientOption.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent",
                "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/49.0.2623.110 Safari/537.36");
            httpClientOption.DefaultRequestHeaders.TryAddWithoutValidation("Access-Control-Request-Headers",
                "accept, authorization, x-requested-with");
            httpClientOption.DefaultRequestHeaders.TryAddWithoutValidation("Access-Control-Request-Method", "GET");

            httpClientOption.DefaultRequestHeaders.Referrer = new Uri("https://pyckio.com/i/");

            return httpClientOption;
        }

        private void initSports()
        {
            this._allSports = new Dictionary<int, string>();
            this._allSports.Add(1, "Badminton");
            this._allSports.Add(2, "Bandy");
            this._allSports.Add(3, "Baseball");
            this._allSports.Add(4, "Basketball");
            this._allSports.Add(6, "Boxing");
            this._allSports.Add(8, "Cricket");
            this._allSports.Add(9, "Curling");
            this._allSports.Add(10, "Darts");
            this._allSports.Add(11, "Darts (Legs)");
            this._allSports.Add(12, "E Sports");
            this._allSports.Add(14, "Floorball");
            this._allSports.Add(15, "Football");
            this._allSports.Add(0x10, "Futsal");
            this._allSports.Add(0x11, "Golf");
            this._allSports.Add(0x12, "Handball");
            this._allSports.Add(0x13, "Hockey");
            this._allSports.Add(20, "Horse Racing");
            this._allSports.Add(0x16, "Mixed Martial Arts");
            this._allSports.Add(0x17, "Other Sports");
            this._allSports.Add(0x18, "Politics");
            this._allSports.Add(0x1a, "Rugby League");
            this._allSports.Add(0x1b, "Rugby Union");
            this._allSports.Add(0x1c, "Snooker");
            this._allSports.Add(0x1d, "Soccer");
            this._allSports.Add(30, "Softball");
            this._allSports.Add(0x1f, "Squash");
            this._allSports.Add(0x20, "Table Tennis");
            this._allSports.Add(0x21, "Tennis");
            this._allSports.Add(0x22, "Volleyball");
            this._allSports.Add(0x23, "Volleyball (Points)");
            this._allSports.Add(0x24, "Water Polo");
            this._allSports.Add(0x27, "Aussie Rules");
            this._allSports.Add(40, "Alpine Skiing");
            this._allSports.Add(0x29, "Biathlon");
            this._allSports.Add(0x2a, "Ski Jumping");
            this._allSports.Add(0x2b, "Cross Country");
            this._allSports.Add(0x2c, "Formula 1");
            this._allSports.Add(0x2d, "Cycling");
            this._allSports.Add(0x2e, "Bobsleigh");
            this._allSports.Add(0x2f, "Figure Skating");
            this._allSports.Add(0x30, "Freestyle Skiing");
            this._allSports.Add(0x31, "Luge");
            this._allSports.Add(50, "Nordic Combined");
            this._allSports.Add(0x33, "Short Track");
            this._allSports.Add(0x34, "Skeleton");
            this._allSports.Add(0x35, "Snow Boarding");
            this._allSports.Add(0x36, "Speed Skating");
        }

        private void btnSetting_Click(object sender, EventArgs e)
        {
            frmSetting frm = new frmSetting();
            if (frm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                saveSettingInfo();
            }
        }

        private string ReadRegistry(string KeyName)
        {
            return
                Registry.CurrentUser.CreateSubKey("SoftWare")
                    .CreateSubKey("bfairBet")
                    .GetValue(KeyName, (object) "")
                    .ToString();
        }

        private void WriteRegistry(string KeyName, string KeyValue)
        {
            Registry.CurrentUser.CreateSubKey("SoftWare").CreateSubKey("bfairBet").SetValue(KeyName, (object) KeyValue);
        }

        private void saveSettingInfo()
        {
            WriteRegistry("bfairusername", Settings.Instance.username);
            WriteRegistry("bfairpassword", Settings.Instance.password);
            WriteRegistry("bfairuseSideOddsMin", Settings.Instance.useSideOddsMin.ToString());
            WriteRegistry("bfairsideOddsMin", Settings.Instance.sideOddsMin.ToString());
            WriteRegistry("bfairuseSideOddsMax", Settings.Instance.useSideOddsMax.ToString());
            WriteRegistry("bfairsideOddsMax", Settings.Instance.sideOddsMax.ToString());
            WriteRegistry("bfairsideDelta", Settings.Instance.sideDelta.ToString());
            WriteRegistry("bfaircurrency", Settings.Instance.currency);
            WriteRegistry("bfairuseProgression", Settings.Instance.useProgression.ToString());
            WriteRegistry("bfairuseFilter", Settings.Instance.useFilter.ToString());
            WriteRegistry("bfairstopstep", Settings.Instance.stopStep.ToString());
            WriteRegistry("bfairflatStake", Settings.Instance.flatStake.ToString());
            WriteRegistry("bfairbetmax", Settings.Instance.betMax.ToString());
            WriteRegistry("bfairdelaykey", Settings.Instance.delayKey);
            WriteRegistry("bfairactivekey", Settings.Instance.activeKey);
            WriteRegistry("bfairpercentBalance", Settings.Instance.percentBalance.ToString());
            WriteRegistry("bfairuseFixedStake", Settings.Instance.useFixedStake.ToString());
            WriteRegistry("bfairusePercentage", Settings.Instance.usePercent.ToString());


            WriteRegistry("bfairstakeCount", Settings.Instance.stakes.Count.ToString());
            for (int i = 0; i < Settings.Instance.stakes.Count; i++)
                WriteRegistry("bfairstake" + i.ToString(), Settings.Instance.stakes[i].ToString());
        }

        private void loadSettingInfo()
        {
            Settings.Instance.username = ReadRegistry("bfairusername");
            Settings.Instance.password = ReadRegistry("bfairpassword");

            Settings.Instance.useSideOddsMin = ReadRegistry("bfairuseSideOddsMin") == "True";

            double sideOddsMin = 0;
            if (double.TryParse(ReadRegistry("bfairsideOddsMin"), style, culture, out sideOddsMin))
                Settings.Instance.sideOddsMin = sideOddsMin;

            Settings.Instance.useSideOddsMax = ReadRegistry("bfairuseSideOddsMax") == "True";

            double sideOddsMax = 0;
            if (double.TryParse(ReadRegistry("bfairsideOddsMax"), style, culture, out sideOddsMax))
                Settings.Instance.sideOddsMax = sideOddsMax;

            int sideDelta = 0;
            if (int.TryParse(ReadRegistry("bfairsideDelta"), style, culture, out sideDelta))
                Settings.Instance.sideDelta = sideDelta;

            Settings.Instance.currency = ReadRegistry("bfaircurrency");

            Settings.Instance.useProgression = ReadRegistry("bfairuseProgression") == "True";
            Settings.Instance.useFilter = ReadRegistry("bfairuseFilter") == "True";

            int stopstep = 0;
            if (int.TryParse(ReadRegistry("bfairstopstep"), style, culture, out stopstep))
                Settings.Instance.stopStep = stopstep;

            double flatStake = 0;
            if (double.TryParse(ReadRegistry("bfairflatStake"), style, culture, out flatStake))
                Settings.Instance.flatStake = flatStake;

            int betmax = 10;
            if (int.TryParse(ReadRegistry("bfairbetmax"), style, culture, out betmax))
                Settings.Instance.betMax = betmax;

            int stakeCount = 0;
            if (int.TryParse(ReadRegistry("bfairstakeCount"), style, culture, out stakeCount))
            {
                //for(int i = 0; i < stakeCount; i ++)
                //{
                //    double stake = 0;
                //    if (double.TryParse(ReadRegistry("stake" + i.ToString()), style, culture, out stake))
                //        Settings.Instance.stakes.Insert(i, stake);
                //}
            }
            Settings.Instance.delayKey = ReadRegistry("bfairdelaykey");
            Settings.Instance.activeKey = ReadRegistry("bfairactivekey");

            double percentBalance = 0;
            if (double.TryParse(ReadRegistry("bfairpercentBalance"), style, culture, out percentBalance))
                Settings.Instance.percentBalance = percentBalance;

            Settings.Instance.useFixedStake = ReadRegistry("bfairuseFixedStake") == "True";
            Settings.Instance.usePercent = ReadRegistry("bfairusePercentage") == "True";
        }

        private void loadStakes(string filename)
        {
            if (!File.Exists(filename))
                return;

            string[] lines = File.ReadAllLines(filename);
            if (lines == null || lines.Length < 1)
                return;

            foreach (string line in lines)
            {
                double stake = 0;
                if (double.TryParse(line, style, culture, out stake))
                    addStake(stake);
            }
        }

        private void initStakes()
        {
            foreach (double stake in Settings.Instance.stakes)
            {
                addStake(stake, false);
            }
        }

        private void addStake(double stake, bool bAdd = true)
        {
            int index = tblStake.Rows.Add();
            if (index < 0)
                return;

            object[] values = new object[]
            {
                index + 1, stake
            };

            tblStake.Rows[index].SetValues(values);
            tblStake.Rows[index].Tag = stake;

            if (bAdd)
                Settings.Instance.stakes.Add(stake);
        }

        private void delStake(int index)
        {
            if (index < 0)
            {
                Messagebox.show("Please select the item!");
                return;
            }

            double stake = (double) tblStake.Rows[index].Tag;
            if (stake < 0)
            {
                Messagebox.show("Cannot delete this item!");
                return;
            }

            tblStake.Rows.RemoveAt(index);
            Settings.Instance.stakes.Remove(stake);
        }

        private void updateStake(int index, double stake)
        {
            if (index < 0)
            {
                Messagebox.show("Please select the item!");
                return;
            }

            double oldStake = (double) tblStake.Rows[index].Tag;
            if (oldStake < 0)
            {
                Messagebox.show("Cannot delete this item!");
                return;
            }

            object[] values = new object[]
            {
                index + 1, stake
            };

            tblStake.Rows[index].SetValues(values);
            tblStake.Rows[index].Tag = stake;

            Settings.Instance.stakes.RemoveAt(index);
            Settings.Instance.stakes.Insert(index, stake);
        }

        private void initStartTime()
        {
            Constants.start = DateTime.Now;
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            initControls();
            loadSettingInfo();
            initStartTime();

            if (Settings.Instance.stakes.Count < 1)
                loadStakes("stake.txt");
            else
                initStakes();

            //Keep
            //            if (!keepService())
            //                this.Close();

            //timerKeep.Start();
        }

        private void initControls()
        {
            btnIcon.Parent = picTitle;
            lblTitle.Parent = picTitle;
            btnMin.Parent = picTitle;
            btnMax.Parent = picTitle;
            btnClose.Parent = picTitle;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            progressStop();
            this.Close();
        }

        private void progressStop()
        {
            if (_slave.IsBusy)
            {
                refreshControls(true);

                forSvList.Abort();
                forExtra.Abort();
                forInplayGames.Abort();
                forLiveInfo.Abort();

                _slave.CancelAsync();
            }
        }

        private void btnMin_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
            this.Show();
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

        private void btnExit_Click(object sender, EventArgs e)
        {
            progressStop();
            this.Close();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            onWriteStatus(getMainLogTitle() + "The bot has been stopped!");
            refreshControls(true);
            progressStop();
        }

        private void refreshControls(bool bState)
        {
            tblBets.Enabled = bState;
            rtLog.Enabled = bState;
            btnSetting.Enabled = bState;
            btnStart.Enabled = bState;
        }

        private bool canStart()
        {
            if (string.IsNullOrEmpty(Settings.Instance.username))
            {
                Messagebox.show("Please enter the username!");
                return false;
            }

            if (string.IsNullOrEmpty(Settings.Instance.password))
            {
                Messagebox.show("Please enter the password!");
                return false;
            }

            if (Settings.Instance.sideOddsMin <= 1)
            {
                Messagebox.show("Please enter correct side odds min!");
                return false;
            }

            if (Settings.Instance.sideOddsMax <= 1)
            {
                Messagebox.show("Please enter correct side odds max!");
                return false;
            }

//            if (string.IsNullOrEmpty(Settings.Instance.currency))
//            {
//                Messagebox.show("Please enter the currency!");
//                return false;
//            }

            if (Settings.Instance.useProgression &&
                (Settings.Instance.stopStep <= 1 || Settings.Instance.stopStep > Settings.Instance.stakes.Count))
            {
                Messagebox.show("Please enter the correct stop step!");
                return false;
            }

            if (Settings.Instance.useFilter && (Settings.Instance.sideDelta.ToString() == ""))
            {
                Messagebox.show("Please enter the correct stop step!");
                return false;
            }

            if (string.IsNullOrEmpty(Settings.Instance.delayKey))
            {
                Messagebox.show("Please enter the correct delay key!");
                return false;
            }

            if (string.IsNullOrEmpty(Settings.Instance.activeKey))
            {
                Messagebox.show("Please enter the correct active key!");
                return false;
            }

            if (Settings.Instance.usePercent && Settings.Instance.percentBalance < 10)
            {
                Messagebox.show("Please enter correct percentage of balance!");
                return false;
            }

            if (Settings.Instance.useFixedStake && Settings.Instance.flatStake < 2)
            {
                Messagebox.show("Please enter correct flat stake! (minimum is 2)");
                return false;
            }


            return true;
        }


        private void btnStart_Click(object sender, EventArgs e)
        {
            if (!canStart())
                return;

            if (this.onWriteStatus == null)
                this.onWriteStatus += writeStatus;

            onWriteStatus(getMainLogTitle() + "The bot has been started!");

            ////////////////////////////////////////////////
            //test();
            ////////////////////////////////////////////////

            if (_slave.IsBusy)
            {
                Messagebox.show("Please try after a few seconds!");
                return;
            }

            _slave.RunWorkerAsync();
        }

        private void writeStatus(string status)
        {
            if (rtLog.InvokeRequired)
                rtLog.Invoke(onWriteStatus, status);
            else
            {
                rtLog.AppendText(((string.IsNullOrEmpty(rtLog.Text) ? "" : "\r\n") + string.Format("{0}", status)));
                rtLog.ScrollToCaret();
            }
        }

        private double getCurrentStake()
        {
            return Settings.Instance.stakes[Constants.step];
        }

        private string getMainLogTitle()
        {
            return string.Format("[{0}] ", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        }

        private void setStep()
        {
            try
            {
                this.Invoke(new Action(() =>
                {
                    lblStep.Text = "Step: " + (Constants.step + 1).ToString();
                }));
            }
            catch (System.Exception e)
            {

            }
        }


        private void updateBalance(Nordicbet task)
        {
            try
            {
                double balance = task.getBalance().Result;
                if (balance < 0)
                    return;

                writeBalance(balance);
            }
            catch (System.Exception)
            {

            }
        }

        private void writeBalance(double balance)
        {
            try
            {
                this.Invoke(new Action(() =>
                {
                    lblBalance.Text = string.Format("€{0:N2}", balance);
                }));
            }
            catch (System.Exception)
            {

            }
        }

        private int getRandomDelay(int min, int max)
        {
            Random r = new Random();
            return r.Next(min, max);
        }

        private bool isExist(List<JsonEl> preList, JsonEl jsonEl)
        {
            foreach (JsonEl el in preList)
            {
                if (jsonEl.en == el.en)
                    return true;
            }

            return false;
        }

        private void LogString(string strContent)
        {
            try
            {
                onWriteStatus(getMainLogTitle() + strContent);

                System.IO.StreamWriter file = new System.IO.StreamWriter("log", true);
                lock (file)
                {
                    string content = DateTime.Now.ToString() + "----->" + strContent;
                    file.WriteLine(content);
                    file.Close();
                }

            }
            catch (Exception)
            {
            }
            //            System.IO.StreamWriter file1 = new System.IO.StreamWriter("logs.txt", true);
            //            file1.WriteLine(content);
            //            file1.Close();
        }

        private void buildSvList()
        {
            while (true)
            {
                Nordicbet taskOne = new Nordicbet(onWriteStatus);
                List<SoccerVistaInfo> svList = null;
                if (!Settings.Instance.useFilter)
                    svList = taskOne.buildSVList(Settings.Instance.sideOddsMin, Settings.Instance.sideOddsMax).Result;
                else
                    svList = taskOne.buildSVList(Settings.Instance.sideOddsMin, Settings.Instance.sideOddsMax, Settings.Instance.sideDelta).Result;

                svList = taskOne.buildMatchInfos(svList).Result;

                _svList.Clear();
                _svList = svList;
                LogString("Fetched SoccerVista Informations (" + _svList.Count.ToString() + ")");
            }
        }

        private void getLiveInfos()
        {
            while (true)
//            for (int i = 0; i < 1; i++)
            {
                if (_allMarketCatalogues.Count > 0)
                {
                    lock (_allMarketCatalogues)
                    {
                        LogString("locked for all market catalogues list");
                        Nordicbet taskOne = new Nordicbet(onWriteStatus);
                        List<LiveMatchInfo> currentMatchInfos = taskOne.getLiveMatchInfos(_allMarketCatalogues);

                        if (currentMatchInfos == null)
                        {
                            _currentMatchInfos.Clear();
                            LogString("Cannot get current match informations from betfair site!");
                            continue;
                        }

                        _currentMatchInfos.Clear();
                        _currentMatchInfos = currentMatchInfos;
                        LogString("Fetched Current Elapsed Time and Score (" + _currentMatchInfos.Count.ToString() + ")");
                    }
                    LogString("unlocked for all market catalogues list");
                }
            }
        }

        private LiveMatchInfo getLiveMatchInfo(string strEventName)
        {
            lock (_currentMatchInfos)
            {
                LogString("locked for current match infos list");
                foreach (LiveMatchInfo curInfo in _currentMatchInfos)
                {
                    if (strEventName == (curInfo.homeName + " v " + curInfo.awayName))
                    {
                        return curInfo;
                    }
                }
            }
            LogString("unlocked for current match infos list");
            return null;
        }

        private void getInplayGames() //-1:no data, 
        {
            while (true)
//            for (int i = 0; i < 1; i++)
            {
                try
                {
                    JsonRpcClient client = null;
                    string clientType = null;

                    client = new JsonRpcClient(Url, Settings.Instance.delayKey, sessionToken);

                    var marketFilter = new MarketFilter();

                    var eventTypes = client.listEventTypes(marketFilter);
                    if (eventTypes == null || eventTypes.Count < 1)
                        return;
                    LogString("Fetched listEventTypes");

                    int maxResult = 1;
                    if (!eventTypes[0].EventType.Name.Equals("Soccer"))
                        return;

                    ISet<string> eventypeIds = new HashSet<string>();
                    foreach (EventTypeResult eventType in eventTypes)
                    {
                        if (eventType.EventType.Name.Equals("Soccer"))
                        {
                            //extracting eventype id
                            eventypeIds.Add(eventType.EventType.Id);
                        }
                    }

                    var time = new TimeRange();
                    time.From = DateTime.Now.AddDays(-1);
                    time.To = DateTime.Now.AddHours(12);

                    maxResult = eventTypes[0].MarketCount;

                    var marketSort = MarketSort.FIRST_TO_START;

                    marketFilter.EventTypeIds = eventypeIds;
                    marketFilter.InPlayOnly = true;
                    marketFilter.MarketStartTime = time;

                    //as an example we requested runner metadata 
                    ISet<MarketProjection> marketProjections = new HashSet<MarketProjection>();
                    marketProjections.Add(MarketProjection.EVENT);
//                    marketProjections.Add(MarketProjection.COMPETITION);
//                    marketProjections.Add(MarketProjection.MARKET_DESCRIPTION);
//                    marketProjections.Add(MarketProjection.RUNNER_DESCRIPTION);
                    marketProjections.Add(MarketProjection.RUNNER_METADATA);

                    LogString("\nGetting the next available match markets");

                    List<MarketCatalogue> allMarketCatalogues = new List<MarketCatalogue>();

                    do
                    {
                        var marketCatalogues = client.listMarketCatalogue(marketFilter, marketProjections, marketSort, 120.ToString());

                        LogString("Got Match Market Infos (" + marketCatalogues.Count + ")");

                        if (marketCatalogues.Count < 1)
                            break;

                        time.From = marketCatalogues[marketCatalogues.Count - 1].Event.OpenDate.Value.AddMinutes(5);
                        marketFilter.MarketStartTime = time;

                        allMarketCatalogues.AddRange(marketCatalogues);
                        
                        if (marketCatalogues.Count < 120)
                            break;

                        if (marketCatalogues[marketCatalogues.Count - 1].Event.OpenDate.Value > time.To)
                            break;
                    } while (true);

                    if (allMarketCatalogues == null || allMarketCatalogues.Count < 1)
                        continue;
                    LogString("\nGot the next available match markets");

                    List<string> marketIDS = new List<string>();

                    foreach (MarketCatalogue markCat in allMarketCatalogues)
                    {
                        if (markCat.MarketName == "Match Odds")
                            marketIDS.Add(markCat.MarketId);
                    }

                    ISet<PriceData> priceData = new HashSet<PriceData>();
                    //get all prices from the exchange
//                    priceData.Add(PriceData.EX_ALL_OFFERS);
                    priceData.Add(PriceData.EX_BEST_OFFERS);
//                    priceData.Add(PriceData.EX_TRADED);
//                    priceData.Add(PriceData.SP_AVAILABLE);
//                    priceData.Add(PriceData.SP_TRADED);

                    var priceProjection = new PriceProjection();
                    priceProjection.PriceData = priceData;

                    IList<MarketBook> marketBooks = client.listMarketBook(marketIDS, priceProjection);

                    _marketBooks.Clear();
                    foreach (MarketBook markBook in marketBooks)
                    {
                        if (markBook.IsInplay)
                            _marketBooks.Add(markBook);
                    }
                    LogString("Fetched List Market Books (" + _marketBooks.Count.ToString() + ")");

                    _marketProfits = client.listMarketProfitAndLoss(marketIDS);
                    //foreach(MarketProfitAndLoss profitAndLose in _marketProfits)
                    //{
                    //    foreach(var runner in profitAndLose.ProfitAndLosses)
                    //    {
                    //        LogString("Profit and Lose: ifLose(" + runner.IfLose.ToString() + ") ifWin(" + runner.IfWin.ToString() + ") SelectionID(" + runner.SelectionId.ToString() + ")");
                    //    }
                    //}

                    _allMarketCatalogues.Clear();
                    foreach (MarketCatalogue eachMark in allMarketCatalogues)
                    {
                        foreach (MarketBook markBook in _marketBooks)
                        {
                            if (eachMark.MarketId == markBook.MarketId)
                                _allMarketCatalogues.Add(eachMark);
                        }
                    }
                    LogString("Fetched All Market Catalogues (" + _allMarketCatalogues.Count.ToString() + ")");
                }
                catch (APINGException e)
                {
                    LogString(e.Message);
                    if (e.ErrorCode.Contains("INVALID_SESSION_INFORMATION"))
                    {
                        while (!task.doLogin().Result)
                        {
                            LogString("Trying to log in!");
                        }
                        sessionToken = task.sessionToken;
                    }
                }
                catch (Exception ee)
                {
                    LogString(ee.Message);
                }
            }
        }

        private bool getCurrentBalance()
        {
            try
            {
                JsonRpcClient jclient = new JsonRpcClient("https://developers.betfair.com/api.betfair.com/exchange/account", Settings.Instance.delayKey, sessionToken);
//                LogString("\nGetting Account Funds!\n");
                AccountFundsResponse fResp = jclient.getAccountFunds(Wallet.UK);

                balance = fResp.AvailableToBetBalance;
            }
            catch (APINGException e)
            {
                LogString(e.Message);
                if (e.ErrorCode.Contains("INVALID_SESSION_INFORMATION"))
                {
                    while (!task.doLogin().Result)
                    {
                        LogString("Trying to log in!");
                    }
                    sessionToken = task.sessionToken;
                }
                return false;
            }
            catch (Exception ee)
            {
                LogString(ee.Message);
                return false;
            }

            return true;
        }

        private bool ifHitHT(string strEventName)
        {
            lock (_currentMatchInfos)
            {
                LogString("locked for current match infos list");
                foreach (LiveMatchInfo currentMatchInfo in _currentMatchInfos)
                {
                    string eventName = currentMatchInfo.homeName + " v " + currentMatchInfo.awayName;

                    if (strEventName.Contains(eventName))
                    {
                        LogString(string.Format("\nThey are the Same match: {0}\nLive Game: {1}\n{2}", eventName, strEventName, currentMatchInfo.currentTime.ToString()));
                        if(currentMatchInfo.currentTime.Contains("HT"))
                            return true;
                    }
                }
            }
            LogString("unlocked for current match infos list");
            return false;
        }

        private bool ifHitQuater(string strEventName)
        {
            lock (_currentMatchInfos)
            {
                LogString("locked for current match infos list");
                foreach (LiveMatchInfo currentMatchInfo in _currentMatchInfos)
                {
                    if (!currentMatchInfo.currentTime.Contains("'"))
                        continue;

                    string eventName = currentMatchInfo.homeName + " v " + currentMatchInfo.awayName;

                    int currentTime =
                        int.Parse(currentMatchInfo.currentTime.Substring(0, currentMatchInfo.currentTime.IndexOf("'")));

                    LogString("Check if Hit Quater : (" + eventName + ": " + currentTime.ToString() + "')");

                    if (currentTime == 60 && eventName == strEventName)
                        return true;
                }
            }
            LogString("unlocked for current match infos list");
            return false;
        }

        private ExecutionReportStatus placeOrderForHT(MarketCatalogue itemMarket)
        {
            getCurrentBalance();
            LogString("Fetched current balance");
            foreach (var runner in itemMarket.Runners)
            {
                if (runner.RunnerName == "The Draw")
                {
                    try
                    {
                        LogString("Try to find if in List to wait");
                        foreach (MarketCatalogue itemToWait in _listToWait)
                        {
                            LogString(itemToWait.Event.Name + " & " + itemMarket.Event.Name);
                            if (itemToWait.Event.Name == itemMarket.Event.Name)
                            {
                                return ExecutionReportStatus.SUCCESS;
                            }
                        }

                        JsonRpcClient client = null;
                        string clientType = null;
                        String marketId = itemMarket.MarketId;

                        clientType = "rescript1";

                        client = new JsonRpcClient(Url, Settings.Instance.activeKey, sessionToken);

                        IList<PlaceInstruction> placeInstructions = new List<PlaceInstruction>();
                        var placeInstruction = new PlaceInstruction();

                        placeInstruction.Handicap = 0;
                        placeInstruction.Side = Side.BACK;
                        placeInstruction.OrderType = OrderType.LIMIT;

                        var limitOrder = new LimitOrder();
                        limitOrder.PersistenceType = PersistenceType.LAPSE;
                        // place a back bet at rediculous odds so it doesn't get matched 
                        
                        lock (_marketBooks)
                        {
                            LogString("locked for market books list");
                            foreach (MarketBook markBook in _marketBooks)
                            {
                                if (markBook.MarketId == itemMarket.MarketId)
                                {
                                    limitOrder.Price = markBook.Runners[2].ExchangePrices.AvailableToBack[0].Price;
                                    if(Settings.Instance.useFixedStake)
                                        limitOrder.Size = (limitOrder.Price - 1.0) * Settings.Instance.flatStake;
                                    break;
                                }
                            }
                        }
                        LogString("unlocked for market books list");

                        //                    limitOrder.Size = 0.1; // placing a bet below minimum stake, expecting a error in report

                        placeInstruction.LimitOrder = limitOrder;
                        placeInstruction.SelectionId = runner.SelectionId;
                        placeInstructions.Add(placeInstruction);

                        var customerRef = "123456";
                        var placeExecutionReport = client.placeOrders(marketId, customerRef, placeInstructions);

                        ExecutionReportErrorCode executionErrorcode = placeExecutionReport.ErrorCode;
                        InstructionReportErrorCode instructionErroCode =
                            placeExecutionReport.InstructionReports[0].ErrorCode;
                        LogString("\nPlaceExecutionReport error code is: " + executionErrorcode +
                                          "\nInstructionReport error code is: " + instructionErroCode);

                        LogString("Placed order for " + itemMarket.Event.Name);

                        //if (executionErrorcode == ExecutionReportErrorCode.OK)
                        //{
                        //    _listToWait.Add(itemMarket);
                        //    LogString("\nSuccessfully placed bet to " + itemMarket.Event.Name);
                        //}
                        //else 
                        if (executionErrorcode != ExecutionReportErrorCode.BET_ACTION_ERROR && instructionErroCode != InstructionReportErrorCode.INVALID_BET_SIZE)
                        {
                            LogString("\nSuccessfully placed bet to " + itemMarket.Event.Name);

                            _listToWait.Add(itemMarket);
                            LogString("added to wait list");

                            return placeExecutionReport.Status;
                        }

//                        bets = task.getBetHistory().Result;
//                        if (bets == null)
//                            continue;
//                        showBetsHistory(bets);

                        LogString("\nDONE!");
                        LogString(itemMarket.Event.Name);
                        return placeExecutionReport.Status;
                    }
                    catch (APINGException ee)
                    {
                        LogString(ee.ErrorCode);
                        LogString(ee.ErrorDetails);
                        LogString(ee.Message);

                        if (ee.ErrorCode.Contains("INVALID_SESSION_INFORMATION"))
                        {
                            while (!task.doLogin().Result)
                            {
                                LogString("Trying to log in!");
                            }
                            sessionToken = task.sessionToken;
                        }
                    }
                    catch (Exception e)
                    {
                        LogString(e.Message);
                    }
                }
            }
            return ExecutionReportStatus.FAILURE;
        }
        private ExecutionReportStatus placeOrderForQuater(MarketCatalogue itemMarket)
        {
            getCurrentBalance();
            foreach (var runner in itemMarket.Runners)
            {
                if (runner.RunnerName == "The Draw")
                {
                    IClient client = null;
                    string clientType = null;
                    String marketId = itemMarket.MarketId;

                    clientType = "rescript1";

                    // if rescript has been passed as the third argument use it otherwise default to json client
                    if (!string.IsNullOrEmpty(clientType) && clientType.Equals("rescript"))
                    {
                        client = new RescriptClient(Url, Settings.Instance.activeKey, sessionToken);
                    }
                    else
                    {
                        client = new JsonRpcClient(Url, Settings.Instance.activeKey, sessionToken);
                    }
                    var selectionId = runner.SelectionId;
                    IList<PlaceInstruction> placeInstructions = new List<PlaceInstruction>();
                    var placeInstruction = new PlaceInstruction();

                    placeInstruction.Handicap = 0;
                    placeInstruction.Side = Side.LAY;
                    placeInstruction.OrderType = OrderType.LIMIT;

                    var limitOrder = new LimitOrder();
                    limitOrder.PersistenceType = PersistenceType.LAPSE;

                    // place a back bet at rediculous odds so it doesn't get matched 

                    lock (_marketBooks)
                    {
                        LogString("locked for market books list");
                        foreach (MarketBook markBook in _marketBooks)
                        {
                            if (markBook.MarketId == itemMarket.MarketId)
                            {
                                limitOrder.Price = markBook.Runners[2].ExchangePrices.AvailableToLay[0].Price;
                                if(Settings.Instance.useFixedStake)
                                    limitOrder.Size = (limitOrder.Price - 1.0) * Settings.Instance.flatStake;
                                break;
                            }
                        }
                    }
                    LogString("unlocked for market books list");

                    // place a back bet at rediculous odds so it doesn't get matched 

                    placeInstruction.LimitOrder = limitOrder;
                    placeInstruction.SelectionId = selectionId;
                    placeInstructions.Add(placeInstruction);

                    var customerRef = "123456";
                    var placeExecutionReport = client.placeOrders(marketId, customerRef, placeInstructions);

                    ExecutionReportErrorCode executionErrorcode = placeExecutionReport.ErrorCode;
                    InstructionReportErrorCode instructionErroCode = placeExecutionReport.InstructionReports[0].ErrorCode;
                    LogString("\nPlaceExecutionReport error code is: " + executionErrorcode + "\nInstructionReport error code is: " + instructionErroCode);

                    if (executionErrorcode != ExecutionReportErrorCode.BET_ACTION_ERROR && instructionErroCode != InstructionReportErrorCode.INVALID_BET_SIZE)
                    {
                        LogString("\nSuccessfully placed bet to " + itemMarket.Event.Name);
                        
                        return placeExecutionReport.Status;
                    }

//                    bets = task.getBetHistory().Result;
//                    if (bets == null)
//                        continue;
//                    showBetsHistory(bets);

                    LogString("\nDONE!");
                    LogString(itemMarket.Event.Name);

                    return placeExecutionReport.Status;
                }
            }
            return ExecutionReportStatus.FAILURE;
        }
        void placeOrderForExtra()
        {
            while (true)
            {
                {
                    for (int i = 0; i < _listToWait.Count; )
                    {
                        MarketCatalogue marketCat = _listToWait[i];

                        LiveMatchInfo curMatchInfo = getLiveMatchInfo(marketCat.Event.Name);

                        if (curMatchInfo == null)
                        {
                            lock (_listToWait)
                            {
                                //_listToWait.RemoveAt(i);
                            }
                            continue;
                        }

                        if (curMatchInfo.currentTime.Contains("'"))
                        {
                            int nCurElapsedTime = int.Parse(curMatchInfo.currentTime.Substring(0, curMatchInfo.currentTime.IndexOf("'")));
                            if (nCurElapsedTime > 60 && nCurElapsedTime < 70)
                            //if (ifHitQuater(marketCat.Event.Name))
                            {
                                LogString("Hit quater game : " + marketCat.Event.Name + "\nScore (" + curMatchInfo.homeScore.ToString() + ":" + curMatchInfo.awayScore.ToString() + ")");
                                if (curMatchInfo.homeScore == curMatchInfo.awayScore)
                                {
                                    ExecutionReportStatus retCode = 0;

                                    do
                                    {
                                        //make lay at the draw
                                        LogString("To Place Order after Quater");
                                        retCode = placeOrderForQuater(marketCat);
                                        LogString("To Place Order after Quater");
                                    } while (retCode != ExecutionReportStatus.SUCCESS);

                                    lock (_listToWait)
                                    {
                                        _listToWait.RemoveAt(i);
                                    }
                                    continue;
                                }
                            }
                        }
                        if (curMatchInfo.currentTime.Contains("END"))
                        {
                            //lock (_listToWait)
                            //{
                            //    //_listToWait.RemoveAt(i);
                            //}
                            LogString("Game Ended : " + marketCat.Event.Name);
                            continue;
                        }

                        //if (curMatchInfo.homeScore == curMatchInfo.awayScore)
                        //{
                        //    //make lay at the draw
                        //    LogString("To Place Order after Quater while waiting the equal score");
                        //    placeOrderForQuater(marketCat);
                        //    LogString("Placed Order after Quater while waiting the equal score");

                        //    _listToWait.RemoveAt(i);
                        //    continue;
                        //}

                        i++;
                    }
                }
            }
        }
        private void Slave_DoWork(object sender, DoWorkEventArgs ee)
        {
//            buildSvList();

//            getInplayGames();

            task = new Nordicbet(onWriteStatus);

            while (!task.doLogin().Result)
            {
                LogString("Trying to log in!");
            }
            sessionToken = task.sessionToken;
            LogString("Login Succeed!");
//            task.getLiveMatchInfos(null);
            
            forSvList = new Thread(buildSvList); //scraped
            forSvList.Start();

            forLiveInfo = new Thread(getLiveInfos);  //current match time and score info
            forLiveInfo.Start();

            forInplayGames = new Thread(getInplayGames); //get inplay games;
            forInplayGames.Start();

            forExtra = new Thread(placeOrderForExtra); //if match score is equal in second half
            forExtra.Start();

//            SoccerVistaInfo testVista = new SoccerVistaInfo();
//            testVista.matchInfos = new List<MatchInfo>();
//            MatchInfo matchinfo = new MatchInfo();
//            matchinfo.homeTeam = "Lokomotiv";
//            matchinfo.awayTeam = "Makhachkala";
//            testVista.matchInfos.Add(matchinfo);
//
//            _svList.Add(testVista);

            while (!this._slave.CancellationPending)
            {

                /////////////////////////////////////////////////////////////////////////////forTest////////////////////////////////////////////////////////////////////////////////
                //                buildSvList();
                //                if (_svList.Count < 1)
                //                    continue;
                //                
                //                getInplayGames();
                //                if (_allMarketCatalogues.Count < 1)
                //                    continue;
                //
                //                getLiveInfos();
                //                if (_currentMatchInfos.Count < 1)
                //                    continue;
                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                try
                {
                    if (_svList.Count < 1)
                        continue;
                    if (_allMarketCatalogues.Count < 1)
                        continue;
                    if (_currentMatchInfos.Count < 1)
                        continue;

                    LogString("There are live games");

                    long runnerId = 0;
                    bool bFlag = false;

                    List<MarketCatalogue> properMarketCats = new List<MarketCatalogue>();

                    lock (_allMarketCatalogues)
                    {
                        LogString("locked for all market catalogues list");
                        foreach (MarketCatalogue marketCat in _allMarketCatalogues)
                        {
                            if (marketCat.MarketName != "Match Odds")
                                continue;

                            bFlag = false;
                            lock (_svList)
                            {
                                LogString("locked for soccer vista info list");
                                foreach (SoccerVistaInfo _info in _svList)
                                {
                                    foreach (MatchInfo _matchInfo in _info.matchInfos)
                                    {
                                        if (_matchInfo.eventName == marketCat.Event.Name)
                                        {
                                            LogString(marketCat.Event.Name);
                                            properMarketCats.Add(marketCat);
                                            bFlag = true;
                                            break;
                                        }

                                        foreach (string subHome in _matchInfo.homeTeam.Split(' '))
                                        {
                                            foreach (string subAway in _matchInfo.awayTeam.Split(' '))
                                            {
                                                if (marketCat.Event.Name.Contains(subHome) && marketCat.Event.Name.Contains(subAway))
                                                {
                                                    LogString(marketCat.Event.Name);
                                                    properMarketCats.Add(marketCat);
                                                    bFlag = true;
                                                    break;
                                                }
                                            }
                                            if (bFlag)
                                                break;
                                        }
                                        if (bFlag)
                                            break;
                                    }
                                    if (bFlag)
                                    {
                                        break;
                                    }
                                }
                            }
                            LogString("unlocked for soccer vista info list");
                        }
                    }
                    LogString("unlocked for all market catalogues list");
                    LogString("Fetched Proper Market Catalogues (" +
                                        properMarketCats.Count.ToString() +
                                        ")");

                    foreach (MarketCatalogue marketCat in properMarketCats)
                    {
                        if (ifHitHT(marketCat.Event.Name))
                        {
                            LiveMatchInfo curMatchInfo = getLiveMatchInfo(marketCat.Event.Name);

                            if (curMatchInfo == null)
                            {
                                LogString("Live MatchInfo = null!");
                                continue;
                            }

                            LogString(string.Format("Actual Score: {0}-{1}", curMatchInfo.homeScore, curMatchInfo.awayScore));

                            if (curMatchInfo.homeScore == curMatchInfo.awayScore)
                            {
                                //make back at the draw
                                LogString("To Place Order after HT to " + marketCat.Event.Name);
                                placeOrderForHT(marketCat);
                                LogString("Placed Order after HT to " + marketCat.Event.Name);
                            }
                        }
                    }
                    LogString("Fetched games to wait (" + _listToWait.Count.ToString() + ")");
                }
                catch(Exception e)
                {
                    LogString(e.Message);
                }
            }
        }

        private bool isExistPendingBets(List<Bet> bets)
        {
            try
            {
                if (bets == null)
                    return true;

                if (bets.Count < 1)
                    return false;

                showBetsHistory(bets);

                foreach(Bet bet in bets)
                {
                    if (bet.status == "Open" || bet.status == "Pending")
                        return true;
                }

                return false;
            }
            catch(System.Exception)
            {
                return false;
            }
        }
        
        private void doSleep(Task task)
        {
            while (!task.IsCompleted)
            {
                Application.DoEvents();
                Thread.Sleep(50);
            }
            for (int k = 0; k < 40; k++)
            {
                Application.DoEvents();
                Thread.Sleep(50);
            }
        }


        private bool isExist(int lineId)
        {

            return false;
        }

        

        private void Slave_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            refreshControls(true);
        }

        private void Pause(int secs)
        {
            for (int i = 0; i < (secs * 20); i++)
            {
                if (this._slave.CancellationPending)
                {
                    return;
                }
                Application.DoEvents();
                Thread.Sleep(50);
            }
        }

        private void timerKeep_Tick(object sender, EventArgs e)
        {
            if (!keepService())
            {
                progressStop();
                this.Close();
            }
        }

        private bool keepService()
        {
            try
            {
                HttpClient httpClientKeep = new HttpClient();
                HttpResponseMessage responseMessageKeep = httpClientKeep.GetAsync("http://61.36.33.194:10386/keep_sbobet/index.php").Result;
                responseMessageKeep.EnsureSuccessStatusCode();

                string responseMessageKeepString = responseMessageKeep.Content.ReadAsStringAsync().Result;
                if (string.IsNullOrEmpty(responseMessageKeepString))
                    return false;

                if (responseMessageKeepString != "success")
                    return false;

                return true;
            }
            catch (System.Exception e)
            {
                return false;
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            frmStake frm = new frmStake();
            if (frm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                addStake(frm.stake);
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            if (tblStake.SelectedRows == null || tblStake.SelectedRows.Count != 1)
            {
                Messagebox.show("Please select correct item!");
                return;
            }

            delStake(tblStake.SelectedRows[0].Index);
        }

        private void btnModify_Click(object sender, EventArgs e)
        {
            if (tblStake.SelectedRows == null || tblStake.SelectedRows.Count != 1)
            {
                Messagebox.show("Please select correct item!");
                return;
            }

            frmStake frm = new frmStake();
            int index = tblStake.SelectedRows[0].Index;
            double oldStake = (double)tblStake.SelectedRows[0].Tag;
            if (oldStake < 0)
            {
                Messagebox.show("Cannot select this item!");
                return;
            }

            frm.stake = oldStake;
            if (frm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                updateStake(index, frm.stake);
            }
        }

        private void btnMax_Click(object sender, EventArgs e)
        {
            if(this.WindowState == FormWindowState.Normal)
            {
                this.WindowState = FormWindowState.Maximized;
                btnMax.BackgroundImage = Properties.Resources.Restore;
            }
            else if(this.WindowState == FormWindowState.Maximized)
            {
                this.WindowState = FormWindowState.Normal;
                btnMax.BackgroundImage = Properties.Resources.Maximize;
            }
        }

        private void bindingSource_CurrentChanged(object sender, EventArgs e)
        {

        }

        private void showBetsHistory(List<Bet> bets)
        {
            this.Invoke(new Action(() =>
                {
                    bindingSource.DataSource = bets;
                    bindingSource.ResetBindings(false);
                }));
        }
    }
}
