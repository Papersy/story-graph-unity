using System;
using System.Collections.Generic;
using System.Linq;
using CodeBase.Infrastructure.Services;
using LocationDir;
using Newtonsoft.Json;
using Player;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Infrastructure.Services
{
    public class GameService : IGameService
    {
        // public void Game _game; <- make all logic here
        
        private PlayerController _player;
        private List<LocationController> _locations;
        private string _currentId = "";

        public event Action<string> OnLocationChanged;
        
        public void GenerateMap()
        {
            var lSide = DeserializeFile();

            _locations = new List<LocationController>();

            GenerateLocations(lSide);
            InitPlayer();
        }
        
        private static LSide DeserializeFile()
        {
            TextAsset text = Resources.Load("JsonFiles/dragons") as TextAsset;
            var json = text.ToString();
            List<Root> roots = JsonConvert.DeserializeObject<List<Root>>(json);

            Root root = roots[0];
            LSide lSide = root.LSide;
            return lSide;
        }
        
        private void GenerateLocations(LSide lSide)
        {
            var xOffset = 0f;
            foreach (var loc in lSide.Locations)
            {
                var prefab = Resources.Load<LocationController>("JsonFiles/Locations/" + loc.Name);
                var locationController = Object.Instantiate(prefab,new Vector3(xOffset, 0, 0), Quaternion.identity);

                locationController.Location = loc;
                _locations.Add(locationController);
            
                if (loc.Name == "Road") 
                    _currentId = loc.Id;
                else
                    locationController.gameObject.SetActive(false);
            
                xOffset += 100f;
            }
        }
        
        private void InitPlayer()
        {
            foreach (var loc in _locations.Where(loc => loc.Location.Name == "Road"))
            {
                var prefab = Resources.Load<PlayerController>("Player/Player");
                _player = Object.Instantiate(prefab, loc.GetSpawnPoint().position, Quaternion.identity);
                
                _player.transform.position = loc.GetSpawnPoint().position;
            }
        }

        public void ChangeLocation(string id)
        {
            foreach (var loc in _locations)
            {
                if (loc.Location.Id == id)
                {
                    loc.gameObject.SetActive(true);
                    _player.EnableCharacterController(false);
                    _player.transform.position = loc.GetSpawnPoint().position;
                    _currentId = id;

                    AllServices.Container.Single<IUIService>().HudContainer.GameCanvas.HideLocationsContainer();
                    OnLocationChanged?.Invoke(GetLocationNameById(_currentId));
                    _player.EnableCharacterController(true);
                }
                else
                    loc.gameObject.SetActive(false);
            }
        }

        public string GetLocationNameById(string id)
        {
            foreach (var location in _locations)
            {
                if (location.Location.Id.Equals(id))
                    return location.Location.Name;
            }

            return "unknown_location";
        }
    }
}