using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace UI
{
    public class InventoryUI : BaseWindow
    {
        private const int InventorySize = 15;
        
        [SerializeField] private InventoryTile _inventoryTilePrefab;
        [SerializeField] private Draggable _itemPrefab;
        
        [SerializeField] private GameObject _itemsInventoryContainer;
        [SerializeField] private GameObject _tilesContainer;
        [SerializeField] private Transform _inventoryRoot;
        [SerializeField] private List<InventoryTile> _tiles = new List<InventoryTile>();
        
        public static List<string> EquipmentId = new List<string>();


        public override void Awake()
        {
            base.Awake();
            
            GenerateTiles();
        }

        public void Show(JToken items)
        {
            _itemsInventoryContainer.SetActive(true);
            ShowInventoryItems(items);
        }

        public void HideInventory()
        {
            Hide();
            _itemsInventoryContainer.SetActive(false);
        }

        private void ShowInventoryItems(JToken items)
        {
            ClearInventory();

            if(items == null)
                return;
            
            var index = 0;
            foreach (var item in items)
            {
                if(IsEquipped(item))
                    continue;
                
                var itemObj = Instantiate(_itemPrefab, _tiles[index].transform);
                itemObj.Init(_inventoryRoot, item);
                itemObj.Type = InventoryType.Main;
                _tiles[index].PutItem(itemObj);

                index++;
            }
        }
        
        private void ClearInventory()
        {
            foreach (var tile in _tiles)
            {
                if (tile.transform.childCount > 0)
                {
                    for (int i = 0; i < tile.transform.childCount; i++)
                        Destroy(tile.transform.GetChild(i).gameObject);
                }
            }
        }
        
        private void GenerateTiles()
        {
            for (var i = 0; i < InventorySize; i++)
            {
                var tile = Instantiate(_inventoryTilePrefab, _tilesContainer.transform);
                tile.CurrentType = InventoryType.Main;
                _tiles.Add(tile);
            }
        }

        private bool IsEquipped(JToken item) => 
            EquipmentId.Any(equip => equip.Equals(item["Id"].ToString()));

        public static void RemoveFromEquipment(string id)
        {
            foreach (var equip in EquipmentId.Where(equip => equip.Equals(id)))
            {
                EquipmentId.Remove(equip);
                break;
            }
        }
    }
}