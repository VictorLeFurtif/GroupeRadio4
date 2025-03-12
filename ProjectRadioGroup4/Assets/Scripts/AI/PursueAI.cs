using Controller;
using UnityEngine;

namespace AI
{
    public class PursueAI : AbstractAI
    {
        private IaPursueDataInstance _iaPursueDataInstance;
        
        private bool isChasing = false;
        
        protected override void AiShift()
        {
            if (_aiFightState == AiFightState.InFight) return;
            
            if (IsPlayerInSight(1)) isChasing = true;

            if (isChasing && !IsPlayerInSight(_iaPursueDataInstance.rangeSightAsp))
            {
                Debug.Log("Stop chasing");
                isChasing = false;
            }

            if (!isChasing) return;
            Vector3 playerTransform = PlayerController.instance.transform.position; // check in update location of the player
            Vector3 direction = (playerTransform - transform.position).normalized;
            transform.Translate(direction * _iaPursueDataInstance.moveSpeed * Time.deltaTime);

        }

        private bool IsPlayerInSight(int increaseRangeSight)
        {
            if (PlayerController.instance != null)
                return Mathf.Abs(PlayerController.instance.transform.position.x - transform.position.x)
                       < _iaPursueDataInstance.rangeSight * increaseRangeSight;
            Debug.LogError("PlayerController.instance is NULL! Assurez-vous qu'un PlayerController existe dans la scène.");
            return false;
        }

        
        protected override void Init()
        {
            base.Init();
            _iaPursueDataInstance = (IaPursueDataInstance)_abstractEntityDataInstance;
        }
        
    }
}
