using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Utility;

namespace UI.Link_To_Radio
{
    public class ModuleDescription : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private TMP_Text textDescription;
        [TextArea][SerializeField] private string description;
        [SerializeField] private string defaultText = "";
        
        private Coroutine typingCoroutine;

        private void Start()
        {
            if (textDescription != null) textDescription.text = defaultText;
        }

        private void ResetText(TMP_Text targetText)
        {
            targetText.text = "";
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (textDescription != null)
            {
                ResetText(textDescription);
                if (typingCoroutine != null)
                {
                    StopCoroutine(typingCoroutine);
                }
                typingCoroutine = StartCoroutine(TypeText(description));
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (textDescription != null)
            {
                ResetText(textDescription);
               
                if (typingCoroutine != null)
                {
                    StopCoroutine(typingCoroutine);
                }
            }
        }
        
        private IEnumerator TypeText( string fullText, float delay = 0.02f)
        {
            
            foreach (char c in fullText)
            {
                textDescription.text += c;
                yield return new WaitForSeconds(delay);
            }
        }
    }
}