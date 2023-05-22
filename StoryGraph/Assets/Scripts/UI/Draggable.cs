using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour, IDragHandler, IEndDragHandler
{
    public Action OnDropItem;
    
    private Ray _ray;
    private RaycastHit _hit;

    private bool _canDrop = false;

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.localPosition = Vector3.zero;
        
        if(_canDrop)
            OnDropItem?.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.CompareTag("Inventory"))
            _canDrop = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Inventory"))
            _canDrop = false;
    }
}