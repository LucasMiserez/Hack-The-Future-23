using System.Net.Http.Json;
using System.Text;
using Newtonsoft.Json;
using System.Security.Cryptography;
using HTFclassLibrary;

namespace Challenge2
{
    internal class Program
    {
        private static readonly HttpClient httpClient = APIClient.Client;

        static async Task Main()
        {
            PostAnswerAsync(DecryptStringAES((await PutAnswerAsync(Decrypt((await GetClient()).symbols))).encryption, "Pyramids of Giza", "Valley of Kings0"));
        }

        static async Task<Exsertus?> GetClient()
        {
            HttpResponseMessage response = await httpClient.GetAsync("/api/challenges/ruins?isTest=true");
            return JsonConvert.DeserializeObject<Exsertus>(await response.Content.ReadAsStringAsync());
        }

        static async Task<Exsertus?> PutAnswerAsync(string answerString)
        {
            HttpResponseMessage putRequest = await httpClient.PutAsJsonAsync<object>("/api/challenges/ruins", new { answer = answerString });
            putRequest.EnsureSuccessStatusCode();
            return JsonConvert.DeserializeObject<Exsertus>(await putRequest.Content.ReadAsStringAsync());
        }

        static async Task PostAnswerAsync(string answerString)
        {
            HttpResponseMessage postRequest = await httpClient.PutAsJsonAsync<object>("/api/challenges/ruins", new { answer = answerString });
            postRequest.EnsureSuccessStatusCode();
        }

        static string Decrypt(string encryptedString)
        {
            Dictionary<char, char> decryptionMap = new Dictionary<char, char>
            {
                {'₳', 'A'}, {'฿', 'B'}, {'₵', 'C'}, {'₫', 'D'}, {'€', 'E'},
                {'₣', 'F'}, {'₲', 'G'}, {'₶', 'H'}, {'₻', 'I'}, {'৳', 'J'},
                {'₭', 'K'}, {'£', 'L'}, {'ℳ', 'M'}, {'₦', 'N'}, {'¤', 'O'},
                {'₱', 'P'}, {'֏', 'Q'}, {'₨', 'R'}, {'$', 'S'}, {'₸', 'T'},
                {'₼', 'U'}, {'₹', 'V'}, {'₩', 'W'}, {'₪', 'X'}, {'¥', 'Y'},
                {'₷', 'Z'}
            };
            return new string(encryptedString.Select(symbol => decryptionMap.TryGetValue(symbol, out char value) ? value : symbol).ToArray());
        }
        static string DecryptStringAES(string cipherText, string key, string iv)
        {
            using Aes aesAlg = Aes.Create();
            aesAlg.Key = Encoding.UTF8.GetBytes(key);
            aesAlg.IV = Encoding.UTF8.GetBytes(iv);

            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using var msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText));
            using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
            using var srDecrypt = new StreamReader(csDecrypt);
            return srDecrypt.ReadToEnd();
        }
        private class Exsertus
        {
            public string? symbols { get; set; }

            public string? encryption { get; set; }
        }
    }

}
