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
        public int Health;
        
        public void Init()
        {
            _npcName.text = GetNpcName();

            var attributes = NpcInfo["Attributes"];
            if(attributes != null && attributes["HP"] != null)
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
            AllServices.Container.Single<IUIService>().HudContainer.GameCanvas.DialogWindow.NpcInfo = NpcInfo;
            AllServices.Container.Single<IUIService>().HudContainer.GameCanvas.DialogWindow.Npc = gameObject;
            
            var path = "JsonFiles/Dialogs/" + GetNpcName();
            var json = Resources.Load<TextAsset>(path).text;
            
            if(json == null)
                json = Resources.Load<TextAsset>("JsonFiles/Dialogs/default").text;
            
            AllServices.Container.Single<IUIService>().HudContainer.GameCanvas.DialogWindow.Dialog = JsonUtility.FromJson<Dialog>(json);
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
                
                Health -= PlayerStats.Damage;

                if (Health <= 0)
                {
                    Debug.Log("NPC DEAD");
                    PlayerStats.NpcBattleInfo = null;
                    
                    var playerName = AllServices.Container.Single<IGameService>().GetGameController().GetPlayerName();
                    AllServices.Container.Single<IGameService>().GetGameController().
                        FightEndWithSomeoneDeath(playerName, NpcInfo["Id"].ToString());
                    gameObject.SetActive(false);
                }
                else 
                    CalculateChance();
            }
        }

        private void CalculateChance()
        {
            var number = Random.Range(0, 10);
            if (number <= 2) // Escape chance
            {
                Debug.Log("NPC ESCAPE");
                PlayerStats.NpcBattleInfo = null;
                
                var playerName = AllServices.Container.Single<IGameService>().GetGameController().GetPlayerName();
                var npcId = NpcInfo["Id"].ToString();
                AllServices.Container.Single<IGameService>().GetGameController().EscapeFromBattle(playerName, npcId);
                
                gameObject.SetActive(false);
            }
            else if (number <= 4) //Attack chance
            {
                var damage = NpcInfo["Attributes"]["HP"].ToString();
                PlayerStats.UpdateHealth((int)(Convert.ToInt32(damage) * -0.2));
            }
        }
    }
}