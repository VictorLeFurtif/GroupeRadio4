using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollingBgUI : MonoBehaviour
{
    [SerializeField] private List<GameObject> bgImaqeList;
    [SerializeField] float imageLenght;
    [SerializeField] private float moveSpeed;

    private void FixedUpdate()
    {
        foreach (var image in bgImaqeList)
        {
            MoveBg(image);
            Requeue(image);
        }
    }

    private void MoveBg(GameObject image)
    {
        image.transform.position = 
            new Vector3(image.transform.position.x + moveSpeed, image.transform.position.y, 0);
    }

    private void Requeue(GameObject image)
    {
        if (image.transform.position.x >= imageLenght + imageLenght/2)
        {
            image.transform.position = new Vector3(-imageLenght -imageLenght/2, image.transform.position.y, 0);
        }
    }
}
