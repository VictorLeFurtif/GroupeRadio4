using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class ModuleDescription : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private TMP_Text textDescription;
        [TextArea][SerializeField] private string description;
        [SerializeField] private string defaultText = "No module detected";

        private void Start()
        {
            if (textDescription != null) textDescription.text = defaultText;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if(textDescription != null)
                textDescription.text = description;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if(textDescription != null)
                textDescription.text = defaultText;
        }
    }
}