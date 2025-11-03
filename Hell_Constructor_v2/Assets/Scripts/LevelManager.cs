using UnityEditor.PackageManager;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public int level;
    private GameObject spawner;
    private string[] spawnerNames = { "Wall", "Doorway", "Floor" };
    public int[,] Amounts = { { 3, 1, 1 }, { 2, 1, 1 } }; // eerste is level, 2de is structure
    public GameObject[] blueprintHouses;
    
    void Start()
    {
        level = 0;
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
            spawnZone.setNewMax(Amounts[level, i]);
            spawnZone.structureName = spawnerNames[i];

        }
        blueprintHouses[level].gameObject.SetActive(true);

        GameObject[] structures = GameObject.FindGameObjectsWithTag("Structure");

        foreach (GameObject s in structures)
        {
            s.GetComponent<StructureScript>().canPickUp = true;
        }
        level++;
    }
}
