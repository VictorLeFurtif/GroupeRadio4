using System;
using System.Collections;
using System.Collections.Generic;
using MANAGER;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    [SerializeField] private GameObject PausePanel;
    private bool isPanelDisplayed;

    private void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex != 0 && Input.GetKeyDown(KeyCode.Escape))
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
        SceneManager.LoadScene(0);
        GameManager.instance.ResetPlayer();
    }

    public void Quit()
    {
        Application.Quit();
    }
}
