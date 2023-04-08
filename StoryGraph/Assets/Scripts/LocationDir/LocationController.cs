using CodeBase.Infrastructure.Services;
using Infrastructure.Services;
using UnityEngine;

namespace LocationDir
{
    public class LocationController : MonoBehaviour
    {
        [SerializeField] private Transform[] npcPositions;
        [SerializeField] private Transform spawnPoint;

        public string Id { get; set; }
        public string Name { get; set; }

        private int _currentIndex = 0;

        public Transform GetSpawnPoint() => spawnPoint;

        public void ShowLocationsToGo() =>
            AllServices.Container.Single<IGameService>().GetGameController().ShowLocationToGo();

        public void SpawnNpc(string npcName)
        {
            var position = npcPositions[_currentIndex].position;

            var npc = Resources.Load("JsonFiles/Npc/" + npcName);
            Instantiate(npc, position, Quaternion.identity);

            _currentIndex++;
        }
    }
}