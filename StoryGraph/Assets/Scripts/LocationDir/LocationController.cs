using CodeBase.Infrastructure.Services;
using Infrastructure.Services;
using UnityEngine;

namespace LocationDir
{
    public class LocationController : MonoBehaviour
    {
        [SerializeField] private Transform spawnPoint;
        public Location Location;

        public Transform GetSpawnPoint() => spawnPoint;

        public void ShowLocationsToGo() => 
            AllServices.Container.Single<IUIService>().HudContainer.GameCanvas.GenerateLocationButtons(Location.Connections);
    }
}