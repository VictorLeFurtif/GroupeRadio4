using System;
using MANAGER;
using UnityEngine;
using UnityEngine.UI;

public class MasterVolumeHelper : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider;
    private SoundManager soundManager;

    private void Start()
    {
        
        soundManager = SoundManager.instance;
        
        if (soundManager == null)
        {
            Debug.LogError("SoundManager non trouvé dans la scène!");
            return;
        }
        
        if (volumeSlider == null)
        {
            volumeSlider = GetComponent<Slider>();
        }

        if (volumeSlider != null)
        {
            volumeSlider.value = soundManager.masterVolume;
            
            volumeSlider.onValueChanged.AddListener(UpdateMasterVolume);
        }
        else
        {
            Debug.LogError("Aucun Slider assigné ou trouvé sur ce GameObject!");
        }
    }
    

    private void UpdateMasterVolume(float newVolume)
    {
        if (soundManager != null)
        {
            soundManager.UpdateMasterVolume(newVolume);
            
        }
    }

    private void Update()
    {
        volumeSlider.value = soundManager.masterVolume;
        
    }

    private void OnDestroy()
    {
        if (volumeSlider != null)
        {
            volumeSlider.onValueChanged.RemoveListener(UpdateMasterVolume);
        }
    }
}