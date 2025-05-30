using UnityEngine;
using UnityEngine.EventSystems;

namespace INTERACT
{
    public class InvetorySlot : MonoBehaviour, IDropHandler
    {
        public void OnDrop(PointerEventData eventData)
        {
            if(transform.childCount == 0)
            {
                GameObject dropped = eventData.pointerDrag;
                DraggableItem draggableItem = dropped.GetComponent<DraggableItem>();
                draggableItem.parentAfterDrag = transform;
            }
            else
            {
                GameObject dropped = eventData.pointerDrag;
                DraggableItem draggableItem = dropped.GetComponent<DraggableItem>();

                GameObject current = transform.GetChild(0).gameObject;
                DraggableItem currentDraggable = current.GetComponent<DraggableItem>();

                currentDraggable.transform.SetParent(draggableItem.parentAfterDrag);
                draggableItem.parentAfterDrag = transform;
            }
        }
    }
}