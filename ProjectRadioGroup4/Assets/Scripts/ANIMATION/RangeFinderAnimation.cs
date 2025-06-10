using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ANIMATION
{
    public class RangeFinderAnimation : MonoBehaviour
    {
        public Animator animatorRangeFinder;
        
        [SerializeField] private float minInterval = 10f; 
        [SerializeField] private float maxInterval = 20f; 
        
        private Coroutine randomAnimCoroutine;
        
        private void Awake()
        {
            animatorRangeFinder = GetComponent<Animator>();
            if (animatorRangeFinder == null)
            {
                Debug.LogError("Animator component missing on RangeFinder!", this);
            }
        }

        private void Start()
        {
            StartRandomAnimation();
        }

        public IEnumerator TurnOnRangeFinder()
        {
            NewPlayerController.instance?.rangeFinderManager?.TurnRangeFinder(true);
            yield return PlayAnimation("RfTurnOn");
            yield return null;
            animatorRangeFinder.Play("RfIdle");
        }
    
        public IEnumerator TurnOffRangeFinder()
        {
            yield return PlayAnimation("RfTurnOn");
            animatorRangeFinder.Play("RfIdle");
            yield return null;
            NewPlayerController.instance?.rangeFinderManager?.TurnRangeFinder(false);
        }

        private IEnumerator PlayAnimation(string nameOfAnimation)
        {
            if (animatorRangeFinder == null) yield break;
            
            animatorRangeFinder.Play(nameOfAnimation);
            
            yield return null;
            
            yield return new WaitForSeconds(animatorRangeFinder.GetCurrentAnimatorStateInfo(0).length);
        }
        
        public void StartRandomAnimation()
        {
            if (randomAnimCoroutine == null)
            {
                randomAnimCoroutine = StartCoroutine(RandomAnimationLoop());
            }
        }

        public void StopRandomAnimation()
        {
            if (randomAnimCoroutine != null)
            {
                StopCoroutine(randomAnimCoroutine);
                randomAnimCoroutine = null;
            }
        }

        private IEnumerator RandomAnimationLoop()
        {
            while (true)
            {
                
                float waitTime = Random.Range(minInterval, maxInterval);
                yield return new WaitForSeconds(waitTime);
                
                animatorRangeFinder.Play("RfBug");
                
                yield return new WaitForSeconds(animatorRangeFinder.GetCurrentAnimatorStateInfo(0).length);
            }
        }

        private void OnDestroy()
        {
            StopRandomAnimation();
        }
    }
}