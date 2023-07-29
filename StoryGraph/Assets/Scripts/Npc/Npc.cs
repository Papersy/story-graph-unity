using System;
using CodeBase.Infrastructure.Services;
using Infrastructure.Services;
using Newtonsoft.Json.Linq;
using Player;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Npc
{
    public class Npc : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _npcName;
    
        public JToken NpcInfo { get; set; }
        public Dialog Dialog;

        public int Health;
        
        public void Init()
        {
            _npcName.text = GetNpcName();

            var attributes = NpcInfo["Attributes"];
            if(attributes["HP"] != null)
            {
                var hp = attributes["HP"].ToString();
                Health = Convert.ToInt32(hp);
            }
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

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Attack"))
            {
                if (PlayerStats.NpcBattleInfo == null)
                    PlayerStats.NpcBattleInfo = NpcInfo;
                else if(PlayerStats.NpcBattleInfo != NpcInfo)
                    return;
                
                Debug.Log("ENTER DAMAGE");
                Health -= PlayerStats.Damage;

                if (Health <= 0)
                {
                    Debug.Log("NPC DEAD");
                    PlayerStats.NpcBattleInfo = null;
                    gameObject.SetActive(false);
                }
                else 
                    ChanceToEscape();
            }
        }

        private void ChanceToEscape()
        {
            var number = Random.Range(0, 10);
            if (number <= 2)
            {
                PlayerStats.NpcBattleInfo = null;
                
                var playerName = AllServices.Container.Single<IGameService>().GetGameController().GetPlayerName();
                var npcId = NpcInfo["Id"].ToString();
                AllServices.Container.Single<IGameService>().GetGameController().EscapeFromBattle(playerName, npcId);
                
                gameObject.SetActive(false);
            }
        }
    }
}