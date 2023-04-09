using CodeBase.Infrastructure.Services;
using Infrastructure.Services;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace LocationDir
{
    public class LocationController : MonoBehaviour
    {
        [SerializeField] private Transform[] npcPositions;
        [SerializeField] private Transform[] itemsPositions;
        [SerializeField] private Transform spawnPoint;

        public string Id { get; set; }
        public string Name { get; set; }

        private int _currentNpcIndex = 0;
        private int _currentItemIndex = 0;

        public Transform GetSpawnPoint() => spawnPoint;

        public void ShowLocationsToGo() =>
            AllServices.Container.Single<IGameService>().GetGameController().ShowLocationToGo();

        public void SpawnNpc(JToken character)
        {
            var position = npcPositions[_currentNpcIndex].position;

            var npc = Resources.Load<Npc.Npc>("JsonFiles/Npc/" + character["Name"]);
            if (npc == null)
                npc = Resources.Load<Npc.Npc>("JsonFiles/Npc/default");
            npc.NpcInfo = character;
            
            Instantiate(npc, position, Quaternion.identity);
            _currentNpcIndex++;
        }

        public void SpawnItem(JToken item)
        {
            var position = itemsPositions[_currentItemIndex].position;

            var itemMesh = Resources.Load<Item>("JsonFiles/Items3D/" + item["Name"]);
            if (itemMesh == null)
                itemMesh = Resources.Load<Item>("JsonFiles/Items3D/default");
            
            itemMesh.ItemInfo = item;

            Instantiate(itemMesh, position, Quaternion.identity);
            _currentItemIndex++;
        }
    }
}