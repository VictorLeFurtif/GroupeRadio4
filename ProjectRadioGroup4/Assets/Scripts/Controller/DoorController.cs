using System.Collections;
using MANAGER;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorController : MonoBehaviour
{
    public enum DoorType { SceneLoader, Teleport }
    
    [Header("Settings")]
    [SerializeField] private DoorType currentDoorType;
    [SerializeField] private string sceneName;
    [SerializeField] private Vector3 positionForPlayer;
    [SerializeField] private float interactionRadius = 1.5f;
    
    private Animator doorAnimator;
    private Transform playerTransform;

    private void Start()
    {
        doorAnimator = GetComponent<Animator>();
        playerTransform = NewPlayerController.instance.transform;
    }

    private void Update()
    {
        if (!NewPlayerController.instance) return;
        
        float distance = Vector2.Distance(playerTransform.position, transform.position);
        bool canInteract = distance <= interactionRadius && 
                           !FightManager.instance.IsInFight();
        
        NewPlayerController.instance.ToggleInteractionUI(canInteract);
        
        if (canInteract && Input.GetKeyDown(KeyCode.F))
        {
            StartCoroutine(Interact());
        }
    }

    private IEnumerator Interact()
    {
        doorAnimator.Play("animDoor");
        NewPlayerController.instance.canMove = false;
        yield return null;
        float timeToWait = doorAnimator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(timeToWait);
        NewPlayerController.instance.canMove = true;
        ChangePlayerPosition();
    }

    private void ChangePlayerPosition()
    {
        switch (currentDoorType)
        {
            case DoorType.SceneLoader:
                SceneManager.LoadScene(sceneName);
                playerTransform.position = positionForPlayer;
                break;
            case DoorType.Teleport:
                playerTransform.position = positionForPlayer;
                break;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}