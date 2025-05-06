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
        private bool isOpen = false;
        [SerializeField] private GameObject codex;

        private void Awake()
        {
            codex.SetActive(isOpen);
        }

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
                return "No effect selected";

            var attack = PlayerController.instance.selectedAttack;
            return $"{attack.descriptionAttackEffect.descriptionEffectOutFight}\n" +
                   $"The cost of this effect is {attack.attack.costBatteryExploration}";
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
                return "No attack selected";

            var attack = PlayerController.instance.selectedAttack;
            return $"Attack Description: {attack.descriptionAttackEffect.descriptionAttack}\n" +
                   $"Cost: {attack.attack.costBatteryInFight}";
        }

        private string GetEffectDescription()
        {
            if (PlayerController.instance?.selectedAttackEffect == null)
                return "No effect";

            var effect = PlayerController.instance.selectedAttackEffect;
            return $"Effect Description: {effect.descriptionAttackEffect.descriptionEffectInFight}\n" +
                   $"Cost Multiplier: {effect.multiplicatorLifeTaken}x attack cost";
        }

        private string GetCombatDetails()
        {
            if (PlayerController.instance?.selectedAttack == null || 
                PlayerController.instance.selectedAttackEffect == null)
                return string.Empty;

            var attackData = PlayerController.instance.selectedAttack.attack;
            var slider = RadioController.instance.sliderOscillationPlayer;
            
            float ratio = slider.maxValue > 0 ? slider.value / slider.maxValue : 0f;
            float damage = attackData.damage + (attackData.damageMaxBonus * ratio);
            float overloadChance = attackData.chanceOfOverload * ratio;
            float lifeCost = attackData.costBatteryInFight * 
                           PlayerController.instance.selectedAttackEffect.multiplicatorLifeTaken;
            float totalCost = lifeCost + attackData.costBatteryInFight;

            return $"Projected Damage: {damage:0.00}\n" +
                   $"Overload Chance: {overloadChance:0.00}%\n" +
                   $"Total Resource Cost: {totalCost:0.00}";
        }

        #endregion

        public void ToggleSwitchCodex()
        {
            isOpen = !isOpen;
            codex.SetActive(isOpen);
        }
        
    }
}