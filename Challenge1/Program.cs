using Newtonsoft.Json;
using HTFclassLibrary;

namespace Challenge1
{
    internal class Program
    {
        private static readonly HttpClient httpClient = APIClient.Client;
        static async Task Main()
        {
            Parallellogram? respJson = await GetClient();

            respJson.jeep = new()
            {
                x = Math.Round(respJson.volcano.x + (respJson.mountain.x -= respJson.you.x), 2),
                y = Math.Round(respJson.volcano.y + (respJson.mountain.y -= respJson.you.y), 2)
            };

            await PostAnswerAsync(respJson.jeep);
        }
        static async Task<Parallellogram?> GetClient()
        {

            HttpResponseMessage response = await httpClient.GetAsync("/api/challenges/find-the-jeep?isTest=false");
            return JsonConvert.DeserializeObject<Parallellogram>(await response.Content.ReadAsStringAsync());
        }

        static async Task PostAnswerAsync(Parallellogram.Coordinaten jeep)
        {
            HttpResponseMessage postResponse = await httpClient.PostAsJsonAsync<object>("/api/challenges/find-the-jeep", new { answer = jeep });
            postResponse.EnsureSuccessStatusCode();
        }

        private class Parallellogram
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
