using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Vector3 offSett = new Vector3(0f,0f,-10f);
    private Transform target;
    [SerializeField] private float smoothTime = 0.25f;
    private Vector3 velocity = Vector3.zero;
    
    private void Start()
    {
        target = PlayerController.instance.gameObject.transform;
    }

    private void FollowPlayer()
    {
        Vector3 targetPosition = target.position + offSett;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }

    private void LateUpdate()
    {
        FollowPlayer();
    }
}
