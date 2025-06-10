using DG.Tweening;
using MANAGER;
using TMPro;
using UnityEngine;

namespace INTERACT
{
    public class PaperStackInteraction : MonoBehaviour
    {
        [Header("Paramètres")]
        [SerializeField] private float interactionRadius = 1.5f;
        [SerializeField] private float uiFadeDuration = 0.3f;

        [Header("Références")]
        [SerializeField] private GameObject interactionUI; 
        [SerializeField] private GameObject paperCanvas;

        [TextArea] [SerializeField] private string description;
        
        [SerializeField] private TMP_Text textField;

        private Transform player;
        private bool playerInRange;

        private void Start()
        {
            player = NewPlayerController.instance.transform;

            textField.text = description;
            
            SetUIState(false);
            SetCanvasState(false);
        }

        private void Update()
        {
            if (!player || !NewPlayerController.instance.canInteract) return;

            float distance = Vector3.Distance(transform.position, player.position);
            bool shouldShowUI = distance <= interactionRadius && !FightManager.instance.IsInFight();
        
            if (shouldShowUI != playerInRange)
            {
                playerInRange = shouldShowUI;
                SetUIState(playerInRange && !paperCanvas.gameObject.activeSelf);
            }

            if (playerInRange && Input.GetKeyDown(KeyCode.F) && !GameManager.instance.pauseMenuController.isPanelDisplayed)
            {
                TogglePaperCanvas();
            }
        }

        private void SetUIState(bool show)
        {
            if (interactionUI == null) return;

            interactionUI.transform.DOKill();
        
            if (show)
            {
                interactionUI.SetActive(true);
                interactionUI.transform.localScale = Vector3.zero;
                interactionUI.transform.DOScale(Vector3.one, uiFadeDuration)
                    .SetEase(Ease.OutBack);
            }
            else
            {
                interactionUI.transform.DOScale(Vector3.zero, uiFadeDuration * 0.7f)
                    .SetEase(Ease.InBack)
                    .OnComplete(() => interactionUI.SetActive(false));
            }
        }

        private void TogglePaperCanvas()
        {
            bool willActivate = !paperCanvas.gameObject.activeSelf;
            SetCanvasState(willActivate);
            SetUIState(!willActivate && playerInRange);
        
            NewPlayerController.instance.canMove = !willActivate;
            NewPlayerController.instance.reading = willActivate;
        }

        private void SetCanvasState(bool active)
        {
            if (paperCanvas == null) return;

            paperCanvas.gameObject.SetActive(active);
        
            paperCanvas.transform.DOKill();
            if (active)
            {
                paperCanvas.transform.localScale = Vector3.zero;
                paperCanvas.transform.DOScale(Vector3.one, uiFadeDuration)
                    .SetEase(Ease.OutBack);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(0, 0.5f, 1f, 0.5f);
            Gizmos.DrawWireSphere(transform.position, interactionRadius);
        }
    }
}