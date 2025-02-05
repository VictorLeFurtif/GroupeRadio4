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

    [Header("Frequence Radio")] 
    [SerializeField] private int frequencyRadio;

    [SerializeField] private Slider sliderHz;

    [SerializeField] private TMP_Text frequenceText;
    
    private bool isRadioOpen = false;
    
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
        canvaRadio.enabled = false;
        sliderHz.onValueChanged.AddListener(UpdateFrequencyFromSlider);
    }

    private void Update()
    {
       OpenClosedRadio();
       UpdateHzMouse();
       UpdateHzSlider();
    }
    
    private void UpdateFrequencyFromSlider(float value)
    {
        frequencyRadio = (int)value;
        UpdateUiHz();
    }

    
    private void OpenClosedRadio()
    {
        if (!Input.GetKeyDown(KeyCode.F)) return;
        switch (isRadioOpen)
        {
            case false : canvaRadio.enabled = true;
                isRadioOpen = true;
                break;
            case true : canvaRadio.enabled = false;
                isRadioOpen = false;
                break;
        } 
    }

    private void UpdateHzMouse()
    {
        if (!isRadioOpen) return;
        switch (Input.GetAxisRaw("Mouse ScrollWheel"))
        {
            case > 0 :
                frequencyRadio++;
                break;
            case < 0 :
                frequencyRadio--;
                break;
        }
        UpdateUiHz();
    }

    private void UpdateUiHz()
    {
        frequencyRadio = Mathf.Clamp(frequencyRadio, 20, 20000);
        frequenceText.text = frequencyRadio.ToString() + " Hz";
    }
    
    private void UpdateHzSlider()
    {
        if (!isRadioOpen) return;
        if (!Mathf.Approximately(sliderHz.value, frequencyRadio)) 
        {
            sliderHz.value = frequencyRadio;
        }
    }
}
