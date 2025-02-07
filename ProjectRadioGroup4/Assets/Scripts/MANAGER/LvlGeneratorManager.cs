using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class LvlGeneratorManager : MonoBehaviour
{
    [SerializeField] private List<Scene> everyScene;
    [SerializeField] private List<Scene> gameScene;
    [SerializeField] private int floor;

    private void Awake()
    {
        GenerateRandomListScene();
    }

    private void GenerateRandomListScene()
    {
        
        int maxFloor = Mathf.Min(floor, everyScene.Count); // au cas ou tu forces sur les floors con de gd

        for (var i = 0; i < maxFloor; i++) 
        {
            if (everyScene.Count == 0) break; 

            var randomValue = Random.Range(0, everyScene.Count);
            gameScene.Add(everyScene[randomValue]);
            everyScene.RemoveAt(randomValue);
        }
    }
}