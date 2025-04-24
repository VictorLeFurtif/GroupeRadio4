using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutoPanelExploration : MonoBehaviour
{
    private TMP_Text descriptionText;
    [SerializeField] private string description;

    private void Start()
    {
        descriptionText = GetComponentInParent<TMP_Text>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        descriptionText.text = description;
    }
}
