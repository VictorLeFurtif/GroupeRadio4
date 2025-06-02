using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LooseScreenController : MonoBehaviour
{
    [SerializeField] private GameObject looseScreenPanel;

    private void Awake()
    {
        looseScreenPanel.SetActive(false);
    }

    private void FixedUpdate()
    {
        if (GameManager.instance.currentGameState.Equals(GameManager.GameState.GameOver))
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        looseScreenPanel.SetActive(true);
    }

    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Menu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
