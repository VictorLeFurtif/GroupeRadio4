using FEEDBACK;
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
            if (FightManager.instance.numberOfSwap <= 0)
            {
                Debug.Log("No more swaps available!");
                return;
            }

            GameObject dropped = eventData.pointerDrag;
            DraggableItem draggableItem = dropped.GetComponent<DraggableItem>();
            
            if (draggableItem == null) return;
            
            if (transform.childCount != 0)
            {
                GameObject current = transform.GetChild(0).gameObject;
                DraggableItem currentDraggable = current.GetComponent<DraggableItem>();

                if (currentDraggable != null)
                {
                    ChipsManager.Instance.SwapChips(
                        draggableItem.originalSlotIndex, 
                        this.slotIndex
                    );
                    
                    currentDraggable.parentAfterDrag = draggableItem.parentAfterDrag;
                    currentDraggable.transform.SetParent(draggableItem.parentAfterDrag);
                    
                    currentDraggable.GetComponent<ChipVisualFeedback>()?.UpdateVisualState();
                    
                    NewPlayerController.instance?.ManageLife(-ChipsManager.Instance.damageIfSwap);
                    FightManager.instance.numberOfSwap--;
                    
                }
            }
            
            draggableItem.parentAfterDrag = transform;
            draggableItem.originalSlotIndex = this.slotIndex; 
            draggableItem.GetComponent<ChipVisualFeedback>()?.UpdateVisualState();
        }
    }
}