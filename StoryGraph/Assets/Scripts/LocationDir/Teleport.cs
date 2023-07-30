using CodeBase.Infrastructure.Services;
using Infrastructure.Services;
using Newtonsoft.Json.Linq;
using Player;
using TMPro;
using UnityEngine;

namespace LocationDir
{
    public class Teleport : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _teleportName;

        private Transform _target;
        private JToken variant;
        public JToken Variant
        {
            get => variant;
            set
            {
                variant = value;
                _teleportName.text = variant[2]["WorldNodeName"].ToString();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Debug.Log("Collide");

                if (PlayerStats.NpcBattleInfo == null)
                {
                    Debug.Log("No battle");
                    AllServices.Container.Single<IGameService>().GetGameController().ChangeLocation(Variant);
                }
                else
                {
                    Debug.Log("Has battle");
                    var fighterName = PlayerStats.NpcBattleInfo["Name"].ToString();
                    var playerId = AllServices.Container.Single<IGameService>().GetGameController().GetMainPlayerId();
                    AllServices.Container.Single<IGameService>().GetGameController().EscapeFromBattle(fighterName, playerId);
                    PlayerStats.NpcBattleInfo = null;
                }
            }
        }
    }
}