using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Challenge1
{
    internal class Program
    {
        static readonly HttpClient client = new();

        static async Task Main()
        {
            client.BaseAddress = new Uri("https://exs-htf-2023.azurewebsites.net/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("Authorization", "team bd486984-077b-4c27-b203-b5d1e317da74");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = await client.GetAsync("/api/challenges/find-the-jeep?isTest=true");
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("Something went wrong");
                return;
            }
            dynamic respJson = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync());

            dynamic You = respJson.you;
            dynamic Montain = respJson.mountain;
            dynamic Vulcan = respJson.volcano;
            
            dynamic DistanceBJ = Montain;
            DistanceBJ.x -= You.x;
            DistanceBJ.y -= You.y;

            dynamic Jeep = Vulcan;
            Jeep.x += DistanceBJ.x;
            Jeep.y += DistanceBJ.y;

            Dictionary<string, string> anwser = new Dictionary<string, string>();
            anwser.Add("answer", JsonConvert.SerializeObject(Jeep));

            Console.WriteLine(JsonConvert.SerializeObject(anwser));

        }
    }
}
