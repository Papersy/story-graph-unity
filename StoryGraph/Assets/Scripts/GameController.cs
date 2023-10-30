using System;
using System.Collections.Generic;
using System.IO;
using ApiController;
using Infrastructure;
using LocationDir;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Player;
using UnityEngine;
using Object = UnityEngine.Object;

public class GameController
{
    private PlayerController _player = null;
    private JToken _playerItems;
    private JToken _playerCharacters;

    private string _mainPlayerId = "";
    private string _mainPlayerName = "";
    private string _currentLocationId = "";
    private const float LocationOffset = 65f;
    private float _locationCoord = 0f;

    private JToken _jWorlds;
    private JToken _jCurrentLocation;
    private JToken _jAvailableProductions;
    private LocationController _currentLocationController;
    private List<LocationController> _locationControllers = new();

    public event Action<string> OnLocationChanged;

    public Vector3 GetPlayerPosition() => _player.Transform.position;
    public JToken GetCurrentProductions() => _jAvailableProductions;
    public string GetMainPlayerId() => _mainPlayerId;
    public string GetCurrentLocationId() => _currentLocationId;
    public JToken GetPlayerItems() => _playerItems;
    public JToken GetPlayerCharacters() => _playerCharacters;

    private delegate bool StatementsCheck(JToken variant, string[] parameters);
    
    public async void DeserializeFile()
    {
        var json = await HttpClientController.GetNewWorld();
        var dict = JToken.Parse(json.ToString());
        _jWorlds = dict["world"];
        _jAvailableProductions = dict["available_productions"];

        WriteLogAboutNewWorld(json);
        GetMainLocationId(dict);
        GetMainPlayerId(dict);

        _jCurrentLocation = GetCurrentLocation(_jWorlds);
        GenerateLocations(_jWorlds);
    }

    private void DeserializeFileAfterLocationChange(string json)
    {
        var dict = JToken.Parse(json);
        _jWorlds = dict["world"];
        _jAvailableProductions = dict["available_productions"];

        GetMainLocationId(dict);
        GetMainPlayerId(dict);
        
        _jCurrentLocation = GetCurrentLocation(_jWorlds);
        UpdateAfterLocationChange();
    }

