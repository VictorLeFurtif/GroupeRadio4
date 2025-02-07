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
    [SerializeField] private PlayerDataInstance inGameData; 
    
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

    public int healthPlayer // C'est celle là que l'on manipule pour que la vie s'Update
    {
        get => inGameData.health;

        set
        {
            inGameData.health = value;
            if (inGameData.IsDead())
            {
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
        var x = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(x  * moveSpeed,rb.velocity.y);
    }
}
