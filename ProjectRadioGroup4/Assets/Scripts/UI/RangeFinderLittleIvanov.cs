using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace UI
{
    public class RangeFinderLittleIvanov : MonoBehaviour
    {
        [Header("Component References")]
        [SerializeField] private Image _playerImage;
        [SerializeField] private Rigidbody2D _playerRigidbody;

        [Header("Swing Settings")]
        [SerializeField] private float _swingAngle = 5f;
        [SerializeField] private float _swingDuration = 0.5f;
        [SerializeField] private float _stopSmoothness = 0.2f;

        private Tween _swingTween;
        private bool _isMoving;

        private void Awake()
        {
            InitComponent();
        }
        
        private void InitComponent()
        {
            if (_playerRigidbody == null && NewPlayerController.instance != null)
                _playerRigidbody = NewPlayerController.instance.rb;
        }

        private void OnEnable()
        {
            ResetRotation();
        }

        private void OnDisable()
        {
            CleanupTweens();
        }

        private void Update()
        {
            HandleMovementState();
        }

        private void HandleMovementState()
        {
            bool movementState = CheckPlayerMovement();
            
            if (movementState != _isMoving)
            {
                _isMoving = movementState;
                UpdateSwingAnimation();
            }
        }

        private bool CheckPlayerMovement()
        {
            if (_playerRigidbody == null) return false;
            return Mathf.Abs(_playerRigidbody.velocity.x) > 0.1f;
        }

        private void UpdateSwingAnimation()
        {
            if (_isMoving)
            {
                StartSwingAnimation();
            }
            else
            {
                StopSwingAnimation();
            }
        }

        private void StartSwingAnimation()
        {
            CleanupTweens();
            
            _swingTween = _playerImage.rectTransform
                .DOLocalRotate(new Vector3(0, 0, _swingAngle), _swingDuration)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);
        }

        private void StopSwingAnimation()
        {
            if (_swingTween != null && _swingTween.IsActive())
            {
                _swingTween.Kill();
                ResetRotation();
            }
        }

        private void ResetRotation()
        {
            _playerImage.rectTransform.DOLocalRotate(Vector3.zero, _stopSmoothness);
        }

        private void CleanupTweens()
        {
            if (_swingTween != null)
            {
                _swingTween.Kill();
                _swingTween = null;
            }
        }
    }
}