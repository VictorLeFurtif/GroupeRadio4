using System;
using System.Collections;
using System.Collections.Generic;
using Controller;
using UnityEngine;
using TMPro;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] 
    private TextMeshProUGUI textComponent;
    [SerializeField] 
    private string[] lines;
    [SerializeField] [Header("lower value = faster")] 
    private float textSpeed;
    [SerializeField] private float offset;
    
    private int index;
    private bool playerControllerIsNull;

    private void Start()
    {
        textComponent.text = string.Empty;
        StartDialogue();
    }

    private void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            if (textComponent.text == lines[index])
            {
                NextLine();
            }
            else
            {
                StopAllCoroutines();
                textComponent.text = lines[index];
            }
        }

        if (!playerControllerIsNull)
        {
            Vector2 currentPos = gameObject.transform.position;
            currentPos = PlayerController.instance.transform.position;
            currentPos.y += offset;
            gameObject.transform.position = currentPos;
        }
    }

    private void FixedUpdate()
    {
        if (PlayerController.instance == null)
        {
            textComponent.text = "playerController.Instance == null";
            playerControllerIsNull = true;
        }
    }

    private void StartDialogue()
    {
        index = 0;
        StartCoroutine(TypeLine());
    }

    private IEnumerator TypeLine()
    {
        foreach (char c in lines[index].ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    private void NextLine()
    {
        if (index < lines.Length - 1)
        {
            index++;
            textComponent.text = string.Empty;
            StartCoroutine(TypeLine());
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
