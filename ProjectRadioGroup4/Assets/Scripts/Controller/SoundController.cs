using System;
using System.Collections;
using System.Collections.Generic;
using Controller;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class SoundController : MonoBehaviour
{
    [Header("Waiting time between sound(s) : Range for random time span")]
   [SerializeField] private float min;
   [SerializeField] private float max;
   [Header("Minimum Distance with Player before playing sound(s)")]
   [SerializeField] private float minDistance;
   [SerializeField] private bool LoopSoundOnFinish;

   [Header("Spatialization")] 
   [SerializeField] private bool volumeBasedOnDistance;
   [SerializeField] [Range(0f,1f)] private float Volume;
   [SerializeField] [Range(0f,1f)] private float dynamicStereoRange;
   
   
    [Header("Sound bank")]
   [SerializeField] private List<AudioClip> audioClips;

   private bool isAlreadyPlaying;
   float distanceWithPlayer; //does not seem to change
   private AudioSource audioSource;

   private void Awake()
   {
       audioSource = GetComponent<AudioSource>();
   }

    private bool IsPlayerCloseEnough()
    {
        Vector3 playerPosition = PlayerController.instance.transform.position;
        distanceWithPlayer = Vector3.Distance(playerPosition, transform.position);
        return distanceWithPlayer < minDistance;
    }

    private void FixedUpdate()
    {
        if (volumeBasedOnDistance) VolumeBasedOnDistance();
        if (dynamicStereoRange != 0) StereoPan();
        if (!isAlreadyPlaying && IsPlayerCloseEnough())
        {
            StartCoroutine(PlaySound());
        }
    }

    private IEnumerator PlaySound()
    {
        isAlreadyPlaying = true;
        float nextPlayTime = Random.Range(min, max);
        yield return new WaitForSeconds(nextPlayTime);
        AudioClip randomClip = audioClips[Random.Range(0, audioClips.Count)];
        audioSource.clip = randomClip;
        audioSource.Play();
        if (LoopSoundOnFinish) // loop even when false ??
        {
            while (true)
            {
                yield return new WaitForSeconds(audioSource.clip.length);
                audioSource.Play();
            }
        }
        audioSource.Play();
        yield return new WaitForSeconds(audioSource.clip.length);
        isAlreadyPlaying = false;
    }

    private void VolumeBasedOnDistance()
    {
        Vector3 playerPosition = PlayerController.instance.transform.position;
        distanceWithPlayer = Vector3.Distance(playerPosition, transform.position);
        float unModifiedVolume = minDistance - (distanceWithPlayer / minDistance);
        audioSource.volume = unModifiedVolume * Volume;
        Debug.Log(distanceWithPlayer); // only return "(object)"
    }

    private void StereoPan() // I don't have any idea about how to make this work
    {
        Vector3 playerPosition = PlayerController.instance.transform.position;
        distanceWithPlayer = Vector3.Distance(playerPosition, transform.position);
    }
    
}
