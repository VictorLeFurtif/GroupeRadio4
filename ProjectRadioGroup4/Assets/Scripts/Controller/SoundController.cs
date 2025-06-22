using System.Collections;
using System.Collections.Generic;
using MANAGER;
using UnityEngine;

namespace Controller
{
    public class SoundController : MonoBehaviour
    { 
        [SerializeField] [Range(0,1)] private float localVolume = 1;
        [SerializeField] private float min;
        [SerializeField] private float max;
        [SerializeField] private bool LoopImmediately;
        [SerializeField] private bool PlaySoundOnlyOnce;
        [SerializeField] private bool DontStopOnExit;
        [SerializeField] private bool waitForFinish;
        [SerializeField] private List<AudioClip> audioClips;

        private bool CanReplay = true;
        private float distanceWithPlayer;
        private AudioSource audioSource;

        private void Awake()
        {
            if (NewPlayerController.instance == null)
            {
                Debug.LogError("PlayerController missing!");
                enabled = false;
                return;
            }
        
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                Debug.LogError("AudioSource component missing!");
                enabled = false;
                return;
            }
        }

        private void Update()
        {
            if (NewPlayerController.instance == null || audioSource == null || audioClips == null || audioClips.Count == 0)
                return;

            distanceWithPlayer = Vector2.Distance(NewPlayerController.instance.transform.position, transform.position);
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
            audioSource.volume = distanceVolume * localVolume * SoundManager.instance.masterVolume;
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
}