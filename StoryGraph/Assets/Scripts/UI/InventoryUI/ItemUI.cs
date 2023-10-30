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