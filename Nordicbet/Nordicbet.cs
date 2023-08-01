using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using System.Net;
using System.Net.Http;
using System.Globalization;
using Api_ng_sample_code.TO;
using Newtonsoft.Json;

using Nordicbet.Json;

using HtmlAgilityPack;
using Exception = System.Exception;

namespace Nordicbet
{
    public class Nordicbet
    {
        NumberStyles style = NumberStyles.Number | NumberStyles.AllowCurrencySymbol | NumberStyles.AllowDecimalPoint;
        CultureInfo culture = CultureInfo.CreateSpecificCulture("en-GB");

        private HttpClient httpClient = null;
        private CookieContainer container = null;
        private onWriteStatusEvent onWriteStatus = null;

        public string sessionToken = null;
        private string token = string.Empty;
        private string merchantId = string.Empty;

        public Nordicbet(onWriteStatusEvent _onWriteStatus)
        {
            if (httpClient == null)
                initHttpClient();

            onWriteStatus = _onWriteStatus;
        }

        private void initHttpClient()
        {
            container = new CookieContainer();
            HttpClientHandler handler = new HttpClientHandler();
            handler.CookieContainer = container;
            httpClient = new HttpClient(handler);

            initHttpHeader();
        }

        private void initHttpHeader()
        {
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept",
                "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
            ;
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent",
                "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.103 Safari/537.36");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Upgrade-Insecure-Requests", "1");
        }

