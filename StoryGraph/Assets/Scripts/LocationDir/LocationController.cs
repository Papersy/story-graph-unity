using CodeBase.Infrastructure.Services;
using Deserialization;
using Infrastructure.Services;
using UnityEngine;

namespace LocationDir
{
    public class LocationController : MonoBehaviour
    {
        [SerializeField] private Transform spawnPoint;
        public World World;

        public Transform GetSpawnPoint() => spawnPoint;

        public void ShowLocationsToGo() => 
            AllServices.Container.Single<IUIService>().HudContainer.GameCanvas.GenerateLocationButtons(World.Connections);
    }
}