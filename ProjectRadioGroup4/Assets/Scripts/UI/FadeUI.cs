using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class FadeUI : MonoBehaviour
{
    [SerializeField] private bool fadeInOnEnter;
    [SerializeField] private bool fadeOutOnExit;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private Ease easeType = Ease.InOutSine ;
    
    private Image imageComponent;
    
    private void Awake()
    {
        imageComponent = GetComponent<Image>();
    }

    private void Start()
    {
        if(fadeInOnEnter) FadeIn();
    }
    
    public void FadeOut()
    {
        imageComponent.color = new Color(
            imageComponent.color.r,
            imageComponent.color.g,
            imageComponent.color.b,
            0f
        );

        imageComponent.DOFade(1f, fadeDuration)
            .SetEase(easeType);
    }
    
    public void FadeIn()
    {
        imageComponent.color = new Color(
            imageComponent.color.r,
            imageComponent.color.g,
            imageComponent.color.b,
            1f
        );

        imageComponent.DOFade(0f, fadeDuration)
            .SetEase(easeType);
    }
}

