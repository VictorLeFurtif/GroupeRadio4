using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PlayerData : ScriptableObject
{
    [field: Header("player health"), SerializeField]
    public int Health { get; private set; }
    
    public PlayerDataInstance Instance()
    {
        return new PlayerDataInstance(this);
    }
}

public class PlayerDataInstance
{
    public int health;
    
    public bool IsDead()
    {
        return health <= 0;
    }
    
    public PlayerDataInstance(PlayerData data)
    {
        health = data.Health;
    }
}