using System;
using System.Collections;
using System.Collections.Generic;
using Controller;
using DATA.Script.Entity_Data.AI;
using DATA.Script.Entity_Data.Player;
using DG.Tweening;
using INTERACT;
using INTERFACE;
using MANAGER;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class NewPlayerController : MonoBehaviour
{
    #region Singleton
    public static NewPlayerController instance;
    #endregion

    #region Fields
    [Header("Speed")]
    [SerializeField] private float moveSpeed;
    public bool canMove = true;

    [Header("Rigidbody2D")]
    public Rigidbody2D rb;

    [Header("Data Player")]
    [SerializeField] private AbstractEntityData _abstractEntityData;
    public AbstractEntityDataInstance _abstractEntityDataInstance;
    public PlayerDataInstance _inGameData;

    public SpriteRenderer spriteRendererPlayer;

    [Header("State Machine")]
    public Phase2Module currentPhase2ModuleState = Phase2Module.Off;

    [Header("Animation")]
    public Animator animatorPlayer;
    
    [Header("Battery")] 
    private BatteryPlayer playerBattery;
    
    [SerializeField]
    private float epsilonValidationOscillation;
    
    private float nextFootstepTime;
    
    [Header("LIST")] 
    public List<IInteractable> ListOfEveryElementInteractables = new List<IInteractable>();
    
    [Header("Scan Settings")]
    public ScanType currentScanType = ScanType.None;
    
    [Header("Phase 2 Module UI")]
    [SerializeField] private Button phase2Button;
    [SerializeField] private Image buttonImage;
    [SerializeField] private Color disabledColor = Color.gray;

    [SerializeField] private bool canTurnOnPhase2Module; 
    
    [Header("Current Selector")] 
    public IInteractable currentInteractableInRange = null;

    public RangeFinderManager rangeFinderManager;
    
    [Header("Life Drain By Scan")] 
    
    [SerializeField] private float lifeTakeWeak;
    [SerializeField] private float lifeTakenMid;
    [SerializeField] private float lifeTakenStrong;

    [Header("Damage")] [SerializeField] private float maxDamageDone;

    [HideInInspector] public ChipsManager chipsManager;
    #endregion

    #region Enums
    public enum ScanType
    {
        Type1 = 0, 
        Type2 = 1, 
        Type3 = 2,
        None = 3
    }
    
    public enum Phase2Module
    {
        On,
        Off
    }
    #endregion

    #region Unity Methods
    private void Awake()
    {
        Init();
    }
    
    private void Update()
    {
        PlayerMove();
        CheckForFlipX();
    }
    #endregion

    #region Initialization
    private void Init()
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
            

        rb = GetComponent<Rigidbody2D>();
        CanTurnOnPhase2Module = false;
        _abstractEntityDataInstance = _abstractEntityData.Instance(gameObject);
        _inGameData = (PlayerDataInstance)_abstractEntityDataInstance;

        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        spriteRendererPlayer = GetComponent<SpriteRenderer>();
        animatorPlayer = GetComponent<Animator>();
        UpdatePhase2ButtonState();
        playerBattery = GetComponent<BatteryPlayer>();
        rangeFinderManager = GetComponent<RangeFinderManager>();
        currentScanType = ScanType.None;
        chipsManager = GetComponent<ChipsManager>();
    }
    #endregion

    #region Life Management & Getter Setter
    public void ManageLife(float valueLifeChanger) 
    {
        HealthPlayer += valueLifeChanger;
    }
    
    private float HealthPlayer 
    {
        get => _inGameData?.hp ?? 0f;
        set
        {
            if (_inGameData == null)
            {
                Debug.LogError(" _inGameData est NULL !");
                return;
            }
            
            _inGameData.hp = Mathf.Max(0, value);
            _inGameData.hp = Mathf.Clamp(value, 0, 100);
            
            playerBattery.UpdateLifeSlider(_inGameData.hp);
            
            if (_inGameData.IsDead())
            {
                canMove = false;
                rb.velocity = Vector2.zero;
                animatorPlayer.Play("Death");
                StartCoroutine(PlayGameOverAfterDeath());
                
                return;
            }
            
            if (FightManager.instance.fightState is FightManager.FightState.InFight)
            {
                animatorPlayer.Play("HitReceived");
            }
            
        }
    }

    IEnumerator PlayGameOverAfterDeath()
    {
        float timeToWait = animatorPlayer.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(timeToWait + 0.5f);
        GameManager.instance.GameOver();
    }

    public float GetPlayerDamage()
    {
        return maxDamageDone;
    }
    
    #endregion
    
    #region Movement
    private void PlayerMove()
    {
        if (!canMove)
        {
            rb.velocity = Vector2.zero;
            animatorPlayer.SetFloat("MoveSpeed", 0);
            return;
        }

        float x = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(x * moveSpeed, rb.velocity.y);
        animatorPlayer.SetFloat("MoveSpeed", Mathf.Abs(rb.velocity.x));

        if (Mathf.Abs(x) > 0.1f && Time.time > nextFootstepTime)
        {
            SoundManager.instance?.PlayMusicOneShot(SoundManager.instance.soundBankData.avatarSound.walk);
            nextFootstepTime = Time.time + 0.35f;
        }
    }

    private bool wasFacingLeft = false;

    private void CheckForFlipX()
    {
        if (Mathf.Abs(rb.velocity.x) > 0.1f)
            wasFacingLeft = rb.velocity.x < 0;

        spriteRendererPlayer.flipX = wasFacingLeft;
    }

    public void ChangeBoolPlayerCanMove(float active)
    {
        canMove = active > 0.5f;
    }
    #endregion

    #region Scanning
    private void Scan(ScanType scanType, float damageDealToPlayer)
    {
        if (FightManager.instance?.fightState == FightManager.FightState.InFight || _inGameData.IsDead() || NewRadioManager.instance.isMatching)
        {
            return;
        }
        animatorPlayer.Play("ScanAround");
        currentScanType = scanType;
        foreach (var interactable in ListOfEveryElementInteractables)
        {
            interactable.OnScan();
        }
        ManageLife(-damageDealToPlayer);
        rangeFinderManager.UpdateUiRangeFinder();
    }

    
    
    
    public void ScanWeak() => Scan(ScanType.Type3,lifeTakeWeak);
    public void ScanMid() => Scan(ScanType.Type2,lifeTakenMid);
    public void ScanStrong() => Scan(ScanType.Type1,lifeTakenStrong);
    #endregion

    #region Phase 2 Module
    public void SwitchRadioPhaseTwo()
    {
        if (!CanTurnOnPhase2Module || currentInteractableInRange is not { CanSecondPhase: true }) return;

        if (currentInteractableInRange is not IWaveInteractable waveInteractable) return;
        switch (currentPhase2ModuleState)
        {
            case Phase2Module.Off when waveInteractable.CanBeActivated():
                canMove = false;
                currentPhase2ModuleState = Phase2Module.On;
                NewRadioManager.instance.StartMatchingGameOutFight();
                break;
            case Phase2Module.On:
                canMove = true;
                waveInteractable.CanSecondPhase = false;
                currentPhase2ModuleState = Phase2Module.Off;
                NewRadioManager.instance.StopMatchingGame();
                waveInteractable.MarkAsUsed();
                break;
        }
    }

    public bool CanTurnOnPhase2Module
    {
        get => canTurnOnPhase2Module;
        set
        {
            if (canTurnOnPhase2Module != value)
            {
                canTurnOnPhase2Module = value;
                UpdatePhase2ButtonState(); 
            }
        }
    }
    
    private void UpdatePhase2ButtonState()
    {
        if (phase2Button == null) return;
        phase2Button.interactable = CanTurnOnPhase2Module;
        if (buttonImage != null)
            buttonImage.color = CanTurnOnPhase2Module ? Color.white : disabledColor;
    }
    #endregion

    #region Interaction
    [Header("Interaction")]
    public bool canInteract = true;
    
    #endregion

    #region Fight

    public void OnMatch()
    {
        FightManager.instance?.OnMatchButtonPressed();
    }

    public void OnReverse()
    {
        FightManager.instance?.OnReverseButtonPressed();
    }

    #endregion
}