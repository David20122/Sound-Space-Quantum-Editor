using New_SSQE.ExternalUtils;
using System.Net;

namespace New_SSQE.Misc.Network
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
            string final = "";

            Task<string> result = Task.Run(async () =>
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
            Task<string> result = Task.Run(async () =>
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

        public static void DownloadFile(string url, string location, FileSource source = FileSource.Other)
        {
            Task<HttpStatusCode> result = Task.Run(async () =>
            {
                HttpClient determined = source == FileSource.Roblox ? robloxClient : client;
                HttpRequestMessage request = new(HttpMethod.Get, url);
                if (source == FileSource.Roblox)
                    request.Headers.Add("User-agent", "RobloxProxy");
                else if (source != FileSource.Other)
                    request.Headers.Add("User-agent", $"SSQE_Import-{source}");

                using HttpResponseMessage response = await determined.SendAsync(request);
                using HttpContent content = response.Content;

                Stream stream = content.ReadAsStreamAsync().Result;

                Logging.Register($"Attempted download of file: {location} - {source} : {response.StatusCode}");

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
