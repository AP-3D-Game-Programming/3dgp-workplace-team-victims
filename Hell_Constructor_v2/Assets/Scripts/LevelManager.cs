using UnityEngine;

/// <summary>
/// Beheert de levelprogressie en stelt de initiële parameters in voor de spawners en de level-specifieke blauwdrukken.
/// </summary>
public class LevelManager : MonoBehaviour
{
    // === Publieke Level Data ===
    [Tooltip("De index van het huidige level (start bij 0).")]
    public int currentLevel;

    [Tooltip("Tweedimensionale array: rijen zijn levels (index), kolommen zijn de max hoeveelheden van structuren (Wall, Doorway, Floor).")]
    // Voorbeeld: { { 3, 1, 1 }, { 2, 1, 1 } }
    // Level 0: 3x Wall, 1x Doorway, 1x Floor
    // Level 1: 2x Wall, 1x Doorway, 1x Floor
    public int[,] Amounts = { { 3, 1, 1 }, { 2, 1, 1 } };

    [Tooltip("De GameObjects die de blauwdrukken/visuele hulpmiddelen voor elk level vertegenwoordigen.")]
    public GameObject[] blueprintHouses;

    // === Interne Spawner Configuratie ===
    // De namen moeten overeenkomen met de tags van de spawner GameObjects in de scène (bv. "SpawnZoneWall").
    private readonly string[] spawnerNames = { "Wall", "Doorway", "Floor" };

    // === Private Hulpvariabelen ===
    private GameObject spawner;
    // Hier zou eventueel een lijst of array van Spawner-componenten opgeslagen kunnen worden voor directere toegang,
    // maar de huidige methode met GameObject.Find wordt behouden zoals gevraagd.

    // -----------------------------------------------------------------------------------------------------------------

    #region Unity Lifecycle Methods

    /// <summary>
    /// Wordt aangeroepen voordat de eerste frame-update plaatsvindt.
    /// Initialiseert het level en start het eerste niveau.
    /// </summary>
    void Start()
    {
        // Start altijd bij level 0
        currentLevel = 0;
        NewLevel();
    }

    /// <summary>
    /// Wordt eenmaal per frame aangeroepen. Momenteel ongebruikt.
    /// </summary>
    void Update()
    {
        // Eventuele level-controle logica hier (bijv. checken of het level voltooid is)
    }

    #endregion

    // -----------------------------------------------------------------------------------------------------------------

    #region Level Management Methods

    /// <summary>
    /// Stelt een nieuw level in door de spawnerlimieten te configureren,
    /// de juiste blauwdruk zichtbaar te maken, en bestaande structuren oppakbaar te maken.
    /// </summary>
    public void NewLevel()
    {
        // 1. Controleer of er nog levels zijn
        if (currentLevel >= Amounts.GetLength(0))
        {
            Debug.Log("Alle levels zijn voltooid!");
            // Voeg hier eventuele logica toe voor Game Over/Victory screen
            return;
        }

        // 2. Configureer alle Spawners voor het huidige level
        ConfigureSpawners();

        // 3. Toon de juiste Level Blauwdruk
        ActivateBlueprint();

        // 4. Maak alle bestaande Structuren oppakbaar voor het nieuwe level
        MakeStructuresPickUpable();

        // 5. Verhoog de levelteller voor de volgende keer
        currentLevel++;
    }

    /// <summary>
    /// Stelt de Max-waarden en namen in voor elke SpawnZone op basis van het huidige level.
    /// </summary>
    private void ConfigureSpawners()
    {
        for (int i = 0; i < spawnerNames.Length; i++)
        {
            // Zoek de spawner op basis van de naam (bijv. "SpawnZoneWall")
            spawner = GameObject.Find($"SpawnZone{spawnerNames[i]}");

            if (spawner != null)
            {
                // Haal de SpawnZone component op
                SpawnZone spawnZone = spawner.GetComponent<SpawnZone>();

                if (spawnZone != null)
                {
                    // Stel de nieuwe maximale hoeveelheid in vanuit de Amounts array
                    // Index: [huidige level, structuur index]
                    spawnZone.setNewMax(Amounts[currentLevel, i]);
                    // Stel de naam van de structuur in
                    spawnZone.structureName = spawnerNames[i];
                }
                else
                {
                    Debug.LogError($"SpawnZone component niet gevonden op {spawner.name}");
                }
            }
            else
            {
                Debug.LogError($"GameObject met de naam 'SpawnZone{spawnerNames[i]}' niet gevonden!");
            }
        }
    }

    /// <summary>
    /// Activeert de blauwdruk voor het huidige level en deactiveert (optioneel, maar hier niet gedaan) de vorige.
    /// </summary>
    private void ActivateBlueprint()
    {
        // Zorg ervoor dat de blauwdruk voor het huidige level bestaat
        if (currentLevel < blueprintHouses.Length && blueprintHouses[currentLevel] != null)
        {
            // Eventuele vorige blauwdruk deactiveren kan hier (indien nodig)
            // Hier wordt alleen de huidige geactiveerd, ervan uitgaande dat Start() of een andere methode ze eerst deactiveert.
            blueprintHouses[currentLevel].gameObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning($"Geen blauwdruk gedefinieerd voor level {currentLevel}.");
        }
    }

    /// <summary>
    /// Zoekt alle GameObjects met de tag "Structure" en maakt ze oppakbaar.
    /// </summary>
    private void MakeStructuresPickUpable()
    {
        // Zoek alle bestaande structuren in de scène
        GameObject[] structures = GameObject.FindGameObjectsWithTag("Structure");

        // Maak elke structuur oppakbaar
        foreach (GameObject s in structures)
        {
            StructureScript structureScript = s.GetComponent<StructureScript>();
            if (structureScript != null)
            {
                structureScript.canPickUp = true;
            }
            // Eventuele foutafhandeling als StructureScript ontbreekt kan hier.
        }
    }

    #endregion
}