﻿using CodeBase.Infrastructure.Services;
using Infrastructure.Services;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;

namespace LocationDir
{
    public class Teleport : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _teleportName;

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
                // AllServices.Container.Single<IUIService>().HudContainer.GameCanvas.ShowLocationContainer();
                // locationController.ShowLocationsToGo();

                AllServices.Container.Single<IGameService>().GetGameController().ChangeLocation(Variant);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
                AllServices.Container.Single<IUIService>().HudContainer.GameCanvas.HideLocationsContainer();
        }
    }
}