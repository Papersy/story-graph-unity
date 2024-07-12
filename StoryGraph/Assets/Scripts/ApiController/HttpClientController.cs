using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UI;
using UnityEngine;

namespace ApiController
{
    public class HttpClientController : MonoBehaviour
    {
        private static HttpClient httpClient = null;
        
        public static async Task<string> GetNewWorld()
        {
            var client = new HttpClient();
            var response = await client.GetAsync("http://127.0.0.1:8000/getWorld");

            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                string jsonFormatted = JValue.Parse(json).ToString(Formatting.Indented);

                return jsonFormatted;
            }

            Console.WriteLine($"Error: {response.StatusCode}");
            return null;
        }
        
        public static async Task<string> PostNewWorld(JToken world, JToken production, JToken variant, string obj)
        {
            var json = $"{"{"}\n\"world\":{world},\n\"production\":{production},\n\"variant\":{variant},\n\"object\":\"{obj}\"{"}"}";

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
                Debug.Log("Get response");

                return responseContent;
            }
            
            Debug.Log($"SMTH WRONG {response.StatusCode}");
            
            // GameCanvas.Instance.DiePanel.SetActive(true);
            
            var dict = JToken.Parse(json);
            var message = dict["message"];
            if(message != null && message.ToString() != "Nie ma postaci Main_hero w świecie. Zniknął główny bohater. Pewno zginął.")
                GameCanvas.Instance.DiePanel.SetActive(true);
            
            return null;
        }
    }
}