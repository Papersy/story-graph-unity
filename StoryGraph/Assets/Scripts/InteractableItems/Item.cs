using Newtonsoft.Json.Linq;
using UnityEngine;

namespace InteractableItems
{
    [RequireComponent(typeof(Rigidbody))]
    public class Item : MonoBehaviour
    {
        public JToken ItemInfo { get; set; }
    }
}