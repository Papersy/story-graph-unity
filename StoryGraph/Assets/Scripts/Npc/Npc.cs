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
        public Dialog Dialog;

        public void Init()
        {
            _npcName.text = GetNpcName();
        }

        public void StartDialog()
        {
            GetDialog();
            
            AllServices.Container.Single<IUIService>().HudContainer.GameCanvas.ShowDialog();
            var item = GetItemName();
            if (item != null)
            {
                Debug.Log("Give item");
                
                AllServices.Container.Single<IUIService>().HudContainer.GameCanvas.DialogWindow.GetItem.onClick.RemoveAllListeners();
                AllServices.Container.Single<IUIService>().HudContainer.GameCanvas.DialogWindow.GetItem.onClick.AddListener(GiveItem);
                AllServices.Container.Single<IUIService>().HudContainer.GameCanvas.DialogWindow.GetItem.gameObject.SetActive(true);
            }
            else
            {
                Debug.Log("No items to give");
                
                AllServices.Container.Single<IUIService>().HudContainer.GameCanvas.DialogWindow.GetItem.gameObject.SetActive(false);
            }

            AllServices.Container.Single<IUIService>().HudContainer.GameCanvas.DialogWindow.StartBattle.gameObject.SetActive(true);
        }

        public void GetDialog()
        {
            AllServices.Container.Single<IUIService>().HudContainer.GameCanvas.DialogWindow.DialogIndex = 0;
            
            var path = "JsonFiles/Dialogs/" + GetNpcName();
            var json = Resources.Load<TextAsset>(path).text;
            
            if(json == null)
                json = Resources.Load<TextAsset>("JsonFiles/Dialogs/default").text;
            
            AllServices.Container.Single<IUIService>().HudContainer.GameCanvas.DialogWindow.Dialog =
                JsonUtility.FromJson<Dialog>(json);
        }
        
        public void GiveItem()
        {
            var itemName = GetItemName();

            if (itemName != null)
            {
                AllServices.Container.Single<IGameService>().GetGameController().GetItemFromNpc(NpcInfo["Name"].ToString(), itemName);
                foreach (var item in NpcInfo["Items"])
                {
                    if(item["Name"].ToString() == itemName)
                        item.Remove();
                }
            }
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