using System.Collections.Generic;
using CodeBase.Infrastructure.Services;
using Infrastructure.Services;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class InventoryUI : MonoBehaviour
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
                tile.container.color = new Color32(0, 0, 0, 0);

            var index = 0;
            foreach (var item in items)
            {
                tiles[index].PutItem(item);
                var index1 = index;
                tiles[index].Button.onClick.AddListener(() =>
                {
                    tiles[index1].DropItem();
                    AllServices.Container.Single<IGameService>().GetGameController().DeletePlayerItem(item);
                });
                index++;
            }
        }
    }
}