using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Link_To_Radio
{
    public class PanelPulldown : MonoBehaviour
    {
        [SerializeField] private RectTransform panelToMove;
        [SerializeField] private Button triggerButton;
        [SerializeField] private float openY = 0f;
        [SerializeField] private float closedY = 451f;
        [SerializeField] private float animationDuration = 0.8f;

        private bool isOpen = false;

        private void Start()
        {
            triggerButton.onClick.AddListener(TogglePanel);
            panelToMove.anchoredPosition = new Vector2(panelToMove.anchoredPosition.x, closedY);
        }

        private void TogglePanel()
        {
            if (isOpen)
            {
                panelToMove.DOAnchorPosY(closedY, animationDuration).SetEase(Ease.InOutElastic);
            }
            else
            {
                panelToMove.DOAnchorPosY(openY, animationDuration).SetEase(Ease.InOutElastic);
            }

            isOpen = !isOpen;
        }
    }
}