using Controller;
using UI.Link_To_Radio;

namespace AI.OLD_AI_BEFORE_FP_V2
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