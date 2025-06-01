using System.Collections;
using UnityEngine;

public class StartingScreenUI : MonoBehaviour
{
    [SerializeField] private float displayTime = 2.5f;
    void Start()
    {
        StartCoroutine(DisableAfterTime());
    }

    private IEnumerator DisableAfterTime()
    {
        yield return new WaitForSeconds(displayTime);
        gameObject.SetActive(false);
    }
}
