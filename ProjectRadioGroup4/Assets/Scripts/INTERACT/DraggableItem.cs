using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace INTERACT
{
    public class DraggableItem : MonoBehaviour, IBeginDragHandler,IDragHandler, IEndDragHandler
    {
        [HideInInspector] public Transform parentAfterDrag;
        public Image image;
        
        public void OnBeginDrag(PointerEventData eventData)
        {
            Debug.Log("Begin Drag");
            parentAfterDrag = transform.parent;
            transform.SetParent(transform.root);
            transform.SetAsLastSibling();
            image.raycastTarget = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            Debug.Log("Dragging");
            transform.position = Input.mousePosition;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            Debug.Log("End");
            transform.SetParent(parentAfterDrag);
            image.raycastTarget = true;
        }
    }
}