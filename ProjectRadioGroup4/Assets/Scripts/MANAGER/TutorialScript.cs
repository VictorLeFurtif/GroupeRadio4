using UnityEngine;
using UnityEngine.Rendering.Universal;

// JACQUES ne gronde pas victor c'est Ethan GD qui a écrit le script.

public class TutorialScript : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private Light2D light;
    [SerializeField] private Light2D torchLight;
    
    private bool done;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == player && !done)
        {
            light.intensity = 0f;
            done = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject == player && torchLight.intensity != 0f)
        {
            light.intensity = 1f;
        }
    }
}