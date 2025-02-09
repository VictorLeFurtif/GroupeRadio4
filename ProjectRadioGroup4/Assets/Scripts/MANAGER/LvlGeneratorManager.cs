using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class LvlGeneratorManager : MonoBehaviour
{
    private List<List<Scene>> everyScene;
    // rempli everyScene avec toutes les listes lvlNiv...
    [SerializeField] private List<Scene> lvlNiv1;
    [SerializeField] private List<Scene> lvlNiv2;
    [SerializeField] private List<Scene> lvlNiv3;
    [SerializeField] private List<Scene> lvlNiv4;
    [SerializeField] private List<Scene> lvlNiv5;
    
    [SerializeField] private List<Scene> gameScene;
    
    [SerializeField] private int floor;

    private void Awake()
    {
        everyScene.Add(lvlNiv1);
        everyScene.Add(lvlNiv2);
        everyScene.Add(lvlNiv3);
        everyScene.Add(lvlNiv4);
        everyScene.Add(lvlNiv5);
        GenerateRandomListScene();
    }

    private void GenerateRandomListScene()
    {
        gameScene.Clear();
        int maxFloor = Mathf.Min(floor, everyScene.Count); 

        for (var i = 0; i < maxFloor; i++) 
        {
            if (everyScene[i].Count == 0) continue;
            var randomValue = Random.Range(0, everyScene[i].Count);
            List<Scene> sceneNewList = everyScene[i];
            gameScene.Add(sceneNewList[randomValue]);
            sceneNewList.RemoveAt(randomValue);
        }
    }
}