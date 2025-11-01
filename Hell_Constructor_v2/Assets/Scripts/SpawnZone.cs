using UnityEngine;

public class SpawnZone : MonoBehaviour
{
    public GameObject structurePrefab;
    [HideInInspector] public GameObject spawnedObject;

    public bool HasStructure()
    {
        return spawnedObject != null;
    }

    public void SpawnStructure()
    {
        if (HasStructure()) return;

        // Get the prefab height (from its mesh bounds)
        float heightOffset = 0f;
        Renderer prefabRenderer = structurePrefab.GetComponentInChildren<Renderer>();
        if (prefabRenderer != null)
            heightOffset = prefabRenderer.bounds.size.y / 2f;

        // Spawn slightly above the spawn zone
        Vector3 spawnPos = transform.position + Vector3.up * heightOffset;

        spawnedObject = Instantiate(structurePrefab, spawnPos, transform.rotation);
    }

}
