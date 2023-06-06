using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class InventoryBackground : MonoBehaviour, IDropHandler
    {
        public void OnDrop(PointerEventData eventData)
        {
            Debug.Log("Drop");
        }
    }
}