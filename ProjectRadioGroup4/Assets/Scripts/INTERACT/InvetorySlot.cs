using MANAGER;
using UnityEngine;
using UnityEngine.EventSystems;

namespace INTERACT
{
    public class InvetorySlot : MonoBehaviour, IDropHandler
    {
        public int slotIndex;
        
        public void OnDrop(PointerEventData eventData)
        {
            GameObject dropped = eventData.pointerDrag;
            DraggableItem draggableItem = dropped.GetComponent<DraggableItem>();

            if (transform.childCount != 0)
            {           
                GameObject current = transform.GetChild(0).gameObject;
                DraggableItem currentDraggable = current.GetComponent<DraggableItem>();

             
                ChipsManager.Instance.SwapChips(
                    draggableItem.originalSlotIndex, 
                    this.slotIndex
                );
                
                currentDraggable.parentAfterDrag = draggableItem.parentAfterDrag;
                currentDraggable.transform.SetParent(draggableItem.parentAfterDrag);
            }
            draggableItem.parentAfterDrag = transform;
            draggableItem.originalSlotIndex = this.slotIndex; 
        }
    }
}