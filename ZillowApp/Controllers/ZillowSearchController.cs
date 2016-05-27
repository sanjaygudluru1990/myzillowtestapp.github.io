using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Xml.Linq;
using ZillowApp.Models;
using System.IO;
using System.Collections.Specialized;
using System.Web;
using ZillowApp.Helpers;

namespace ZillowApp.Controllers
{
    [RoutePrefix("api/ZillowSearch")]
    public class ZillowSearchController : ApiController
    {
        private const string baseUri = "http://www.zillow.com/webservice/GetSearchResults.htm";
        private const string zwsId = "X1-ZWz1dyb53fdhjf_6jziz";
        private ZillowExceptionHandler expHandler = null;

        /// <summary>
        /// Search Route
        /// </summary>
        /// <returns></returns>
        [Route("Search/{address}")]
        [HttpGet]
        public async Task<IHttpActionResult> Search(string address)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("nl-NL"));
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));

                string requestUri = QueryStringBuilder(address);
                //if (requestUri == null) { expHandler = new ZillowExceptionHandler(400,"Bad Request"); }
                
                HttpResponseMessage response = await client.GetAsync(requestUri);
                if (response.IsSuccessStatusCode)
                {
                    string apiResult = response.Content.ReadAsStringAsync().Result;
                    ZException exObj = ProcessException(apiResult);
                    if (exObj.StatusCode > 0) { expHandler = new ZillowExceptionHandler(exObj.StatusCode, exObj.ErrorDescription); return expHandler; };
                    IEnumerable<Zproperty> apiModel = GetSerchResultsFromXml(apiResult);
                    return Ok(apiModel);
                }
                expHandler = new ZillowExceptionHandler(404,"Resource Not Found");
                return expHandler;
            }
        }

        /// <summary>
        /// Process exception node from xml
        /// </summary>
        /// <param name="xmlString"></param>
        /// <returns></returns>
        public ZException ProcessException(string xmlString)
        {
            XDocument doc = XDocument.Parse(xmlString);
            ZException _exObj = new ZException()
            {
                StatusCode = (int)doc.Root.Element("message").Element("code"),
                ErrorDescription = (string)doc.Root.Element("message").Element("text")
            };
            return _exObj;
        }

        /// <summary>
        /// Contruct Query String based on input parameters
        /// </summary>
        /// <returns></returns>
        private string QueryStringBuilder(string address)
        {
            string _param2 = null;
            string[] queryParameters = address.Split(new[] { ',' }, 2);
            if (queryParameters.Length > 1) { _param2 = queryParameters[1]; };
            NameValueCollection queryParametersCollection = new NameValueCollection
                {
                    { "zws-id", zwsId },
                    { "address", queryParameters[0] },
                    { "citystatezip", _param2 }
                };
            UriBuilder builder = new UriBuilder(baseUri);
            // Create query string with all values
            builder.Query = string.Join("&", queryParametersCollection.AllKeys.Select(key => string.Format("{0}={1}", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(queryParametersCollection[key]))));
            return builder.Uri.AbsoluteUri;
        }

        /// <summary>
        /// Parse Xml & map it Domain Model
        /// </summary>
        /// <param name="xmlString"></param>
        /// <returns></returns>
        private IEnumerable<Zproperty> GetSerchResultsFromXml(string xmlString)
        {
            List<Zproperty> zproperties = new List<Zproperty>();
            XDocument doc = XDocument.Parse(xmlString);
            zproperties = (from c in doc.Descendants("result")
                                        select new Zproperty()
                                        {
                                            
                                            ZipId = (int)c.Element("zpid"),
                                            LocalNeighbourHoods = (from n in c.Element("localRealEstate").Descendants("region")
                                                                   select new LocalNeighbourHood()
                                                                   {
                                                                       Type = (string)n.Attribute("type").Value,
                                                                       NeighbourHoodId = Int32.Parse(n.Attribute("id").Value),
                                                                       NeighbourHoodName = (string)n.Attribute("name").Value,
                                                                       ZIndexValue = (string)n.Element("zindexValue"),
                                                                       NeighbourHoodRegionLinks = new NeighbourHoodRegionLinks()
                                                                       {
                                                                           OverView = (string)n.Element("links").Element("overview"),
                                                                           ForSaleByOwner = (string)n.Element("links").Element("forSaleByOwner"),
                                                                           forSale = (string)n.Element("links").Element("forSale")
                                                                       },
                                                                   }).ToList(),
                                            Zlinks = new Zlinks()
                                            {
                                                HomeDetails = (string)c.Element("links").Element("homedetails"),
                                                GraphSandData = (string)c.Element("links").Element("graphsanddata"),
                                                MapthisHome = (string)c.Element("links").Element("mapthishome"),
                                                Comparables = (string)c.Element("links").Element("comparables")
                                            },
                                            Address = new Address()
                                            {
                                                Street = (string)c.Element("address").Element("street"),
                                                ZipCode = (string)c.Element("address").Element("zipcode"),
                                                City = (string)c.Element("address").Element("city"),
                                                State = (string)c.Element("address").Element("state"),
                                                Latitude = (double)c.Element("address").Element("latitude"),
                                                Longitude = (double)c.Element("address").Element("longitude")
                                            },
                                            Zestimate = new Zestimate()
                                            {
                                                Amount = (int)c.Element("zestimate").Element("amount"),
                                                LastUpdated = (string)c.Element("zestimate").Element("last-updated"),
                                                IsDeprecated = c.Element("zestimate").Element("oneWeekChange").Attribute("deprecated").Value == "true" ? true : false,
                                                maxValue = (int)c.Element("zestimate").Element("valuationRange").Element("high"),
                                                minValue = (int)c.Element("zestimate").Element("valuationRange").Element("low"),
                                                PropertyValueChange = new PropertyValueChange()
                                                {
                                                    Duration = (int)c.Element("zestimate").Element("valueChange").Attribute("duration"),
                                                    PriceDeviation = (int)c.Element("zestimate").Element("valueChange")
                                                }
                                            }
                                        }).ToList();
            return zproperties;
        }
    }
}