        private string getLogTitle()
        {
            return string.Format("[{0}] ", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        }

        public async Task<List<SoccerVistaInfo>> buildSVList(double minPercent, double maxPercent, int deltaOffset = -1)
        {
            List<SoccerVistaInfo> svList = new List<SoccerVistaInfo>();
            try
            {
                HttpResponseMessage responseMessageMain = await httpClient.GetAsync("http://www.soccervista.com/soccer_leagues_ordered_by_number_of_draws.php");
                responseMessageMain.EnsureSuccessStatusCode();

                string responseMessageStateString = await responseMessageMain.Content.ReadAsStringAsync();
                if (string.IsNullOrEmpty(responseMessageStateString))
                    return null;

                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(responseMessageStateString);

                string[] strClasses = { "onem1", "onem2", "onem", "onem4", "onem5" };

                foreach (string strClass in strClasses)
                {
                    IEnumerable<HtmlNode> nodeInfos =
                        doc.DocumentNode.Descendants("tr")
                            .Where(node => node.Attributes["class"] != null && node.Attributes["class"].Value == strClass);
                    if (nodeInfos == null || nodeInfos.LongCount() < 1)
                        return null;

                    for (int i = 0; i < nodeInfos.LongCount(); i++)
                    {
                        IEnumerable<HtmlNode> nodeTds = nodeInfos.ToArray()[i].Descendants("td");
                        if (nodeTds == null || nodeTds.LongCount() != 7)
                            continue;

                        SoccerVistaInfo _newSVInfo = new SoccerVistaInfo();

                        _newSVInfo.country = nodeTds.ToArray()[1].InnerText;
                        _newSVInfo.league = nodeTds.ToArray()[2].InnerText;
                        _newSVInfo.homewin = int.Parse(nodeTds.ToArray()[3].InnerText);
                        _newSVInfo.homedraw = int.Parse(nodeTds.ToArray()[4].InnerText);
                        _newSVInfo.homelost = int.Parse(nodeTds.ToArray()[5].InnerText);
                        _newSVInfo.percent = double.Parse(nodeTds.ToArray()[6].InnerText);

                        if (_newSVInfo.percent > maxPercent)
                            continue;
                        if (_newSVInfo.percent < minPercent)
                            continue;

                        if (deltaOffset != -1 && Math.Abs(_newSVInfo.homewin - _newSVInfo.homelost) > deltaOffset)
                            continue;

                        IEnumerable<HtmlNode> nodeAs = nodeTds.ToArray()[2].Descendants("a");

                        if (nodeAs == null || nodeAs.LongCount() < 1)
                            continue;

                        _newSVInfo.leagueUrl = nodeAs.ToArray()[0].GetAttributeValue("href", "");

                        svList.Add(_newSVInfo);
                    }
                }

                return svList;
            }
            catch (System.Exception e)
            {
                return null;
            }
        }

        public async Task<List<SoccerVistaInfo>> buildMatchInfos(List<SoccerVistaInfo> svList)
        {
            for (int idx = 0; idx < svList.LongCount(); idx++)
            {
                SoccerVistaInfo svItem = svList[idx];

                try
                {
                    HttpResponseMessage responseMessageMain = await httpClient.GetAsync("http://www.soccervista.com/" + svItem.leagueUrl);
                    responseMessageMain.EnsureSuccessStatusCode();

                    string responseMessageStateString = await responseMessageMain.Content.ReadAsStringAsync();
                    if (string.IsNullOrEmpty(responseMessageStateString))
                        return null;

                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(responseMessageStateString);

                    IEnumerable<HtmlNode> nodeInfos =
                        doc.DocumentNode.Descendants("table")
                            .Where(
                                node => node.Attributes["class"] != null && node.Attributes["class"].Value == "upcoming");
                    if (nodeInfos == null || nodeInfos.LongCount() < 1)
                        return null;
                    IEnumerable<HtmlNode> nodeTBodys = nodeInfos.ToArray()[0].Descendants("tbody");
                    if (nodeTBodys == null || nodeTBodys.LongCount() < 1)
                        return null;

                    IEnumerable<HtmlNode> nodeTrs = nodeTBodys.ToArray()[0].Descendants("tr");
                    if (nodeTBodys == null || nodeTBodys.LongCount() != 1)
                        continue;

                    for (int i = 0; i < nodeTrs.LongCount(); i++)
                    {
                        IEnumerable<HtmlNode> nodeTds = nodeTrs.ToArray()[i].Descendants("td");
                        if (nodeTds == null || nodeTds.LongCount() != 11)
                            continue;

                        MatchInfo _newMatchInfo = new MatchInfo();

                        _newMatchInfo.date = nodeTds.ToArray()[0].InnerText;
                        _newMatchInfo.round = int.Parse(nodeTds.ToArray()[1].InnerText);
                        _newMatchInfo.homeTeam = nodeTds.ToArray()[2].Descendants("a").ToArray()[0].InnerText;
                        _newMatchInfo.awayTeam = nodeTds.ToArray()[3].Descendants("a").ToArray()[0].InnerText;
                        _newMatchInfo.homeWin = double.Parse(nodeTds.ToArray()[4].Descendants("a").ToArray()[0].InnerText);
                        _newMatchInfo.homeDraw = double.Parse(nodeTds.ToArray()[5].Descendants("a").ToArray()[0].InnerText);
                        _newMatchInfo.homeLost = double.Parse(nodeTds.ToArray()[6].Descendants("a").ToArray()[0].InnerText);
                        _newMatchInfo.resultPrediction = nodeTds.ToArray()[7].InnerText;
                        _newMatchInfo.resultTip = nodeTds.ToArray()[8].InnerText;
                        _newMatchInfo.resultPick = nodeTds.ToArray()[9].InnerText;
                        _newMatchInfo.eventName = _newMatchInfo.homeTeam + " v " + _newMatchInfo.awayTeam;

                        svItem.matchInfos.Add(_newMatchInfo);
                    }

                    svList[idx] = svItem;
                }
                catch (System.Exception e)
                {
                    string str;
                    str = "";
                }
            }
            return svList;
        }

        public async Task<LoginResult> login()
        {
            LoginResult loginResult = new LoginResult();

            try
            {
                onWriteStatus(getLogTitle() + "Trying to login to betfair...");

                bool bLogin = await doLogin();
                if (!bLogin)
                {
                    onWriteStatus(getLogTitle() + "Login Error!");
                    loginResult.error = "Login Error!";
                    return loginResult;
                }

                onWriteStatus(getLogTitle() + "Getting the events...");
                List<JsonEl> elList = await getEvents();
                if (elList == null || elList.Count < 1)
                {
                    onWriteStatus(getLogTitle() + "No Events!");
                    loginResult.error = "No Events!";
                    return loginResult;
                }

                elList = getTodayEvents(elList);
                if (elList == null || elList.Count < 1)
                {
                    onWriteStatus(getLogTitle() + "No Today Events!");
                    loginResult.error = "No Today Events!";
                    return loginResult;
                }

                List<JsonElExt> elExtList = getFilterEvents(elList);
                if (elExtList == null || elList.Count < 1)
                {
                    onWriteStatus(getLogTitle() + "No Draw Events!");
                    loginResult.error = "No Draw Events!";
                    return loginResult;
                }

                loginResult.el = elExtList;
                loginResult.success = true;
                return loginResult;
            }
            catch (Exception e)
            {
                return loginResult;
            }
        }

        public List<LiveMatchInfo> getLiveMatchInfos(List<MarketCatalogue> allMarketCatalogues)
        {
            List<LiveMatchInfo> liveMatchInfos = new List<LiveMatchInfo>();

            LiveMatchInfo _newMatchInfo = null;

            try
            {
                HttpResponseMessage responseMessageMain = httpClient.GetAsync("https://www.betfair.com/exchange/inplay").Result;
                responseMessageMain.EnsureSuccessStatusCode();

                string responseMessageStateString = responseMessageMain.Content.ReadAsStringAsync().Result;
                if (string.IsNullOrEmpty(responseMessageStateString))
                    return null;

                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(responseMessageStateString);

                IEnumerable<HtmlNode> nodeInfoms = doc.DocumentNode.Descendants("td").Where(node => node.Attributes["class"] != null && node.Attributes["class"].Value == "name");

                if (nodeInfoms == null || nodeInfoms.LongCount() < 1)
                    return null;

                foreach (HtmlNode nodeInfos in nodeInfoms)
                {
                    string homeName = "";
                    string score = "";
                    string awayName = "";
                    string currentTime = "";

                    try
                    {
                        homeName = nodeInfos.Descendants("span").Where(node => node.Attributes["class"] != null && node.Attributes["class"].Value == "home-team").ToArray()[0].InnerText;
                        score = nodeInfos.Descendants("span").Where(node => node.Attributes["class"] != null && node.Attributes["class"].Value == "result").ToArray()[0].InnerText;
                        awayName = nodeInfos.Descendants("span").Where(node => node.Attributes["class"] != null && node.Attributes["class"].Value == "away-team").ToArray()[0].InnerText;
                        currentTime = nodeInfos.Descendants("span").Where(node => node.Attributes["class"] != null && node.Attributes["class"].Value == "start-time period-emphasis").ToArray()[0].InnerText;
                    }
                    catch (Exception)
                    {
                        continue;
                    }

                    if(homeName.Length < 3 || awayName.Length < 3 || score.Length < 5 || currentTime.Length < 2)
                        continue;

                    if (!score.Contains("-"))
                        continue;

                    foreach(MarketCatalogue itemMarket in allMarketCatalogues)
                    {
                        if(itemMarket.Event.Name.Contains(homeName) && itemMarket.Event.Name.Contains(awayName))
                        {
                            _newMatchInfo = new LiveMatchInfo();
                            _newMatchInfo.homeName = homeName;
                            _newMatchInfo.awayName = awayName;
                            _newMatchInfo.homeScore = int.Parse(score.Substring(0, score.IndexOf(" - ")));
                            _newMatchInfo.awayScore = int.Parse(score.Substring(score.IndexOf(" - ") + 3));
                            _newMatchInfo.currentTime = currentTime.Substring(0, currentTime.Length);

                            liveMatchInfos.Add(_newMatchInfo);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                onWriteStatus(e.Message);
                return null;
            }
            return liveMatchInfos;
        }

        public async Task<bool> doLogin()
        {
            try
            {
                HttpResponseMessage responseMessageMain = await httpClient.GetAsync("http://www.betfair.com/");
                responseMessageMain.EnsureSuccessStatusCode();

                string mainReferer = responseMessageMain.RequestMessage.RequestUri.AbsoluteUri;
                if (string.IsNullOrEmpty(mainReferer))
                    return false;

                string responseMessageMainString = await responseMessageMain.Content.ReadAsStringAsync();
                if (string.IsNullOrEmpty(responseMessageMainString))
                    return false;

                HtmlNode.ElementsFlags.Remove("form");
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(responseMessageMainString);

                IEnumerable<HtmlNode> nodeForms = doc.DocumentNode.Descendants("form");
                if (nodeForms == null || nodeForms.LongCount() < 1)
                    return false;

                string action = nodeForms.ToArray()[0].GetAttributeValue("action", "");
                if (string.IsNullOrEmpty(action))
                    return false;

                IEnumerable<HtmlNode> nodeInputs =
                    nodeForms.ToArray()[0].Descendants("input").Where(node => node.Attributes["name"] != null);
                if (nodeInputs == null || nodeInputs.LongCount() < 1)
                    return false;

                string refererUrl = string.Empty;
                List<KeyValuePair<string, string>> inputs = new List<KeyValuePair<string, string>>();
                foreach (HtmlNode nodeInput in nodeInputs)
                {
                    string inputName = nodeInput.GetAttributeValue("name", "");
                    if (string.IsNullOrEmpty(inputName))
                        continue;

                    string inputValue = nodeInput.GetAttributeValue("value", "");
                    if (inputValue == null)
                        inputValue = string.Empty;

                    if (inputName == "username")
                        inputValue = Settings.Instance.username;

                    if (inputName == "password")
                        inputValue = Settings.Instance.password;

                    if (inputName == "ioBlackBox")
                        inputValue =
                            "0400R9HVeoYv1gsNf94lis1ztshEYOhBj8SQVGjDxyKtjEYJx9qSw/0rX3UPUavsVGNNP8qFSAZ42op95eoW86m0kwJH0I8JjLitr3JYlvaDSSoJdOHStouX5odMpSRnzNZujwZOoEVFPaadm2GNPflphIK4mjKAMqULsj69LfM7VRbla+gcYC1FQHWwCqQ/IvlFFmQWppRl6Bv4pp48B2PR0LUM6Rn3JtHEfF9hXdZ4DRRiwxmZVjl9I8/xOY8SaJdW7jj6V+dG7j9MyPpjzYuuV+VhT2JQxvTdTn3ScfrBfYTh6ImRigH/M5hg3W9g0TRnvoLZ8SpzAFLDWCmiO4sAs/qr/BWAZmps3sMxM3Tsu4+Oh9ltHZfQIggPoVjqnTR6FC6A4XHytZ7Tn6b+z24SKe+wPwB7YcyEaf1wmOpmO8iM1eyAwSCEbXPCKc6iDsD88+g/mDVDCxFKZ8U9CkoINzcLZ9/2RBpYQZsdELNYToJhO1Q2jmVgfPrA9XDC0LRFicgHXRoRWukWp8XlE1kYrSaUGABVKw5X0ikb/3h1d0Im6Fy7sPS5usVQSvGb6Q+XXiK1sQAL4CPUyKVnDKqtQrhW1HQIpGtAAfOPjzqyODIf9I+PRImeQ49qpLt2ZL85hw6+vl8ZjUN3p2VjxRMgSIkpIh3A707ln/wqsPiGBcfEpkWTlnMi5g4mmYKxVbRrSUpGCTU5RhDBmCQpzv+TwwM6wvQ9/pjGYj2YJXfnh939vTlgH8k0GP35GUqEwdlQH6AzAyX2sumr8GO9YMyocNGc3h38I509+L9GpM8mFWoWiIYYjbEvXCXVTb29yNd9DKThVP1hAyB7YMdbHsyiYBQ1h4tNGHTh8CNjY0flnyuelCnt2ypgfW/m3Zqxt3w1Bn6LxoWOnZu64rr07LwRGSM2wu7XD+lw1lMSgm8hl0E6JnXlF7diYNwTidneZ5WeqDQRJDWdY1eYOcNXGWh5eh0R9vRNj5evLzdF6XhbUceysRqfEatYu7JOeKvET7v2DVHRyVdfAWJVJqKtzBRNTqF/WnZgA3qvxiWvyGPt4xFJHNu5eG3HXPo1chRhwl6109lnHDNLhXiDaeModH3MykVCcuAzYjmBq2CObWM3m2Jxa2GEjcGvISFrwoQO0Yr3iynFSjKIXJbJoUCUMhuSPrCT16pHvpt1JeGMGYj7gJYGwcZRDWBe40N3RSizQUPUSdu3DZ2D0kFXG69EaoZ1RI0f9uYinjZJCJitlNUbN+M7DqaJyGOl8Gv+HZ+NFeJF+TjyWQJjI3NMC63Cwm7dHkusepnG3VLacD4ALLQF1a0FYqvfxn854SSXEvUuPt0pFsY23fB/+pEjrnavoBjx28jKWCbjdtkiyP5Y9ou+oQuf/feQ29ef/02xPtU9AeQHnnkJD5ghHNv+1znnh45aMwwjA2Ui9cgPlAgeaCqCy3cXjx33ffITvy2wv1MWLu4AO0fwrpVgx/siiN2XyvHCfqE2r+5inDLSN1TCyOxhTq/let4ltP27ta6LO+lD+YmKtgN4Oh10Gib60E9YGc40dTHbDZ5kt2K49+MixLQpfdYCKC/P29Tu8zwBp1GJQgbFvd7r7/TrYV9DpG+vj9JhImGqAbqHUNQ4M2FEG+0RRIU=";

                    if (inputName == "url")
                        refererUrl = inputValue;

                    inputs.Add(new KeyValuePair<string, string>(inputName, inputValue));
                }

                httpClient.DefaultRequestHeaders.Referrer = new Uri(mainReferer);
                HttpResponseMessage responseMessageLogin = await httpClient.PostAsync(action,(HttpContent) new FormUrlEncodedContent((IEnumerable<KeyValuePair<string, string>>) inputs));
                responseMessageLogin.EnsureSuccessStatusCode();

                string responseMessageLoginString = await responseMessageLogin.Content.ReadAsStringAsync();
                if (string.IsNullOrEmpty(responseMessageLoginString))
                    return false;

                doc.LoadHtml(responseMessageLoginString);

                nodeForms =
                    doc.DocumentNode.Descendants("form")
                        .Where(node => node.Attributes["name"] != null && node.Attributes["name"].Value == "postLogin");
                if (nodeForms == null || nodeForms.LongCount() < 1)
                    return false;

                action = nodeForms.ToArray()[0].GetAttributeValue("action", "");
                if (string.IsNullOrEmpty(action))
                    return false;

                nodeInputs = nodeForms.ToArray()[0].Descendants("input").Where(node => node.Attributes["name"] != null);
                if (nodeInputs == null || nodeInputs.LongCount() < 1)
                    return false;

                inputs = new List<KeyValuePair<string, string>>();
                foreach (HtmlNode nodeInput in nodeInputs)
                {
                    string name = nodeInput.GetAttributeValue("name", "");
                    if (string.IsNullOrEmpty(name))
                        continue;

                    string value = nodeInput.GetAttributeValue("value", "");
                    if (value == null)
                        value = string.Empty;

                    inputs.Add(new KeyValuePair<string, string>(name, value));
                }

                httpClient.DefaultRequestHeaders.Referrer =
                    new Uri(string.Format("https://identitysso.betfair.com/view/login?product=prospect-page&url={0}",
                        WebUtility.UrlEncode(refererUrl)));
                HttpResponseMessage responseMessagePostLogin = await httpClient.PostAsync(action,(HttpContent) new FormUrlEncodedContent((IEnumerable<KeyValuePair<string, string>>) inputs));
                responseMessagePostLogin.EnsureSuccessStatusCode();

                string responseMessagePostLoginString = await responseMessagePostLogin.Content.ReadAsStringAsync();
                if (string.IsNullOrEmpty(responseMessagePostLoginString))
                    return false;

                if (!responseMessagePostLoginString.Contains(Settings.Instance.username))
                    return false;

                CookieCollection cookies = container.GetCookies(new Uri("https://www.betfair.com/"));
                if (cookies == null || cookies.Count < 1)
                    return false;

                foreach (Cookie cookie in cookies)
                {
                    if (cookie.Name == "ssoid")
                    {
                        sessionToken = cookie.Value;
                        break;
                    }
                }

                return true;
            }
            catch (APINGException e)
            {
//                if (e.Message.Contains(""))
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }

        private bool setToken(string callbackUrl)
        {
            GroupCollection groups = Regex.Match(callbackUrl, "token=(?<token>.*)&sid=").Groups;
            if (groups == null || groups["token"] == null)
                return false;

            token = groups["token"].Value;
            if (string.IsNullOrEmpty(token))
                return false;

            return true;
        }

        private bool setMerchantId(string content)
        {
            GroupCollection groups = Regex.Match(content, "\"merchantId\":\"(?<merchantId>\\d*)\"").Groups;
            if (groups == null || groups["merchantId"] == null)
                return false;

            merchantId = groups["merchantId"].Value;
            if (string.IsNullOrEmpty(merchantId))
                return false;

            return true;
        }

        private async Task<List<JsonEl>> getEvents()
        {
            List<JsonEl> elList = new List<JsonEl>();

            try
            {
                httpClient.DefaultRequestHeaders.Remove("Accept");
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
                HttpResponseMessage responseMessageMain = await httpClient.GetAsync("https://www.nordicbet.com/en/odds");
                responseMessageMain.EnsureSuccessStatusCode();

                string responseMessageMainString = await responseMessageMain.Content.ReadAsStringAsync();
                if (string.IsNullOrEmpty(responseMessageMainString))
                    return elList;

                //GroupCollection groups = Regex.Match(responseMessageMainString, "sbReloadURL_callback\\('(?<callbackUrl>.*)' \\+ anchorValue\\);").Groups;
                GroupCollection groups = Regex.Match(responseMessageMainString, "sbReloadURL_callback\\('(?<callbackUrl>.*)'\\);").Groups;
                if (groups == null || groups["callbackUrl"] == null)
                    return elList;

                string callbackUrl = groups["callbackUrl"].Value;
                if (string.IsNullOrEmpty(callbackUrl))
                    return elList;

                callbackUrl = WebUtility.UrlDecode(callbackUrl);

                if (!setToken(callbackUrl))
                    return elList;

                httpClient.DefaultRequestHeaders.Referrer = new Uri("https://www.nordicbet.com/en/odds");
                HttpResponseMessage responseMessageCallback = await httpClient.GetAsync(callbackUrl);
                responseMessageCallback.EnsureSuccessStatusCode();

                string responseMessageCallbackString = await responseMessageCallback.Content.ReadAsStringAsync();
                if (string.IsNullOrEmpty(responseMessageCallbackString))
                    return elList;

                if (!setMerchantId(responseMessageCallbackString))
                    return elList;

                httpClient.DefaultRequestHeaders.Remove("Accept");
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "application/json, text/plain, */*");
                httpClient.DefaultRequestHeaders.Referrer = new Uri(callbackUrl);

                HttpResponseMessage responseMessageGroup = await httpClient.GetAsync("https://sbsitefacade2.bpsgameserver.com/isa/v2/901/en/betGroupGrouping?subCategoryIds=6134,2612,665,3,377,4,5,6,12,645,15,1874,16,9,2315,1036,4000,1993,3263,26,5611,9479,158,156,9690,159,4539,8172,10717,10718,8228,6362,10769,10767,19,1215,6049,13334,1415,2651,25,250,128,129,134,130,133,734,4653,3413,3417,3418,3419,3420,3421,3422,3423,3424,3425,3426,5989,5979,6007,6005,5995,5996,5983,5984,5985,6004,6000,5980,5981,5982,5976,5991,5992,5993,5997,5999,5988,6006,6001,5998,5987,12994,12995,12996,12756,12853,12852,12857,12997,12649,12650,12758,12694,12602,10858,10863,10908,10750,10820,10843,12744,12879,12747,12746,12762,12771,12769,12763,12770,12757,12759,12781,12782,12783,12690,12691,12692,12693,12619,12766,12767,12768,12731,10741,10742,10721,10722,10723,10707,10705,10706,10739,10715,10716,10720,10724,10725,10737,6081,2128,6375,6376,361,353,9689,1228,610,38,1116,3486,5292,1,108,489,488,4309,110,2376,2377,2378,2383,10609,10294,10295,10296,10301,10299,10300,10302,10305,10306,10307,10308,10312,10315,10314,10317,10316,10319,10318,10321,10322,10323,10320,10324,10325,10326,10327,10328,10329,10330,10309,10311,10310,12864,12865,12793,12797,12800,12801,10777,12794,12796,12799,12802,12803,12804,12806,12807,12809,12808,12810,12813,12811,12812,12815,12816,12814,12817,12818,12819,12821,12820,12822,12824,12823,12825,12826,12827,12844,12829,12830,12831,12832,12835,12836,12834,12833,12837,12838,12839,12843,12842,12841,33,1609,8738,325,505,29,125,45,35,151,5035");
                responseMessageGroup.EnsureSuccessStatusCode();

                int page = 1;
                int pageCnt = 1;
                while(true)
                {
                    HttpResponseMessage responseMessageEvents = await httpClient.GetAsync(string.Format("https://sbsitefacade2.bpsgameserver.com/isa/v2/901/en/event?marketCount=50&page={0}&subCategoryIds=6134,2612,665,3,377,4,5,6,12,645,15,1874,16,9,2315,1036,4000,1993,3263,26,5611,9479,158,156,9690,159,4539,8172,10717,10718,8228,6362,10769,10767,19,1215,6049,13334,1415,2651,25,250,128,129,134,130,133,734,4653,3413,3417,3418,3419,3420,3421,3422,3423,3424,3425,3426,5989,5979,6007,6005,5995,5996,5983,5984,5985,6004,6000,5980,5981,5982,5976,5991,5992,5993,5997,5999,5988,6006,6001,5998,5987,12994,12995,12996,12756,12853,12852,12857,12997,12649,12650,12758,12694,12602,10858,10863,10908,10750,10820,10843,12744,12879,12747,12746,12762,12771,12769,12763,12770,12757,12759,12781,12782,12783,12690,12691,12692,12693,12619,12766,12767,12768,12731,10741,10742,10721,10722,10723,10707,10705,10706,10739,10715,10716,10720,10724,10725,10737,6081,2128,6375,6376,361,353,9689,1228,610,38,1116,3486,5292,1,108,489,488,4309,110,2376,2377,2378,2383,10609,10294,10295,10296,10301,10299,10300,10302,10305,10306,10307,10308,10312,10315,10314,10317,10316,10319,10318,10321,10322,10323,10320,10324,10325,10326,10327,10328,10329,10330,10309,10311,10310,12864,12865,12793,12797,12800,12801,10777,12794,12796,12799,12802,12803,12804,12806,12807,12809,12808,12810,12813,12811,12812,12815,12816,12814,12817,12818,12819,12821,12820,12822,12824,12823,12825,12826,12827,12844,12829,12830,12831,12832,12835,12836,12834,12833,12837,12838,12839,12843,12842,12841,33,1609,8738,325,505,29,125,45,35,151,5035&betgroupgroupingids=36&betgroupgroupingids=36&eventPhase=1", page));
                    responseMessageEvents.EnsureSuccessStatusCode();

                    string responseMessageEventsString = await responseMessageEvents.Content.ReadAsStringAsync();
                    if (string.IsNullOrEmpty(responseMessageEventsString))
                        break;

                    JsonEvent jsonEvent = JsonConvert.DeserializeObject<JsonEvent>(responseMessageEventsString);
                    if (jsonEvent == null || jsonEvent.el == null)
                        break;

                    pageCnt = jsonEvent.tp;
                    elList.AddRange(jsonEvent.el);

                    page++;
                    if (page > pageCnt)
                        break;
                }

                return elList;
            }
            catch(Exception e)
            {
                return elList;
            }
        }

        private List<JsonEl> getTodayEvents(List<JsonEl> elList)
        {
            DateTime now = DateTime.Now;

            List<JsonEl> newElList = new List<JsonEl>();
            foreach(JsonEl el in elList)
            {
                DateTime sd = DateTime.Now;
                if (!DateTime.TryParse(el.sd, out sd))
                    continue;

                if (sd < now)
                    continue;

                if (sd > new DateTime(now.Year, now.Month, now.Day, 23, 59, 59))
                    continue;

                newElList.Add(el);
            }

            return newElList;
        }

        private List<JsonElExt> getFilterEvents(List<JsonEl> elList)
        {
            List<JsonElExt> newElList = new List<JsonElExt>();
            foreach(JsonEl el in elList)
            {
                if (el.ml == null || el.ml.Count < 1)
                    continue;

                foreach(JsonElMl ml in el.ml)
                {
                    if (ml.bgn != "Match result")
                        continue;

                    if (ml.msl == null || ml.msl.Count < 3)
                        continue;

                    double home = ml.msl[0].msp;
                    double draw = ml.msl[1].msp;
                    double away = ml.msl[2].msp;

                    if (home < 1 || draw < 1 || away < 1)
                        continue;

                    if (draw < 3)
                        continue;

                    if (Settings.Instance.useSideOddsMin && (home < Settings.Instance.sideOddsMin || away < Settings.Instance.sideOddsMin))
                        continue;

                    if (Settings.Instance.useSideOddsMax && (home > Settings.Instance.sideOddsMax || away > Settings.Instance.sideOddsMax))
                        continue;

                    if (Math.Abs(home - away) > Settings.Instance.sideDelta)
                        continue;

                    newElList.Add(new JsonElExt(el, Math.Abs(home - away)));
                    break;
                }
            }

            newElList = newElList.OrderBy(o => o.diff).ToList();
            return newElList;
        }

        public async Task<List<Bet>> getBetHistory()
        {
            List<Bet> bets = new List<Bet>();

            try
            {
                //string betHistoryUrl = string.Format("https://bettingwebapi.bpsgameserver.com/bettingwebapi/api/BetHistory?sessionId={0}&merchantId={1}&fromDate={2}&toDate={3}&numberOfCoupons={4}&betStatus=0&pageNumber=0", token, merchantId, Constants.start.ToString("yyyy-MM-dd") + " 00:00:00", DateTime.Now.ToString("yyyy-MM-dd") + " 23:59:59", Settings.Instance.betMax);
                string betHistoryUrl = string.Format("https://uk.site.sports.betfair.com/reporting/bettingpandl/BettingPandLForSport.do?timeperiod=today&startDateString={0}+00%3A00&endDateString={1}+23%3A59", Constants.start.ToString("yyyy-MM-dd"), DateTime.Now.ToString("yyyy-MM-dd"));

                httpClient.DefaultRequestHeaders.Remove("Accept");
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "application/json, text/javascript, */*; q=0.01");

                HttpResponseMessage responseMessageBetHistory = await httpClient.GetAsync(betHistoryUrl);
                responseMessageBetHistory.EnsureSuccessStatusCode();

                string responseMessageBetHistoryString = await responseMessageBetHistory.Content.ReadAsStringAsync();
                if (string.IsNullOrEmpty(responseMessageBetHistoryString))
                    return null;

//                HtmlDocument doc = new HtmlDocument();
//                doc.LoadHtml(responseMessageBetHistoryString);
//
//                IEnumerable<HtmlNode> nodeInfos = doc.DocumentNode.Descendants("tr").Where(node => node.Attributes["class"] != null && node.Attributes["class"].Value == strClass);
//                if (nodeInfos == null || nodeInfos.LongCount() < 1)
//                    return null;

//for (int i = 0; i < nodeInfos.LongCount(); i++)
//{
//    IEnumerable<HtmlNode> nodeTds = nodeInfos.ToArray()[i].Descendants("td");

//                JsonBetHistory history = JsonConvert.DeserializeObject<JsonBetHistory>(responseMessageBetHistoryString);
//                if (history == null || history.betHistoryItems == null)
//                    return null;
//
//                foreach(JsonBetHistoryItem historyItem in history.betHistoryItems)
//                {
//                    string betHistoryDetailUrl = string.Format("https://bettingwebapi.bpsgameserver.com/bettingwebapi/api/BetHistoryDetails?merchantId={0}&sessionId={1}&couponId={2}&timeZone=Romance%2520Standard%2520Time&segmentId=901&languageCode=en", merchantId, token, historyItem.couponId);
//                    HttpResponseMessage responseMessageHistoryDetail = await httpClient.GetAsync(betHistoryDetailUrl);
//                    responseMessageHistoryDetail.EnsureSuccessStatusCode();
//
//                    string responseMessageHistoryDetailString = await responseMessageHistoryDetail.Content.ReadAsStringAsync();
//                    if (string.IsNullOrEmpty(responseMessageHistoryDetailString))
//                        continue;
//
//                    JsonBetHistoryItemDetail historyItemDetail = JsonConvert.DeserializeObject<JsonBetHistoryItemDetail>(responseMessageHistoryDetailString);
//                    if (historyItemDetail == null)
//                        continue;
//
//                    if (historyItemDetail.coupon == null || historyItemDetail.coupon.bets == null || historyItemDetail.coupon.bets.Count < 1 || historyItemDetail.coupon.bets[0] == null || historyItemDetail.coupon.bets[0].betSelections == null || historyItemDetail.coupon.bets[0].betSelections.Count < 1 || historyItemDetail.market == null || historyItemDetail.market.Count < 1 || historyItemDetail.subCategory == null || historyItemDetail.subCategory.Count < 1)
//                        continue;
//
//                    Bet bet = getBetInfoFromHistory(historyItem, historyItemDetail);
//                    if (bet == null)
//                        continue;
//
//                    bets.Add(bet);
//                }

                return bets;
            }
            catch(Exception e)
            {
                return null;
            }
        }

        public async Task<double> getBalance()
        {
            try
            {
                HttpResponseMessage responseMessageBalance = await httpClient.PostAsync("https://www.nordicbet.com/en/odds/web/Service/LobbyService.svc/GetBalance", new StringContent("{\"bonusContext\":\"NordicBet.Sportsbook\"}", Encoding.UTF8, "application/json"));
                responseMessageBalance.EnsureSuccessStatusCode();

                string responseMessageBalanceString = await responseMessageBalance.Content.ReadAsStringAsync();
                if (string.IsNullOrEmpty(responseMessageBalanceString))
                    return -1;

                GroupCollection groups = Regex.Match(responseMessageBalanceString, "\"FundAmount\":(?<balance>(\\d*\\.\\d*|\\d*)),").Groups;
                if (groups == null || groups["balance"] == null)
                    return -1;

                double balance = -1;
                if (double.TryParse(groups["balance"].Value, style, culture, out balance))
                    return balance;
                else
                    return -1;
            }
            catch(Exception e)
            {
                return -1;
            }
        }

        private Bet getBetInfoFromHistory(JsonBetHistoryItem historyItem, JsonBetHistoryItemDetail historyItemDetail)
        {
            Bet bet = new Bet();

            DateTime ad = DateTime.Now;
            if (DateTime.TryParse(historyItem.couponArriveDate, out ad))
                bet.time = ad.ToString("dd/MM/yyyy HH:mm");
            else
                return null;

            bet.status = Constants.getBetStatus(historyItem.betStatus);
            bet.odds = historyItemDetail.coupon.bets[0].betSelections[0].odds;
            bet.stake = historyItem.stake;
            bet.match = historyItemDetail.market[0].eventName;
            bet.league = historyItemDetail.subCategory[0].regionName + "-" + historyItemDetail.subCategory[0].subCategoryName;

            return bet;
        }    
    
        public async Task<bool> placeBet(JsonEl el, double stake)
        {
            try
            {
                onWriteStatus(getLogTitle() + string.Format("Placing bet to {0}, with stake {1}", el.en, stake));

                string param = string.Format("{{\"Unblock\":\"false\",\"BrandName\":\"NordicBet.com\",\"SegmentId\":\"901\",\"SessionId\":\"{0}\",\"TimezoneName\":\"Romance Standard Time\",\"CouponRequest\":{{\"Currency\":\"{1}\",\"HasCouponsForManualAttest\":\"false\",\"Coupons\":[{{\"CouponType\":1,\"IsForManualAttest\":false,\"Bets\":[{{\"Currency\":\"{2}\",\"Stake\":{3},\"BetType\":1,\"Betselections\":[{{\"EventId\":{4},\"MarketId\":{5},\"MarketSelectionId\":{6},\"Odds\":{7}}}],\"BonusCustomerId\":\"\"}}]}}]}}}}", token, Settings.Instance.currency, Settings.Instance.currency, stake, el.ei, el.ml[0].mi, el.ml[0].msl[1].msi, el.ml[0].msl[1].msp);

                HttpResponseMessage responseMessageTax = await httpClient.PostAsync("https://sb.bpsgameserver.com/AJAX/responseFroms/GetTax.aspx", (HttpContent)new FormUrlEncodedContent((IEnumerable<KeyValuePair<string, string>>)new KeyValuePair<string, string>[3]{
                new KeyValuePair<string, string>("json[params][json]", param),
                new KeyValuePair<string, string>("json[url]", "AJAX/responseFroms/PlaceCouponV2.aspx"),
                    new KeyValuePair<string, string>("json[type]", "POST")
                }));
                responseMessageTax.EnsureSuccessStatusCode();

                HttpResponseMessage responseMessagePlace = await httpClient.PostAsync("https://sb.bpsgameserver.com/AJAX/responseFroms/PlaceCouponV2.aspx", (HttpContent)new FormUrlEncodedContent((IEnumerable<KeyValuePair<string, string>>)new KeyValuePair<string, string>[1]{
                    new KeyValuePair<string, string>("json", param)
                }));
                responseMessagePlace.EnsureSuccessStatusCode();

                string responseMessagePlaceString = await responseMessagePlace.Content.ReadAsStringAsync();
                if (string.IsNullOrEmpty(responseMessagePlaceString))
                    return false;

                if (!responseMessagePlaceString.Contains("\"sMsg\":\"Success\""))
                    return false;

                return true;
            }
            catch(Exception e)
            {
                return false;
            }
        }
    }
}
