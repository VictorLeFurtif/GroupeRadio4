using System;
using System.Collections;
using Controller;
using UnityEngine;
using Random = UnityEngine.Random;

public class SoundController : MonoBehaviour
{
    [Header("WaitBeforePlaying Range")]
   [SerializeField] private float min;
   [SerializeField] private float max;
   [Header("Minimum Distance with Player before playing")]
   [SerializeField] private float minDistance;

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
        audioSource.Play();
        isAlreadyPlaying = false;
    }
}
