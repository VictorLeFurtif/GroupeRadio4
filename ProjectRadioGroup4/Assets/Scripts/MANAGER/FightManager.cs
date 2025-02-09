using System;
using System.Collections;
using UnityEngine;

public class FightManager : MonoBehaviour
{
    public FightState currentFighter = FightState.NoFight;
    public static FightManager instance; // besoin pour par exemple pour check si le joueur peu bouger, en combat cela sera non

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public enum FightState
    {
        Player,
        Ennemy,
        NoFight,
    }

    public void EndTurn()
    {
        currentFighter = currentFighter switch
        {
            FightState.Player => FightState.Ennemy,
            FightState.Ennemy => FightState.Player,
            _ => currentFighter
        };
    }


    
}