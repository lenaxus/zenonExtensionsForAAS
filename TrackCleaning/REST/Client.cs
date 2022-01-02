using RestSharp;
using System.Configuration;

namespace TrackCleaning.RESTClient
{
    /// <summary>
    /// The Client for the RestSharp library. 
    /// </summary>
    static class RESTClient
    {
        private static readonly RestClient client = new RestClient(ConfigurationManager.AppSettings["AASServerURL"]);

        /// <summary>
        /// Does a REST GET-Request and returns the response.
        /// </summary>
        /// <param name="URLpath">The path of the REST-Endpoint. </param>
        /// <returns></returns>
        public static IRestResponse GetRequest(string URLpath)
        {
            var request = new RestRequest(URLpath, Method.GET);
            var response = client.Execute(request);
            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="URLpath">The path of the REST-Endpoint. </param>
        /// <param name="payload">The body of the request as an object.</param>
        /// <returns></returns>
        public static IRestResponse PutRequest(string URLpath, object payload)
        {
            var request = new RestRequest(URLpath, Method.PUT)
            {
                RequestFormat = DataFormat.Json
            };
            request.AddBody(payload);
            var response = client.Execute(request);
            return response;
        }
    }
}
