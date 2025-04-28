using UnityEngine;

namespace UI
{
    public class ZoneTutorialTrigger : MonoBehaviour
    {
        [TextArea]
        public string tutorialMessage;

        [SerializeField] private TutorialUIManager tutorialUI;

        private bool playerInside = false;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            playerInside = true;
            tutorialUI.ShowTutorial(tutorialMessage);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag("Player") || !playerInside) return;
            playerInside = false;
            tutorialUI.HideTutorial();
        }
    }
}