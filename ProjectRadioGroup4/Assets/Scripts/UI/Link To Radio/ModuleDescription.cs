using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace UI.Link_To_Radio
{
    public class ModuleDescription : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private TMP_Text textDescription;
        [TextArea][SerializeField] private string description;
        [SerializeField] private string defaultText = "no module detected";
        [SerializeField] private float typingSpeed = 0.02f;
        [SerializeField] private float disappearSpeed = 0.01f;
        
        private Sequence currentSequence;

        private void Start()
        {
            Init();
        }

        private void Init()
        {
            if (textDescription != null) 
            {
                textDescription.text = defaultText;
                textDescription.maxVisibleCharacters = defaultText.Length;
            }
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (textDescription == null) return;
            
            currentSequence?.Kill();
            AnimateTextTransition(description);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (textDescription == null) return;
            
            currentSequence?.Kill();
            AnimateTextTransition(defaultText);
        }

        private void AnimateTextTransition(string targetText)
        {
            currentSequence = DOTween.Sequence();
            
            currentSequence.Append(
                DOTween.To(
                    () => textDescription.maxVisibleCharacters,
                    x => textDescription.maxVisibleCharacters = x,
                    0,
                    textDescription.text.Length * disappearSpeed)
                .OnComplete(() => textDescription.text = targetText));
            
            currentSequence.Append(
                DOTween.To(
                    () => textDescription.maxVisibleCharacters,
                    x => textDescription.maxVisibleCharacters = x,
                    targetText.Length,
                    targetText.Length * typingSpeed));
        }

        private void OnDestroy()
        {
            currentSequence?.Kill();
        }
    }
}