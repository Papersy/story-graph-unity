using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ApiController;
using CodeBase.Infrastructure.Services;
using Deserialization;
using LocationDir;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

        private string _mainLocationId = "";
        private string _mainPlayerId = "";
        private string _mainPlayerName = "";
        private string _currentLocationId = "";

        private JToken _currentLocation;
        
        public event Action<string> OnLocationChanged;

        public void GenerateMap()
        {
            // HttpClientController httpClientController = new HttpClientController();
            // List<World> lSide;

            // string json = await httpClientController.GetNewWorld();
            // lSide = DeserializeFile(json);
            DeserializeFile();

            // _locations = new List<LocationController>();

            // GenerateLocations(lSide);
            // InitPlayer();
        }

        private void DeserializeFile()
        {
            TextAsset text = Resources.Load("JsonFiles/dragons2") as TextAsset;
            var json = text.ToString();

            var dict = JToken.Parse(json);
            var availableProductions = dict["available_productions"];
            var worlds = dict["world"];

            GetMainLocationId(dict);
            GetMainPlayerId(dict);
            _currentLocation = GetFirstLocationOrNull(worlds);

            GenerateLocations(worlds);
        }

        private JToken GetFirstLocationOrNull(JToken worlds)
        {
            foreach (var location in worlds)
            {
                if (location["Id"]?.ToString() == _mainLocationId)
                {
                    if (location["Characters"] != null)
                    {
                        var characters = location["Characters"];
                        foreach (var character in characters)
                        {
                            Debug.Log(character["Name"]);

                            if (character["Id"].ToString() == _mainPlayerId)
                                _mainPlayerName = character["Name"].ToString();
                        }
                    }

                    return location;
                }
            }

            return null;
        }

        private void GenerateLocations(JToken worlds)
        {
            var xOffset = 0f;
            foreach (var world in worlds)
            {
                var prefab = Resources.Load<LocationController>("JsonFiles/Locations/" + world["Name"]);
                var locationController = Object.Instantiate(prefab, new Vector3(xOffset, 0, 0), Quaternion.identity);

                // locationController.World = world;
                // _locations.Add(locationController);

                if (world["Id"].ToString() != _mainLocationId)
                    locationController.gameObject.SetActive(false);
                else 
                    InitPlayer(locationController);

                xOffset += 100f;
            }
        }

        private void GetMainLocationId(JToken dict)
        {
            var locationInfo = dict["location_info"];
            _mainLocationId = locationInfo["main_location_id"].ToString();
            _currentLocationId = _mainLocationId;
        }

        private void GetMainPlayerId(JToken dict)
        {
            _mainPlayerId = dict["main_character"].ToString();
        }

        private void InitPlayer(LocationController loc)
        {
            var prefab = Resources.Load<PlayerController>("Player/Player");
            _player = Object.Instantiate(prefab, loc.GetSpawnPoint().position, Quaternion.identity);

            _player.transform.position = loc.GetSpawnPoint().position;
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
                    _currentLocationId = id;

                    AllServices.Container.Single<IUIService>().HudContainer.GameCanvas.HideLocationsContainer();
                    OnLocationChanged?.Invoke(GetLocationNameById(_currentLocationId));
                    _player.EnableCharacterController(true);

                    loc.ShowCharacters(loc.World);
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