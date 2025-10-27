using UnityEngine;
using TMPro;

public class SpawnManager : MonoBehaviour
{
    public GameObject[] walls;              // Array of possible wall prefabs
    public Transform spawnZone;             // Where to spawn
    public float interactDistance = 3f;
    public float checkRadius = 1f;
    public TextMeshProUGUI popupText;       // UI popup

    private Camera playerCamera;
    private bool isLookingAtButton = false;
    private int spawnedCount = 0;           // Track how many walls have been spawned

    void Start()
    {
        playerCamera = Camera.main;
        popupText.gameObject.SetActive(false);
    }

    void Update()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            if (hit.collider.CompareTag("Button"))
            {
                isLookingAtButton = true;

                if (spawnedCount >= walls.Length)
                {
                    popupText.text = "No more walls available!";
                }
                else if (IsWallAtSpawnZone())
                {
                    popupText.text = "A wall is already here";
                }
                else
                {
                    popupText.text = "Press F to spawn wall";
                }

                popupText.gameObject.SetActive(true);

                if (Input.GetKeyDown(KeyCode.F))
                {
                    TrySpawnWall();
                }

                return;
            }
        }

        if (isLookingAtButton)
        {
            isLookingAtButton = false;
            popupText.gameObject.SetActive(false);
        }
    }

    void TrySpawnWall()
    {
        if (spawnedCount >= walls.Length)
        {
            Debug.Log("No more walls available!");
            return;
        }

        if (IsWallAtSpawnZone())
        {
            Debug.Log("A wall already exists here!");
            return;
        }

        // Choose the next wall from the array
        GameObject wallPrefab = walls[spawnedCount];
        Vector3 spawnPos = spawnZone.position + Vector3.up * (wallPrefab.transform.localScale.y / 2);

        Instantiate(wallPrefab, spawnPos, spawnZone.rotation);
        spawnedCount++;

        Debug.Log("Spawned wall " + spawnedCount + "/" + walls.Length);
    }

    bool IsWallAtSpawnZone()
    {
        Collider[] nearbyObjects = Physics.OverlapSphere(spawnZone.position, checkRadius);
        foreach (Collider col in nearbyObjects)
        {
            if (col.CompareTag("Wall"))
                return true;
        }
        return false;
    }


}
