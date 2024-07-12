using System.Collections.Generic;
using System.Text.RegularExpressions;
using ActionButtons;
using CodeBase.Infrastructure.Services;
using Infrastructure.Services;
using InteractableItems;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace UI
{
    public class AllActions : BaseWindow
    {
        [SerializeField] private ButtonAction _btnPrefab;
        [SerializeField] private Transform _contentParent;
        
        private JToken _currentProductions;
        private JToken _currentProduction;

        private Dictionary<JToken, JToken> _prodVariants;
        private List<string> _skippedProdNames = new List<string>() { "Teleportation / Teleportacja", "Location change / Zmiana lokacji" };

        public override void OnShow()
        {
            _currentProductions = GameService.GetGameController().GetCurrentProductions();
            InitObjects();
            
            base.OnShow();
        }

        public void Init()
        {
            InitObjects();
        }

        private void InitObjects()
        {
            ClearContentView(_contentParent);
            var colliders = GetInteractions();

            foreach (var collider in colliders) 
            {
                if (collider.TryGetComponent(out Npc.Npc npc))
                {
                    var btn = Instantiate(_btnPrefab, _contentParent);
                    btn.SetText(npc.NpcInfo["Name"].ToString());
                    
                    btn.Button.onClick.AddListener(() => InitProductions(npc.NpcInfo["Id"].ToString())); 
                }
                else if (collider.TryGetComponent(out Item item))
                {
                    var btn = Instantiate(_btnPrefab, _contentParent);
                    btn.SetText(item.ItemInfo["Name"].ToString()); 
                    
                    btn.Button.onClick.AddListener(() => InitProductions(item.ItemInfo["Id"].ToString())); 
                }
            }
            
            var other = Instantiate(_btnPrefab, _contentParent);
            other.SetText("Other");
            other.Button.onClick.AddListener(() => InitProductions2(""));
        }
        
        private void InitProductions(string objectId)
        {
            _prodVariants = new Dictionary<JToken, JToken>();
            ClearContentView(_contentParent);
            
            foreach (var production in _currentProductions)
            {
                List<string> entitiesOnLocationNames = new List<string>();
                
                var prod = production["prod"];
                var variants = production["variants"];
                var title = prod["Title"].ToString();
                var lSide = prod["LSide"];
                var locations = lSide["Locations"];
                var location = locations[0];

                var locationId = location["Id"].ToString();
                var characters = location["Characters"];
                var items = location["Items"];
                
                if(_skippedProdNames.Contains(title))
                    continue;
                
                entitiesOnLocationNames.Add(locationId);
                
                if (characters != null)
                {
                    foreach (var character in characters)
                    {
                        var characterId = character["Id"];
                        if (characterId == null)
                            characterId = character["Name"];
                        
                        if(characterId != null)
                            entitiesOnLocationNames.Add(characterId.ToString());
                    }
                }

                if (items != null)
                {
                    foreach (var item in items)
                    {
                        var itemId = item["Id"];
                        if (itemId == null)
                            itemId = item["Name"];
                        
                        if(itemId != null)
                            entitiesOnLocationNames.Add(itemId.ToString());
                    }
                }
                
                if (CanInteractWithVariant(production, variants, entitiesOnLocationNames, objectId))
                {
                    var btn = Instantiate(_btnPrefab, _contentParent);
                    btn.SetText(GetPolishPart(title));
                    btn.Button.onClick.AddListener(() => OnProductionClick(production)); 
                }
            }
        }

        private void InitProductions2(string objectId)
        {
            _prodVariants = new Dictionary<JToken, JToken>();
            ClearContentView(_contentParent);
            
            foreach (var production in _currentProductions)
            {
                List<string> entitiesOnLocationNames = new List<string>();
                
                var prod = production["prod"];
                var variants = production["variants"];
                var title = prod["Title"].ToString();
                var lSide = prod["LSide"];
                var locations = lSide["Locations"];
                var location = locations[0];

                var locationId = location["Id"];
                var characters = location["Characters"];
                var items = location["Items"];
                
                if(_skippedProdNames.Contains(title))
                    continue;
                
                if(locationId != null)
                    entitiesOnLocationNames.Add(locationId.ToString());
                
                if (characters != null)
                {
                    foreach (var character in characters)
                    {
                        var characterId = character["Id"];
                        if (characterId == null)
                            characterId = character["Name"];
                        
                        if(characterId != null)
                            entitiesOnLocationNames.Add(characterId.ToString());
                    }
                }

                if (items != null)
                {
                    foreach (var item in items)
                    {
                        var itemId = item["Id"];
                        if (itemId == null)
                            itemId = item["Name"];
                        
                        if(itemId != null)
                            entitiesOnLocationNames.Add(itemId.ToString());
                    }
                }
                
                if (CanInteractWithVariant2(production, variants, entitiesOnLocationNames, objectId))
                {
                    var btn = Instantiate(_btnPrefab, _contentParent);
                    btn.SetText(GetPolishPart(title));
                    btn.Button.onClick.AddListener(() => OnProductionClick(production)); 
                }
            }
        }
        
        private bool CanInteractWithVariant(JToken production, JToken variants, List<string> entitiesOnLocationNames, string objectId)
        {
            var playerId = GameService.GetGameController().GetMainPlayerId();
            var currentLocationId = GameService.GetGameController().GetCurrentLocationId();
            // var collisionIds = CheckInteraction.CollisionIds;
            var colliders = GetInteractions();

            bool functionResult = false;
            
            foreach (var variant in variants)
            {
                var result = true;
                var resultId = false;
                foreach (var podVariant in variant)
                {
                    var LSNodeRef = podVariant["LSNodeRef"].ToString();
                    if (entitiesOnLocationNames.Contains(LSNodeRef))
                    {
                        var WorldNodeId = podVariant["WorldNodeId"].ToString();
                        if (WorldNodeId == objectId)
                            resultId = true;
                        // if (WorldNodeId != playerId && WorldNodeId != currentLocationId && !collisionIds.ContainsKey(WorldNodeId))
                        // {
                        //     result = false;
                        //     break;
                        // }
                        if (WorldNodeId != playerId && WorldNodeId != currentLocationId && !ContainIdOnColliders(WorldNodeId, colliders))
                        {
                            result = false;
                            break;
                        }
                    }
                }

                if (result && resultId)
                {
                    _prodVariants.Add(variant, production);
                    
                    functionResult = true;
                }
            }
            
            return functionResult;
        }
        private bool CanInteractWithVariant2(JToken production, JToken variants, List<string> entitiesOnLocationNames, string objectId)
        {
            var playerId = GameService.GetGameController().GetMainPlayerId();
            var currentLocationId = GameService.GetGameController().GetCurrentLocationId();
            // var collisionIds = CheckInteraction.CollisionIds;

            bool functionResult = false;

            foreach (var variant in variants)
            {
                var result = true;
                foreach (var podVariant in variant)
                {
                    var LSNodeRef = podVariant["LSNodeRef"].ToString();
                    if (entitiesOnLocationNames.Contains(LSNodeRef))
                    {
                        var WorldNodeId = podVariant["WorldNodeId"].ToString();
                        if (WorldNodeId == playerId || WorldNodeId == currentLocationId)
                        {
                            //ok
                        }
                        else
                        {
                            result = false;
                            break;
                        }
                    }
                }

                if (result)
                {
                    _prodVariants.Add(variant, production);
                    
                    functionResult = true;
                }
            }
            
            return functionResult;
        }
        
        private void InitVariants()
        {
            foreach (var i in _prodVariants)
            {
                var prod = i.Value["prod"];
                var description = prod["Description"].ToString();
                
                if (i.Value == _currentProduction)
                {
                    var btn = Instantiate(_btnPrefab, _contentParent);
                    string newDescription = description;

                    var variant = i.Key;
                    
                    foreach (var podVariant in variant)
                    {
                        var searchWord = podVariant["LSNodeRef"].ToString();
                        var changeTo = podVariant["WorldNodeName"].ToString();
                    
                        newDescription = RenameDescription(newDescription, searchWord, changeTo);
                    }
                
                    btn.SetText(newDescription);
                    btn.Button.onClick.AddListener(() => OnVariantClick(prod, variant));
                }       
            }
        }
        
        private void OnProductionClick(JToken production)
        {
            _currentProduction = production;
            ClearContentView(_contentParent);
            InitVariants();
        }

        private void OnVariantClick(JToken prod, JToken variant)
        {
            GameService.GetGameController().PostNewWorld(prod, variant);
            
            GameCanvas.IsUiActive = false;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            
            Hide();
        }

        private Collider[] GetInteractions()
        {
            var playerPos = AllServices.Container.Single<IGameService>().GetGameController().GetPlayerPosition();
            Collider[] colliders = Physics.OverlapSphere(playerPos, 5f);
            
            return colliders;
        }

        private bool ContainIdOnColliders(string id, Collider[] colliders)
        {
            foreach (var collider in colliders)
            {
                if(collider.TryGetComponent(out Npc.Npc npc)) 
                    if (npc.NpcInfo["Id"].ToString() == id) return true;
                if(collider.TryGetComponent(out Item item)) 
                        if (item.ItemInfo["Id"].ToString() == id) return true;
            }

            return false;
        }
        
        private string RenameDescription(string desc, string target, string replaceWord)
        {
            string pattern = @"«(.*?)»";
            
            string result = Regex.Replace(desc, pattern, match =>
            {
                string valueBetweenQuotes = match.Value.Trim('«', '»');
                
                if (valueBetweenQuotes == target)
                    return replaceWord;
                else
                    return match.Value;
            });

            return result;
        }
        
        private void ClearContentView(Transform content)
        {
            if(content.childCount == 0)
                return;
            
            for (var i = content.childCount - 1; i >= 0; i--)
                Destroy(content.GetChild(i).gameObject);
        }
        
        private string GetPolishPart(string title)
        {
            char[] delimiter = {'/'};
        
            string[] words = title.Split(delimiter);

            if (words.Length > 1)
                return words[1].Trim();
            
            return words[0].Trim();
        }
    }
}