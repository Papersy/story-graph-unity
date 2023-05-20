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
        public int Id;
        public Button Button;
        public Image itemImage;
        public JToken item;

        private Ray ray;
        private RaycastHit hit;
        
        public void PutItem(JToken item)
        {
            this.item = item;
            
            itemImage.color = new Color32(255, 255, 255, 255);
            itemImage.sprite = Resources.Load<Sprite>($"JsonFiles/Items/{item["Name"].ToString().ToLower()}");
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

        private void OnMouseDown()
        {
            Debug.Log("Mouse down");
        }

        private void OnMouseDrag()
        {
            Debug.Log("Mouse drag");
        }

        private void OnMouseUp()
        {
            Debug.Log("Mouse Up");
        }
    }
}