using System;
using System.Collections;
using System.Collections.Generic;
using Controller;
using UnityEngine;
using Random = UnityEngine.Random;

public class SoundController : MonoBehaviour
{
    [Header("Waiting time between sound(s) : Range for random time span")]
   [SerializeField] private float min;
   [SerializeField] private float max;
   [Header("Minimum Distance with Player before playing sound(s)")]
   [SerializeField] private float minDistance;
    [Header("Sound bank")]
   [SerializeField] private List<AudioClip> audioClips;

   private bool isAlreadyPlaying;
   private AudioSource audioSource;

   private void Awake()
   {
       audioSource = GetComponent<AudioSource>();
   }

    private bool IsCloseEnoughPlayer()
    {
        Vector3 playerPosition = PlayerController.instance.transform.position;
        float distance = Vector3.Distance(playerPosition, transform.position);
        return distance < minDistance;
    }

    private void FixedUpdate()
    {
        if (!isAlreadyPlaying && IsCloseEnoughPlayer())
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
        isAlreadyPlaying = false;
    }
}
