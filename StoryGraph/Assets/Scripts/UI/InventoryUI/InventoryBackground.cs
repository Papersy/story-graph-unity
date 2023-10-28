using CodeBase.Infrastructure.Services;
using Infrastructure.Services;
using InteractableItems;
using LocationDir;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class InventoryBackground : MonoBehaviour, IDropHandler
    {
        private Draggable _itemDraggable;
        
        public void OnDrop(PointerEventData eventData)
        {
            Debug.Log("Drop");
            _itemDraggable = eventData.pointerDrag.GetComponent<Draggable>();
            // DropItem();
            Destroy(eventData.pointerDrag.gameObject);
        }
        
        private void DropItem()
        {
            // if (_itemDraggable == null)
            //     return;
            //
            // var prefab = Resources.Load<Item>($"JsonFiles/Items3D/default");
            // var spawnPos = AllServices.Container.Single<IGameService>().GetGameController().GetPlayerTransform();
            // var droppedItem = Instantiate(prefab, spawnPos.TransformPoint(Vector3.forward * 2 + Vector3.up),
            //     Quaternion.identity);
            //
            // droppedItem.ItemInfo = _itemDraggable.Item;
            //
            //
            // AllServices.Container.Single<IGameService>().GetGameController().DropItem(_itemDraggable.Item["Name"].ToString());
            //
            // _itemDraggable = null;
        }
    }
}