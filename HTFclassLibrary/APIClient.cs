using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace HTFclassLibrary
{
    public class APIClient
    {
        public static HttpClient Client { get; private set; }

        static APIClient()
        {
            InitializeClient();
        }

        private static void InitializeClient()
        {
            Client = new HttpClient();
            Client.BaseAddress = new Uri("https://exs-htf-2023.azurewebsites.net/");
            Client.DefaultRequestHeaders.Accept.Clear();
            Client.DefaultRequestHeaders.Add("Authorization", "team bd486984-077b-4c27-b203-b5d1e317da74");
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
    }
}
