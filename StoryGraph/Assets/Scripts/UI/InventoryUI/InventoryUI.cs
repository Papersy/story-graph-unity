using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace UI
{
    public class InventoryUI : BaseWindow
    {
        [SerializeField] private GameObject container;
        [SerializeField] private List<InventoryTile> tiles;

        public void Show(JToken items)
        {
            container.SetActive(true);
            ShowInventoryItems(items);
        }

        public void Hide()
        {
            container.SetActive(false);
        }

        private void ShowInventoryItems(JToken items)
        {
            foreach (var tile in tiles)
                tile.itemImage.color = new Color32(0, 0, 0, 0);

            if(items == null)
                return;

            var index = 0;
            foreach (var item in items)
            {
                tiles[index].PutItem(item);
                tiles[index].Id = index;
                index++;
            }
        }
    }
}