using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MANAGER
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;
        public Canvas gameOver;
    
        public GameState currentGameState = GameState.Menu;
        private void Awake()
        {
            if (instance == null) instance = this;
            else Destroy(gameObject);
        }

        private void Start()
        {
            CurrentGameState = GameState.Game;
        }

        public GameState CurrentGameState
        {
            get => currentGameState;
            set
            {
                switch (value)
                {
                    case GameState.GameOver : gameOver.enabled = true;
                        NewRadioManager.instance.canvaRadio.enabled = false;
                        break;
                    case GameState.Game : gameOver.enabled = false;
                        NewRadioManager.instance.canvaRadio.enabled = true;
                        break;
                    case GameState.Menu : gameOver.enabled = false;
                        NewRadioManager.instance.canvaRadio.enabled = false;
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

