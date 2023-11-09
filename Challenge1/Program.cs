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
            client.BaseAddress = new Uri("https://exs-htf-2023.azurewebsites.net/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("Authorization", "team bd486984-077b-4c27-b203-b5d1e317da74");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = await client.GetAsync("/api/challenges/find-the-jeep?isTest=false");
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("Something went wrong");
                return;
            }
            dynamic respJson = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync());

            dynamic You = respJson.you;
            dynamic Montain = respJson.mountain;
            dynamic Vulcan = respJson.volcano;

            Console.WriteLine(You);
            Console.WriteLine(Montain);
            Console.WriteLine(Vulcan);

            dynamic DistanceBJ = Montain;
            DistanceBJ.x -= You.x;
            DistanceBJ.y -= You.y;

            dynamic Jeep = Vulcan;
            Jeep.x += DistanceBJ.x;
            Jeep.y += DistanceBJ.y;

            Jeep.x = Math.Round((Jeep.x.Value), 2);
            Jeep.y = Math.Round((Jeep.y.Value), 2);

            dynamic answer = new
            {
                answer = new
                {
                    x = Math.Round((decimal)Jeep.x, 2),
                    y = Math.Round((decimal)Jeep.y, 2)
                }
            };

            Console.WriteLine(JsonConvert.SerializeObject((object) answer));


            HttpResponseMessage postResponse = await client.PostAsJsonAsync<object>("/api/challenges/find-the-jeep", (object) answer);
            
            string response1 = await postResponse.Content.ReadAsStringAsync();
            Console.WriteLine(postResponse.Headers.Location);
        }
    }
}
