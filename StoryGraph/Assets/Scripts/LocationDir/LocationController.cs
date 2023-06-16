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
        private List<GameObject> _teleports = new List<GameObject>();

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
            foreach (var teleport in _teleports)
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

        public void ShowLocationsToGo() =>
            AllServices.Container.Single<IGameService>().GetGameController().ShowLocationToGo();

        public void GenerateNpc(JToken character)
        {
            var position = GetPointForEntitySpawn();

            var npc = Resources.Load<Npc.Npc>("JsonFiles/Npc/" + character["Name"]);
            if (npc == null)
                npc = Resources.Load<Npc.Npc>("JsonFiles/Npc/default_npc");

            Debug.Log(character.ToString());

            var obj = Instantiate(npc, position, Quaternion.identity);
            obj.NpcInfo = character;
            obj.Init();
            _characters.Add(obj.gameObject);
        }

        public void GenerateItems(JToken item)
        {
            var position = GetPointForEntitySpawn();

            var itemMesh = Resources.Load<Item>("JsonFiles/Items3D/" + item["Name"]);
            if (itemMesh == null)
                itemMesh = Resources.Load<Item>("JsonFiles/Items3D/default");

            var obj = Instantiate(itemMesh, position, Quaternion.identity);
            obj.ItemInfo = item;
            _items.Add(obj.gameObject);
        }

        public void GeneratePortals(JToken variants)
        {
            var path = "Prefabs/Location/Teleport";

            foreach (var variant in variants)
            {
                var position = GetPointForEntitySpawn();
                var itemMesh = Resources.Load<Teleport>(path);
                
                var obj = Instantiate(itemMesh, position, Quaternion.identity);
                obj.Variant = variant;
                
                _teleports.Add(obj.gameObject);
            }
        }
    }
}