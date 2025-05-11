using System;
using System.Collections;
using System.Collections.Generic;
using Controller;
using UI;
using UnityEngine;
using UnityEngine.UIElements;

public class DetectEnnemiTUTO : MonoBehaviour
{
    [SerializeField] private GameObject radio;
    private RadioController radioController;
    [SerializeField] private List<string> scanMessage;
    [SerializeField] private TutorialUIManager tutorialUI;

    private void Start()
    {
        radioController = radio.GetComponent<RadioController>();
        if (radioController == null)
        {
            Debug.LogError("RadioController non trouvé.");
        }
    }

    private void Update()
    {
        if (!_playerIsInZone) return;

        if (radioController.listOfDetectedEnemy.Count == 1)
        {
            DispayMessage();
            enabled = false;
        }
    }

    private bool _playerIsInZone;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _playerIsInZone = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _playerIsInZone = false;
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void DispayMessage()
    {
        GetComponent<ZoneTutorialTrigger>().text = scanMessage;
        GetComponent<ZoneTutorialTrigger>().readerID = 0;
        tutorialUI.ShowTutorial(GetComponent<ZoneTutorialTrigger>().text[GetComponent<ZoneTutorialTrigger>().readerID]);
    }
}
