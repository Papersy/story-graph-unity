using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace LocationDir
{
    public class Storage : MonoBehaviour
    {
        public List<JToken> ListOfItems { get; set; }
    }
}