using System;
using System.Collections.Generic;
using CodeBase.Infrastructure.Services;
using Infrastructure.Services;
using LocationDir;
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

    private JToken _currentLocation;
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
    
    public void DeserializeFile()
    {
        TextAsset text = Resources.Load("JsonFiles/dragons2") as TextAsset;
        var json = text.ToString();

        var dict = JToken.Parse(json);
        var availableProductions = dict["available_productions"];
        var worlds = dict["world"];

        GetMainLocationId(dict);
        GetMainPlayerId(dict);
        GenerateLocations(worlds);
        
        _currentLocation = GetFirstLocationOrNull(worlds);
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
                        {
                            _currentLocationController.SpawnNpc(character["Name"].ToString());
                        }
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
        var prefab = Resources.Load<PlayerController>("Player/Player");
        _player = Object.Instantiate(prefab, loc.GetSpawnPoint().position, Quaternion.identity);

        _player.transform.position = loc.GetSpawnPoint().position;
    }

    public void ShowLocationToGo()
    {
        AllServices.Container.Single<IUIService>().HudContainer.GameCanvas.GenerateLocationButtons(_currentLocation["Connections"]);
    }

    public void ChangeLocation(string id)
    {
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
}