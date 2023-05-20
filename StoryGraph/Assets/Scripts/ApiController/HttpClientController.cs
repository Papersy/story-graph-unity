using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace ApiController
{
    public class HttpClientController : MonoBehaviour
    {
        public static async Task<string> GetNewWorld()
        {
            var client = new HttpClient();
            var response = await client.GetAsync("http://127.0.0.1:8000/getWorld");

            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                string jsonFormatted = JValue.Parse(json).ToString(Formatting.Indented);
                // Parse the JSON response here

                return jsonFormatted;
            }
            
            Console.WriteLine($"Error: {response.StatusCode}");
            return null;
        }
        
        public async Task<string> GenerateMap()
        {
            var client = new HttpClient();
            var response = await client.GetAsync("http://127.0.0.1:8000/generateMap");

            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                string jsonFormatted = JValue.Parse(json).ToString(Formatting.Indented);
                // Parse the JSON response here
                Debug.Log(json);

                return jsonFormatted;
            }
            else
            {
                Console.WriteLine($"Error: {response.StatusCode}");
            }

            return null;
        }


        private static HttpClient httpClient = null;
        public static async Task<string> PostNewWorld(JToken world, JToken production, JToken variant, string obj)
        {
            var json = $"{"{"}\n\"world\":{world},\n\"production\":{production},\n\"variant\":{variant},\n\"object\":\"{obj}\"{"}"}";

            Debug.Log(json);
            
            if(httpClient == null)
                httpClient = new HttpClient();
            
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync("http://127.0.0.1:8000/postNewWorld", content);

            string filePath = "Assets/Resources/JsonFiles/PostToHttp.json";

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                string jsonFormatted = JValue.Parse(json.ToString()).ToString(Formatting.Indented);
                writer.Write(jsonFormatted);
            }
            
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();

                return responseContent;
            }
            
            Debug.Log($"SMTH WRONG {response.StatusCode}");
            return null;
        }
    }
}