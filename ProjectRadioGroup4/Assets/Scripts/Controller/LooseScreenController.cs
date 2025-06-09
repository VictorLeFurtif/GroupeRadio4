using System.Collections;
using System.Collections.Generic;
using MANAGER;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Controller
{
    public class LooseScreenController : MonoBehaviour
    {
        public GameObject looseScreenPanel;
        [SerializeField] private Image glassImageReference;
        [SerializeField] private List<Sprite> glassAnimFrameList;
        [SerializeField] private float screenShakeRange;
        [SerializeField] private float animationSpeed;
        
        private void Awake()
        {
            looseScreenPanel.SetActive(false);
        }

        private void FixedUpdate()
        {
            if (GameManager.instance.CurrentGameState == GameManager.GameState.GameOver && !looseScreenPanel.activeSelf)
            {
                GameOver();
            }
        }

        private void GameOver()
        {
            looseScreenPanel.SetActive(true);
            StartCoroutine(GlassImageAnimation());
        }

        public void Retry()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            GameManager.instance.CurrentGameState = GameManager.GameState.Game;
            GameManager.instance.ResetPlayer();
        }

        public void Menu()
        {
            SceneManager.LoadScene(0);
            GameManager.instance.ResetPlayer();
            GameManager.instance.CurrentGameState = GameManager.GameState.Menu;
        }

        public IEnumerator GlassImageAnimation()
        {
            foreach (var sprite in glassAnimFrameList)
            {
                glassImageReference.sprite = sprite;
                //NewRandomImagePosition();
                yield return new WaitForSeconds(animationSpeed);
            }
        }

        public void NewRandomImagePosition()
        {
            /*
             //works like trash
            glassImageReference.gameObject.transform.position += 
                new Vector3(Random.Range(0f, screenShakeRange), Random.Range(0f, screenShakeRange), 0);
                Debug.Log(glassImageReference.gameObject.transform.position);
            */
        }
    }
}
