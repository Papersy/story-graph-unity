using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Npc
{
    public class Npc : MonoBehaviour
    {
        private JToken _npcInfo;
        
        public JToken NpcInfo { get; set; }
    }
}