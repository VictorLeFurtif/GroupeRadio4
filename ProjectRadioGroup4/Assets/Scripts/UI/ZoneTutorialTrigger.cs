using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// JACQUES ne gronde pas victor c'est Ethan en GD qui a modifié le script.

namespace UI
{
    public class ZoneTutorialTrigger : MonoBehaviour
    {
        [TextArea]
        [SerializeField]
        public List<string> text;
        public string tutorialMessage;
        public int readerID;
        
        public Button nextButton;
        [SerializeField] private float cooldown = 0.2f;

        [SerializeField] private TutorialUIManager tutorialUI;

        private bool playerInside;

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

        //C'est moi qui ai fait tout seul comme un grand !, c'est pour changer le texte avec un bouton.
        public void StepForward()
        {
            if (!playerInside) return;

            if (readerID + 1 < text.Count)
            {
                readerID++;
                tutorialMessage = text[readerID];
                tutorialUI.ShowTutorial(tutorialMessage);
            }
            else
            {
                readerID = 0;
                tutorialUI.HideTutorial();
            }
            StartCoroutine(ClickCooldown());
        }
        IEnumerator ClickCooldown() //Cooldown du bouton 
        {
            nextButton.interactable = false;
            yield return new WaitForSeconds(cooldown);
            nextButton.interactable = true;
        }
    }
}