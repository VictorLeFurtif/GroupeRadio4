using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.BUTTON
{
    public class BouttonFeedBack : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [Header("Sprite Settings")]
        [SerializeField] private Sprite normalSprite;   
        [SerializeField] private Sprite hoverSprite;    
        [SerializeField] private Image buttonImage;   

        private void Awake()
        {
            if (buttonImage == null)
            {
                buttonImage = GetComponent<Image>();
            }

            if (buttonImage != null && normalSprite != null)
            {
                buttonImage.sprite = normalSprite;
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (buttonImage != null && hoverSprite != null)
            {
                buttonImage.sprite = hoverSprite;
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (buttonImage != null && normalSprite != null)
            {
                buttonImage.sprite = normalSprite;
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (buttonImage != null && hoverSprite != null)
            {
                buttonImage.sprite = normalSprite;
            }


        }
    }
}