using System;
using MANAGER;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace INTERACT
{
    public class DraggableItem : MonoBehaviour, IBeginDragHandler,IDragHandler, IEndDragHandler
    {
        [HideInInspector] public Transform parentAfterDrag;
        public Image image;
        public int chipIndex;
        private ChipsManager chipsManager;

        private void Start()
        {
            chipsManager = NewPlayerController.instance?.chipsManager;
        }

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
            image.raycastTarget = true;
            int newIndex = Array.IndexOf(chipsManager.itemSlotsGO, parentAfterDrag.gameObject);
    
            if (newIndex != chipIndex && newIndex != -1) 
            {
                chipsManager.SwapChips(chipIndex, newIndex);
                transform.SetParent(parentAfterDrag);
                chipIndex = newIndex;
            }
            else
            {
                transform.SetParent(parentAfterDrag);
            }
        }
    }
}
