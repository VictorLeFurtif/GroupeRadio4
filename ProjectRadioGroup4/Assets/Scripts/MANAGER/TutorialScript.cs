using UI;
using UnityEngine;
using UnityEngine.Rendering.Universal;

// JACQUES ne gronde pas victor c'est Ethan GD qui a écrit le script.

public class TutorialScript : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private new Light2D light;
    [SerializeField] private Light2D torchLight;
    [SerializeField] private string succesMessage;
    [SerializeField] private TutorialUIManager tutorialUI;
    
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
            tutorialUI.ShowTutorial(succesMessage);
        }
    }
}