using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace ZillowApp.Helpers
{
    public class ZillowExceptionHandler : IHttpActionResult
    {
        private ZillowErrorObject _zillowErrorContent = null;
        private HttpStatusCode _statusCode;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="zillowstatusCode"></param>
        /// <param name="message"></param>
        public ZillowExceptionHandler(int zillowstatusCode, string message)
        {
            _zillowErrorContent = new ZillowErrorObject(zillowstatusCode, message, ProcessResolutionMessage(zillowstatusCode));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            HttpResponseMessage response = new HttpResponseMessage(_statusCode)
            {
                Content = new ZillowErrorContent(_zillowErrorContent) 
            };
            return Task.FromResult(response);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        public string ProcessResolutionMessage(int statusCode)
        {
            string _resValue = null;
            switch (statusCode)
            {
                case 1:
                    _resValue = "Check to see if your url is properly formed: delimiters, character cases, etc.";
                    _statusCode = HttpStatusCode.InternalServerError;
                    break;
                case 2:
                    _resValue = "Check if you have provided a ZWSID in your API call. If yes, check if the ZWSID is keyed in correctly. If it still doesn't work, contact Zillow to get help on fixing your ZWSID.";
                    _statusCode = HttpStatusCode.InternalServerError;
                    break;
                case 3:
                    _resValue = "The Zillow Web Service is currently not available. Please come back later and try again.";
                    _statusCode = HttpStatusCode.NotFound;
                    break;
                case 4:
                    _resValue = "The Zillow Web Service is currently not available. Please come back later and try again.";
                    _statusCode = HttpStatusCode.NotFound;
                    break;
                case 400:
                    _resValue = "Please Enter a Valid Search";
                    _statusCode = HttpStatusCode.NotFound;
                    break;
                case 404:
                    _resValue = "Resource Not Found";
                    _statusCode = HttpStatusCode.NotFound;
                    break;
                case 500:
                    _resValue = "Check if the input address matches the format specified in the input parameters table. When inputting a city name, include the state too. A city name alone will not result in a valid address.";
                    _statusCode = HttpStatusCode.InternalServerError;
                    break;
                case 501:
                    _resValue = "Check if the input address matches the format specified in the input parameters table. When inputting a city name, include the state too. A city name alone will not result in a valid address.";
                    _statusCode = HttpStatusCode.NotImplemented;
                    break;
                case 502:
                    _resValue = "Sorry, the address you provided is not found in Zillow's property database.";
                    _statusCode = HttpStatusCode.BadGateway;
                    break;
                case 503:
                    _resValue = "Please check to see if the city/state you entered is valid. If you provided a ZIP code, check to see if it is valid.";
                    _statusCode = HttpStatusCode.ServiceUnavailable;
                    break;
                case 504:
                    _resValue = "The specified area is not covered by the Zillow property database. To see our property coverage tables, click here.";
                    _statusCode = HttpStatusCode.GatewayTimeout;
                    break;
                case 505:
                    _resValue = "Your request timed out. The server could be busy or unavailable. Try again later.";
                    _statusCode = HttpStatusCode.HttpVersionNotSupported;
                    break;
                case 506:
                    _resValue = "If address is valid, try using abbreviations.";
                    _statusCode = HttpStatusCode.InternalServerError;
                    break;
                case 507:
                    _resValue = "Verify that the given address is correct.";
                    _statusCode = HttpStatusCode.InternalServerError;
                    break;
                default:
                    
                    break;
            }
            return _resValue;
        }

        /// <summary>
        /// 
        /// </summary>
        public class ZillowErrorContent : HttpContent
        {
            private readonly MemoryStream _Stream = new MemoryStream();

            /// <summary>
            /// 
            /// </summary>
            /// <param name="value"></param>
            public ZillowErrorContent(ZillowErrorObject value)
            {

                var jw = new JsonTextWriter(new StreamWriter(_Stream)) {Formatting = Formatting.Indented};
                var serializer = new JsonSerializer();
                serializer.Serialize(jw, value);
                jw.Flush();
                _Stream.Position = 0;

            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="stream"></param>
            /// <param name="context"></param>
            /// <returns></returns>
            protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
            {
                _Stream.CopyTo(stream);
                var tcs = new TaskCompletionSource<object>();
                tcs.SetResult(null);
                return tcs.Task;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="length"></param>
            /// <returns></returns>
            protected override bool TryComputeLength(out long length)
            {
                length = _Stream.Length;
                return true;
            }
        }

        public class ZillowErrorObject
        {
            public int ZillowErrorStatusCode { get; set; }
            public string ErrorMessage { get; set; }
            public string ErrorResolution { get; set; }

            public ZillowErrorObject(int errorCode, string errorMessage, string errorResolution)
            {
                ZillowErrorStatusCode = errorCode;
                ErrorMessage = errorMessage;
                ErrorResolution = errorResolution;
            }
        }
    }
}

