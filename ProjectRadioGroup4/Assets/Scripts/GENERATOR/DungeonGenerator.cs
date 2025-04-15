using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

//MADE BY ETHAN GD_TECH
//victor a dit que mon code est super UwU
//j'ai pas utilisé chat gpt je suis super intelligent
public class DungeonGenerator : MonoBehaviour
{
    [SerializeField] private int curentFloor;
    [SerializeField] private List<string> dungeon; // Liste des salles du donjon
    
    private readonly List<List<string>> floors = new(); // Liste des étages
    
    // Liste des niveaux par étages (1,2,3,4,5)
    [SerializeField] private  List<string> levels1;
    [SerializeField] private  List<string> levels2;
    [SerializeField] private  List<string> levels3;
    [SerializeField] private  List<string> levels4;
    [SerializeField] private  List<string> levels5;

    [SerializeField] private int seed;
    
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        Random.InitState(seed);
        floors.AddRange(new List<List<string>> { levels1, levels2, levels3, levels4, levels5 }); // Initialise la liste des étages avec les listes des niveaux
        GenerateDungeon();
        GoToNextFloor();
    }
    
    private void GenerateDungeon()
    {
        foreach (var floor in floors)
        {
            int pick = Random.Range(0, floor.Count);
            dungeon.Add(floor[pick]);
        }
    }
    
    //TODO will be called by a collider in the future
    public void GoToNextFloor()
    {
        curentFloor++;

        if (curentFloor <= dungeon.Count)
        {
            SceneManager.LoadScene(dungeon[curentFloor]);
        }
        else
        {
            Debug.Log("Fin du donjon, il n'existe plus aucun étage accessible.");
        }
    }
}