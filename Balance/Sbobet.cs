using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using BeloSoft.Betfair.API;
using BeloSoft.Betfair.API.Models;
using BeloSoft.Betfair.API.Operations;
using HtmlAgilityPack;

using BFApi;
using BFApi.TO;
using BFApi.Json;
using CSharpAPI6.betfair.api.exchange;
using global::CSharpAPI6.betfair.api.global;
using Exception = System.Exception;
using MarketFilter = BFApi.TO.MarketFilter;
using MarketSort = BFApi.TO.MarketSort;
using TimeRange = BFApi.TO.TimeRange;

namespace Balance
{
    class Sbobet
    {
        private HttpClient httpClient = null;
        private CookieContainer container = null;

        public CBetfairAPI MyBetfair;
        public bool bLoggedOn;
        public Sbobet()
        {
            initHttpClient();

            MyBetfair = new CBetfairAPI();

            bLoggedOn = false;
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
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent",
                "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.103 Safari/537.36");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Upgrade-Insecure-Requests", "1");
        }
        private CSharpAPI6.betfair.api.exchange.GetAllMarketsResp GetAllRaceMarkets()
        {
            // Get session from session manager
//            CSharpAPI6.betfair.api.exchange.GetAllMarketsReq request = new GetAllMarketsReq();
//
//            request.header = new CSharpAPI6.betfair.api.exchange.APIRequestHeader();
//            request.header.sessionToken = session;
//
//
//            request.eventTypeIds = new int?[1] { 1 };
//            request.fromDate = DateTime.Now;
//            request.toDate = DateTime.Now.AddHours(12);// AddDays(1);
//
//            try
//            {
//                CSharpAPI6.betfair.api.exchange.GetAllMarketsResp resp = simpleAPIWrapper.BFExchangeServiceByExchange(GlobalDefs._UK_EXCHANGE_ID).getAllMarkets(request);
//                return resp;
//            }
//            catch
//            {
//                return null;
//            }
            return null;
        }
        public void liveAccountFund()
        {
            try
            {
//                string uuu = "jeliso";
//                string ppp = "Pepparkaka1";
//
//
//                if (bLoggedOn == false)
//                {
//                    if (MyBetfair.Login(uuu, ppp) == true)
//                    {
//                        bLoggedOn = true;
//                        Console.WriteLine("Logged On To Betfair");
//
//                    }
//                    else
//                    {
//                        Console.WriteLine("Log on FAILED, Check Debug Messages");
//                    }
//                }
//                else
//                {
//                    if (MyBetfair.LogOut() == true)
//                    {
//                        bLoggedOn = false;
//                        Console.WriteLine("NOT Logged On To Betfair");
//                    }
//                    else
//                    {
//                        Console.WriteLine("Log Out FAILED, Check Debug Messages");
//                    }
//                }
//
//
//                GetEventTypesResp Response = new GetEventTypesResp();
//
//                if (MyBetfair.getActiveEventTypes(ref Response) != true)
//                {
//                    Debug.WriteLine("Error: MenuTree->Initialise()->m_Betfair.getActiveEventTypes() FAILED ( returned false )");
////                    return (false);
//                }
//
//                if (Response.eventTypeItems != null)
//                {
//                    foreach (CSharpAPI6.betfair.api.global.EventType et in Response.eventTypeItems)
//                    {
//                        Console.WriteLine(string.Format("Init-----EventItem: ({0}), ({1})", et.name, et.id));
//                    }
//
//                }
//                else
//                {
//                    Console.WriteLine("MenuTree->Initialise()->m_BetfairgetActiveEventTypes() returned 0 Events ?!");
//
//                }
////                return true;







                GetAccountFundsResp fResp = new GetAccountFundsResp();

                CSharpAPI6.betfair.api.exchange.APIRequestHeader _exchReqHdr = new CSharpAPI6.betfair.api.exchange.APIRequestHeader();

                _exchReqHdr.sessionToken = "h3kvQhHHJ9leObAOu0Iq5IHXYOBkyFRV+PI053WstwU=";

                bool bRetCode;
                const string serviceName = "getAccountFunds";

                BFExchangeService _bfExchange = new BFExchangeService();
                _bfExchange.Url = "https://developers.betfair.com/api.betfair.com/exchange/json-rpc/v1/SportsAPING/v1.0/getmarketPrices";


                var reqForPrices = new GetMarketPricesReq();

            reqForPrices.header = _exchReqHdr;

                GetMarketPricesResp fResp1 = _bfExchange.getMarketPrices(reqForPrices);
                _bfExchange.getMarketPrices(reqForPrices);

                var request = new GetAccountFundsReq();

                request.header = _exchReqHdr;


                fResp = _bfExchange.getAccountFunds(request);

                //            bRetCode = CheckResponse(serviceName, Convert.ToString(fResp.header.errorCode), Convert.ToString(fResp.errorCode), fResp.header.sessionToken);
                //
                //            if (bRetCode == false)
                //            {
                //                return false;
                //            }
                //
                //            return true;

                //            if (MyBetfair.getAccFunds(ref fResp) == true)
                //            {
                //                butFunds.Text = fResp.availBalance.ToString("£0.00");
                //            }

            }
            catch (APINGException e)
            {
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public async Task<bool> doLogin()
        {
            liveAccountFund();
            LoginResult loginResult = new LoginResult();

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

                IEnumerable<HtmlNode> nodeInputs = nodeForms.ToArray()[0].Descendants("input").Where(node => node.Attributes["name"] != null);
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
                        inputValue = "0400R9HVeoYv1gsNf94lis1ztshEYOhBj8SQVGjDxyKtjEYJx9qSw/0rX3UPUavsVGNNP8qFSAZ42op95eoW86m0kwJH0I8JjLitr3JYlvaDSSoJdOHStouX5odMpSRnzNZujwZOoEVFPaadm2GNPflphIK4mjKAMqULsj69LfM7VRbla+gcYC1FQHWwCqQ/IvlFFmQWppRl6Bv4pp48B2PR0LUM6Rn3JtHEfF9hXdZ4DRRiwxmZVjl9I8/xOY8SaJdW7jj6V+dG7j9MyPpjzYuuV+VhT2JQxvTdTn3ScfrBfYTh6ImRigH/M5hg3W9g0TRnvoLZ8SpzAFLDWCmiO4sAs/qr/BWAZmps3sMxM3Tsu4+Oh9ltHZfQIggPoVjqnTR6FC6A4XHytZ7Tn6b+z24SKe+wPwB7YcyEaf1wmOpmO8iM1eyAwSCEbXPCKc6iDsD88+g/mDVDCxFKZ8U9CkoINzcLZ9/2RBpYQZsdELNYToJhO1Q2jmVgfPrA9XDC0LRFicgHXRoRWukWp8XlE1kYrSaUGABVKw5X0ikb/3h1d0Im6Fy7sPS5usVQSvGb6Q+XXiK1sQAL4CPUyKVnDKqtQrhW1HQIpGtAAfOPjzqyODIf9I+PRImeQ49qpLt2ZL85hw6+vl8ZjUN3p2VjxRMgSIkpIh3A707ln/wqsPiGBcfEpkWTlnMi5g4mmYKxVbRrSUpGCTU5RhDBmCQpzv+TwwM6wvQ9/pjGYj2YJXfnh939vTlgH8k0GP35GUqEwdlQH6AzAyX2sumr8GO9YMyocNGc3h38I509+L9GpM8mFWoWiIYYjbEvXCXVTb29yNd9DKThVP1hAyB7YMdbHsyiYBQ1h4tNGHTh8CNjY0flnyuelCnt2ypgfW/m3Zqxt3w1Bn6LxoWOnZu64rr07LwRGSM2wu7XD+lw1lMSgm8hl0E6JnXlF7diYNwTidneZ5WeqDQRJDWdY1eYOcNXGWh5eh0R9vRNj5evLzdF6XhbUceysRqfEatYu7JOeKvET7v2DVHRyVdfAWJVJqKtzBRNTqF/WnZgA3qvxiWvyGPt4xFJHNu5eG3HXPo1chRhwl6109lnHDNLhXiDaeModH3MykVCcuAzYjmBq2CObWM3m2Jxa2GEjcGvISFrwoQO0Yr3iynFSjKIXJbJoUCUMhuSPrCT16pHvpt1JeGMGYj7gJYGwcZRDWBe40N3RSizQUPUSdu3DZ2D0kFXG69EaoZ1RI0f9uYinjZJCJitlNUbN+M7DqaJyGOl8Gv+HZ+NFeJF+TjyWQJjI3NMC63Cwm7dHkusepnG3VLacD4ALLQF1a0FYqvfxn854SSXEvUuPt0pFsY23fB/+pEjrnavoBjx28jKWCbjdtkiyP5Y9ou+oQuf/feQ29ef/02xPtU9AeQHnnkJD5ghHNv+1znnh45aMwwjA2Ui9cgPlAgeaCqCy3cXjx33ffITvy2wv1MWLu4AO0fwrpVgx/siiN2XyvHCfqE2r+5inDLSN1TCyOxhTq/let4ltP27ta6LO+lD+YmKtgN4Oh10Gib60E9YGc40dTHbDZ5kt2K49+MixLQpfdYCKC/P29Tu8zwBp1GJQgbFvd7r7/TrYV9DpG+vj9JhImGqAbqHUNQ4M2FEG+0RRIU=";

                    if (inputName == "url")
                        refererUrl = inputValue;

                    inputs.Add(new KeyValuePair<string, string>(inputName, inputValue));
                }

                httpClient.DefaultRequestHeaders.Referrer = new Uri(mainReferer);
                HttpResponseMessage responseMessageLogin = await httpClient.PostAsync(action, (HttpContent)new FormUrlEncodedContent((IEnumerable<KeyValuePair<string, string>>)inputs));
                responseMessageLogin.EnsureSuccessStatusCode();

                string responseMessageLoginString = await responseMessageLogin.Content.ReadAsStringAsync();
                if (string.IsNullOrEmpty(responseMessageLoginString))
                    return false;

                doc.LoadHtml(responseMessageLoginString);

                nodeForms = doc.DocumentNode.Descendants("form").Where(node => node.Attributes["name"] != null && node.Attributes["name"].Value == "postLogin");
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

                httpClient.DefaultRequestHeaders.Referrer = new Uri(string.Format("https://identitysso.betfair.com/view/login?product=prospect-page&url={0}", WebUtility.UrlEncode(refererUrl)));
                HttpResponseMessage responseMessagePostLogin = await httpClient.PostAsync(action, (HttpContent)new FormUrlEncodedContent((IEnumerable<KeyValuePair<string, string>>)inputs));
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
//                        sessionToken = cookie.Value;
                        break;
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        private string getSbobetTk(string oldTk)
        {
            string newTk = string.Empty;

            string[] tkArray = oldTk.Split(new char[] { ',' }, StringSplitOptions.None);
            if (tkArray == null || tkArray.Length < 12)
                return string.Empty;

            for (int i = 0; i < tkArray.Length; i++)
            {
                if (i == 1)
                    continue;

                if (i == 9)
                    tkArray[i] = tkArray[i + 2];

                newTk += (tkArray[i] + ",");
            }

            newTk = tkArray[0] + "," + newTk.TrimEnd(',');

            return newTk.Replace("'", "");
        }
    }
}
