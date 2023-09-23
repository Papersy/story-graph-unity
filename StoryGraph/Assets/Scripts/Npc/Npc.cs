using System;
using CodeBase.Infrastructure.Services;
using Infrastructure.Services;
using InteractableItems;
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
            if (attributes != null && attributes["HP"] != null)
            {
                var hp = attributes["HP"].ToString();
                Health = Convert.ToInt32(hp);
            }
            else
                Health = 10;
        }

        public void StartDialog()
        {
            AllServices.Container.Single<IUIService>().HudContainer.GameCanvas.DialogWindow.NpcInfo = NpcInfo;
            AllServices.Container.Single<IUIService>().HudContainer.GameCanvas.DialogWindow.Npc = gameObject;
            AllServices.Container.Single<IUIService>().HudContainer.GameCanvas.DialogWindow.InitDialog();
            
            AllServices.Container.Single<IUIService>().HudContainer.GameCanvas.ShowDialog();
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
                    JToken json = JObject.Parse(@"{""Name"": ""Corpse""}");
                    
                    var itemMesh = Resources.Load<Item>("JsonFiles/Items3D/corpse");
                    var obj = Instantiate(itemMesh, transform.position, Quaternion.identity);
                    obj.ItemInfo = json;
                    obj.transform.position = transform.position;
                    
                    AllServices.Container.Single<IGameService>().GetGameController().NpcLostItems(NpcInfo, transform.position);
                    AllServices.Container.Single<IGameService>().GetGameController().FightEndWithSomeoneDeath(playerName, NpcInfo["Id"].ToString());
                    gameObject.SetActive(false);
                }
                else 
                    CalculateChance();
            }
        }

        private void CalculateChance()
        {
            var number = Random.Range(0, 10);
            // if (number <= 2) // Escape chance
            // {
            //     Debug.Log("NPC ESCAPE");
            //     PlayerStats.NpcBattleInfo = null;
            //     
            //     var playerName = AllServices.Container.Single<IGameService>().GetGameController().GetPlayerName();
            //     var npcId = NpcInfo["Id"].ToString();
            //     
            //     AllServices.Container.Single<IGameService>().GetGameController().NpcLostItems(NpcInfo, transform.position);
            //     AllServices.Container.Single<IGameService>().GetGameController().EscapeFromBattle(playerName, npcId);
            //     
            //     gameObject.SetActive(false);
            // }
            // else if (number <= 4) //Attack chance
            // {
            //     var damage = NpcInfo["Attributes"]["HP"].ToString();
            //     PlayerStats.UpdateHealth((int)(Convert.ToInt32(damage) * -0.2));
            // }
        }
    }
}