using System;
using UnityEngine;

namespace UI.Link_To_Radio
{
    public class UiRadioSaver : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
