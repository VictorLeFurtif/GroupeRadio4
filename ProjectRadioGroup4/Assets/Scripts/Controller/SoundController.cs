using System;
using System.Collections;
using System.Collections.Generic;
using Controller;
using MANAGER;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class SoundController : MonoBehaviour
{ 
    [SerializeField] [Range(0,1)] private float localVolume = 1;
    
    [Header("Waiting time between sound(s) :\nRange for random time span")]
    [SerializeField] private float min;
    [SerializeField] private float max;
   
    [Header("! Waiting times will be ignored if enabled !")]
    [SerializeField] private bool LoopImmediately;
    [SerializeField] private bool PlaySoundOnlyOnce;
   
    [Header("Sound will keep playing at volume 0 so\nit doesn't start all over when player come close again\n( ! Expensive Method ! )")]
    [SerializeField] private bool DontStopOnExit;
   
    [Header("Will wait for current sound to end\nbefore starting countdown for next sound")]
    [SerializeField] private bool waitForFinish;
   
    [Header("Sound bank")]
    [SerializeField] private List<AudioClip> audioClips;

    private bool CanReplay = true;
    private float distanceWithPlayer;
    private AudioSource audioSource;

    private void Awake()
    {
        if (NewPlayerController.instance == null)
        {
            Debug.LogError("PlayerController missing !");
            enabled = false;
            return;
        }
        
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("AudioSource component missing !");
            enabled = false;
            return;
        }
    }

    private void Update()
    {
        if (NewPlayerController.instance == null || audioSource == null || audioClips == null || audioClips.Count == 0)
            return;

        distanceWithPlayer = Vector3.Distance(NewPlayerController.instance.transform.position, transform.position);
        SetVolume();

        if (DontStopOnExit)
        {
            if (CanReplay && IsPlayerCloseEnough() && !audioSource.isPlaying)
            {
                StartCoroutine(PlaySound());
            }
            else if (!audioSource.isPlaying && !PlaySoundOnlyOnce)
            {
                CanReplay = true;
            }
        }
        else
        {
            if (CanReplay && IsPlayerCloseEnough())
            {
                StartCoroutine(PlaySound());
            }
            else if (!IsPlayerCloseEnough() && audioSource.isPlaying)
            {
                audioSource.Pause();
                if (!PlaySoundOnlyOnce)
                {
                    CanReplay = true;
                }
            }
        }
    }

    private bool IsPlayerCloseEnough()
    {
        return distanceWithPlayer < audioSource.maxDistance;
    } 

    private void SetVolume()
    {
        if (SoundManager.instance == null)
            return;

        float distanceVolume = 1 - (distanceWithPlayer / audioSource.maxDistance);
        float sfxVolume = SoundManager.instance.sfxVolume;
        
        audioSource.volume = distanceVolume * localVolume * sfxVolume;
    }
    
    private IEnumerator PlaySound()
    {
        if (audioClips == null || audioClips.Count == 0)
            yield break;

        CanReplay = false;
        
        float nextPlayTime = Random.Range(min, max);
        yield return new WaitForSeconds(nextPlayTime);
        
        AudioClip randomClip = audioClips[Random.Range(0, audioClips.Count)];
        if (randomClip == null)
            yield break;
            
        audioSource.clip = randomClip;
        
        if (LoopImmediately)
        {
            while (IsPlayerCloseEnough() && audioSource != null)
            {
                audioSource.Play();
                yield return new WaitForSeconds(audioSource.clip.length);
            }
        }
        else
        {
            audioSource.Play();
            if (waitForFinish)
            {
                yield return new WaitForSeconds(audioSource.clip.length);
            }
            if (!PlaySoundOnlyOnce)
            {
                CanReplay = true;
            }
        }
    }
}