using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [HideInInspector] public Transform ParentAfterDrag;
    public Image ItemImage;
    public Transform Root;

    public JToken Item;

    private RaycastHit[] _hits;
    private bool _canDrop = false;

    public void Init(Transform root, JToken item)
    {
        Root = root;
        Item = item;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        ParentAfterDrag = transform.parent;
        transform.SetParent(Root);
        transform.SetAsLastSibling();
        ItemImage.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(ParentAfterDrag);
        ItemImage.raycastTarget = true;
    }
}