using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MANAGER
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;
        public GlobalVolumeManager globalVolumeManager;
    
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
                /*
                switch (value)
                {
                    case GameState.GameOver:
                        NewRadioManager.instance.canvaRadio.enabled = false;
                        break;
                    case GameState.Game:
                        NewRadioManager.instance.canvaRadio.enabled = true;
                        break;
                    case GameState.Menu:
                        NewRadioManager.instance.canvaRadio.enabled = false;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(value), value, null);
                }*/
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

        public void ReloadActualScene()
        {
            string nameScene = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(nameScene);
        }
    
        public void LoadSceneByName(string _name)
        {
            SceneManager.LoadScene(_name);
        }
    }
}

