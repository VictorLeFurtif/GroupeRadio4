using UnityEngine;

namespace AI
{
    public class SurpriseAi : AbstractAI
    {
        private SpriteRenderer enemySpriteRenderer;
    
        protected override void Init()
        {
            base.Init();
            enemySpriteRenderer = GetComponent<SpriteRenderer>();
            enemySpriteRenderer.enabled = false;
            //SwitchSpriteRenderer(enemySpriteRenderer); //Set spriteRenderer to False to hide the enemy.

        }
        protected override void AiShift()
        {
            //no need like for the static ai
        }

        protected override void OnTriggerEnter2D(Collider2D other)
        {
            base.OnTriggerEnter2D(other);
            if (other.CompareTag("Player"))
            {
                enemySpriteRenderer.enabled = true;
            }
        }

        private void SwitchSpriteRenderer(SpriteRenderer _spriteRenderer)
        {
            _spriteRenderer.enabled = _spriteRenderer.enabled switch
            {
                true => false,
                false => true
            };
        }
    }
}
