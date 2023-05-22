using System;
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
        [SerializeField] private Draggable container;
        
        public int Id;
        public Button Button;
        public Image itemImage;
        public JToken item;

        private void OnEnable()
        {
            container.OnDropItem += DropItem;
        }

        private void OnDisable()
        {
            container.OnDropItem -= DropItem;
        }

        public void PutItem(JToken item)
        {
            this.item = item;
            
            itemImage.color = new Color32(255, 255, 255, 255);
            itemImage.sprite = Resources.Load<Sprite>($"JsonFiles/Items/{item["Name"].ToString().ToLower()}");
        }

        private void DropItem()
        {
            if(item == null)
                return;
            
            var dropItem = Resources.Load<Item>($"JsonFiles/Items3D/default");
            dropItem.ItemInfo = item;

            var spawnPos = AllServices.Container.Single<IGameService>().GetGameController().GetPlayerTransform();
            Instantiate(dropItem, spawnPos.TransformPoint(Vector3.forward * 2 + Vector3.up), Quaternion.identity);

            itemImage.color = new Color32(0, 0, 0, 0);
            
            AllServices.Container.Single<IGameService>().GetGameController().DropItem(item["Name"].ToString());

            item = null;
        }
    }
}