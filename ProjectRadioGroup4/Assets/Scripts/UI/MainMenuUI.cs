using System.Collections;
using System.Collections.Generic;
using MANAGER;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class MainMenuUI : MonoBehaviour
    {
    
    
        [SerializeField] private GameObject optionsPanel;

        [SerializeField] private GameObject mainMenuPanel;
        [SerializeField] private TMP_Text displayText;
        [SerializeField, TextArea(3, 10)] private List<string> textSequence;
        [SerializeField] private float typingSpeed = 0.05f;
        [SerializeField] private float delayBetweenTexts = 1f;
        [SerializeField] private GameObject storyLineOb;

        private Coroutine currentSequence;
        private string targetSceneName;
        private bool isTyping = false;
    
        private AsyncOperation sceneLoadingOperation;
    
        private void Start()
        {
            ChangeWindowMode(true);
        }

        private void Update()
        {
            if (isTyping && Input.GetKeyDown(KeyCode.Space))
            {
                SkipIntro();
            }
        }

        public void LoadGameByNameScene(string _nameScene)
        {
            if (currentSequence != null)
                StopCoroutine(currentSequence);
        
            targetSceneName = _nameScene;
            currentSequence = StartCoroutine(TextSequenceCoroutine(_nameScene));
            if (SoundManager.instance.soundMenu != null)
            {
                SoundManager.instance.soundMenu.SetActive(false);
            }

            if (targetSceneName == "MainMenuNew")
            {
                SoundManager.instance.soundMenu.SetActive(true);
            }

            
        }

        private IEnumerator TextSequenceCoroutine(string sceneName)
        {
            storyLineOb.SetActive(true);
            isTyping = true;

            foreach (string text in textSequence)
            {
                yield return StartCoroutine(TypeTextCoroutine(text));
                yield return new WaitForSeconds(delayBetweenTexts);
            }

            FinishSequence(sceneName);
        }

        private IEnumerator TypeTextCoroutine(string text)
        {
            displayText.text = "";
        
            foreach (char letter in text.ToCharArray())
            {
                displayText.text += letter;
                yield return new WaitForSeconds(typingSpeed);
            }
        }

        private void FinishSequence(string sceneName)
        {
            isTyping = false;
            SceneManager.LoadScene(sceneName);
        
            if (sceneName == "LV1" && NewPlayerController.instance != null)
            {
                NewPlayerController.instance.transform.position = new Vector3(6, 0, 0);
            }
        
            if (GameManager.instance != null && sceneName != "MainMenuNew")
            {
                GameManager.instance.CurrentGameState = GameManager.GameState.Game;
            }
        }

        private void SkipIntro()
        {
            if (currentSequence != null)
                StopCoroutine(currentSequence);
        
            if (textSequence.Count > 0 && displayText != null)
            {
                displayText.text = textSequence[^1];
            }
        
            FinishSequence(targetSceneName);
        }

    

        public void OpenOptions()
        {
            optionsPanel.SetActive(true);
            mainMenuPanel.SetActive(false);
        }

        public void QuitOptions()
        {
            optionsPanel.SetActive(false);
            mainMenuPanel.SetActive(true);
        }
    

        public void Quit()
        {
            Application.Quit();
        }

        public void ChangeWindowMode(bool toggled)
        {
            if (GameManager.instance != null)
            {
                GameManager.instance.FullScreen(toggled);
            }
        
        }
    }
}
