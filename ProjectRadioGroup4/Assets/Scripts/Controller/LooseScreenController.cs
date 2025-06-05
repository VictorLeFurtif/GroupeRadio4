using MANAGER;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Controller
{
    public class LooseScreenController : MonoBehaviour
    {
        [SerializeField] private GameObject looseScreenPanel;

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
            Debug.LogError("ACTIVATION PANEL");
            looseScreenPanel.SetActive(true);
        }

        public void Retry()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void Menu()
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}
