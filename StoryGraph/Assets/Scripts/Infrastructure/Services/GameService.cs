using System;
using System.Collections.Generic;
using System.Linq;
using ApiController;
using CodeBase.Infrastructure.Services;
using Deserialization;
using LocationDir;
using Newtonsoft.Json;
using Player;
using UnityEditor.VersionControl;
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

        public async void GenerateMap()
        {
            HttpClientController httpClientController = new HttpClientController();
            List<World> lSide;
                
            string json = await httpClientController.GetNewWorld();
            lSide = DeserializeFile(json);

            _locations = new List<LocationController>();

            GenerateLocations(lSide);
            InitPlayer();
        }
        
        private static List<World> DeserializeFile(string json)
        {
            TextAsset text = Resources.Load("JsonFiles/dragons2") as TextAsset;
            var json2 = text.ToString();
            Root roots = JsonConvert.DeserializeObject<Root>(json2);
            
            Debug.Log(json2);

            Root root = roots;
            List<World> worlds = root.world;
            
            return worlds;
        }
        
        private void GenerateLocations(List<World> worlds)
        {
            var xOffset = 0f;
            foreach (var world in worlds)
            {
                var prefab = Resources.Load<LocationController>("JsonFiles/Locations/" + world.Name);
                var locationController = Object.Instantiate(prefab,new Vector3(xOffset, 0, 0), Quaternion.identity);

                locationController.World = world;
                _locations.Add(locationController);
            
                if (world.Name == "Road") 
                    _currentId = world.Id;
                else
                    locationController.gameObject.SetActive(false);
            
                xOffset += 100f;
            }
        }
        
        private void InitPlayer()
        {
            foreach (var loc in _locations.Where(loc => loc.World.Name == "Road"))
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
                if (loc.World.Id == id)
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
                if (location.World.Id.Equals(id))
                    return location.World.Name;
            }

            return "unknown_location";
        }
    }
}