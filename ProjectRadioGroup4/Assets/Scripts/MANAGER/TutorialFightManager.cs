using System.Collections.Generic;
using DATA.Script.Tutorial;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MANAGER
{
    public class TutorialFightManager : MonoBehaviour
    {
        public static TutorialFightManager instance;

        public bool isInTutorialCombat = false;
        public CombatTutorialStep currentStep = CombatTutorialStep.ExplainBattery;
        
        public FightTutorialTextData tutorialText;

        [Header("UI Tuto")]
        public GameObject tutoPanel;
        public TextMeshProUGUI tutoText;
        public Button nextButton;
        
        public List<TutorialStepObjects> stepObjects;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            ShowCurrentStep();
            nextButton.onClick.AddListener(AdvanceStep);
        }
        
        private int _lastAdvanceFrame;
        public void AdvanceStep()
        {
            
            if (Time.frameCount == _lastAdvanceFrame) return;
            _lastAdvanceFrame = Time.frameCount;
            Debug.Log("=== AdvanceStep Called === From step: " + currentStep);
            
            if (currentStep >= CombatTutorialStep.Finished)
            {
                EndTutorial();
                return;
            }

            CombatTutorialStep nextStep = currentStep + 1;

            if (nextStep < CombatTutorialStep.Finished && StepRequiresPlayerAction(nextStep))
            {
                Debug.Log("New current State" + nextStep);
                currentStep = nextStep;
                ShowCurrentStep();
                return;
            }
            Debug.Log("Gros caca");
            currentStep = nextStep;
            ShowCurrentStep();
        }


        private bool StepRequiresPlayerAction(CombatTutorialStep step)
        {
            foreach (var stepObj in stepObjects)
            {
                if (stepObj.step == step)
                    return stepObj.requiresPlayerAction;
            }
            return false;
        }


        public void ShowCurrentStep()
        {
            if (!isInTutorialCombat)
            {
                tutoPanel.SetActive(false);
                return;
            }

            tutoPanel.SetActive(true);
            string message = "";

            switch (currentStep)
            {
                case CombatTutorialStep.ExplainBattery:
                    message = tutorialText.explainBattery;
                    break;
                case CombatTutorialStep.ExplainEnemyOscillation:
                    message = tutorialText.explainEnemyOscillation;
                    break;
                case CombatTutorialStep.ExplainAMFM:
                    message = tutorialText.explainAMFM;
                    break;
                case CombatTutorialStep.ExplainRadioSlider:
                    message = tutorialText.explainRadioSlider;
                    break;
                case CombatTutorialStep.ExplainLockFM:
                    message = tutorialText.explainLockFM;
                    break;
                case CombatTutorialStep.ExplainPlayerOscillation:
                    message = tutorialText.explainPlayerOscillation;
                    break;
                case CombatTutorialStep.ExplainPlayButton:
                    message = tutorialText.explainPlayButton;
                    break;
                case CombatTutorialStep.Finished:
                    message = tutorialText.finished;
                    break;
            }

            tutoText.text = message;
            UpdateStepObjects();
        }

        private void UpdateStepObjects()
        {
            bool foundCurrentStep = false;

            foreach (TutorialStepObjects stepObj in stepObjects)
            {
                bool isCurrentStep = (stepObj.step == currentStep);

                if (!isCurrentStep) continue;
                foundCurrentStep = true;
                nextButton.gameObject.SetActive(!stepObj.requiresPlayerAction);
                    
                foreach (var element in stepObj.elementsToEnable)
                {
                    if (element != null)
                    {
                        element.interactable = true;
                    }
                }

                foreach (var element in stepObj.elementsToDisable)
                {
                    if (element != null)
                    {
                        element.interactable = false;
                    }
                }
            }

            if (!foundCurrentStep)
            {
                nextButton.gameObject.SetActive(false);
            }
        }

        
        private void EndTutorial()
        {
            isInTutorialCombat = false;
            tutoPanel.SetActive(false);
            Debug.Log("Tutoriel terminé !");
        }
        
    }

    public enum CombatTutorialStep
    {
        ExplainBattery,
        ExplainEnemyOscillation,
        ExplainAMFM,
        ExplainRadioSlider,
        ExplainLockFM,
        ExplainPlayerOscillation,
        ExplainPlayButton,
        Finished
    }
    
    [System.Serializable]
    public struct TutorialStepObjects
    {
        public CombatTutorialStep step;
        [Header("UI Elements")]
        public Selectable[] elementsToEnable;
        public Selectable[] elementsToDisable;
        [Header("Step Behavior")] 
        public bool requiresPlayerAction; 
    }
    
    
}