using Controller;

namespace AI
{
    public class OnlyAttackAi : AbstractAI
    {
        protected override void AiBehavior()
        {
            base.AiBehavior();
            PlayerController.instance?.ManageLife(-1); 
            animatorEnemyAi.Play("attackAi");
            isPerformingAttack = true;
            canAttack = false;
        }
    }
}