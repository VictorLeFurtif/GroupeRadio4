using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// JACQUES gronde pas victor c'est Ethan GD qui as modifié le script.

namespace UI
{
    public class ZoneTutorialTrigger : MonoBehaviour
    {
        [TextArea]
        [SerializeField] private List<string> text;
        public string tutorialMessage;
        [SerializeField] private int readerID;
        
        public Button nextButton;

        [SerializeField] private TutorialUIManager tutorialUI;

        private bool playerInside = false;

        private void Start()
        {
            nextButton.onClick.AddListener(StepForward);
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            playerInside = true;
            tutorialMessage = text[readerID];
            tutorialUI.ShowTutorial(tutorialMessage);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag("Player") || !playerInside) return;
            playerInside = false;
            tutorialUI.HideTutorial();
        }

        //C'est moi qui ai fait tout seul comme un grand ! c'est pour changer le texte avec un bouton
        public void StepForward()
        {
            Debug.Log("step forward");
            Debug.Log($"CLICK at time {Time.time}");

            if (!playerInside) return;

            if (readerID < text.Count)
            {
                readerID++;
                Debug.Log(readerID);
                tutorialMessage = text[readerID];
                tutorialUI.ShowTutorial(tutorialMessage);
            }
            else
            {
                readerID = 0;
                tutorialUI.HideTutorial(); // ou text[0] si tu veux boucler
            }
        }
    }
}