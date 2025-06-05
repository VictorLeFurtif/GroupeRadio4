using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace MANAGER
{
    public class GlobalVolumeManager : MonoBehaviour
    {
        [Header("Vignette Colors")]
        [SerializeField] private Color colorAdvantage;
        [SerializeField] private Color colorDesadvantage;
        [SerializeField] private Color colorExplo;

        [Header("Volume Reference")]
        [SerializeField] private Volume globalVolume; 

        private Vignette vignette;

        private void Awake()
        {
            if (!globalVolume.profile.TryGet(out vignette))
            {
                Debug.LogError("Vignette not found in the Global Volume profile!");
            }
        }

        private void Start()
        {
            GvColorToExplo(); 
        }

        private void ChangeColor(Color newColor)
        {
            if (vignette != null)
            {
                vignette.color.Override(newColor);
            }
        }

        public void GvColorToAdvantage() => ChangeColor(colorAdvantage);
        public void GvColorToDesadvantage() => ChangeColor(colorDesadvantage);
        public void GvColorToExplo() => ChangeColor(colorExplo);
    }
}