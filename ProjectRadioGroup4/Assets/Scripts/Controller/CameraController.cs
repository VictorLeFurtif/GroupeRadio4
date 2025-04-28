using System;
using System.Collections;
using MANAGER;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Controller
{
    public class CameraController : MonoBehaviour
    {
        public enum ShakeMode { Vertical, Horizontal, Both }

        [Header("Follow Settings")]
        [SerializeField] private Vector3 offSett = new Vector3(0f, 0f, -10f);
        [SerializeField] private float smoothTime = 0.25f;
    
        [Header("Shake Settings")]
        [SerializeField] private float defaultShakeDuration = 0.5f;
        [SerializeField] private float defaultShakeMagnitude = 0.15f;
        [SerializeField] private float shakeRecoverySpeed = 1f;

        private Transform target;
        private FightManager fightManager;
        private Vector3 velocity = Vector3.zero;
        private Vector3 originalLocalPos;
        private Coroutine shakeRoutine;
        private Vector3 shakeOffset;

        public static CameraController instance;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            target = PlayerController.instance.gameObject.transform;
            originalLocalPos = transform.localPosition;
            fightManager = FightManager.instance;
        }

        #region FollowPlayer

        private void FollowPlayer()
        {
            Vector3 targetPosition = target.position + offSett;
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition + shakeOffset, ref velocity, smoothTime);
        }

        private void CameraBehavior()
        {
            if (fightManager.fightState == FightManager.FightState.OutFight)
            {
                FollowPlayer();
            }
            else
            {
                FollowCombatView();
            }
        }
        
        private void FollowCombatView()
        {
            if (FightManager.instance == null || 
                FightManager.instance.listOfJustEnemiesAlive.Count == 0 || 
                PlayerController.instance == null )
            {
                FollowPlayer();
                return;
            }

            var lastEnemyWrapper = FightManager.instance.listOfJustEnemiesAlive[^1];
            
            if (lastEnemyWrapper == null || lastEnemyWrapper.entity == null)
            {
                FollowPlayer();
                return;
            }

            Transform playerTransform = PlayerController.instance.transform;
            Transform lastEnemyTransform = lastEnemyWrapper.entity.transform;

            if (lastEnemyTransform == null)
            {
                FollowPlayer();
                return;
            }

            Vector3 middlePoint = (playerTransform.position + lastEnemyTransform.position) / 2f;

            if (FightManager.instance.currentOrder != null && FightManager.instance.currentOrder.Count > 0)
            {
                var firstInOrder = FightManager.instance.currentOrder[0];
                if (firstInOrder != null && firstInOrder.entity != null && firstInOrder.entity.transform != null)
                {
                    middlePoint = Vector3.Lerp(middlePoint, firstInOrder.entity.transform.position, 0.25f);
                }
            }

            Vector3 targetPosition = middlePoint + offSett;
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition + shakeOffset, ref velocity, smoothTime);
        }



        private void LateUpdate()
        {
            CameraBehavior();
        }

        #endregion

        #region ShakingCamera

        public void Shake(ShakeMode mode, float duration = -1f, float magnitude = -1f)
        {
            if (duration < 0) duration = defaultShakeDuration;
            if (magnitude < 0) magnitude = defaultShakeMagnitude;

            if (shakeRoutine != null)  //important au cas où pour pas que plusieurs coroutine se démarre, normalement ça devrait aps arriver
                StopCoroutine(shakeRoutine);
        
            shakeRoutine = StartCoroutine(PerformShake(mode, duration, magnitude));
        }

        private IEnumerator PerformShake(ShakeMode mode, float duration, float magnitude)
        {
            float elapsed = 0f;
        
            while (elapsed < duration)
            {
                Vector3 direction = GetShakeDirection(mode);
                float progress = elapsed / duration;
                //décroissance moins brusque grâce au shakeRevoverySpeed et merci les maths
                float currentMagnitude = Mathf.Lerp(magnitude, 0f, progress * shakeRecoverySpeed);
            
                shakeOffset = direction * currentMagnitude; //en jouant sur le shake Offset ici cela est Update dans le FollowPlayer
            
                elapsed += Time.deltaTime;
                yield return null;
            }
        
            shakeOffset = Vector3.zero;
            shakeRoutine = null;
        }

        private Vector3 GetShakeDirection(ShakeMode mode)
        {
            return mode switch
            {
                ShakeMode.Vertical => new Vector3(0f, Random.Range(-1f, 1f), 0f),
                ShakeMode.Horizontal => new Vector3(Random.Range(-1f, 1f), 0f, 0f),
                ShakeMode.Both => new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f),
                _ => Vector3.zero //ça représente le default case mais le déboucher est impossible
            };
        }

        #endregion
        
    }
}