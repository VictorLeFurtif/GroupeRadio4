using MANAGER;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Controller
{
    public class LooseScreenController : MonoBehaviour
    {
        public GameObject looseScreenPanel;

        private void Awake()
        {
            looseScreenPanel.SetActive(false);
        }

        private void FixedUpdate()
        {
            if (GameManager.instance.CurrentGameState == GameManager.GameState.GameOver)
            {
                GameOver();
            }
        }

        private void GameOver()
        {
            looseScreenPanel.SetActive(true);
        }

        public void Retry()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            GameManager.instance.CurrentGameState = GameManager.GameState.Game;
            GameManager.instance.ResetPlayer();
        }

        public void Menu()
        {
            SceneManager.LoadScene("MainMenu");
            GameManager.instance.CurrentGameState = GameManager.GameState.Menu;
        }
    }
}
