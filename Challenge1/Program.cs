using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Challenge1
{
    internal class Program
    {
        static readonly HttpClient client = new();

        static async Task Main()
        {

            Parallellogram respJson = await GetClient();

            respJson.jeep = new()
            {
                x = Math.Round(respJson.volcano.x + (respJson.mountain.x -= respJson.you.x), 2),
                y = Math.Round(respJson.volcano.y + (respJson.mountain.y -= respJson.you.y), 2)
            };

            if (!string.IsNullOrEmpty(await PostAnswerAsync(respJson)))
            {
                Console.WriteLine("Answser is wrong");
                return;
            }
            Console.WriteLine("Correct answer");
        }

        static async Task<Parallellogram?> GetClient()
        {
            client.BaseAddress = new Uri("https://exs-htf-2023.azurewebsites.net/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("Authorization", "team bd486984-077b-4c27-b203-b5d1e317da74");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = await client.GetAsync("/api/challenges/find-the-jeep?isTest=false");
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("Something went wrong");
                Environment.Exit(0);
            }
            return JsonConvert.DeserializeObject<Parallellogram>(await response.Content.ReadAsStringAsync());
        }

        static async Task<string?> PostAnswerAsync(Parallellogram parallellogram)
        {
            HttpResponseMessage postResponse = await client.PostAsJsonAsync<object>("/api/challenges/find-the-jeep", new { answer = parallellogram.jeep });
            return await postResponse.Content.ReadAsStringAsync();
        }
        
        public class Parallellogram
        {
            public Coordinaten you { get; set; }
            public Coordinaten volcano { get; set; }
            public Coordinaten mountain { get; set; }
            public Coordinaten jeep { get; set; }

            public class Coordinaten
            {
                public double x { get; set; }
                public double y { get; set; }
            }
        }

    }
}
