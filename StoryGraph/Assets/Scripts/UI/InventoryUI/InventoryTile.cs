using CodeBase.Infrastructure.Services;
using Infrastructure.Services;
using Player;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class InventoryTile : MonoBehaviour, IDropHandler
    {
        public InventoryType CurrentType;
        public EquipmentType EquipmentType = EquipmentType.None;
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
            if (transform.childCount > 0)
            {
                var itemDraggable = eventData.pointerDrag.GetComponent<Draggable>();
                var firstItemId = itemDraggable.Item["Id"].ToString();
                var secondItemId = transform.GetComponentInChildren<Draggable>().Item["Id"].ToString();

                if (_gameController.CanCreateOpakowanie(firstItemId, secondItemId))
                {
                    _gameController.CreateOpakowanieInInventory(firstItemId, secondItemId);
                    Destroy(itemDraggable.transform.gameObject);
                }
                    
                return;
            }

            GameObject obj = eventData.pointerDrag;
            _itemDraggable = obj.GetComponent<Draggable>();

            if (CurrentType == InventoryType.Equipment)
            {
                if (EquipmentType == EquipmentType.Attack)
                {
                    var isWeapon = _itemDraggable.Item["Attributes"]["IsWeapon"].ToString();
                    if (isWeapon != null)
                    {
                        Debug.Log(isWeapon);
                        if (isWeapon.Equals("True")){}
                        {
                            var weaponId = _itemDraggable.Item["Id"].ToString();
                            _itemDraggable.Type = InventoryType.Equipment;
                            InventoryUI.EquipmentId.Add(weaponId);
                            EquipmentManager.Instance.PickUpWeapon(_itemDraggable.Item);
                        }
                    }
                }
                else
                    return;
            }

            _itemDraggable.ParentAfterDrag = transform;

            if (_itemDraggable.Type != CurrentType)
            {
                Debug.Log(CurrentType);

                if (CurrentType == InventoryType.Main && _itemDraggable.Type == InventoryType.Equipment)
                {
                    EquipmentManager.Instance.ClearEquipment(_itemDraggable.Item);
                    InventoryUI.RemoveFromEquipment(_itemDraggable.Item["Id"].ToString());
                }
                else if(CurrentType == InventoryType.Item)
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

    public enum EquipmentType
    {
        None,
        Head,
        Body,
        Attack,
        Defence,
        Lags,
        Shoes
    }
}