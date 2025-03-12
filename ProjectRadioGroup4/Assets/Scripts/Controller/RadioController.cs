using System;
using System.Collections;
using System.Collections.Generic;
using AI;
using Controller;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;


public class RadioController : MonoBehaviour
{
    public static RadioController instance;
    [Header("Canvas")]
    [SerializeField] private Canvas canvaRadio;

    [Header("Frequence Radio")] [SerializeField]
    private RawImage imageRadio;
    private Material mat;

    private RadioState currentRadioState;

    private AbstractAI currentClosestEnemy;
    
    [Header("Radio detection enemy parameters")]
    public List<AbstractAI> listOfEveryEnemy;

    [SerializeField] private float minDistance;
    [SerializeField] private float maxDistance;

    private void Update()
    {
        UpdateRadioUiInExploration();   
    }

    private enum RadioState
    {
        InFight,
        OutFight
    }

    private void Start()
    {
        currentRadioState = RadioState.OutFight;
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        mat = imageRadio.material;
    }

    public void AddWave(float i)
    {
        var value = mat.GetFloat("_waves_Amount");
        mat.SetFloat("_waves_Amount", value + i);
    }

    public void AddWaveAmp(float i)
    {
        var value = mat.GetFloat("_waves_Amp");
        mat.SetFloat("_waves_Amp",value + i);
    }
    
    //TODO: dans le futur sort la list en x sera beaucoup plus performant zebi ...
    
    private float CheckForClosestPlayer() 
    {
        float stockDistance = Mathf.Infinity;
        AbstractAI stockEntity = null;
        float playerX = PlayerController.instance.transform.position.x;

        foreach (AbstractAI enemy in listOfEveryEnemy) {
            float enemyX = enemy.transform.position.x;
            float distance = Mathf.Abs(enemyX - playerX); 
            if (distance < stockDistance) {
                stockDistance = distance;
                stockEntity = enemy;
            }
        }
        currentClosestEnemy = stockEntity;
        return stockDistance;
    }

    private void UpdateRadioUiInExploration()
    {
        if (listOfEveryEnemy.Count == 0 || currentRadioState == RadioState.InFight)
        {
           return; 
        }
        var distance = CheckForClosestPlayer();
        
        float waveAmp = CalculateWaveAmplitude(distance, minDistance,maxDistance);
        float waveAmount = CalculateWaveAmount(distance, minDistance,maxDistance);
        
        mat.SetFloat("_waves_Amp", waveAmp);
        mat.SetFloat("_waves_Amount", waveAmount);
    }
    
    private float CalculateWaveAmplitude(float distance, float _minDistance, float _maxDistance) {
        float normalizedDistance = Mathf.InverseLerp(_minDistance, _maxDistance, distance);
        return Mathf.Lerp(1f, 0.1f, normalizedDistance);
    }
    
    private float CalculateWaveAmount(float distance, float _minDistance, float _maxDistance) {
        float normalizedDistance = Mathf.InverseLerp(_minDistance, _maxDistance, distance);
        return Mathf.Lerp(20f, 1f, normalizedDistance);
    }
    
}
