using System;
using UnityEngine;

namespace UI.Link_To_Radio
{
    public class UiRadioSaver : MonoBehaviour
    {
        private static bool _created = false;

        private void Awake()
        {
            if (!_created)
            {
                DontDestroyOnLoad(gameObject);
                _created = true;
            }
            else
            {
                Destroy(gameObject); 
            }
        }
    }
}
