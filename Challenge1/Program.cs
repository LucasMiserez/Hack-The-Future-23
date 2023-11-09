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

            Input respJson = await GetClient();


            Answer Jeep = new()
            {
                x = Math.Round(respJson.volcano.x + (respJson.mountain.x -= respJson.you.x), 2),
                y = Math.Round(respJson.volcano.y + (respJson.mountain.y -= respJson.you.y), 2)
            };

            if (!string.IsNullOrEmpty(await PostAnswerAsync(Jeep))) {
                Console.WriteLine("Answser is wrong");
                return;
            }
            Console.WriteLine("Correct answer");
        }

        static async Task<Input?> GetClient()
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
            return JsonConvert.DeserializeObject<Input>(await response.Content.ReadAsStringAsync());
        }

        static async Task<string?> PostAnswerAsync(object Answer)
        {
            HttpResponseMessage postResponse = await client.PostAsJsonAsync<object>("/api/challenges/find-the-jeep", new { answer = Answer});
            return await postResponse.Content.ReadAsStringAsync();
        }

        class Answer
        {
            public double x { get; set; }

            public double y { get; set; }

            public override string ToString()
            {
                return $"{{ x: {x}, y: {y} }}";
            }

        }

        public class Mountain
        {
            public double x { get; set; }
            public double y { get; set; }
        }

        public class Input
        {
            public You you { get; set; }
            public Volcano volcano { get; set; }
            public Mountain mountain { get; set; }
        }

        public class Volcano
        {
            public double x { get; set; }
            public double y { get; set; }
        }

        public class You
        {
            public double x { get; set; }
            public double y { get; set; }
        }
    }
}
