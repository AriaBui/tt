using System.Net.Http;

namespace VismaKart.Utils
{
    public static class HttpClientFactory
    {
        private static readonly HttpClient HttpClient = new HttpClient();

        public static HttpClient Create()
        {
            return HttpClient;
        }
    }
}
