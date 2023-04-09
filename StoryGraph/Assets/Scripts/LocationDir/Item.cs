using Newtonsoft.Json.Linq;
using UnityEngine;

namespace LocationDir
{
    public class Item : MonoBehaviour
    {
        private JToken _itemInfo;
    
        public JToken ItemInfo { get; set; }
    }
}