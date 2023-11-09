using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace Challenge1
{
    internal class Program
    {
        static readonly HttpClient client = new();

        static void Main()
        {
            dynamic respJson = ConnectClient();
        }   

        static async Task<object?> ConnectClient() {
            client.BaseAddress = new Uri("https://exs-htf-2023.azurewebsites.net/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("Authorization", "team bd486984-077b-4c27-b203-b5d1e317da74");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = await client.GetAsync("/api/challenges/find-the-jeep?isTest=true");
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("Something went wrong");
                Environment.Exit(0);
            }
            return JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync());
        }
    }
}
