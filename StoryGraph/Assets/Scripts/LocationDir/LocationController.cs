using System.Collections.Generic;
using CodeBase.Infrastructure.Services;
using Deserialization;
using Infrastructure.Services;
using UnityEngine;

namespace LocationDir
{
    public class LocationController : MonoBehaviour
    {
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private List<CharacterInfo> charactersInfo;
        public World World;

        public Transform GetSpawnPoint() => spawnPoint;

        public void ShowLocationsToGo() => 
            AllServices.Container.Single<IUIService>().HudContainer.GameCanvas.GenerateLocationButtons(World.Connections);

        public void ShowCharacters(World world)
        {
            foreach (var character in world.Characters)
            {
                foreach (var characterInfo in charactersInfo)
                {
                    if(character.Name == characterInfo.GetName())
                        characterInfo.gameObject.SetActive(true);
                }
            }
        }
    }
}