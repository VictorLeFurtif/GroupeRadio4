using System;
using System.Collections;
using System.Collections.Generic;
using Controller;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class SoundController : MonoBehaviour
{
    [Header("Waiting time between sound(s) :\nRange for random time span")]
   [SerializeField] private float min;
   [SerializeField] private float max;
   [SerializeField] private bool PlaySoundOnlyOnce;
   [Header("Will wait for current sound to end\nbefore starting countdown for next sound")]
   [SerializeField] private bool waitForFinish;
   [Header("Waiting times will be ignored if enabled")]
   [SerializeField] private bool LoopImmediately;
   

    [Header("Sound bank")]
   [SerializeField] private List<AudioClip> audioClips;

   private bool CanReplay = true;
   private float distanceWithPlayer;
   private AudioSource audioSource;

    private void Awake()
    {
       audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (CanReplay && IsPlayerCloseEnough())
        {
            StartCoroutine(PlaySound());
        }
        else if (!IsPlayerCloseEnough() && audioSource.isPlaying)
        {
            audioSource.Stop();
            if (!PlaySoundOnlyOnce)
            {
                CanReplay = true;
            }
        }
    }

    private bool IsPlayerCloseEnough()
    {
        if (PlayerController.instance == null) return false;
        
        Vector3 playerPosition = PlayerController.instance.transform.position;
        distanceWithPlayer = Vector3.Distance(playerPosition, transform.position);
        return distanceWithPlayer < audioSource.maxDistance;
    }
    
    private IEnumerator PlaySound()
    {
        CanReplay = false;
        float nextPlayTime = Random.Range(min, max);
        yield return new WaitForSeconds(nextPlayTime);
        AudioClip randomClip = audioClips[Random.Range(0, audioClips.Count)];
        audioSource.clip = randomClip;
        if (LoopImmediately)
        {
            while (IsPlayerCloseEnough())
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
