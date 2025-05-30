using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace INTERACT
{
    public class DraggableItem : MonoBehaviour, IBeginDragHandler,IDragHandler, IEndDragHandler
    {
        [HideInInspector] public Transform parentAfterDrag;
        public Image image;
        public int originalSlotIndex;
        
        public void OnBeginDrag(PointerEventData eventData)
        {
            parentAfterDrag = transform.parent;
            originalSlotIndex = parentAfterDrag.GetComponent<InvetorySlot>().slotIndex;
            transform.SetParent(transform.root);
            transform.SetAsLastSibling();
            image.raycastTarget = false;
            
            var feedback = GetComponent<ChipVisualFeedback>();
            if (feedback != null) feedback.SetSelected(false);
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