using MANAGER;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class PauseMenuController : MonoBehaviour
    {
        [SerializeField] private GameObject PausePanel;
        [SerializeField] private Slider sliderMaster; 
    
        private bool isPanelDisplayed;
    
        private void Awake()
        {
            sliderMaster.value = SoundManager.instance.masterVolume;
        }

        private void Update()
        {
            if (ShouldTogglePause())
            {
                ChangePanelState();
                ShowPanel();
            }
            
            //SetTimeScale();
        }

        private bool ShouldTogglePause()
        {
            return SceneManager.GetActiveScene().buildIndex != 0 
                   && Input.GetKeyDown(KeyCode.Escape) 
                   && FightManager.instance.fightState is not FightManager.FightState.InFight && 
                   NewPlayerController.instance.reading is not true;
        }

        private void SetTimeScale()
        {
            Time.timeScale = isPanelDisplayed ? 0 : 1;
        }

        public void ChangePanelState()
        {
            isPanelDisplayed = !isPanelDisplayed;
        
       
            if (!isPanelDisplayed)
            {
                SoundManager.instance.UpdateMasterVolume(sliderMaster.value);
            }
        }

        private void ShowPanel()
        {
            PausePanel.SetActive(isPanelDisplayed);
            NewPlayerController.instance.canMove = !isPanelDisplayed;
        }
    
        public void ReturnToMenu()
        {
            GameManager.instance.CurrentGameState = GameManager.GameState.Menu;
            isPanelDisplayed = false;
            Time.timeScale = 1;
            SceneManager.LoadScene("MainMenuNew");
            GameManager.instance.ResetPlayer();
            PausePanel.SetActive(false);
        }

        public void QuitGame()
        {
            Application.Quit();
        }

        public void OnMasterVolumeChanged(float value)
        {
            SoundManager.instance.masterVolume = value;
        }
    }
}