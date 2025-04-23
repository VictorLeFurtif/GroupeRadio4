using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    public GameState currentGameState = GameState.Menu;
    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }
    public enum GameState
    {
        Menu,
        Game,
        GameOver,
    }

    public void GameOver()
    {
        currentGameState = GameState.GameOver;
    }
}

