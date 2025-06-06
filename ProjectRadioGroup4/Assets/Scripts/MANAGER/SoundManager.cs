using System;
using DATA.Script.Sound_Data;
using UnityEngine;
using UnityEngine.UI;

namespace MANAGER
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager instance { get; private set; }
        
        [SerializeField] 
        private AudioSource audioSource;

        [Range(0,1)] public float sfxVolumeSlider = 1;
        public float sfxVolume; //after being calculated with the general volume
        [SerializeField]
        private SoundBankData soundBankDataBrut;

        public SoundBankDataInstance soundBankData;
        
        
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            
            else
                Destroy(gameObject);
        }
        
        private void Start()
        {
            soundBankData = soundBankDataBrut.Instance();
            InitialisationAudioObjectDestroyAtEnd(soundBankData.enviroSound.whiteNoiseVentilation,true,true,1f,"Main Sound");
        }

        private void Update()
        {
            sfxVolume = sfxVolumeSlider;
            sfxVolume *= audioSource.volume;
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
        public GameObject InitialisationAudioObjectDestroyAtEnd(AudioClip audioClipTarget, bool looping, bool playingAwake, float volumeSound, string _name)
        {
            GameObject emptyObject = new GameObject
            {
                name = _name
            };

            emptyObject.transform.SetParent(gameObject.transform);

            AudioSource audioSourceGeneral = emptyObject.AddComponent<AudioSource>();
            audioSourceGeneral.clip = audioClipTarget;
            audioSourceGeneral.loop = looping;
            audioSourceGeneral.playOnAwake = playingAwake;
            audioSourceGeneral.volume = volumeSound;
            audioSourceGeneral.Play();

            if (!looping)
            {
                Destroy(emptyObject, audioClipTarget.length);
            }

            return emptyObject;
        }

        public AudioSource GetAudioSourceFromSoundManager()
        {
            return audioSource;
        }

        public void UpdateSfxVolumeSlider(float sliderValue)
        {
            sfxVolumeSlider = sliderValue;
        }
    }
}