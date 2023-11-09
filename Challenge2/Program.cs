using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Security.Cryptography;

namespace Challenge2
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

            HttpResponseMessage response = await client.GetAsync("/api/challenges/ruins?isTest=false");
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("Something went wrong");
                return;
            }
            dynamic respJson = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync());
            Console.WriteLine(respJson);

            string encryptedString = respJson.symbols;
            string decryptedString = Decrypt(encryptedString);

            Console.WriteLine("Encrypted String: " + encryptedString);
            Console.WriteLine("Decrypted String: " + decryptedString);
            dynamic answer1 = new
            {
                answer = decryptedString
            };

            HttpResponseMessage putResponse = await client.PutAsJsonAsync<object>("/api/challenges/ruins", (object)answer1);
            putResponse.EnsureSuccessStatusCode();
            string response1 = await putResponse.Content.ReadAsStringAsync();

            dynamic respJson_D2 = JsonConvert.DeserializeObject(await putResponse.Content.ReadAsStringAsync());
            Console.WriteLine(respJson_D2);

            string encryptedText = respJson_D2.encryption;

            string key = "Pyramids of Giza";
            string iv = "Valley of Kings0";

            string decryptedText = DecryptStringAES(encryptedText, key, iv);

            Console.WriteLine("Ontcijferd tekst: " + decryptedText);
            dynamic answer2 = new
            {
                answer = decryptedText
            };
            HttpResponseMessage postResponse = await client.PostAsJsonAsync<object>("/api/challenges/ruins", (object)answer2);
            postResponse.EnsureSuccessStatusCode();
            string response2 = await postResponse.Content.ReadAsStringAsync();
            Console.WriteLine("output:" + response2);

        }

        static string Decrypt(string encryptedString)
        {
            Dictionary<string, char> decryptionMap = new Dictionary<string, char>
            {
                {"₳", 'A'}, {"฿", 'B'}, {"₵", 'C'}, {"₫", 'D'}, {"€", 'E'},
                {"₣", 'F'}, {"₲", 'G'}, {"₶", 'H'}, {"₻", 'I'}, {"৳", 'J'},
                {"₭", 'K'}, {"£", 'L'}, {"ℳ", 'M'}, {"₦", 'N'}, {"¤", 'O'},
                {"₱", 'P'}, {"֏", 'Q'}, {"₨", 'R'}, {"$", 'S'}, {"₸", 'T'},
                {"₼", 'U'}, {"₹", 'V'}, {"₩", 'W'}, {"₪", 'X'}, {"¥", 'Y'},
                {"₷", 'Z'}
            };

            string output = "";

            foreach (var symbol in encryptedString)
            {
                foreach (var mapping in decryptionMap)
                {
                    if (mapping.Key[0] == symbol)
                    {
                        output += mapping.Value;
                        break;
                    }
                }
            }

            return output;
        }
        static string DecryptStringAES(string cipherText, string key, string iv)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(key);
                aesAlg.IV = Encoding.UTF8.GetBytes(iv);

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (var msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
                using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                using (var srDecrypt = new StreamReader(csDecrypt))
                {
                    return srDecrypt.ReadToEnd();
                }
            }
        }
    }
}
