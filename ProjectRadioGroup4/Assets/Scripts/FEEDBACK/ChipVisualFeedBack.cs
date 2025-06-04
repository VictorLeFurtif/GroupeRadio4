using DG.Tweening;
using MANAGER;
using UnityEngine;
using UnityEngine.EventSystems;

namespace INTERACT
{
    public class ChipVisualFeedback : MonoBehaviour, IPointerClickHandler
    {
        [Header("Animation Settings Dotween")]
        [SerializeField] private float pulseStrength = 0.1f;
        [SerializeField] private float pulseDuration = 0.5f;

        private Vector3 originalScale;
        private Tween pulseTween;

        private bool isSelected;

        private void Awake()
        {
            originalScale = transform.localScale;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                ToggleSelection();
            }
        }

        public void SetSelected(bool selected, bool animate = true)
        {
            if (isSelected == selected) return;
            
            isSelected = selected;
            UpdateVisualState(animate);
            
            var slot = GetComponentInParent<InvetorySlot>();
            if (slot != null)
            {
                ChipsManager.Instance.chipsDatasTab[slot.slotIndex].isSelected = isSelected;
            }
        }

        public void ToggleSelection()
        {
            SetSelected(!isSelected);
        }

        public void UpdateVisualState(bool animate = true)
        {
            transform.DOKill(); 

            if (isSelected)
            {
                pulseTween = transform.DOPunchScale(
                        new Vector3(pulseStrength, pulseStrength, 0f), 
                        pulseDuration, 
                        1, 
                        0.5f)
                    .SetLoops(-1, LoopType.Restart)
                    .SetEase(Ease.InOutSine);
            }
            else
            {
                pulseTween?.Kill();
                if (animate)
                {
                    transform.DOScale(originalScale, 0.2f);
                }
                else
                {
                    transform.localScale = originalScale;
                }
            }
        }

        private void OnDestroy()
        {
            pulseTween?.Kill();
        }
    }
}