using System;
using System.Collections;
using Controller;
using INTERACT;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MANAGER
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;
        public GlobalVolumeManager globalVolumeManager;
        
        [SerializeField] private LooseScreenController looseScreenController;
    
        public GameState currentGameState = GameState.Menu;

        [SerializeField] private GameObject prefabSoundManager;

        public LooseScreenController GetLooseScreen()
        {
            return looseScreenController != null ? looseScreenController : null;
        }        
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

        private void Start()
        {
            CurrentGameState = GameState.Game;
            globalVolumeManager = GetComponentInChildren<GlobalVolumeManager>();
        }

        public GameState CurrentGameState
        {
            get => currentGameState;
            set
            {
                currentGameState = value; 
                
                switch (value)
                {
                    case GameState.GameOver:
                        looseScreenController.looseScreenPanel.SetActive(true);
                        break;
                    case GameState.Game:
                    case GameState.Menu:
                        looseScreenController.looseScreenPanel.SetActive(false);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(value), value, null);
                }
            }
        }
    
        public enum GameState
        {
            Menu,
            Game,
            GameOver,
        }

        public void GameOver()
        {
            CurrentGameState = GameState.GameOver;
        }

        public void ResetPlayer()
        {
            NewPlayerController.instance.InitData();
            NewPlayerController.instance.animatorPlayer.Play("IdlePlayer");
            NewPlayerController.instance.transform.position = NewPlayerController.instance.spawnPosition;
            FightManager.instance.fightState = FightManager.FightState.OutFight; // so under good
            NewRadioManager.instance.UpdateTypeOfUiByFightState(); //should correct error when die in fight ?
            StartCoroutine(NewRadioManager.instance.HandleRadioTransition(new WaveSettings(0,0,0)));
            NewRadioManager.instance.ResetLights();
            StartCoroutine(ResetSoundManager());
            NewPlayerController.instance.rangeFinderManager.TurnRangeFinder(true);
            NewPlayerController.instance.rangeFinderManager.rfAnimation.animatorRangeFinder.Play("RfIdle");
            NewRadioManager.instance.UpdateTypeOfUiByFightState();
            StartCoroutine(NewRadioManager.instance.RadioBehaviorDependingFightState());
        }

        private IEnumerator ResetSoundManager()
        {
            yield return null;
            
            foreach (Transform sound in SoundManager.instance.transform)
            {
                switch (sound.gameObject.name)
                {
                    case "FightSound":
                        FightManager.instance.soundForFight = sound.gameObject;
                        break;
                    case "EnemyBreath":
                        FightManager.instance.soundEnemyInFight = sound.gameObject;
                        break;
                }
                sound.gameObject.SetActive(false);
            }
            SoundManager.instance.InitSoundBlanc();
        }
        
        public void FullScreen(bool toggled)
        {
            if (toggled) Screen.fullScreen = true;  
            else Screen.fullScreen = false;  
        }
    }
}

