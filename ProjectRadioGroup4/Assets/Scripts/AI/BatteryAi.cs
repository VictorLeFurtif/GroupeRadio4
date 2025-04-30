using Controller;
using UnityEngine;

namespace AI
{
    public class BatteryAi : AbstractAI
    {
        protected override void AiBehavior()
        {
            base.AiBehavior();
            float randomValue = Random.Range(0f, 1f);
            
            if (_abstractEntityDataInstance.IsBatteryMoreThanHundred())
            {
                Kamikaze();
                return;
            }

            bool isLowBattery = !_abstractEntityDataInstance.IsBatteryEqualOrMoreThanFifty();
            float healthPercentage = PvEnemy / _abstractEntityData.Hp;

            if (isLowBattery)
            {
                HandleLowBatteryActions(randomValue, healthPercentage);
            }
            else
            {
                HandleHighBatteryActions(randomValue, healthPercentage);
            }
        }

        private void HandleLowBatteryActions(float randomValue, float healthPercentage)
        {
            healthPercentage = Mathf.Clamp01(healthPercentage);
            switch (healthPercentage)
            {
                case < 0.33f:
                {
                    if (randomValue < 0.15f) NormalAttack();
                    else if (randomValue < 0.30f) HeavyAttack();
                    else if (randomValue < 0.55f) StealBatteries();
                    else StealALotBatteries();
                    break;
                }
                case < 0.66f:
                {
                    if (randomValue < 0.3f) NormalAttack();
                    else if (randomValue < 0.5f) HeavyAttack();
                    else if (randomValue < 0.8f) StealBatteries();
                    else StealALotBatteries();
                    break;
                }
                default:
                {
                    if (randomValue < 0.4f) NormalAttack();
                    else if (randomValue < 0.7f) HeavyAttack();
                    else if (randomValue < 0.9f) StealBatteries();
                    else StealALotBatteries();
                    break;
                }
            }
        }
        
        private void HandleHighBatteryActions(float randomValue, float healthPercentage)
        {
            healthPercentage = Mathf.Clamp01(healthPercentage);

            switch (healthPercentage)
            {
                case < 0.33f:
                {
                    if (randomValue < 0.15f) HeavyAttack();
                    else if (randomValue < 0.5f) StealBatteries();
                    else if (randomValue < 0.75f) StealALotBatteries();
                    else ElectricalLeak();
                    break;
                }
                case < 0.66f:
                {
                    if (randomValue < 0.33f) HeavyAttack();
                    else if (randomValue < 0.56f) StealBatteries();
                    else if (randomValue < 0.77f) StealALotBatteries();
                    else ElectricalLeak();
                    break;
                }
                default:
                {
                    if (randomValue < 0.35f) HeavyAttack();
                    else if (randomValue < 0.5f) StealBatteries();
                    else if (randomValue < 0.6f) StealALotBatteries();
                    else ElectricalLeak();
                    break;
                }
            }
        }
        
        #region AI Thought Process
        
        private void Kamikaze()
        {
            PlayerController.instance.ManageLife(-38); //temporary floating value
            animatorEnemyAi.Play("attackAi");
            isPerformingAttack = true;
            PvEnemy = 0;
            canAttack = false;

        }
        
        private void ClassicAttack(float _damageDeal, float _batteryGain, string _attackName)
        {
            if (!canAttack) return;

            canAttack = false;
            isPerformingAttack = true; 
    
            animatorEnemyAi.Play("attackAi");
            Debug.Log(_attackName);
            
            if (PlayerController.instance._inGameData.grosBouclier)
            {
                float damageEnemy = _damageDeal / 2;
                PlayerController.instance.ManageLife(damageEnemy);
                _abstractEntityDataInstance.battery += _batteryGain;
                Debug.Log(_attackName + " (bouclier)");
            }
            else if (PlayerController.instance._inGameData.classicEcho)
            {
                float damageEnemy = _damageDeal * 0.75f;
                float damageForEnemy = _damageDeal * 0.25f;
                PlayerController.instance.ManageLife(damageEnemy);
                PvEnemy -= damageForEnemy;
                _abstractEntityDataInstance.battery += _batteryGain;
                Debug.Log(_attackName + " (echo)");
            }
            else
            {
                PlayerController.instance.ManageLife(_damageDeal);
                _abstractEntityDataInstance.battery += _batteryGain;
                Debug.Log(_attackName);
            }
        }
        
        private void NormalAttack() => ClassicAttack(
            _abstractEntityDataInstance.normalAttack.damage,
            _abstractEntityDataInstance.normalAttack.batteryGain,
            "Normal Attack Done");
        
        private void HeavyAttack() => ClassicAttack(
            _abstractEntityDataInstance.heavyAttack.damage,
            _abstractEntityDataInstance.heavyAttack.batteryGain,
            "Heavy Attack Done");
        private void StealBatteries() => ClassicAttack(
            _abstractEntityDataInstance.stealBatteries.damage,
            _abstractEntityDataInstance.stealBatteries.batteryGain,
            "Steal Batteries Done");
        private void StealALotBatteries() => ClassicAttack( 
            _abstractEntityDataInstance.stealALotBatteries.damage,
            _abstractEntityDataInstance.stealALotBatteries.batteryGain,
            "Steal a lot of Batteries Done");

        private void ElectricalLeak() => ClassicAttack(
            -_abstractEntityDataInstance.battery,
            -_abstractEntityDataInstance.battery,
            "Electrical Leak Done");

        #endregion
    }
}