    private void DeserializeFileAfterInventoryChange(string json)
    {
        var dict = JToken.Parse(json);
        _jWorlds = dict["world"];
        _jAvailableProductions = dict["available_productions"];

        foreach (var location in _jWorlds)
        {
            if (location["Id"]?.ToString() == _currentLocationId)
            {
                _jCurrentLocation = location;
                
                var characters = location["Characters"];

                foreach (var character in characters)
                {
                    if (character["Id"].ToString() == _mainPlayerId)
                    {
                        _playerItems = character["Items"];
                        _playerCharacters = character["Characters"];
                    }
                }
            }
        }
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

                var attributes = character["Attributes"];
                if (attributes != null)
                {
                    var hp = attributes["HP"];
                    if (hp != null)
                    {
                        PlayerStats.Health = Convert.ToInt32(hp.ToString());
                    }
                }
                
            }
        }
    }

    private void UpdateAfterLocationChange()
    {
        foreach (var controller in _locationControllers)
        {
            if (controller._locationInfo["Id"].ToString() == _currentLocationId)
            {
                if (!controller.IsInited)
                {
                    _currentLocationController = controller;
                    _currentLocationController.InitLocation(_jCurrentLocation, FindVariantsOfLocationChange("Location change", _jAvailableProductions)); 
                }
                
                InitPlayer(controller);
            }
        }
    }
    
    private void GenerateLocations(JToken worlds)
    {
        foreach (var world in worlds)
        {
            var prefab = GetLocationPrefab(world);
            var locationController = Object.Instantiate(prefab, new Vector3(0, 0, _locationCoord), Quaternion.identity);
            
            if (world["Id"].ToString() == _currentLocationId)
            {
                _currentLocationController = locationController;
                _currentLocationController.InitLocation(_jCurrentLocation, FindVariantsOfLocationChange("Location change", _jAvailableProductions));

                InitPlayer(locationController);
            }
            else
                locationController.SetLocationInfo(world);
            
            _locationControllers.Add(locationController);
            _locationCoord += LocationOffset;
        }
    }

    private static LocationController GetLocationPrefab(JToken world)
    {
        var prefab = Resources.Load<LocationController>("JsonFiles/Locations/" + world["Name"]);
        if (prefab == null)
            prefab = Resources.Load<LocationController>("JsonFiles/Locations/default_location");
        return prefab;
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

    public async void PostNewWorld(JToken prod, JToken variant)
    {
        var json = await HttpClientController.PostNewWorld(_jWorlds, prod, variant, _mainPlayerName);
        
        WriteLogAboutNewWorld(json);
        DeserializeFileAfterInventoryChange(json);
        UpdateLocation(json);
    }
    
    private async void UpdateLocation(string json)
    {
        var dict = JToken.Parse(json);
        _jAvailableProductions = dict["available_productions"];
        var newJTokenWorld = dict["world"];
        var locationInfo = dict["location_info"];
        var mainLocationId = locationInfo["main_location_id"].ToString();

        var newLocationToken = GetNewLocationToken(newJTokenWorld, mainLocationId);
        
        if (_currentLocationId != mainLocationId)
        {
            _currentLocationId = mainLocationId;
            UpdateAfterLocationChange();
        }
        
        var characters = newLocationToken["Characters"];
        var items = newLocationToken["Items"];
        
        await _currentLocationController.UpdateCharacters(characters);
        await _currentLocationController.UpdateItems(items);
        // _currentLocationController.SetPositions(_jAvailableProductions);
    }

    private JToken GetNewLocationToken(JToken newWorld, string newLocationId)
    {
        foreach (var world in newWorld)
        {
            var worldId = world["Id"].ToString();
            if (worldId == newLocationId)
                return world;
        }

        return null;
    }
    
    public async void ChangeLocation(JToken variant)
    {
        var json = await HttpClientController.PostNewWorld(_jWorlds,
            FindProd("Location change", _jAvailableProductions), variant, _mainPlayerName);

        WriteLogAboutNewWorld(json);

        _player.EnableCharacterController(false);

        OnLocationChanged?.Invoke(variant[2]["WorldNodeName"].ToString());

        DeserializeFileAfterLocationChange(json);
    }
    
    
    public async void HeroDeath()
    {
        var productionName = "Fight ending with character’s escape";
        var json = await HttpClientController.PostNewWorld(_jWorlds,
            FindProd(productionName, _jAvailableProductions),
            FindVariant(productionName, null, StatementPlayerDeath),
            _mainPlayerName);

        WriteLogAboutNewWorld(json);

        DeserializeFileAfterLocationChange(json);
    }
    
    private JToken FindProd(string name, JToken tokenForSearch, string parameter = "Title")
    {
        foreach (var entity in tokenForSearch)
        {
            var firstWord = GetFirstWord(entity, parameter);

            if (firstWord == name)
                return entity["prod"];
        }

        return null;
    }

    private JToken FindVariantsOfLocationChange(string name, JToken tokenForSearch)
    {
        foreach (var entity in tokenForSearch)
        {
            var firstWord = GetFirstWord(entity);

            if (firstWord == name)
                return entity["variants"];
        }
        return null;
    }
    
    private JToken FindVariant(string productionName, string[] parameters, StatementsCheck statementsCheck)
    {
        foreach (var entity in _jAvailableProductions)
        {
            var firstWord = GetFirstWord(entity);

            if (firstWord == productionName)
            {
                foreach (var variant in entity["variants"])
                {
                    if (statementsCheck(variant, parameters))
                        return variant;
                }
            }
        }

        return null;
    }
    
    private bool StatementPlayerDeath(JToken variant, string[] parameters)
    {
        if (variant[1]["WorldNodeName"].ToString() == _mainPlayerName)
            return true;

        return false;
    }
    
    
    private void WriteLogAboutNewWorld(string json)
    {
        var filePath = "Assets/Resources/JsonFiles/CurrentWorld.json";

        using (StreamWriter writer = new StreamWriter(filePath))
        {
            string jsonFormatted = JValue.Parse(json.ToString()).ToString(Formatting.Indented);
            writer.Write(jsonFormatted);
        }
    }

    private string GetFirstWord(JToken entity, string parameter = "Title")
    {
        char[] delimiter = {'/'};
        
        var title = entity["prod"][parameter].ToString();
        string[] words = title.Split(delimiter);
        string firstWord = words[0].Trim();

        return firstWord;
    }

    public JToken GetPlayerInfo()
    {
        var characters = _jCurrentLocation["Characters"];
        foreach (var character in characters)
        {
            if (character["Name"].ToString() == _mainPlayerName)
                return character;
        }

        return null;
    }
}