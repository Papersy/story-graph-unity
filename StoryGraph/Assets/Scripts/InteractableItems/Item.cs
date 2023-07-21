using LocationDir;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace InteractableItems
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(BoxCollider))]
    public class Item : MonoBehaviour
    {
        public JToken ItemInfo { get; set; }
        public Storage Storage = new Storage();
    }
}