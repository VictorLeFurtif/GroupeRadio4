using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class RangeFinderLittleIvanov : MonoBehaviour
    {
        [SerializeField] private RectTransform playerImage;
        
        private void Update()
        {
            CheckForFlipX();
        }
        
        private bool wasFacingLeft = false;

        private void CheckForFlipX()
        {
            if (Mathf.Abs(NewPlayerController.instance.rb.velocity.x) > 0.1f)
                wasFacingLeft = NewPlayerController.instance.rb.velocity.x < 0;

            playerImage.rotation =
                wasFacingLeft ? Quaternion.Euler(0, 180, 0) : Quaternion.Euler(0, 0, 0);
            
        }
    }
}
