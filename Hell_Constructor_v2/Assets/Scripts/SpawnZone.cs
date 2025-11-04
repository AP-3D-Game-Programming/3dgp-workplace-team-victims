using UnityEngine;

/// <summary>
/// Definieert een zone van waaruit specifieke bouwstructuren gespawned kunnen worden.
/// Beheert de voorraadlimieten voor de structuren in deze zone.
/// </summary>
public class SpawnZone : MonoBehaviour
{
    // -----------------------------------------------------------------------------------------------------------------

    #region Configuratie & Prefabs

    [Header("Structuur Definitie")]
    [Tooltip("Het Prefab van de structuur die in deze zone gespawned moet worden.")]
    public GameObject structurePrefab;

    [Tooltip("De naam van de structuur (bijv. 'Wall', 'Doorway'). Wordt ingesteld door LevelManager.")]
    // Wordt door LevelManager ingesteld. Publiek om makkelijk te debuggen.
    public string structureName;

    #endregion

    // -----------------------------------------------------------------------------------------------------------------

    #region Voorraad Status

    [Header("Voorraad Status")]
    [Tooltip("De maximale hoeveelheid structuren die gespawned kunnen worden in dit level.")]
    public int maxAmount;

    [Tooltip("De resterende hoeveelheid structuren die nog gespawned kunnen worden.")]
    public int amount;

    // Houdt het laatst gespawnede object bij, hoewel het niet direct gebruikt wordt na het spawnen.
    [HideInInspector]
    public GameObject spawnedObject;

    #endregion

    // -----------------------------------------------------------------------------------------------------------------

    #region Publieke Methoden

    /// <summary>
    /// Stelt de maximale en huidige voorraad voor het nieuwe level in (aangeroepen door LevelManager).
    /// </summary>
    /// <param name="max">De nieuwe maximale voorraad voor dit level.</param>
    public void setNewMax(int max)
    {
        maxAmount = max;
        amount = maxAmount;
    }

    /// <summary>
    /// Instantieert een structuur op de locatie van de SpawnZone, met een verticale correctie.
    /// </summary>
    public void SpawnStructure()
    {
        // 1. Controleer voorraad
        if (amount <= 0)
        {
            Debug.LogWarning($"Kan geen {structureName} spawnen: Voorraad is 0.");
            return;
        }

        // 2. Bepaal de verticale offset (zodat het object op de grond staat)
        float heightOffset = CalculateSpawnHeightOffset();

        // 3. Bereken de spawnpositie (boven de SpawnZone)
        Vector3 spawnPos = transform.position + Vector3.up * heightOffset;

        // 4. Instantieer het object
        spawnedObject = Instantiate(structurePrefab, spawnPos, transform.rotation);

        // 5. Configureer de StructureScript
        ConfigureSpawnedStructure(spawnedObject);
    }

    #endregion

    // -----------------------------------------------------------------------------------------------------------------

    #region Private Hulpmethoden

    /// <summary>
    /// Berekent de benodigde verticale offset om de structuur correct te plaatsen.
    /// </summary>
    /// <returns>De hoogte-offset (de helft van de hoogte van de prefab).</returns>
    private float CalculateSpawnHeightOffset()
    {
        float heightOffset = 0f;

        // Probeer de Renderer op de prefab of in de children te vinden om de hoogte te bepalen.
        Renderer prefabRenderer = structurePrefab.GetComponentInChildren<Renderer>();

        if (prefabRenderer != null)
        {
            // Neem de helft van de Y-grootte van de bounds van de mesh
            heightOffset = prefabRenderer.bounds.extents.y;
        }
        else
        {
            // Fallback als er geen Renderer is
            Debug.LogWarning($"Geen Renderer gevonden op of in Prefab {structurePrefab.name}. Gebruik offset 0.");
        }
        return heightOffset;
    }

    /// <summary>
    /// Stelt de naam van de structuur in op de StructureScript van het gespawnde object.
    /// </summary>
    /// <param name="newStructure">Het zojuist geïnstantieerde GameObject.</param>
    private void ConfigureSpawnedStructure(GameObject newStructure)
    {
        StructureScript script = newStructure.GetComponent<StructureScript>();

        if (script != null)
        {
            script.structureName = structureName;
            // Zorg ervoor dat de zojuist gespawnde structuur NIET direct oppakbaar is
            script.canPickUp = false;
        }
        else
        {
            Debug.LogError($"StructureScript mist op prefab: {structurePrefab.name}");
        }
    }

    #endregion
}