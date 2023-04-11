using System.Net;

namespace SSQE_Updater
{
    internal class WebClient
    {
        private static readonly HttpClient client = new();
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

        public static void DownloadFile(string url, string location)
        {
            var result = Task.Run(async () =>
            {
                var request = new HttpRequestMessage(HttpMethod.Get, url);

                using HttpResponseMessage response = await client.SendAsync(request);
                using HttpContent content = response.Content;

                var stream = content.ReadAsStreamAsync().Result;

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
