using System;
using DATA.Script.Sound_Data;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MANAGER
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager instance { get; private set; }
        
        public AudioSource audioSource;

        [Range(0,1)] public float sfxVolumeSlider = 1;
        public float sfxVolume; //after being calculated with the general volume
        
        [Range(0,1)] public float vgmVolumeSlider = 1;
        public float vgmVolume; //after being calculated with the general volume
        
        [Header("Slider References")]
        [SerializeField] private Slider sliderAll;
        [SerializeField] private Slider sliderSfx;
        [SerializeField] private Slider sliderVgm;

        
        [SerializeField] private SoundBankData soundBankDataBrut;
        public SoundBankDataInstance soundBankData;
        public GameObject soundBlanc;
        
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            
            else
                Destroy(gameObject);
            
            sliderAll.value = audioSource.volume;
            sliderSfx.value = sfxVolumeSlider;
            sliderVgm.value = vgmVolumeSlider;
        }
        
        private void Start()
        {
            soundBankData = soundBankDataBrut.Instance();
            InitSoundBlanc();
        }

        public void InitSoundBlanc()
        {
            if (soundBlanc == null)
            {
                soundBlanc = InitialisationAudioObjectDestroyAtEnd
                (soundBankData.enviroSound.whiteNoiseVentilation,
                    true, true, 1f, "Main Sound");
            }
            else
            {
                soundBlanc.SetActive(true);
            }
        }
        
        private void Update()
        {
            sfxVolume = sfxVolumeSlider;
            sfxVolume *= audioSource.volume;
           
            vgmVolume = vgmVolumeSlider;
            vgmVolume *= audioSource.volume;
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
        
        public void UpdateVgmVolumeSlider(float sliderValue)
        {
            vgmVolumeSlider = sliderValue;
        }
    }
}