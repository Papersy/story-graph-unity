using System;
using System.Collections.Generic;
using System.IO;
using ApiController;
using CodeBase.Infrastructure.Services;
using Infrastructure;
using Infrastructure.Services;
using LocationDir;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Player;
using UnityEngine;
using Object = UnityEngine.Object;

public class GameController
{
    private PlayerController _player;
    private List<LocationController> _locations = new List<LocationController>();
    private JToken _playerItems;

    private string _mainLocationId = "";
    private string _mainPlayerId = "";
    private string _mainPlayerName = "";
    private string _currentLocationId = "";

    private JToken _worlds;
    private JToken _currentLocation;
    private JToken _availableProductions;
    private LocationController _currentLocationController;
    
    public event Action<string> OnLocationChanged;

    public Vector3 GetPlayerPosition() => _player.transform.position;
    public JToken GetPlayerItems() => _playerItems;

    public void DeletePlayerItem(JToken item)
    {
        foreach (var playerItem in _playerItems)
        {
            if (playerItem["Id"] == item["Id"])
            {
                playerItem.Remove();
                break;
            }
        }
    }
    
    public async void DeserializeFile()
    {
        var json = await HttpClientController.GetNewWorld();
        var dict = JToken.Parse(json.ToString());
        _worlds = dict["world"];
        _availableProductions = dict["available_productions"];

        GetMainLocationId(dict);
        GetMainPlayerId(dict);
        GenerateLocations(_worlds);
        
        _currentLocation = GetFirstLocationOrNull(_worlds);
        GenerateItemsForLocation(_currentLocation);
    }

    public void DeserializeFile(string json)
    {
        var dict = JToken.Parse(json);
        _worlds = dict["world"];
        _availableProductions = dict["available_productions"];

        GetMainLocationId(dict);
        GetMainPlayerId(dict);
        GenerateLocations(_worlds);
        
        _currentLocation = GetFirstLocationOrNull(_worlds);
        GenerateItemsForLocation(_currentLocation);
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
                        if (character["Id"].ToString() == _mainPlayerId)
                        {
                            _mainPlayerName = character["Name"].ToString();
                            _playerItems = character["Items"];
                        }
                        else
                            _currentLocationController.SpawnNpc(character);
                    }
                }

                return location;
            }
        }

        return null;
    }

    private void GenerateItemsForLocation(JToken world)
    {
        var items = world["Items"];
        if (items != null)
        {
            foreach (var item in items)
            {
                _currentLocationController.SpawnItem(item);
            }
        }
        
    }

    private void GenerateItemsForNpc(JToken items)
    {
        
    }

    private void GenerateLocations(JToken worlds)
    {
        var xOffset = 0f;
        foreach (var world in worlds)
        {
            var prefab = Resources.Load<LocationController>("JsonFiles/Locations/" + world["Name"]);
            var locationController = Object.Instantiate(prefab, new Vector3(xOffset, 0, 0), Quaternion.identity);

            locationController.Id = world["Id"].ToString();
            locationController.Name = world["Name"].ToString();
            _locations.Add(locationController);

            if (world["Id"].ToString() != _mainLocationId)
                locationController.gameObject.SetActive(false);
            else
            {
                _currentLocationController = locationController;
                InitPlayer(locationController);
            }

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
        var prefab = Resources.Load<PlayerController>(ConstantsData.PlayerAddress);
        _player = Object.Instantiate(prefab, loc.GetSpawnPoint().position, Quaternion.identity);

        _player.transform.position = loc.GetSpawnPoint().position;
    }

    public void ShowLocationToGo()
    {
        char[] delimiter = { '/' };
        JToken teleportationVariants = null;
        foreach (var availableProduction in _availableProductions)
        {
            var title = availableProduction["prod"]["Title"].ToString();
            string[] words = title.Split(delimiter);
            string firstWord = words[0].Trim();

            if (firstWord == "Location change")
            {
                teleportationVariants = availableProduction["variants"];
            }

        }
        AllServices.Container.Single<IUIService>().HudContainer.GameCanvas.GenerateLocationButtons(teleportationVariants);
    }

    public async void ChangeLocation(string id, JToken variant)
    {
        var json = await HttpClientController.PostNewWorld(_worlds, FindProd("Location change", _availableProductions), variant, _mainPlayerName);
        
        string filePath = "Assets/Resources/JsonFiles/CurrentWorld.json";

        using (StreamWriter writer = new StreamWriter(filePath))
        {
            string jsonFormatted = JValue.Parse(json.ToString()).ToString(Formatting.Indented);
            writer.Write(jsonFormatted);
        }
        
        foreach (var loc in _locations)
        {
            if (loc.Id == id)
            {
                loc.gameObject.SetActive(true);
                _player.EnableCharacterController(false);
                _player.transform.position = loc.GetSpawnPoint().position;
                _currentLocationId = id;

                AllServices.Container.Single<IUIService>().HudContainer.GameCanvas.HideLocationsContainer();
                OnLocationChanged?.Invoke(GetLocationNameById(_currentLocationId));
                _player.EnableCharacterController(true);
            }
            else
                loc.gameObject.SetActive(false);
        }
        
        DeserializeFile(json);
    }

    public string GetLocationNameById(string id)
    {
        foreach (var location in _locations)
        {
            if (location.Id.Equals(id))
                return location.Name;
        }

        return "unknown_location";
    }

    private JToken FindProd(string name, JToken tokenForSearch)
    {
        char[] delimiter = { '/' };
        
        foreach (var entity in tokenForSearch)
        {
            var title = entity["prod"]["Title"].ToString();
            string[] words = title.Split(delimiter);
            string firstWord = words[0].Trim();

            if (firstWord == name)
            {
                return entity["prod"];
            }
        }

        return null;
    }
}