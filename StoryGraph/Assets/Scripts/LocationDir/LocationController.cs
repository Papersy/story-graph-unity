using System.Collections.Generic;
using System.Threading.Tasks;
using CodeBase.Infrastructure.Services;
using Infrastructure.Services;
using InteractableItems;
using Newtonsoft.Json.Linq;
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

        public bool IsInited = false;
        private int teleportIndex = 0;
        public JToken _locationInfo;
        private JToken _locationVariants;

        public Transform GetSpawnPoint() => spawnPoint;

        public void SetLocationInfo(JToken locationInfo)
        {
            _locationInfo = locationInfo;
        }
        
        public void InitLocation(JToken locationInfo, JToken locationTeleportsVariants)
        {
            IsInited = true;
            teleportIndex = 0;
            _locationInfo = locationInfo;
            _locationVariants = locationTeleportsVariants;

            GenerateNpcs(_locationInfo);
            GenerateItems(_locationInfo);
            GeneratePortals(_locationVariants);
        }

        public async Task UpdateItems(JToken items, bool isNearPlayer)
        {
            if (items == null)
            {
                foreach (var item in _items)
                    Destroy(item);        
                
                _items.Clear();
                return;
            }

            List<GameObject> deleteItems = new List<GameObject>();
            //delete items

            if (_items.Count > 0)
            {
                foreach (var item in _items)
                {
                    if(item == null)
                        continue;
                    
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
                foreach (var oldItem in _items)
                {
                    if(oldItem == null)
                        continue;
                    
                    var oldItemName = oldItem.GetComponent<Item>().ItemInfo["Name"].ToString();
                    if (newItemName == oldItemName)
                        spawnItem = false;
                }
                
                if(spawnItem)
                    SpawnOneItem(newItem, isNearPlayer);
            }
        }

        public async Task UpdateCharacters(JToken characters, bool isNearPlayer)
        {
            Debug.Log(characters);
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
                    if(character == null)
                        continue;
                    
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
                Debug.Log(newCharacter["Name"].ToString());
                var spawnCharacter = true;
                foreach (var oldCharacter in _characters)
                {
                    if(oldCharacter == null)
                        continue;
                    
                    var oldCharacterName = oldCharacter.GetComponent<Npc.Npc>().NpcInfo["Id"].ToString();
                    if (newNpcName == oldCharacterName)
                        spawnCharacter = false;
                }
                
                if(spawnCharacter)
                    SpawnOneNpc(newCharacter, isNearPlayer);
            }
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
        
        public void GenerateNpcs(JToken locationInfo)
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

        private void SpawnOneNpc(JToken character, bool isNearPlayer)
        {
            Vector3 position;
            
            if(!isNearPlayer)
                position = GetPointForEntitySpawn();
            else
                position = AllServices.Container.Single<IGameService>().GetGameController().GetPlayerPosition() + new Vector3(Random.Range(2f, 3f), 0, Random.Range(2f, 3f));
            
            
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

        private void SpawnOneItem(JToken item, bool isNearPlayer)
        {
            Vector3 position;
            
            if(!isNearPlayer)
                position = GetPointForEntitySpawn();
            else
            {
                var playerPosition = AllServices.Container.Single<IGameService>().GetGameController().GetPlayerPosition();
                var vectorToCenter = spawnPoint.position - playerPosition;
                position =  playerPosition + vectorToCenter.normalized * 2.5f;
                position.y = .5f;
            }

            var itemMesh = Resources.Load<Item>("JsonFiles/Items3D/" + item["Name"]);
            if (itemMesh == null)
                itemMesh = Resources.Load<Item>("JsonFiles/Items3D/default");

            var obj = Instantiate(itemMesh, position, Quaternion.identity);
            obj.ItemInfo = item;
            _items.Add(obj.gameObject);
        }
        
    }
}