using System.Collections.Generic;
using InteractableItems;
using UnityEngine;

namespace UI
{
    public class EquipmentUI : BaseWindow
    {
        [SerializeField] private List<InventoryTile> _tiles = new List<InventoryTile>();
    }
}