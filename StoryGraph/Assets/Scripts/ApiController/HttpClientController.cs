using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace ApiController
{
    public class HttpClientController : MonoBehaviour
    {
        public async Task<string> GetNewWorld()
        {
            HttpClient client = new HttpClient();

            HttpResponseMessage response = await client.GetAsync("http://127.0.0.1:8000/getWorld");

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
    }
}