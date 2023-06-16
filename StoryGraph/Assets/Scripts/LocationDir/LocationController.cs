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
        private JToken _locationVariants;

        public Transform GetSpawnPoint() => spawnPoint;

        public void InitLocation(JToken locationInfo, JToken locationTeleportsVariants)
        {
            _locationInfo = locationInfo;
            _locationVariants = locationTeleportsVariants;

            Debug.Log(_locationInfo);
            Debug.Log(_locationVariants);

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

        private void GenerateNpc(JToken locationInfo)
        {
            var characters = locationInfo["Characters"];

            if (characters == null)
                return;

            foreach (var character in characters)
            {
                var position = GetPointForEntitySpawn();
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
                var position = GetPointForEntitySpawn();
                var itemMesh = Resources.Load<Teleport>(path);

                var obj = Instantiate(itemMesh, position, Quaternion.identity);
                obj.Variant = variant;

                _teleports.Add(obj.gameObject);
            }
        }
    }
}