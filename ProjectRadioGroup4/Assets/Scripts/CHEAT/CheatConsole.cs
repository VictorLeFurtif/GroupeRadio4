using System;
using MANAGER;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CHEAT
{
    public class CheatConsole : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject consolePanel;
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private TMP_Text feedbackText;

        public static CheatConsole instance;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                ToggleConsole();
            }

            if (consolePanel.activeSelf && Input.GetKeyDown(KeyCode.Return) && !string.IsNullOrEmpty(inputField.text))
            {
                ExecuteCommand(inputField.text);
                inputField.text = "";
                inputField.ActivateInputField();
            }
        }

        private void ToggleConsole()
        {
            consolePanel.SetActive(!consolePanel.activeSelf);
            if (consolePanel.activeSelf)
            {
                inputField.ActivateInputField();
                Time.timeScale = 0f; 
            }
            else
            {
                Time.timeScale = 1f; 
            }
        }

        private void ExecuteCommand(string command)
        {
            command = command.Trim().ToLower();
            string[] parts = command.Split(' ');
            
            try
            {
                switch (parts[0])
                {
                    case "tp":
                        if (parts.Length > 1) TeleportTo(parts[1]);
                        else ShowFeedback("Syntaxe: tp [index_scène]");
                        break;
                        
                    case "heal":
                        HealPlayer(parts.Length > 1 ? int.Parse(parts[1]) : 100);
                        break;
                        
                    case "killenemies":
                        KillAllEnemies();
                        break;
                        
                    case "kill":
                        KillPlayer();
                        break;
                        
                    case "killenemy":
                        KillCurrentEnemy();
                        break;
                        
                    case "help":
                        ShowHelp();
                        break;
                        
                    default:
                        ShowFeedback($"Commande inconnue: {command}");
                        break;
                }
            }
            catch (System.Exception e)
            {
                ShowFeedback($"Erreur: {e.Message}");
                Debug.LogError($"Cheat Error: {e}");
            }
        }

        private void ShowHelp()
        {
            string helpText = "Commandes disponibles:\n" +
                             "tp [index] de 0 à 5- Téléportation\n" +
                             "heal Player - Soin\n" +
                             "killenemies - Tous éliminer\n" +
                             "killenemy - Ennemi courant en combat\n" +
                             "kill - Auto-destruction";
            ShowFeedback(helpText);
        }

        private void TeleportTo(string destination)
        {
            if (!int.TryParse(destination, out int sceneIndex))
            {
                ShowFeedback("Index de scène doit être un nombre");
                return;
            }

            if (sceneIndex < 0 || sceneIndex >= SceneManager.sceneCountInBuildSettings)
            {
                ShowFeedback($"Index invalide (0-{SceneManager.sceneCountInBuildSettings - 1})");
                return;
            }

            SceneManager.LoadScene(sceneIndex);
            ShowFeedback($"Téléporté à la scène {sceneIndex}");
        }

        private void HealPlayer(int amount = 100)
        {
            if (NewPlayerController.instance == null)
            {
                ShowFeedback("Joueur introuvable");
                return;
            }

            NewPlayerController.instance.ManageLife(amount);
            ShowFeedback($"Soigné de {amount} HP");
        }

        private void KillAllEnemies()
        {
            if (NewRadioManager.instance == null || NewRadioManager.instance.listOfEveryEnemy.Count == 0)
            {
                ShowFeedback("Aucun ennemi trouvé");
                return;
            }

            int count = NewRadioManager.instance.listOfEveryEnemy.Count;
            foreach (var enemy in NewRadioManager.instance.listOfEveryEnemy)
            {
                if (enemy != null) Destroy(enemy.gameObject);
            }
            NewRadioManager.instance.listOfEveryEnemy.Clear();
            
            ShowFeedback($"{count} ennemis éliminés");
        }
        
        private void KillCurrentEnemy()
        {
            if (FightManager.instance == null || FightManager.instance.GetCurrentEnemy() == null)
            {
                ShowFeedback("Aucun ennemi en combat");
                return;
            }

            var enemy = FightManager.instance.GetCurrentEnemy();
            enemy.PvEnemy = 0;
            ShowFeedback("Ennemi courant éliminé");
        }

        private void KillPlayer()
        {
            if (NewPlayerController.instance == null)
            {
                ShowFeedback("Joueur introuvable");
                return;
            }

            NewPlayerController.instance.ManageLife(-200);
            ShowFeedback("Auto-destruction activée");
        }

        private void ShowFeedback(string message)
        {
            feedbackText.text = message;
            Debug.Log($"[CHEAT] {message}");
        }
    }
}