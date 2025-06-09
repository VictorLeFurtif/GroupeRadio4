using UnityEngine;
using UnityEngine.SceneManagement;

public class ScrollingText : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private int stopY;
    
    // Update is called once per frame
    void FixedUpdate()
    {
        transform.Translate(Vector2.up * speed);
        if (transform.position.y > stopY)
        {
            speed = 0;
        }
    }
}
