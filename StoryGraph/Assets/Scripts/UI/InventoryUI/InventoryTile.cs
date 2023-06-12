using CodeBase.Infrastructure.Services;
using Infrastructure.Services;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class InventoryTile : MonoBehaviour, IDropHandler
    {
        public InventoryType CurrentType;
        private Draggable _itemDraggable;

        private GameController _gameController;
        
        private void Awake()
        {
            _gameController = AllServices.Container.Single<IGameService>().GetGameController();
        }

        public void PutItem(Draggable item)
        {
            _itemDraggable = item;
        }

        public void OnDrop(PointerEventData eventData)
        {
            if(transform.childCount > 0)
                return;
            
            GameObject obj = eventData.pointerDrag;
            _itemDraggable = obj.GetComponent<Draggable>();
            _itemDraggable.ParentAfterDrag = transform;

            if (_itemDraggable.Type != CurrentType)
            {
                Debug.Log(CurrentType);
                
                if(CurrentType == InventoryType.Item)
                    _gameController.PuttingItem(_itemDraggable.Item["Name"].ToString());
                else if(CurrentType == InventoryType.Main)
                    _gameController.PullingItem(_itemDraggable.Item["Name"].ToString());
            }
        }
    }

    public enum InventoryType
    {
        None,
        Main,
        Item,
        Equipment
    }
}