using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BlockchainTest.Bitcoind
{
    static class BitcoindClient
    {
        public static async Task<T> SendRequest<T>(string userName, string password, string url, string method,
            object[] parameters)
        {
            var request = (HttpWebRequest) WebRequest.Create(url);
            request.ContentType = "application/json-rpc";
            request.Method = "POST";

            var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(userName + ":" + password));
            request.Headers.Add("Authorization", "Basic " + credentials);

            var requestData = parameters != null
                ? JsonConvert.SerializeObject(new
                {
                    jsonrpc = "1.0",
                    id = "BitcoinClient",
                    method = method,
                    @params = parameters
                })
                : JsonConvert.SerializeObject(new
                {
                    jsonrpc = "1.0",
                    id = "BitcoinClient",
                    method = method
                });

            byte[] byteArray = Encoding.UTF8.GetBytes(requestData);
            request.ContentLength = byteArray.Length;
            using (var dataStream = request.GetRequestStream())
            {
                dataStream.Write(byteArray, 0, byteArray.Length);
            }

            using (var response = await request.GetResponseAsync())
            using (var stream = response.GetResponseStream())
            using (var streamReader = new StreamReader(stream))
            {
                var responseString = streamReader.ReadToEnd();
                return JsonConvert.DeserializeObject<T>(responseString);
            }
        }
    }
}