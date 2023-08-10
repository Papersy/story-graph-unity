using System.Collections.Generic;
using InteractableItems;
using UnityEngine;

namespace UI
{
    public class EquipmentUI : BaseWindow
    {
        [SerializeField] private List<InventoryTile> _tiles = new List<InventoryTile>();
        
        
        // public void ShowEquipmentItems(Item item)
        // {
        //     ClearInventory();
        //     
        //     if(item.Storage.ListOfItems == null)
        //         return;
        //     if(_tiles.Count <= 0)
        //         GenerateTiles();
        //     
        //     var index = 0;
        //     foreach (var i in item.Storage.ListOfItems)
        //     {
        //         var itemObj = Instantiate(_itemPrefab, _tiles[index].transform);
        //         itemObj.Init(_inventoryRoot, i);
        //         itemObj.Type = InventoryType.Item;
        //         _tiles[index].PutItem(itemObj);
        //         index++;
        //     }
        // }
    }
}