using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class MainMenuUI : MonoBehaviour
{
    //[SerializeField] private Image newGameButtonImage;
    //[SerializeField] private Image continueButtonImage;
    //[SerializeField] private Image optionsButtonImage;
    //[SerializeField] private Image quitButtonImage;
    //[SerializeField] private Image onSelectNewGameButtonImage;
    //[SerializeField] private Image onSelectContinueButtonImage;
    //[SerializeField] private Image onSelectOptionsButtonImage;
    //[SerializeField] private Image onSelectQuitButtonImage;
    
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject continueScreenPanel;
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private TMP_Text completionPercentageText;
    [SerializeField] private TMP_Text playTimeText;
    private void Start()
    {
        ChangeWindowMode(true);
        //PlayTimeText = 
        //CompletionPercentageText = 
    }
    
    public void NewGame()
    {
        
    }

    public void Continue()
    {
        continueScreenPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
    }

    public void OpenOptions()
    {
        optionsPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
    }

    public void QuitOptions()
    {
        optionsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void QuitLoadScreen()
    {
        continueScreenPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void ChangeWindowMode(bool fullscreen)
    {
        if (fullscreen) Screen.fullScreen = true;
        if (!fullscreen) Screen.fullScreen = false;
    }
}
