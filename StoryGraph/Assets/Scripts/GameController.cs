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
    public string GetPlayerName() => _mainPlayerName;
    public Transform GetPlayerTransform() => _player.Transform;
    public JToken GetPlayerItems() => _playerItems;

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
        GenerateLocation(_jWorlds);
    }

    private void DeserializeFileAfterLocationChange(string json)
    {
        Debug.Log("Location variant" + json);

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

                var hp = character["Attributes"]["HP"].ToString();
                PlayerStats.Health = Convert.ToInt32(hp);
            }
            // else
            //     _currentLocationController.GenerateNpc(character);
        }
    }

    private void GenerateLocation(JToken worlds)
    {
        foreach (var world in worlds)
        {
            if (world["Id"].ToString() == _currentLocationId)
            {
                var prefab = Resources.Load<LocationController>("JsonFiles/Locations/" + world["Name"]);
                if (prefab == null)
                    prefab = Resources.Load<LocationController>("JsonFiles/Locations/default_location");
                var locationController = Object.Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.identity);

                if (_currentLocationController != null)
                {
                    _currentLocationController.ClearLocation();
                    Object.Destroy(_currentLocationController.gameObject);
                }

                _currentLocationController = locationController;
                _currentLocationController.InitLocation(_jCurrentLocation,
                    FindVariantsOfLocationChange("Location change", _jAvailableProductions));

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
        var productionName = "Dropping item";
        string[] parameters = {droppingItemName};
        var json = await HttpClientController.PostNewWorld(_jWorlds,
            FindProd(productionName, _jAvailableProductions),
            FindVariant(productionName, parameters, StatementDrop),         
            _mainPlayerName);

        WriteLogAboutNewWorld(json);

        DeserializeFileAfterInventoryChange(json);
    }

    public async void PickItem(string pickingItemName)
    {
        var productionName = "Picking item up";
        string[] parameters = {pickingItemName};
        var json = await HttpClientController.PostNewWorld(_jWorlds,
            FindProd(productionName, _jAvailableProductions),
            FindVariant(productionName, parameters, StatementPickUp),
            _mainPlayerName);

        WriteLogAboutNewWorld(json);

        DeserializeFileAfterInventoryChange(json);
    }

    public async void GiveItemToNpc(string npcName)
    {
        var productionName = "Item acquisition from another character";
        string[] parameters = {npcName};
        var json = await HttpClientController.PostNewWorld(_jWorlds,
            FindProd(productionName, _jAvailableProductions),
            FindVariant(productionName, parameters, StatementGiveItemToNpc),
            _mainPlayerName);
    
        WriteLogAboutNewWorld(json);
        DeserializeFileAfterInventoryChange(json);
    }

    public async void TakeItemFromNpc(string npcName)
    {
        var productionName = "Item acquisition from another character";
        string[] parameters = { npcName };
        var json = await HttpClientController.PostNewWorld(_jWorlds,
            FindProd(productionName, _jAvailableProductions),
            FindVariant(productionName, parameters, StatementTakeItemFromNpc),
            _mainPlayerName);

        WriteLogAboutNewWorld(json);
        DeserializeFileAfterInventoryChange(json);
    }

    public async void PuttingItem(string puttingItemName)
    {
        var productionName = "Putting item in";
        string[] parameters = {puttingItemName};
        var json = await HttpClientController.PostNewWorld(_jWorlds,
            FindProd(productionName, _jAvailableProductions),
                    FindVariant(productionName, parameters, StatementPullIn),      
            _mainPlayerName);

        WriteLogAboutNewWorld(json);

        DeserializeFileAfterInventoryChange(json);
    }

    public async void PullingItem(string pullingItemName)
    {
        var productionName = "Pulling item out";
        string[] parameters = {pullingItemName};
        var json = await HttpClientController.PostNewWorld(_jWorlds,
            FindProd(productionName, _jAvailableProductions),
            FindVariant(productionName, parameters, StatementPullOut),
            _mainPlayerName);

        WriteLogAboutNewWorld(json);

        DeserializeFileAfterInventoryChange(json);
    }

    public async void GroupCharacter(string npcName)
    {
        var productionName = "Overwhelming character";
        string locationId = _currentLocationId;
        string[] parameters = {locationId, npcName};
        var json = await HttpClientController.PostNewWorld(_jWorlds,
            FindProd(productionName, _jAvailableProductions),
            FindVariant(productionName, parameters, StatementCreateGroup),
            _mainPlayerName);

        WriteLogAboutNewWorld(json);

        DeserializeFileAfterInventoryChange(json);
    }

    public async void TradeWithCharacter(string npcName)
    {
        var productionName = "Exchanging item for item";
        string locationId = _currentLocationId;
        string[] parameters = {locationId, npcName};
        var json = await HttpClientController.PostNewWorld(_jWorlds,
            FindProd(productionName, _jAvailableProductions),
            FindVariant(productionName, parameters, StatementTradeWithCharacters),
            _mainPlayerName);

        WriteLogAboutNewWorld(json);

        DeserializeFileAfterInventoryChange(json);
    }
    
    

    public async void EscapeFromBattle(string fighterName, string escaperId)
    {
        var productionName = "Fight ending with character’s escape";
        string locationName = _currentLocationId;
        string[] parameters = { locationName, fighterName, escaperId };
        
        var json = await HttpClientController.PostNewWorld(_jWorlds,
            FindProd(productionName, _jAvailableProductions),
            FindVariant(productionName, parameters ,StatementEscapeFight),
            _mainPlayerName);

        WriteLogAboutNewWorld(json);

        DeserializeFileAfterInventoryChange(json);
        // DeserializeFileAfterLocationChange(json);
    }

    public async void FightEndWithSomeoneDeath(string fighterName, string escaperId)
    {
        var productionName = "Fight ending with character’s death";
        string locationName = _currentLocationId;
        string[] parameters = { locationName, fighterName, escaperId };

        var json = await HttpClientController.PostNewWorld(_jWorlds,
            FindProd(productionName, _jAvailableProductions),
            FindVariant(productionName, parameters, StatementFightEndWithDeath),
            _mainPlayerName);

        WriteLogAboutNewWorld(json);

        // DeserializeFileAfterLocationChange(json);
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
    
    private JToken FindProd(string name, JToken tokenForSearch)
    {
        foreach (var entity in tokenForSearch)
        {
            var firstWord = GetFirstWord(entity);

            if (firstWord == name)
                return entity["prod"];
        }

        return null;
    }

    public bool IsItStore(string boxName, string productionName)
    {
        foreach (var entity in _jAvailableProductions)
        {
            var firstWord = GetFirstWord(entity);

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
    public bool CanWeGiveToNpc(string npcName)
    {
        var productionName = "Item acquisition from another character";
        string[] parameters = {npcName};
        var result = FindVariant(productionName, parameters, StatementGiveItemToNpc);

        if (result != null)
            return true;

        return false;
    }
    public bool CanWeTakeFromNpc(string npcName)
    {
        var productionName = "Item acquisition from another character";
        string[] parameters = {npcName};
        var result = FindVariant(productionName, parameters, StatementTakeItemFromNpc);

        if (result != null)
            return true;

        return false;
    }
    public bool CanBeGrouped(string npcName)
    {
        var productionName = "Overwhelming character";
        string locationId = _currentLocationId;
        string[] parameters = {locationId, npcName};
        var result = FindVariant(productionName, parameters, StatementCreateGroup);

        if (result != null)
            return true;

        return false;
    }
    public bool CanTradeItem(string npcName)
    {
        var productionName = "Exchanging item for item";
        string locationId = _currentLocationId;
        string[] parameters = {locationId, npcName};
        var result = FindVariant(productionName, parameters, StatementTradeWithCharacters);

        if (result != null)
            return true;

        return false;
    }

    public JToken FindVariantOfGiveItemToNpc(string npcName)
    {
        foreach (var entity in _jAvailableProductions)
        {
            var firstWord = GetFirstWord(entity);

            if (firstWord == "Item acquisition from another character")
            {
                foreach (var variant in entity["variants"])
                {
                    if (variant[1]["WorldNodeName"].ToString() == npcName)
                        return variant;
                }
            }
        }

        return null;
    }

    public JToken FindVariantOfGetItemFromNpc(string npcName)
    {
        foreach (var entity in _jAvailableProductions)
        {
            var firstWord = GetFirstWord(entity);

            if (firstWord == "Item acquisition from another character")
            {
                foreach (var variant in entity["variants"])
                {
                    if (variant[2]["WorldNodeName"].ToString() == npcName)
                        return variant;
                }
            }
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
    
    
    private bool StatementGiveItemToNpc(JToken variant, string[] parameters)
    {
        if (variant[1]["WorldNodeName"].ToString() == parameters[0])
            return true;

        return false;
    }
    
    private bool StatementTakeItemFromNpc(JToken variant, string[] parameters)
    {
        if (variant[2]["WorldNodeName"].ToString() == parameters[0])
            return true;

        return false;
    }
    
    private bool StatementDrop(JToken variant, string[] parameters)
    {
        if (variant[2]["WorldNodeName"].ToString() == parameters[0])
            return true;

        return false;
    }
    
    private bool StatementPickUp(JToken variant, string[] parameters)
    {
        if (variant[2]["WorldNodeName"].ToString() == parameters[0])
            return true;

        return false;
    }
    
    private bool StatementPullIn(JToken variant, string[] parameters)
    {
        if (variant[2]["WorldNodeName"].ToString() == parameters[0])
            return true;

        return false;
    }
    
    private bool StatementPullOut(JToken variant, string[] parameters)
    {
        if (variant[3]["WorldNodeName"].ToString() == parameters[0])
            return true;

        return false;
    }

    private bool StatementEscapeFight(JToken variant, string[] parameters)
    {
        if (variant[0]["WorldNodeId"].ToString() == parameters[0] &&
            variant[1]["WorldNodeName"].ToString() == parameters[1] &&
            variant[2]["WorldNodeId"].ToString() == parameters[2])
            return true;

        return false;
    }

    private bool StatementPlayerDeath(JToken variant, string[] parameters)
    {
        if (variant[1]["WorldNodeName"].ToString() == _mainPlayerName)
            return true;

        return false;
    }

    private bool StatementFightEndWithDeath(JToken variant, string[] parameters)
    {
        if (variant[0]["WorldNodeId"].ToString() == parameters[0] &&
            variant[1]["WorldNodeName"].ToString() == parameters[1] &&
            variant[2]["WorldNodeId"].ToString() == parameters[2])
            return true;

        return false;
    }

    private bool StatementCreateGroup(JToken variant, string[] parameters)
    {
        if (variant[0]["WorldNodeId"].ToString() == parameters[0] &&
            variant[2]["WorldNodeName"].ToString() == parameters[1])
            return true;

        return false;
    }

    private bool StatementTradeWithCharacters(JToken variant, string[] parameters)
    {
        if (variant[0]["WorldNodeId"].ToString() == parameters[0] &&
            variant[2]["WorldNodeName"].ToString() == parameters[1])
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

    private string GetFirstWord(JToken entity)
    {
        char[] delimiter = {'/'};
        
        var title = entity["prod"]["Title"].ToString();
        string[] words = title.Split(delimiter);
        string firstWord = words[0].Trim();

        return firstWord;
    }
}