using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using MANAGER;

public class DoorController : MonoBehaviour
{
    public enum DoorType { SceneLoader, Teleport }

    [Header("Door Settings")]
    [SerializeField] private DoorType currentDoorType;
    [SerializeField] private string sceneName;
    [SerializeField] private Vector3 positionForPlayer;
    [SerializeField] private float interactionRadius = 1.5f;
    [SerializeField] private float fadeDuration = 0.3f;

    [Header("UI References")]
    [SerializeField] private GameObject interactionUI;
    [SerializeField] private Animator doorAnimator;

    private Transform player;
    private bool playerInRange;
    private bool isInteracting;

    private void Start()
    {
        player = NewPlayerController.instance.transform;
        
        // Initialisation UI
        if (interactionUI != null)
        {
            interactionUI.SetActive(false);
            interactionUI.transform.localScale = Vector3.zero;
        }
    }

    private void Update()
    {
        if (!player || isInteracting || !NewPlayerController.instance.canInteract) return;

        float distance = Vector2.Distance(transform.position, player.position);
        bool shouldShow = distance <= interactionRadius && !FightManager.instance.IsInFight();
        
        if (shouldShow != playerInRange)
        {
            playerInRange = shouldShow;
            ToggleInteractionUI(playerInRange);
        }

        if (playerInRange && Input.GetKeyDown(KeyCode.F))
        {
            StartInteraction();
        }
    }

    private void ToggleInteractionUI(bool show)
    {
        if (interactionUI == null) return;

        interactionUI.transform.DOKill();

        if (show)
        {
            interactionUI.SetActive(true);
            interactionUI.transform.DOScale(Vector3.one, fadeDuration)
                .SetEase(Ease.OutBack);
        }
        else
        {
            interactionUI.transform.DOScale(Vector3.zero, fadeDuration * 0.7f)
                .SetEase(Ease.InBack)
                .OnComplete(() => interactionUI.SetActive(false));
        }
    }

    private void StartInteraction()
    {
        isInteracting = true;
        NewPlayerController.instance.canMove = false;
        
        if (interactionUI != null)
        {
            interactionUI.transform.DOKill();
            interactionUI.SetActive(false);
        }

        if (doorAnimator != null)
        {
            doorAnimator.Play("animDoor");
            float animLength = doorAnimator.GetCurrentAnimatorStateInfo(0).length;
            Invoke(nameof(CompleteInteraction), animLength);
        }
        else
        {
            CompleteInteraction();
        }
    }

    private void CompleteInteraction()
    {
        switch (currentDoorType)
        {
            case DoorType.SceneLoader:
                SceneManager.LoadScene(sceneName);
                NewPlayerController.instance.transform.position = positionForPlayer;
                break;
                
            case DoorType.Teleport:
                NewPlayerController.instance.transform.position = positionForPlayer;
                break;
        }

        NewPlayerController.instance.canMove = true;
        isInteracting = false;
        playerInRange = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}