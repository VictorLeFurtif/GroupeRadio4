using System;
using System.Collections;
using System.Collections.Generic;
using MANAGER;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour
{
    [SerializeField] private GameObject PausePanel;
    private bool isPanelDisplayed;
    [SerializeField] private Slider sliderAll;
    [SerializeField] private Slider sliderSfx;
    [SerializeField] private Slider sliderVgm;
    
    private void Awake()
    {
        sliderAll.value = SoundManager.instance.audioSource.volume;
        sliderSfx.value = SoundManager.instance.sfxVolumeSlider;
        sliderVgm.value = SoundManager.instance.vgmVolumeSlider;
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex != 0 && Input.GetKeyDown(KeyCode.Escape) && FightManager.instance.fightState is not FightManager.FightState.InFight)
        {
            ChangePanelState();
        }
        ShowPanel();
        SetTimeScale();
    }

    private void SetTimeScale()
    {
        if (isPanelDisplayed)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }

    public void ChangePanelState()
    {
        isPanelDisplayed = !isPanelDisplayed;
    }

    private void ShowPanel()
    {
         PausePanel.SetActive(isPanelDisplayed);
    }
    
    // In-PausePanel buttons

    public void Menu()
    {
        isPanelDisplayed = false;
        SceneManager.LoadScene("MainMenuNew");
        GameManager.instance.ResetPlayer();
    }

    public void Quit()
    {
        Application.Quit();
    }
}
