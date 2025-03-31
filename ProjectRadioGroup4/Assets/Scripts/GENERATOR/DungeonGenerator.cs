using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DungeonGenerator : MonoBehaviour
{
    ///////////////////////
    ////// VARIABLES //////
    ///////////////////////
    
    [SerializeField] private  List<string> dungeon; // Liste des salles du donjon
    
    private readonly List<List<string>> floors = new(); // Liste des étages
    
    // Liste des niveaux par étages (1,2,3,4,5)
    [SerializeField] private  List<string> levels1;
    [SerializeField] private  List<string> levels2;
    [SerializeField] private  List<string> levels3;
    [SerializeField] private  List<string> levels4;
    [SerializeField] private  List<string> levels5;

    [SerializeField] private int seed; // Graine de génération du pseudo-aléatoire
    
    // Start is called before the first frame update
    void Start()
    {
        Random.InitState(seed);
        floors.AddRange(new List<List<string>> { levels1, levels2, levels3, levels4, levels5 }); // Initialise la liste des étages avec les listes des niveaux
        GenerateDungeon();
    }
    
    private void Update()
    {
        // DEBUG ONLY
        // Permet de recommencer la génération
        if (Input.GetKeyDown(KeyCode.Return))
        {
            RestartGeneration();
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void GenerateDungeon()
    {
        foreach (var f in floors)
        {
            int pick = Random.Range(0, f.Count);
            dungeon.Add(f[pick]);
            Debug.Log("Room " + f[pick] + " has been added to the dungeon.");
        }
    }
    
    // ReSharper disable Unity.PerformanceAnalysis
    private void RestartGeneration()
    {
        Debug.Log("Restarting dungeon generation...");
        dungeon.Clear();
        GenerateDungeon();
    }
}
