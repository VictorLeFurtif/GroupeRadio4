using System;
using DATA.Script.Sound_Data;
using UnityEngine;

namespace MANAGER
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager instance { get; private set; }
        
        [SerializeField] private AudioSource audioSource;

        [SerializeField]
        private SoundBankData soundBankDataBrut;

        public SoundBankDataInstance soundBankData;
        
        
        private void Awake()
        {
            if (instance == null)
                instance = this;
            else
                Destroy(this.gameObject);
        }
        
        private void Start()
        {
            soundBankData = soundBankDataBrut.Instance();
            InitialisationAudioObjectDestroyAtEnd(soundBankData.enviroSound.whiteNoiseVentilation,true,true,1f);
        }

        public void PlayMusicOneShot(AudioClip _audioClip)
        {
            if (_audioClip == null)
            {
                Debug.LogError("The audioClip you tried to play is null");
                return;
            }
            audioSource.PlayOneShot(_audioClip);
        }
        
        //horror in kind of optimisation but cool for music general
        private void InitialisationAudioObjectDestroyAtEnd(AudioClip audioClipTarget, bool looping, bool playingAwake, float volumeSound)
        {
            GameObject emptyObject = new GameObject
            {
                name = "Sound Effect"
            };
            
            AudioSource audioSourceGeneral = emptyObject.AddComponent<AudioSource>();
            audioSourceGeneral.clip = audioClipTarget;
            audioSourceGeneral.loop = looping;
            audioSourceGeneral.playOnAwake = playingAwake;
            audioSourceGeneral.Play();
            audioSourceGeneral.volume = volumeSound;
            Destroy(emptyObject,audioClipTarget.length);
        }
    }
}