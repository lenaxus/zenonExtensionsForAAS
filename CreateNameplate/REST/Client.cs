using System.IO;
using RestSharp;
using RestSharp.Extensions;

namespace CreateNameplate.REST
{
    /// <summary>
    /// The Client for the RestSharp library. 
    /// </summary>
    public class Client
    {
        /// <summary>
        /// The Rest-Endpoint of the client of the AAS-Server.
        /// </summary>
        public static string serverURL;

        /// <summary>
        /// The RestSharp-RestClient for doing the Requests.
        /// </summary>
        public static RestClient client;

        /// <summary>
        /// Does a REST GET-Request and returns the response.
        /// </summary>
        /// <param name="URLpath">The path of the REST-Endpoint. </param>
        /// <returns>The response of the GET-Request. </returns>
        public static IRestResponse GetRequest(string URLpath)
        {
            var request = new RestRequest(URLpath, Method.GET);
            var response = client.Execute(request);
            return response;
        }

        /// <summary>
        /// Does a REST GET-REQUEST and downloads the data in the temp directory on the local disk.
        /// </summary>
        /// <param name="URLpath">The path of the REST-Endpoint. </param>
        /// <param name="fileName">The name which is used for storing the file. </param>
        public static void GetFile(string URLpath, string fileName)
        {
            var request = new RestRequest(URLpath, Method.GET);
            string tempPath = Path.GetTempPath();
            client.DownloadData(request).SaveAs(tempPath + fileName);
        }

        /// <summary>
        /// Does a REST PUT-REQUEST with a json body as payload. 
        /// </summary>
        /// <param name="URLpath">The path of the REST-Endpoint. </param>
        /// <param name="payload">The body of the request as an object. </param>
        /// <returns>The response of the PUT-Request. </returns>
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
