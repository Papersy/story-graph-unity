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
            if (HasItems())
            {
                Debug.Log("Give item");
                
                AllServices.Container.Single<IUIService>().HudContainer.GameCanvas.DialogWindow.GiveItem.onClick.RemoveAllListeners();
                AllServices.Container.Single<IUIService>().HudContainer.GameCanvas.DialogWindow.TakeItem.onClick.RemoveAllListeners();
                
                
                AllServices.Container.Single<IUIService>().HudContainer.GameCanvas.DialogWindow.GiveItem.onClick.AddListener(GiveItem);
                AllServices.Container.Single<IUIService>().HudContainer.GameCanvas.DialogWindow.TakeItem.onClick.AddListener(TakeItem);
            }
            else
            {
                Debug.Log("No items to give");
                
                AllServices.Container.Single<IUIService>().HudContainer.GameCanvas.DialogWindow.GiveItem.gameObject.SetActive(false);
            }

            AllServices.Container.Single<IUIService>().HudContainer.GameCanvas.DialogWindow.StartBattle.gameObject.SetActive(true);
        }

        private void GetDialog()
        {
            AllServices.Container.Single<IUIService>().HudContainer.GameCanvas.DialogWindow.DialogIndex = 0;
            
            var path = "JsonFiles/Dialogs/" + GetNpcName();
            var json = Resources.Load<TextAsset>(path).text;
            
            if(json == null)
                json = Resources.Load<TextAsset>("JsonFiles/Dialogs/default").text;
            
            AllServices.Container.Single<IUIService>().HudContainer.GameCanvas.DialogWindow.Dialog =
                JsonUtility.FromJson<Dialog>(json);
        }

        private void GiveItem()
        {
            AllServices.Container.Single<IGameService>().GetGameController().GiveItemToNpc(NpcInfo["Name"].ToString());
        }

        private void TakeItem()
        {
            if (HasItems())
                AllServices.Container.Single<IGameService>().GetGameController().TakeItemFromNpc(NpcInfo["Name"].ToString());
        }

        private bool HasItems()
        {
            if (NpcInfo["Items"] == null)
                return false;

            return true;
        }

        private string GetNpcName()
        {
            return NpcInfo["Name"].ToString();
        }
    }
}