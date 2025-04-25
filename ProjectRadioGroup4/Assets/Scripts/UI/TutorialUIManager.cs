using DG.Tweening;
using TMPro;
using UnityEngine;

namespace UI
{
    public class TutorialUIManager : MonoBehaviour
    {
        [SerializeField] private CanvasGroup tutorialCanvasGroup;
        [SerializeField] private TextMeshProUGUI tutorialText;
        [SerializeField] private float fadeDuration = 0.5f;

        private void Awake()
        {
            tutorialCanvasGroup.alpha = 0;
            tutorialCanvasGroup.interactable = false;
            tutorialCanvasGroup.blocksRaycasts = false;
        }

        public void ShowTutorial(string message)
        {
            tutorialText.text = message;
            tutorialCanvasGroup.DOFade(1f, fadeDuration);
            tutorialCanvasGroup.interactable = true;
            tutorialCanvasGroup.blocksRaycasts = true;
        }

        public void HideTutorial()
        {
            tutorialCanvasGroup.DOFade(0f, fadeDuration);
            tutorialCanvasGroup.interactable = false;
            tutorialCanvasGroup.blocksRaycasts = false;
        }
    }
}