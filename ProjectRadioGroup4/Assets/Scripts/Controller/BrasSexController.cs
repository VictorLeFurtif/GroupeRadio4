using System.Collections;
using UnityEngine;

namespace Controller
{
    public class BrasSexController : MonoBehaviour
    {
        [SerializeField] private Animator brasSexAnimator;
        [SerializeField] private string animationName = "animationBrasSex";

        private void Awake()
        {
            if (brasSexAnimator == null)
            {
                brasSexAnimator = GetComponent<Animator>();
                
                if (brasSexAnimator == null)
                {
                    Debug.LogError("No Animator component found on BrasSexController!", this);
                }
            }
        }

        public IEnumerator TransitionBrasSexUi()
        {
            if (brasSexAnimator == null)
            {
                Debug.LogError("Animator reference is null in BrasSexController!", this);
                yield break;
            }

            
            if (!HasAnimation(animationName))
            {
                Debug.LogError($"Animation '{animationName}' not found in Animator!", this);
                yield break;
            }

            brasSexAnimator.Play(animationName, 0, 0f);
            
            float time = GetAnimationLength(animationName);
            if (time <= 0)
            {
                Debug.LogWarning($"Animation '{animationName}' has 0 or negative length, using default 1 second", this);
                time = 1f;
            }

            yield return new WaitForSeconds(time);
        }

        private bool HasAnimation(string animName)
        {
            if (brasSexAnimator.runtimeAnimatorController == null)
                return false;

            foreach (AnimationClip clip in brasSexAnimator.runtimeAnimatorController.animationClips)
            {
                if (clip.name == animName)
                    return true;
            }
            return false;
        }

        private float GetAnimationLength(string animName)
        {
            if (brasSexAnimator.runtimeAnimatorController == null)
                return 0f;

            foreach (AnimationClip clip in brasSexAnimator.runtimeAnimatorController.animationClips)
            {
                if (clip.name == animName)
                    return clip.length;
            }
            return 0f;
        }
    }
}