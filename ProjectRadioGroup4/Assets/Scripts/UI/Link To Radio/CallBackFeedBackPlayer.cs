using TMPro;
using UnityEngine;
using DG.Tweening;

namespace UI.Link_To_Radio
{
    public class CallBackFeedBackPlayer : MonoBehaviour
    {
        [SerializeField] private TMP_Text textDescriptionAttackEffect;
        public static CallBackFeedBackPlayer Instance { get; private set; }
        
        private Sequence _currentAnimation;
        
        [SerializeField] private float typingSpeed = 0.02f;
        [SerializeField] private float disappearSpeed = 0.01f;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            ShowMessage("");
        }
        
        public void ShowMessage(string message)
        {
            if (_currentAnimation != null && _currentAnimation.IsActive())
            {
                _currentAnimation.Kill();
            }
           
            textDescriptionAttackEffect.text = message;
            textDescriptionAttackEffect.maxVisibleCharacters = 0;
            textDescriptionAttackEffect.alpha = 1;
            
            _currentAnimation = DOTween.Sequence();
            
            _currentAnimation.Append(
                DOTween.To(
                    () => textDescriptionAttackEffect.maxVisibleCharacters,
                    x => textDescriptionAttackEffect.maxVisibleCharacters = x,
                    message.Length,
                    message.Length * typingSpeed) 
                .SetEase(Ease.Linear));
            
            _currentAnimation.AppendInterval(1f);
            
            _currentAnimation.Append(
                DOTween.To(
                    () => textDescriptionAttackEffect.maxVisibleCharacters,
                    x => textDescriptionAttackEffect.maxVisibleCharacters = x,
                    0,
                    message.Length * disappearSpeed) 
                .SetEase(Ease.Linear));
            
        }
        
        private void OnDestroy()
        {
            _currentAnimation?.Kill();
        }
    }
}