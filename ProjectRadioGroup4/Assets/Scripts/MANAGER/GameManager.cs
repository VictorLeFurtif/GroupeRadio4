using System;
using Controller;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MANAGER
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;
        public GlobalVolumeManager globalVolumeManager;
        [SerializeField] private LooseScreenController looseScreenController;
    
        public GameState currentGameState = GameState.Menu;
        
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            CurrentGameState = GameState.Game;
            globalVolumeManager = GetComponentInChildren<GlobalVolumeManager>();
        }

        public GameState CurrentGameState
        {
            get => currentGameState;
            set
            {
                currentGameState = value; 
                
                switch (value)
                {
                    case GameState.GameOver:
                        looseScreenController.looseScreenPanel.SetActive(true);
                        break;
                    case GameState.Game:
                    case GameState.Menu:
                        looseScreenController.looseScreenPanel.SetActive(false);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(value), value, null);
                }
            }
        }
    
        public enum GameState
        {
            Menu,
            Game,
            GameOver,
        }

        public void GameOver()
        {
            CurrentGameState = GameState.GameOver;
        }

        public void ResetPlayer()
        {
            NewPlayerController.instance.InitData();
            NewPlayerController.instance.animatorPlayer.Play("IdlePlayer");
            NewPlayerController.instance.transform.position = NewPlayerController.instance.spawnPosition;
        }
    }
}

