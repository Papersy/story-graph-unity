using System.Collections.Generic;
using InteractableItems;
using LocationDir;
using UnityEngine;

namespace UI
{
    public class ItemUI : BaseWindow
    {
        [SerializeField] private InventoryTile _inventoryTilePrefab;
        [SerializeField] private Draggable _itemPrefab;
        [SerializeField] private Transform _inventoryRoot;
        [SerializeField] private GameObject _tilesContainer;
        
        private List<InventoryTile> _tiles = new List<InventoryTile>();
        
        private const int InventorySize = 6;

        public override void Awake()
        {
            base.Awake();
            
            GenerateTiles();
        }

        public void ShowInventoryItems(Item item)
        {
            ClearInventory();
            
            if(item.Storage.ListOfItems == null)
                return;
            if(_tiles.Count <= 0)
                GenerateTiles();
            
            var index = 0;
            foreach (var i in item.Storage.ListOfItems)
            {
                var itemObj = Instantiate(_itemPrefab, _tiles[index].transform);
                itemObj.Init(_inventoryRoot, i);
                itemObj.Type = InventoryType.Item;
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
                tile.CurrentType = InventoryType.Item;
                _tiles.Add(tile);
            }
        }
    }
}