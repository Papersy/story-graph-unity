using System;
using System.Collections.Generic;
using CodeBase.Infrastructure.Services;
using Infrastructure.Services;
using Newtonsoft.Json.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace LocationDir
{
    public class LocationController : MonoBehaviour
    {
        [SerializeField] private BoxCollider colliderForItems;
        [SerializeField] private Transform spawnPoint;

        private List<GameObject> _characters = new List<GameObject>();
        private List<GameObject> _items = new List<GameObject>();

        public string Id { get; set; }
        public string Name { get; set; }

        private JToken _locationInfo;
        
        public Transform GetSpawnPoint() => spawnPoint;

        public void InitLocation(JToken loc)
        {
            _locationInfo = loc;
            
            foreach (var character in _characters)
                Destroy(character);
            foreach (var item in _items)
                Destroy(item);
            
            
        }

        private Vector3 GetPointForEntitySpawn()
        {
            var point = new Vector3(
                Random.Range(colliderForItems.bounds.min.x, colliderForItems.bounds.max.x),
                Random.Range(colliderForItems.bounds.min.y, colliderForItems.bounds.max.y),
                Random.Range(colliderForItems.bounds.min.z, colliderForItems.bounds.max.z)
            );

            return point;
        }

        public void ShowLocationsToGo() =>
            AllServices.Container.Single<IGameService>().GetGameController().ShowLocationToGo();

        public void SpawnNpc(JToken character)
        {
            var position = GetPointForEntitySpawn();

            var npc = Resources.Load<Npc.Npc>("JsonFiles/Npc/" + character["Name"]);
            if (npc == null)
                npc = Resources.Load<Npc.Npc>("JsonFiles/Npc/default_npc");
            npc.NpcInfo = character;
            
            var obj = Instantiate(npc, position, Quaternion.identity);
            _characters.Add(obj.gameObject);

        }

        public void SpawnItem(JToken item)
        {
            var position = GetPointForEntitySpawn();

            var itemMesh = Resources.Load<Item>("JsonFiles/Items3D/" + item["Name"]);
            if (itemMesh == null)
                itemMesh = Resources.Load<Item>("JsonFiles/Items3D/default");
            itemMesh.ItemInfo = item;

            var obj = Instantiate(itemMesh, position, Quaternion.identity);
            _items.Add(obj.gameObject);
        }
    }
}