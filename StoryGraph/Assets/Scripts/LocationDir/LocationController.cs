using System.Collections.Generic;
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

        public string Id { get; set; }
        public string Name { get; set; }

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
            Debug.Log(characters);

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
        
        public void SpawnItems(JToken npcInfo, Vector3 position)
        {
            var items = npcInfo["Items"];
            
            if(items == null)
                return;

            foreach (var item in items)
            {
                var itemMesh = Resources.Load<Item>("JsonFiles/Items3D/" + item["Name"]);
                if (itemMesh == null)
                    itemMesh = Resources.Load<Item>("JsonFiles/Items3D/default");

                var obj = Instantiate(itemMesh, position, Quaternion.identity);
                obj.ItemInfo = item;
                _items.Add(obj.gameObject);
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
    }
}