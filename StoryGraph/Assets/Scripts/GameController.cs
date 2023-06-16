﻿using System;
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
    private List<LocationController> _locations = new List<LocationController>();
    private JToken _playerItems;

    private float _xOffset = 0f;

    private string _mainPlayerId = "";
    private string _mainPlayerName = "";
    private string _currentLocationId = "";

    private JToken _jWorlds;
    private JToken _jCurrentLocation;
    private JToken _jAvailableProductions;
    private LocationController _currentLocationController;

    public event Action<string> OnLocationChanged;

    public Vector3 GetPlayerPosition() => _player.Transform.position;
    public string GetMainPlayerId() => _mainPlayerId;
    public Transform GetPlayerTransform() => _player.Transform;
    public JToken GetPlayerItems() => _playerItems;

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
        GenerateLocation(_jWorlds);
    }

    private void DeserializeFileAfterLocationChange(string json)
    {
        var dict = JToken.Parse(json);
        _jWorlds = dict["world"];
        _jAvailableProductions = dict["available_productions"];

        GetMainLocationId(dict);
        GetMainPlayerId(dict);

        _jCurrentLocation = GetCurrentLocation(_jWorlds);
        GenerateLocation(_jWorlds);
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
                        _playerItems = character["Items"];
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
            }
            // else
            //     _currentLocationController.GenerateNpc(character);
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
                var locationController = Object.Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.identity);

                if (_currentLocationController != null)
                {
                    _currentLocationController.ClearLocation();
                    Object.Destroy(_currentLocationController.gameObject);
                }

                _currentLocationController = locationController;
                _currentLocationController.InitLocation(_jCurrentLocation, FindVariantsOfLocationChange("Location change", _jAvailableProductions));

                InitPlayer(locationController);
            }
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

    public async void ChangeLocation(JToken variant)
    {
        var json = await HttpClientController.PostNewWorld(_jWorlds,
            FindProd("Location change", _jAvailableProductions), variant, _mainPlayerName);

        WriteLogAboutNewWorld(json);

        _player.EnableCharacterController(false);

        OnLocationChanged?.Invoke(variant[2]["WorldNodeName"].ToString());

        DeserializeFileAfterLocationChange(json);
    }

    public async void DropItem(string droppingItemName)
    {
        var json = await HttpClientController.PostNewWorld(_jWorlds, FindProd("Dropping item", _jAvailableProductions),
            FindVariantOfDropping(droppingItemName), _mainPlayerName);

        WriteLogAboutNewWorld(json);

        DeserializeFileAfterInventoryChange(json);
    }

    public async void PickItem(string pickingItemName)
    {
        var json = await HttpClientController.PostNewWorld(_jWorlds,
            FindProd("Picking item up", _jAvailableProductions), FindVariantOfPicking(pickingItemName),
            _mainPlayerName);

        WriteLogAboutNewWorld(json);

        DeserializeFileAfterInventoryChange(json);
    }

    public async void GetItemFromNpc(string npcName, string pickingItemName)
    {
        var json = await HttpClientController.PostNewWorld(_jWorlds,
            FindProd("Item acquisition from another character", _jAvailableProductions),
            FindVariantOfGettingFromNpc(npcName, pickingItemName), _mainPlayerName);

        WriteLogAboutNewWorld(json);
    }

    public async void PuttingItem(string puttingItemName)
    {
        var json = await HttpClientController.PostNewWorld(_jWorlds,
            FindProd("Putting item in", _jAvailableProductions), FindPullIn(puttingItemName, "Putting item in"),
            _mainPlayerName);

        WriteLogAboutNewWorld(json);

        DeserializeFileAfterInventoryChange(json);
    }

    public async void PullingItem(string pullingItemName)
    {
        var json = await HttpClientController.PostNewWorld(_jWorlds,
            FindProd("Pulling item out", _jAvailableProductions), FindPullOut(pullingItemName, "Pulling item out"),
            _mainPlayerName);

        WriteLogAboutNewWorld(json);

        DeserializeFileAfterInventoryChange(json);
    }

    private JToken FindProd(string name, JToken tokenForSearch)
    {
        char[] delimiter = {'/'};

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

    private JToken FindVariantTest(string productionName, string lsNodeTarget, string lsNodeItem)
    {
        char[] delimiter = {'/'};

        foreach (var entity in _jAvailableProductions)
        {
            var title = entity["prod"]["Title"].ToString();
            string[] words = title.Split(delimiter);
            string firstWord = words[0].Trim();

            if (firstWord == productionName)
            {
                foreach (var variant in entity["variants"][0])
                {
                    if (variant["LSNodeRef"].ToString() == lsNodeTarget)
                        return variant;
                }
            }
        }

        return null;
    }

    private JToken FindPullIn(string searchingItem, string productionName)
    {
        char[] delimiter = {'/'};

        foreach (var entity in _jAvailableProductions)
        {
            var title = entity["prod"]["Title"].ToString();
            string[] words = title.Split(delimiter);
            string firstWord = words[0].Trim();

            if (firstWord == productionName)
            {
                foreach (var variant in entity["variants"])
                {
                    if (variant[2]["WorldNodeName"].ToString() == searchingItem)
                        return variant;
                }
            }
        }

        return null;
    }

    private JToken FindPullOut(string searchingItem, string productionName)
    {
        char[] delimiter = {'/'};

        foreach (var entity in _jAvailableProductions)
        {
            var title = entity["prod"]["Title"].ToString();
            string[] words = title.Split(delimiter);
            string firstWord = words[0].Trim();

            if (firstWord == productionName)
            {
                foreach (var variant in entity["variants"])
                {
                    if (variant[3]["WorldNodeName"].ToString() == searchingItem)
                        return variant;
                }
            }
        }

        return null;
    }

    public bool IsItStore(string boxName, string productionName)
    {
        char[] delimiter = {'/'};

        foreach (var entity in _jAvailableProductions)
        {
            var title = entity["prod"]["Title"].ToString();
            string[] words = title.Split(delimiter);
            string firstWord = words[0].Trim();

            if (firstWord == productionName)
            {
                foreach (var variant in entity["variants"])
                {
                    foreach (var item in variant)
                    {
                        if (item["LSNodeRef"].ToString() == "Opakowanie" &&
                            item["WorldNodeName"].ToString() == boxName)
                            return true;
                    }
                }
            }
        }

        return false;
    }

    private JToken FindVariantOfDropping(string itemName)
    {
        char[] delimiter = {'/'};

        foreach (var entity in _jAvailableProductions)
        {
            var title = entity["prod"]["Title"].ToString();
            string[] words = title.Split(delimiter);
            string firstWord = words[0].Trim();

            if (firstWord == "Dropping item")
            {
                foreach (var variant in entity["variants"])
                {
                    if (variant[2]["WorldNodeName"].ToString() == itemName)
                        return variant;
                }
            }
        }

        return null;
    }

    private JToken FindVariantOfPicking(string itemName)
    {
        char[] delimiter = {'/'};

        foreach (var entity in _jAvailableProductions)
        {
            var title = entity["prod"]["Title"].ToString();
            string[] words = title.Split(delimiter);
            string firstWord = words[0].Trim();

            if (firstWord == "Picking item up")
            {
                foreach (var variant in entity["variants"])
                {
                    if (variant[2]["WorldNodeName"].ToString() == itemName)
                        return variant;
                }
            }
        }

        return null;
    }

    private JToken FindVariantOfGettingFromNpc(string npcName, string itemName)
    {
        char[] delimiter = {'/'};

        foreach (var entity in _jAvailableProductions)
        {
            var title = entity["prod"]["Title"].ToString();
            string[] words = title.Split(delimiter);
            string firstWord = words[0].Trim();

            if (firstWord == "Item acquisition from another character")
            {
                foreach (var variant in entity["variants"])
                {
                    if (variant[3]["WorldNodeName"].ToString() == itemName &&
                        variant[0]["WorldNodeName"].ToString() == npcName)
                        return variant;
                }
            }
        }

        return null;
    }

    private JToken FindVariantsOfLocationChange(string name, JToken tokenForSearch)
    {
        char[] delimiter = {'/'};

        foreach (var entity in tokenForSearch)
        {
            var title = entity["prod"]["Title"].ToString();
            string[] words = title.Split(delimiter);
            string firstWord = words[0].Trim();

            if (firstWord == name)
            {
                return entity["variants"];
            }
        }

        return null;
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
}