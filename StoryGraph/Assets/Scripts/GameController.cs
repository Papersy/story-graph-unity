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
    private PlayerController _player = null;
    private List<LocationController> _locations = new List<LocationController>();
    private JToken _playerItems;

    private float _xOffset = 0f;
    
    private string _mainPlayerId = "";
    private string _mainPlayerName = "";
    private string _currentLocationId = "";

    private JToken _worlds;
    private JToken _currentLocation;
    private JToken _availableProductions;
    private LocationController _currentLocationController;
    
    public event Action<string> OnLocationChanged;

    public Vector3 GetPlayerPosition() => _player.Transform.position;
    public Transform GetPlayerTransform() => _player.Transform;
    public JToken GetPlayerItems() => _playerItems;

    public async void DeserializeFile()
    {
        var json = await HttpClientController.GetNewWorld();
        var dict = JToken.Parse(json.ToString());
        _worlds = dict["world"];
        _availableProductions = dict["available_productions"];

        GetMainLocationId(dict);
        GetMainPlayerId(dict);
        GenerateLocation(_worlds);
        
        _currentLocation = GetCurrentLocation(_worlds);
        GenerateItemsForLocation(_currentLocation);
    }

    private void DeserializeFileAfterLocationChange(string json)
    {
        var dict = JToken.Parse(json);
        _worlds = dict["world"];
        _availableProductions = dict["available_productions"];

        GetMainLocationId(dict);
        GetMainPlayerId(dict);
        // GenerateLocation(_worlds);
        
        _currentLocation = GetCurrentLocation(_worlds);
        GenerateItemsForLocation(_currentLocation);
    }

    private JToken GetCurrentLocation(JToken worlds)
    {
        foreach (var location in worlds)
        {
            if (location["Id"]?.ToString() == _currentLocationId)
            {
                if (location["Characters"] != null)
                    GenerateCharactersFromLocation(location);

                return location;
            }
        }

        return null;
    }

    private void GenerateCharactersFromLocation(JToken location)
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

    private void GenerateLocation(JToken worlds)
    {
        foreach (var world in worlds)
        {
            if (world["Id"].ToString() == _currentLocationId)
            {
                var prefab = Resources.Load<LocationController>("JsonFiles/Locations/" + world["Name"]);
                var locationController = Object.Instantiate(prefab, new Vector3(_xOffset, 0, 0), Quaternion.identity);
                
                locationController.Id = world["Id"].ToString();
                locationController.Name = world["Name"].ToString();
                
                if(_currentLocationController != null)
                    GameObject.Destroy(_currentLocationController.gameObject);
                _currentLocationController = locationController;
                
                InitPlayer(locationController);
            }
            _xOffset += 100f;
        }
    }

    private void GetMainLocationId(JToken dict)
    {
        var locationInfo = dict["location_info"];
        _currentLocationId = locationInfo["main_location_id"].ToString();
    }

    private void GetMainPlayerId(JToken dict)
    {
        _mainPlayerId = dict["main_character"].ToString();
    }

    private void InitPlayer(LocationController loc)
    {
        if (_player == null)
        {
            var prefab = Resources.Load<PlayerController>(ConstantsData.PlayerAddress);
            _player = Object.Instantiate(prefab, loc.GetSpawnPoint().position, Quaternion.identity);
        }
        
        
        _player.Transform.position = loc.GetSpawnPoint().position;
        _player.EnableCharacterController(true);
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

        WriteLogAboutNewWorld(json);

        _player.EnableCharacterController(false);
        _currentLocationId = id;
        
        AllServices.Container.Single<IUIService>().HudContainer.GameCanvas.HideLocationsContainer();
        OnLocationChanged?.Invoke(variant[2]["WorldNodeName"].ToString());

        DeserializeFileAfterLocationChange(json);
    }

    public async void DropItem(string droppingItemName)
    {
        var json = await HttpClientController.PostNewWorld(_worlds, FindProd("Dropping item", _availableProductions), FindVariantOfDropping(droppingItemName), _mainPlayerName);

        WriteLogAboutNewWorld(json);

        DeserializeFileAfterLocationChange(json);
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

    private JToken FindVariantOfDropping(string itemName)
    {
        char[] delimiter = { '/' };
        
        foreach (var entity in _availableProductions)
        {
            var title = entity["prod"]["Title"].ToString();
            string[] words = title.Split(delimiter);
            string firstWord = words[0].Trim();

            if (firstWord == "Dropping item")
            {
                Debug.Log(entity);
                foreach (var variant in entity["variants"])
                {
                    if (variant[2]["WorldNodeName"].ToString() == itemName)
                        return variant;
                }
            }
        }

        return null;
    }

    private void WriteLogAboutNewWorld(string json)
    {
        string filePath = "Assets/Resources/JsonFiles/CurrentWorld.json";

        using (StreamWriter writer = new StreamWriter(filePath))
        {
            string jsonFormatted = JValue.Parse(json.ToString()).ToString(Formatting.Indented);
            writer.Write(jsonFormatted);
        }
    }
}