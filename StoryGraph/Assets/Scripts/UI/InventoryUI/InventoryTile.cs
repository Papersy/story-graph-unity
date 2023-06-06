using CodeBase.Infrastructure.Services;
using Infrastructure.Services;
using LocationDir;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class InventoryTile : MonoBehaviour, IDropHandler
    {
        public int Id;
        private Draggable _itemDraggable;

        public void PutItem(Draggable item)
        {
            _itemDraggable = item;
        }

        private void DropItem()
        {
            if (_itemDraggable == null)
                return;

            var prefab = Resources.Load<Item>($"JsonFiles/Items3D/default");
            var spawnPos = AllServices.Container.Single<IGameService>().GetGameController().GetPlayerTransform();
            var droppedItem = Instantiate(prefab, spawnPos.TransformPoint(Vector3.forward * 2 + Vector3.up),
                Quaternion.identity);

            droppedItem.ItemInfo = _itemDraggable.Item;
            

            AllServices.Container.Single<IGameService>().GetGameController().DropItem(_itemDraggable.Item["Name"].ToString());

            _itemDraggable = null;
        }

        public void OnDrop(PointerEventData eventData)
        {
            if(transform.childCount > 0)
                return;
            
            GameObject obj = eventData.pointerDrag;
            _itemDraggable = obj.GetComponent<Draggable>();
            _itemDraggable.ParentAfterDrag = transform;
        }
    }
}