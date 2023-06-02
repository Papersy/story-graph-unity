using System;
using CodeBase.Infrastructure.Services;
using Infrastructure.Services;
using UnityEngine;

namespace LocationDir
{
    public class Teleport : MonoBehaviour
    {
        [SerializeField] private LocationController locationController;
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                AllServices.Container.Single<IUIService>().HudContainer.GameCanvas.ShowLocationContainer();
                locationController.ShowLocationsToGo();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if(other.CompareTag("Player"))
                AllServices.Container.Single<IUIService>().HudContainer.GameCanvas.HideLocationsContainer();
        }
    }
}