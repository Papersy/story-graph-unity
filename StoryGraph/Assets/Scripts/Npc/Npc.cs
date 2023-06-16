using CodeBase.Infrastructure.Services;
using Infrastructure.Services;
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
        
        public void GiveItem()
        {
            var itemName = GetItemName();

            if (itemName != null)
                AllServices.Container.Single<IGameService>().GetGameController().GetItemFromNpc(NpcInfo["Name"].ToString(), itemName);
        }

        private string GetItemName()
        {
            if (NpcInfo["Items"] == null)
                return null;

            foreach (var item in NpcInfo["Items"])
            {
                return item["Name"].ToString();
            }

            return null;
        }

        private string GetNpcName()
        {
            return NpcInfo["Name"].ToString();
        }
    }
}