using System;
using UnityEngine;

namespace INTERFACE
{
    public class EventTrigger : MonoBehaviour
    {
        private Animator animatorEvent;

        private void Start()
        {
            animatorEvent = GetComponent<Animator>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                animatorEvent.Play("IdleEvent");
                Debug.Log("ZIZI");
            }
        }
    }
}
