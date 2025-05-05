using UnityEngine;
using UnityEngine.Rendering.Universal;

public class TutorialScript : MonoBehaviour
{
    [SerializeField] private GameObject _player;
    [SerializeField] private Light2D _light;
    [SerializeField] private Light2D _torchLight;

    private bool _isInLightZone = false;

    public bool IsInLightZone
    {
        get => _isInLightZone;
        set
        {
            if (_isInLightZone != value)
            {
                _isInLightZone = value;
                OnLightZoneChanged();
            }
        }
    }

    private void OnLightZoneChanged()
    {
        if (_light == null || _torchLight == null) return;
        
        if (_torchLight.intensity != 0f)
        {
            _light.intensity = 1f;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == _player)
        {
            IsInLightZone = true;
            _light.intensity = 0f;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject == _player)
        {
            IsInLightZone = false;
            _light.intensity = 1f;
        }
    }
}