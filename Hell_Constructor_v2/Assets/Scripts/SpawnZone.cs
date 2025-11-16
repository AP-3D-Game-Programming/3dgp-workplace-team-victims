using UnityEngine;

public class SpawnZone : MonoBehaviour
{
    public GameObject structurePrefab;
    [HideInInspector] public GameObject spawnedObject;

    public string structureName;
    public int maxAmount;
    public int amount;

    public void setNewMax(int max)
    {
        maxAmount = max;
        amount += maxAmount;
    }


    public void SpawnStructure()
    {
        if (amount == 0)
            return;

        // Get the prefab height (from its mesh bounds)
        float heightOffset = 0f;
        Renderer prefabRenderer = structurePrefab.GetComponentInChildren<Renderer>();
        if (prefabRenderer != null)
            heightOffset = prefabRenderer.bounds.size.y / 2f;

        // Spawn slightly above the spawn zone
        Vector3 spawnPos = transform.position + Vector3.up * heightOffset;

        spawnedObject = Instantiate(structurePrefab, spawnPos, transform.rotation);
        StructureScript script = spawnedObject.GetComponent<StructureScript>();
        script.structureName = structureName;
    }

}