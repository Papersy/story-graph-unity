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