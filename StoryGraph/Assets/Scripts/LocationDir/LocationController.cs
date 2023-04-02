using CodeBase.Infrastructure.Services;
using Infrastructure.Services;
using UnityEngine;

namespace LocationDir
{
    public class LocationController : MonoBehaviour
    {
        [SerializeField] private Vector3 npcPositions;
        [SerializeField] private Transform spawnPoint;

        public string Id { get; set; }
        public string Name { get; set; }

        public Transform GetSpawnPoint() => spawnPoint;

        public void ShowLocationsToGo() =>
            AllServices.Container.Single<IGameService>().GetGameController().ShowLocationToGo();
    }
}