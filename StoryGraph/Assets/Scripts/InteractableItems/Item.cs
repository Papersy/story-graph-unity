using Newtonsoft.Json.Linq;
using UnityEngine;

namespace LocationDir
{
    public class Item : MonoBehaviour
    {
        public JToken ItemInfo { get; set; }
        public Storage Storage = new Storage();
    }
}