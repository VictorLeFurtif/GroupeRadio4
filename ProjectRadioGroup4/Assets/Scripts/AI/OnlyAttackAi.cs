using Controller;
using UI.Link_To_Radio;

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
            CallBackFeedBackPlayer.Instance.ShowMessage("Enemy Attack");
        }
    }
}