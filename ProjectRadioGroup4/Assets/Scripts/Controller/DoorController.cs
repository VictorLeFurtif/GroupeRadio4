using System;
using MANAGER;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Controller
{
    public class DoorController : MonoBehaviour
    {
        [SerializeField] private DoorType currentDoorType;
        [SerializeField] private string sceneName;
        [SerializeField] private Vector3 positionForPlayer;
        private Animator doorAnimator;
        
        private void Start()
        {
            doorAnimator = GetComponent<Animator>();
        }

        enum DoorType
        {
            SceneLoader,
            [Tooltip("Stay in the scene")]Teleport
        }

        private void ChangePlayerPosition()
        {
            switch (currentDoorType)
            {
                case DoorType.SceneLoader:
                    SceneManager.LoadScene(sceneName);
                    break;
                case DoorType.Teleport:
                    if (PlayerController.instance != null)
                    {
                        PlayerController.instance.transform.position = positionForPlayer;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        private void OnMouseDown()
        {
            if (FightManager.instance?.fightState == FightManager.FightState.InFight)
            {
                return;
            }
            doorAnimator.Play("Door");
        }
    }
}
