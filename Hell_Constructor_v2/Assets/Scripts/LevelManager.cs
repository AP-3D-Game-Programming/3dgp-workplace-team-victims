using UnityEditor.PackageManager;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private int level;
    private GameObject spawner;
    private string[] spawnerNames = { "Wall", "Doorway", "Floor" };
    private int[,] Amounts = { { 3, 1, 1 }, { 2, 1, 1 } }; // eerste is level, 2de is structure
    
    void Start()
    {
        level = 1;
        newLevel();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void newLevel()
    {
        for (int i = 0; i < spawnerNames.Length; i++)
        {
            spawner = GameObject.Find($"SpawnZone{spawnerNames[i]}");
            SpawnZone spawnZone = spawner.GetComponent<SpawnZone>();
            spawnZone.setNewMax(Amounts[level - 1, i]);
            spawnZone.structureName = spawnerNames[i];


        }
    }
}
