using System;
using System.IO;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;

namespace New_SSQE
{
    internal class WebClient
    {
        private static readonly HttpClient client = new();
        private static readonly HttpClient robloxClient = new(new HttpClientHandler()
        {
            AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip
        });
        private static readonly HttpClient redirectClient = new(new HttpClientHandler()
        {
            AllowAutoRedirect = false
        });

        public static string GetRedirect(string url)
        {
            var final = "";

            var result = Task.Run(async () =>
            {
                using HttpResponseMessage response = await redirectClient.GetAsync(url);

                if (response.StatusCode == HttpStatusCode.Redirect)
                    final = (response.Headers.Location ?? new Uri("")).ToString();
            }).ContinueWith(t =>
            {
                return final;
            });

            return result.Result;
        }

        public static string DownloadString(string url)
        {
            var result = Task.Run(async () =>
            {
                using HttpResponseMessage response = await client.GetAsync(url);
                using HttpContent content = response.Content;

                return content.ReadAsStringAsync().Result;
            });

            if (result.Result != null)
                return result.Result;
            else
                throw new WebException();
        }

        public static void DownloadFile(string url, string location, bool fromRoblox = false)
        {
            var result = Task.Run(async () =>
            {
                var determined = fromRoblox ? robloxClient : client;
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                if (fromRoblox)
                    request.Headers.Add("User-agent", "RobloxProxy");

                using HttpResponseMessage response = await determined.SendAsync(request);
                using HttpContent content = response.Content;

                var stream = content.ReadAsStreamAsync().Result;

                ActionLogging.Register($"Attempted download of file: {location} - {fromRoblox} : {response.StatusCode}");

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using FileStream fs = new(location, FileMode.Create);
                    await stream.CopyToAsync(fs);
                }

                return response.StatusCode;
            });

            if (result.Result != HttpStatusCode.OK)
                throw new WebException($"({(int)result.Result}) {result.Result}");
        }
    }
}
