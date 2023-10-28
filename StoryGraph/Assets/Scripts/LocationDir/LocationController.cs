using System.Collections.Generic;
using System.Threading.Tasks;
using CodeBase.Infrastructure.Services;
using Infrastructure.Services;
using InteractableItems;
using Newtonsoft.Json.Linq;
using Player;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

namespace LocationDir
{
    public class LocationController : MonoBehaviour
    {
        [SerializeField] private BoxCollider colliderForItems;
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private Teleport _portalPrefab;
        [SerializeField] private List<Teleport> _initedTeleports;
        
        private List<GameObject> _characters = new List<GameObject>();
        private List<GameObject> _items = new List<GameObject>();
        private List<GameObject> _teleports = new List<GameObject>();

        private int teleportIndex = 0;
        private JToken _locationInfo;
        private JToken _locationVariants;

        public Transform GetSpawnPoint() => spawnPoint;

        public void InitLocation(JToken locationInfo, JToken locationTeleportsVariants)
        {
            teleportIndex = 0;
            _locationInfo = locationInfo;
            _locationVariants = locationTeleportsVariants;

            GenerateNpc(_locationInfo);
            GenerateItems(_locationInfo);
            GeneratePortals(_locationVariants);
        }

        public async Task UpdateItems(JToken items)
        {
            if (items == null)
            {
                foreach (var item in _items)
                    Destroy(item);        
                
                _items.Clear();
                return;
            }
            
            //TODO: Check ids not names


            List<GameObject> deleteItems = new List<GameObject>();
            //delete items

            if (_items.Count > 0)
            {
                foreach (var item in _items)
                {
                    var objItem = item.GetComponent<Item>().ItemInfo;

                    if (!HasInList(items, objItem["Name"].ToString(), "Name"))
                        deleteItems.Add(item);
                }

                for (int i = 0; i < deleteItems.Count; i++)
                {
                    _items.Remove(deleteItems[i]);
                    Destroy(deleteItems[i]);
                }
            }
            
            //add items
            foreach (var newItem in items)
            {
                var newItemName = newItem["Name"].ToString();
                var spawnItem = true;
                foreach (var oldCharacter in _items)
                {
                    var oldCharacterName = oldCharacter.GetComponent<Item>().ItemInfo["Name"].ToString();
                    if (newItemName == oldCharacterName)
                        spawnItem = false;
                }
                
                if(spawnItem)
                    SpawnOneItem(newItem);
            }
        }

        public async Task UpdateCharacters(JToken characters)
        {
            if (characters == null)
            {
                foreach (var item in _characters)
                    Destroy(item); 
                
                return;
            }
            
            List<GameObject> deleteItems = new List<GameObject>();
            //delete characters
            if (_characters.Count > 0)
            {
                foreach (var character in _characters)
                {
                    var npc = character.GetComponent<Npc.Npc>();
                    if (!HasInList(characters, npc.NpcInfo["Id"].ToString(), "Id"))
                        deleteItems.Add(character);
                }
                
                for (int i = 0; i < deleteItems.Count; i++)
                {
                    _items.Remove(deleteItems[i]);
                    Destroy(deleteItems[i]);
                }
            }
            
            
            //add characters
            foreach (var newCharacter in characters)
            {
                var newNpcName = newCharacter["Id"].ToString();
                var spawnCharacter = true;
                foreach (var oldCharacter in _characters)
                {
                    var oldCharacterName = oldCharacter.GetComponent<Npc.Npc>().NpcInfo["Id"].ToString();
                    if (newNpcName == oldCharacterName)
                        spawnCharacter = false;
                }
                
                if(spawnCharacter)
                    SpawnOneNpc(newCharacter);
            }
        }

        public async Task SetPositions(JToken productions)
        {
            foreach (var production in productions)
            {
                var variants = production["variants"];
                
                foreach (var variant in variants)
                {
                    Vector3 position = Vector3.zero;
                    foreach (var podVariant in variant)
                    {
                        var id = podVariant["WorldNodeId"].ToString();
                        var result = FindIdInNpc(id);
                        if (result == null)
                            result = FindIdInItems(id);
                    
                        if (result != null)
                        {
                            if (position == Vector3.zero)
                                position = result.transform.localPosition;
                            else
                            {
                                var prod = production["prod"];
                                
                                result.transform.localPosition = position + new Vector3(Random.Range(0, 3), 0, Random.Range(0, 3));
                                // position = result.transform.localPosition;
                            }
                        }
                    }
                }
            }
        }

        private GameObject FindIdInNpc(string id)
        {
            foreach (var character in _characters)
            {
                if(character == null)
                    continue;
                
                var npcInfo = character.GetComponent<Npc.Npc>().NpcInfo;
                if(npcInfo["Id"].ToString() == id)
                    return character;
            }

            return null;
        }
        
