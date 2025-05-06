using System.Collections;
using TMPro;
using UnityEngine;

namespace Utility
{
    public static class TypewriterUtility
    {
        public static bool reset = false;
        
        public static IEnumerator TypeText(TMP_Text targetText, string fullText, float delay = 0.02f)
        {
            
            foreach (char c in fullText)
            {
                if (reset)
                {
                    reset = false;
                    yield break;
                }
                targetText.text += c;
                yield return new WaitForSeconds(delay);
            }
        }
        
    }
}