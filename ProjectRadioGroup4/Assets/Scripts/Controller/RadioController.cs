using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class RadioController : MonoBehaviour
{
    public static RadioController instance;
    [Header("Canvas")]
    [SerializeField] private Canvas canvaRadio;

    [Header("Frequence Radio")] [SerializeField]
    private RawImage imageRadio;
    private Material mat;
    
    
    
    
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
    
}
