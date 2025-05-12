using System;
using System.Collections;
using Controller;
using MANAGER;
using TMPro;
using UnityEngine;

namespace UI.Link_To_Radio
{
    public class DescriptionAttackEffect : MonoBehaviour
    {
        [SerializeField] private TMP_Text textDescriptionAttackEffect;

        private void Update() => DisplayDescriptionBasedOnFightState();

        #region Display

        private void DisplayDescriptionBasedOnFightState()
        {
            if (FightManager.instance == null) return;

            textDescriptionAttackEffect.text = FightManager.instance.fightState switch
            {
                FightManager.FightState.OutFight => GetOutOfFightDescription(),
                FightManager.FightState.InFight => GetInFightDescription(),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private string GetOutOfFightDescription()
        {
            if (PlayerController.instance?.selectedAttack == null)
                return "<i>No effect selected</i>";

            var attack = PlayerController.instance.selectedAttack;
            return $"<b><u color=#5E0000>{attack.attack.name}</u></b> : \n" +
                   $"{attack.descriptionAttackEffect.descriptionEffectOutFight}\n";
        }
        private string GetInFightDescription()
        {
            var attackDescription = GetAttackDescription();
            var effectDescription = GetEffectDescription();
            var combatDetails = GetCombatDetails();

            return $"{attackDescription}\n{effectDescription}\n{combatDetails}";
        }

        private string GetAttackDescription()
        {
            if (PlayerController.instance?.selectedAttack == null)
                return "<i>No attack selected</i>";

            var attack = PlayerController.instance.selectedAttack;
            return $"<b><u color=#5E0000>{attack.attack.name}</u></b>: {attack.descriptionAttackEffect.descriptionAttack}\n";
        }

        private string GetEffectDescription()
        {
            if (PlayerController.instance?.selectedAttackEffect == null)
                return "No effect";

            var effect = PlayerController.instance.selectedAttackEffect;
            return $"<b><u color=#5E0000>{effect.attack.name}</u></b>: {effect.descriptionAttackEffect.descriptionEffectInFight}\n";
        }

        private string GetCombatDetails()
        {
            if (PlayerController.instance?.selectedAttack == null)
                return string.Empty;

            var attackData = PlayerController.instance.selectedAttack.attack;
            var slider = RadioController.instance.sliderOscillationPlayer;
            
            float ratio = slider.maxValue > 0 ? slider.value / slider.maxValue : 0f;
            float damage = attackData.damage + (attackData.damageMaxBonus * ratio);
            float overloadChance = attackData.chanceOfOverload * ratio;

            float lifeCost = 0;
            if (PlayerController.instance.selectedAttackEffect != null)
            {
                lifeCost = attackData.costBatteryInFight * 
                         PlayerController.instance.selectedAttackEffect.multiplicatorLifeTaken;
            }
            
            float totalCost = lifeCost + attackData.costBatteryInFight;

            return $"Je vais faire {damage:0.00} dégâts, ce qui coûtera {totalCost:0.00}% mais j'ai {overloadChance:0.00}% de chance de surcharger";
        }

        #endregion
    }
}