        private GameObject FindIdInItems(string id)
        {
            foreach (var item in _items)
            {
                if(item == null)
                    continue;
                
                var itemInfo = item.GetComponent<Item>().ItemInfo;
                if(itemInfo["Id"].ToString() == id)
                    return item;
            }

            return null;
        }
        
        private bool HasInList(JToken list, string npcName, string newNpcKey)
        {
            foreach (var entity in list)
            {
                var newEntityName = entity[newNpcKey].ToString();
                if (npcName == newEntityName)
                    return true;
            }

            return false;
        }
        
        
        
        
        public void ClearLocation()
        {
            foreach (var character in _characters)
                Destroy(character);
            foreach (var item in _items)
                Destroy(item);
            foreach (var teleport in _teleports)
                Destroy(teleport);
            foreach (var teleport in _initedTeleports)
                Destroy(teleport);
        }

        private Vector3 GetPointForEntitySpawn()
        {
            var bounds = colliderForItems.bounds;
            var point = new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(colliderForItems.bounds.min.y, bounds.max.y),
                Random.Range(colliderForItems.bounds.min.z, bounds.max.z)
            );

            return point;
        }
        
        public void GenerateNpc(JToken locationInfo)
        {
            var characters = locationInfo["Characters"];

            if (characters == null)
                return;

            foreach (var character in characters)
            {
                var position = GetPointForEntitySpawn();
                var characterId = character["Id"].ToString();
                
                if(characterId == AllServices.Container.Single<IGameService>().GetGameController().GetMainPlayerId())
                    continue;

                var npc = Resources.Load<Npc.Npc>("JsonFiles/Npc/" + character["Name"]);
                if (npc == null)
                    npc = Resources.Load<Npc.Npc>("JsonFiles/Npc/default_npc");

                var obj = Instantiate(npc, position, Quaternion.identity);
                obj.NpcInfo = character;
                obj.Init();

                _characters.Add(obj.gameObject);
            }
        }
        
        private void GenerateItems(JToken locationInfo)
        {
            var items = locationInfo["Items"];

            if (items == null)
                return;

            foreach (var item in items)
            {
                var position = GetPointForEntitySpawn();

                var itemMesh = Resources.Load<Item>("JsonFiles/Items3D/" + item["Name"]);
                if (itemMesh == null)
                    itemMesh = Resources.Load<Item>("JsonFiles/Items3D/default");

                var obj = Instantiate(itemMesh, position, Quaternion.identity);
                obj.ItemInfo = item;
                _items.Add(obj.gameObject);
            }
        }

        private void GeneratePortals(JToken variants)
        {
            var path = "Prefabs/Location/Teleport";

            foreach (var variant in variants)
            {
                if (teleportIndex < _initedTeleports.Count)
                {
                    _initedTeleports[teleportIndex].gameObject.SetActive(true);
                    _initedTeleports[teleportIndex].GetComponent<Teleport>().Variant = variant;
                    teleportIndex++;
                }
                else
                {
                    var position = GetPointForEntitySpawn();
                
                    if(_portalPrefab == null)
                        _portalPrefab = Resources.Load<Teleport>(path);

                    var obj = Instantiate(_portalPrefab, position, Quaternion.identity);
                    obj.Variant = variant;

                    _teleports.Add(obj.gameObject);
                }
            }
        }

        private void SpawnOneNpc(JToken character)
        {
            var position = AllServices.Container.Single<IGameService>().GetGameController().GetPlayerPosition() + Vector3.forward;
            var characterId = character["Id"].ToString();
                
            if(characterId == AllServices.Container.Single<IGameService>().GetGameController().GetMainPlayerId())
                return;

            var npc = Resources.Load<Npc.Npc>("JsonFiles/Npc/" + character["Name"]);
            if (npc == null)
                npc = Resources.Load<Npc.Npc>("JsonFiles/Npc/default_npc");

            var obj = Instantiate(npc, position, Quaternion.identity);
            obj.NpcInfo = character;
            obj.Init();

            _characters.Add(obj.gameObject);
        }

        private void SpawnOneItem(JToken item)
        {
            var position = AllServices.Container.Single<IGameService>().GetGameController().GetPlayerPosition() + Vector3.forward;

            var itemMesh = Resources.Load<Item>("JsonFiles/Items3D/" + item["Name"]);
            if (itemMesh == null)
                itemMesh = Resources.Load<Item>("JsonFiles/Items3D/default");

            var obj = Instantiate(itemMesh, position, Quaternion.identity);
            obj.ItemInfo = item;
            _items.Add(obj.gameObject);
        }
        
    }
}