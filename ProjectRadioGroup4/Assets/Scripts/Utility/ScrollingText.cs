using System;
using System.Collections;
using MANAGER;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScrollingText : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private int stopY;
    private bool isCall = false;
    
    void FixedUpdate()
    {
        transform.Translate(Vector2.up * speed);
        if (transform.position.y > stopY)
        {
            speed = 0;
            if (!isCall)
            {
                isCall = true;
                StartCoroutine(GoingBackToMenu());
            }
        }
    }

    private IEnumerator GoingBackToMenu()
    {
        yield return new WaitForSeconds(4f);
        GameManager.instance.GetLooseScreen().Menu();
    }
}
