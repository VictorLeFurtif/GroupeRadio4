using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeTransitionUI : MonoBehaviour
{
    [SerializeField] private float speedScale = 1f;
    [SerializeField] private Color fadeColor = Color.black;
    [SerializeField] private AnimationCurve Curve = new AnimationCurve(new Keyframe(0,1), 
        new Keyframe(0.5f, 0.5f, -1.5, -1.5f), new Keyframe(1,0));
    [SerializeField] private bool startFadedOut = false;


    private float alpha = 0f;
    private Texture2D texture;
    private int direction = 0; //1 = fadeIn, -1 = fadeOut
    private float time = 0f;

    private void Start()
    {
        if (startFadedOut) alpha = 1f; else alpha = 0f;
        texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, new Color(fadeColor.r, fadeColor.g, fadeColor.b, alpha));
        texture.Apply();
    }

    private void Update()
    {
        if (direction == 0 && Input.GetKeyDown(key))
        {
            if (alpha >= 1f) // Fully faded out
            {
                alpha = 1f;
                time = 0f;
                direction = 1;
            }
            else // Fully faded in
            {
                alpha = 0f;
                time = 1f;
                direction = -1;
            }
                 
        }
    }
    public void OnGUI()
    {
        if (alpha > 0f) GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), texture);
        if (direction != 0)
        {
            time += direction * Time.deltaTime * speedScale;
            alpha = Curve.Evaluate(time);
            texture.SetPixel(0, 0, new Color(fadeColor.r, fadeColor.g, fadeColor.b, alpha));
            texture.Apply();
            if (alpha <= 0f || alpha >= 1f) direction = 0;
        }
    }
}

