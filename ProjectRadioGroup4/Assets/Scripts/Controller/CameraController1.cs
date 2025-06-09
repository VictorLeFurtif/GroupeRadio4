using System;
using UnityEngine;

namespace Controller
{
    public class CameraController1 : MonoBehaviour
    {
        [Header("CAMERA SETTINGS")] 
        [SerializeField] private float xMin;
        [SerializeField] private float xMax;
        [SerializeField] private Vector3 startingPosition;
        private int direction = 1;
        [SerializeField] private float speed;

        private void Awake()
        {
            InitCamera();
        }

        private void Update()
        {
            CameraShift();
        }

        private void CameraShift()
        {
            transform.Translate(new Vector2(direction, 0) * speed * Time.deltaTime);
            
            if (direction > 0)
            {
                if (transform.position.x >= xMax)
                {
                    direction = -1;
                }
            }
            else
            {
                if (transform.position.x <= xMin)
                {
                    direction = 1;
                }
            }
        }

        private void InitCamera()
        {
            transform.position = startingPosition;
        }
    }
}