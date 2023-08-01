using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using CSharpAPI6.betfair.api.exchange;
using global::CSharpAPI6.betfair.api.global;

namespace Balance
{
    class CBetfairAPI
    {


        #region Variables

        private BFGlobalService _bfGlobal;
        private BFExchangeService _bfExchange;

        private CSharpAPI6.betfair.api.global.APIRequestHeader _globReqHdr;
        private CSharpAPI6.betfair.api.exchange.APIRequestHeader _exchReqHdr;

        private string _username;
        private string _password;
        private int _productId;
        private int _vendorSoftwareId;
        private string _sessionToken;

        private string _currency;




        #endregion

        #region Constuctor

        public  CBetfairAPI()
        {
            Debug.WriteLine( string.Format( "{0} - Initializing CBetfairAPI", DateTime.Now ) );

            _bfGlobal = new BFGlobalService();

            _bfGlobal.EnableDecompression = true;

            _bfExchange = new BFExchangeService();

            _bfExchange.EnableDecompression = true;


            _globReqHdr = new CSharpAPI6.betfair.api.global.APIRequestHeader();
            _exchReqHdr = new CSharpAPI6.betfair.api.exchange.APIRequestHeader();


            _username = "";
            _password = "";

            _sessionToken = "";

            _productId = 82;
            _vendorSoftwareId = 0;

            _currency = "";
        }

        #endregion



        public bool Login( string UserName, string PassWord  )
        {
            bool bRetCode;
            const string serviceName = "Login";

            Debug.WriteLine( string.Format( "{0} - CBetfairAPI - {1}", DateTime.Now, serviceName ) );

            _username = UserName;
            _password = PassWord;


            var request = new LoginReq {    username = _username, 
                                            password = _password, 
                                            productId = _productId, 
                                            vendorSoftwareId = _vendorSoftwareId };

            LoginResp response = _bfGlobal.login( request );

            bRetCode = CheckResponse(   serviceName, 
                                        Convert.ToString( response.header.errorCode ),
                                        Convert.ToString( response.errorCode ), 
                                        response.header.sessionToken );
            if( bRetCode == false )
            {
                Debug.WriteLine( string.Format( "{0} - CBetfairAPI - {1} - FAILED", DateTime.Now, serviceName ) );
                return false;
            }

            _currency = response.currency;

            Debug.WriteLine( string.Format( "{0} - CBetfairAPI - {1} - OK", DateTime.Now, serviceName ) );

            return true;
        }

        public bool LogOut()
        {
            bool bRetCode;
            const string serviceName = "LogOut";

            Debug.WriteLine( string.Format( "{0} - CBetfairAPI - {1}", DateTime.Now, serviceName ) );


            var request = new LogoutReq();

            request.header = _globReqHdr;

            LogoutResp response = _bfGlobal.logout( request );

            bRetCode = CheckResponse(   serviceName,
                                        Convert.ToString( response.header.errorCode ),
                                        Convert.ToString( response.errorCode ),
                                        response.header.sessionToken );

            if( bRetCode == false )
            {
                Debug.WriteLine( string.Format( "{0} - CBetfairAPI - {1} - FAILED", DateTime.Now, serviceName ) );
                return false;
            }

            Debug.WriteLine( string.Format( "{0} - CBetfairAPI - {1} - OK", DateTime.Now, serviceName ) );

            return true;

        }



        private bool CheckResponse( string serviceName, string hdrErrCd, string respErrCd, string sessionToken )
        {

            if( ! string.IsNullOrEmpty( sessionToken ) )
            {
                _sessionToken = sessionToken;
                _globReqHdr.sessionToken = sessionToken;
                _exchReqHdr.sessionToken = sessionToken;
            }

            if( hdrErrCd != "OK" )
            {
                Debug.WriteLine( string.Format( "{0} - FAILED: Response.Header.ErrorCode = {1}", serviceName, hdrErrCd ) );
                return ( false );
            }
            if( respErrCd != "OK" )
            {
                Debug.WriteLine( string.Format( "{0} - FAILED: Response.ErrorCode = {1}", serviceName, respErrCd ) );
                return ( false );
            }

            return ( true );
        }
        public bool getActiveEventTypes(ref GetEventTypesResp response)
        {
            bool bRetCode;
            const string serviceName = "getActiveEventTypes";

            Debug.WriteLine(string.Format("{0} - CBetfairAPI - {1}", DateTime.Now, serviceName));


            var request = new GetEventTypesReq();

            request.header = _globReqHdr;

            response = _bfGlobal.getActiveEventTypes(request);

            bRetCode = CheckResponse(serviceName,
                                        Convert.ToString(response.header.errorCode),
                                        Convert.ToString(response.errorCode),
                                        response.header.sessionToken);

            if (bRetCode == false)
            {
                return false;
            }

            //
            // If you want to see what is returned from this call then 
            // uncomment the foreach loop below
            //
            //foreach (EventType et in response.eventTypeItems)
            //{
            //    Debug.WriteLine(string.Format("EventItem: ({0}), ({1})", et.name, et.id));
            //}


            return true;

        }
        public bool GetEvents(ref GetEventsResp response, int evtID)
        {
            bool bRetCode;
            const string serviceName = "getEvents";

            Debug.WriteLine(string.Format("{0} - CBetfairAPI - {1}", DateTime.Now, serviceName));


            var request = new GetEventsReq();

            request.header = _globReqHdr;
            request.eventParentId = evtID;

            response = _bfGlobal.getEvents(request);

            bRetCode = CheckResponse(serviceName,
                                        Convert.ToString(response.header.errorCode),
                                        Convert.ToString(response.errorCode),
                                        response.header.sessionToken);

            if (bRetCode == false)
            {
                return false;
            }

            return true;
        }
    }
}
