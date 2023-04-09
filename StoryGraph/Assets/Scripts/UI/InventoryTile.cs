using CodeBase.Infrastructure.Services;
using Infrastructure.Services;
using LocationDir;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class InventoryTile : MonoBehaviour
    {
        public Button Button;
        public Image container;
        public JToken item;

        public void PutItem(JToken item)
        {
            this.item = item;
            
            container.color = new Color32(255, 255, 255, 255);
            container.sprite = Resources.Load<Sprite>($"JsonFiles/Items/{item["Name"].ToString().ToLower()}");
        }

        public void DropItem()
        {
            if(item == null)
                return;
            
            var dropItem = Resources.Load<Item>($"JsonFiles/Items3D/default");
            dropItem.ItemInfo = item;

            var spawnPos = AllServices.Container.Single<IGameService>().GetGameController().GetPlayerPosition();
            Instantiate(dropItem, spawnPos + Vector3.forward, Quaternion.identity);

            item = null;
        }
    }
}