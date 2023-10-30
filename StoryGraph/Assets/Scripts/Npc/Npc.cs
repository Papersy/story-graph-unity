using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;

namespace Npc
{
    public class Npc : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _npcName;
    
        public JToken NpcInfo { get; set; }
        
        public void Init()
        {
            _npcName.text = GetNpcName();
        }

        private string GetNpcName()
        {
            return NpcInfo["Name"].ToString();
        }
    }
}