using FEEDBACK;
using MANAGER;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace INTERACT
{
    public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [HideInInspector] public Transform parentAfterDrag;
        public Image image;
        public int originalSlotIndex;
        private bool canDrag = true;

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (FightManager.instance.numberOfSwap <= 0)
            {
                canDrag = false;
                return;
            }

            canDrag = true;
            parentAfterDrag = transform.parent;
            originalSlotIndex = parentAfterDrag.GetComponent<InvetorySlot>().slotIndex;
            transform.SetParent(transform.root);
            transform.SetAsLastSibling();
            image.raycastTarget = false;
            NewPlayerController.instance.currentDraggedItem = this;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!canDrag) return;
            
            transform.position = Input.mousePosition;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!canDrag)
            {
                canDrag = true; 
                return;
            }

            transform.SetParent(parentAfterDrag);
            image.raycastTarget = true;
            NewPlayerController.instance.currentDraggedItem = null;
        }
    }
}