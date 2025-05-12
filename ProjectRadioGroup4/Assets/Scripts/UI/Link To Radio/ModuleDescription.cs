using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using MANAGER;

namespace UI.Link_To_Radio
{
    public class ModuleDescription : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private TMP_Text textDescription;
        [Header("Hors Combat")]
        [TextArea][SerializeField] private string descriptionOutOfCombat;
        [TextArea][SerializeField] private string defaultTextOutOfCombat = "no module detected";
        [Header("En Combat")]
        [TextArea][SerializeField] private string descriptionInCombat;
        [TextArea][SerializeField] private string defaultTextInCombat = "select module";
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
                textDescription.text = GetDefaultText();
                textDescription.maxVisibleCharacters = textDescription.text.Length;
            }
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (textDescription == null) return;
            
            currentSequence?.Kill();
            AnimateTextTransition(GetDescription());
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (textDescription == null) return;
            
            currentSequence?.Kill();
            AnimateTextTransition(GetDefaultText());
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

        private string GetDescription()
        {
            return FightManager.instance != null && FightManager.instance.fightState == FightManager.FightState.InFight 
                ? descriptionInCombat 
                : descriptionOutOfCombat;
        }

        private string GetDefaultText()
        {
            return FightManager.instance != null && FightManager.instance.fightState == FightManager.FightState.InFight 
                ? defaultTextInCombat 
                : defaultTextOutOfCombat;
        }

        private void OnDestroy()
        {
            currentSequence?.Kill();
        }
    }
}