using UnityEngine;
using UnityEngine.UI;

namespace MANAGER
{
    public class NewRadioManager : MonoBehaviour
    {
        [Header("Shader Material Player")]
        [SerializeField] private RawImage imageRadioPlayer;
        [SerializeField] private RawImage imageRadioEnemy;
        
        private Material matRadioPlayer;
        private Material matRadioEnemy;

        [Header("Sliders")]
        [SerializeField] private Slider sliderAmplitude;
        [SerializeField] private Slider sliderFrequency;
        [SerializeField] private Slider sliderStep;

        [Header("Shader Property Limits")]
        [SerializeField] private float maxAmplitude = 0.4f;
        [SerializeField] private float maxFrequency = 1f;
        [SerializeField] private float maxStep = 1f;

        public static NewRadioManager instance;
        
        [Header("Canvas")] public Canvas canvaRadio;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            if (imageRadioPlayer != null)
            {
                matRadioPlayer = imageRadioPlayer.material;
                matRadioEnemy = imageRadioEnemy.material;
            }
                
        }

        private void Start()
        {
            InitializeSliders();
            SetOscillationTo0(matRadioEnemy,"_waves_Amount","_waves_Amp","_Step");
            SetOscillationTo0(matRadioPlayer,"_waves_Amount","_waves_Amp","_Step");
        }

        private void InitializeSliders()
        {
            if (sliderAmplitude != null)
            {
                sliderAmplitude.maxValue = maxAmplitude;
                sliderAmplitude.onValueChanged.AddListener((value) => UpdateOscillationParameters(value, "_waves_Amp"));
            }

            if (sliderFrequency != null)
            {
                sliderFrequency.maxValue = maxFrequency;
                sliderFrequency.onValueChanged.AddListener((value) => UpdateOscillationParameters(value, "_waves_Amount"));
            }

            if (sliderStep != null)
            {
                sliderStep.maxValue = maxStep;
                sliderStep.onValueChanged.AddListener((value) => UpdateOscillationParameters(value, "_Step"));
            }
        }

        private void UpdateOscillationParameters(float value,string nameParameters)
        {
            if (matRadioPlayer != null)
                matRadioPlayer.SetFloat(nameParameters, value);
        }
        
        private void SetOscillationTo0(Material targetMaterial,string waveAmount,string waveAmp,string step)
        {
            targetMaterial.SetFloat(waveAmount, 0);
            targetMaterial.SetFloat(waveAmp, 0);
            targetMaterial.SetFloat(step, 0);
        }
    }
}
