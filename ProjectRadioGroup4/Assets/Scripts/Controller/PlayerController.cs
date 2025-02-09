using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D),typeof(CapsuleCollider2D))]
public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;
    
    [Header("Speed")] [SerializeField] private float moveSpeed;

    [Header("Rigidbody2D")] [SerializeField]
    private Rigidbody2D rb;
    
    [Header("Data Player")]
    [SerializeField] private PlayerData playerData; 
    private PlayerDataInstance inGameData; 
    
    public enum PlayerState
    {
        Idle,
        Running,
        Walking,
        Dead
    }
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        inGameData = playerData.Instance();
    }

    public void ManageLife(int valueLifeChanger)
    {
        healthPlayer += valueLifeChanger;
    }
    
    public int healthPlayer 
    {
        get => inGameData.health;

        set
        {
            inGameData.health = value;
            if (inGameData.IsDead())
            {
                GameManager.instance.GameOver();
                //They lose
            }
        }
    }
    
    private void Update()
    {
        PlayerMove();
    }

    private void PlayerMove()
    {
        if (FightManager.instance.currentFighter != FightManager.FightState.NoFight) return;
        var x = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(x  * moveSpeed,rb.velocity.y);

    }
}
