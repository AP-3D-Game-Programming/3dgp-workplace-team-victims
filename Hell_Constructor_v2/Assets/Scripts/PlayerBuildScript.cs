using TMPro;
using UnityEngine;

/// <summary>
/// Beheert de interacties van de speler met bouwstructuren: oppakken, vergelijken met blauwdrukken en plaatsen.
/// </summary>
public class PlayerBuildScript : MonoBehaviour
{
    // === Component Verwijzingen ===
    private LevelManager levelManager;
    private UIScript ui;
    private AudioSource audioSource;

    // === UI Elementen ===
    [Header("UI Elementen")]
    [Tooltip("TextMeshProUGUI voor de 'Pick Up' prompt (E).")]
    public TextMeshProUGUI pickUpText;
    [Tooltip("TextMeshProUGUI voor de 'Build' prompt (B).")]
    public TextMeshProUGUI buildText;

    // === Audio ===
    [Header("Audio")]
    [Tooltip("Geluidsfragment om af te spelen bij een bouw-fout.")]
    public AudioClip errorSound;

    // === State & Data ===
    [Header("Speler State")]
    [Tooltip("De structuur die de speler momenteel draagt. Null als de speler niets draagt.")]
    private GameObject heldStructure;

    [Tooltip("Een nabije structuur die de speler kan oppakken. Tijdelijke opslag.")]
    private GameObject tempPickupStructure;

    [Tooltip("De nabije blauwdruk waarmee de gedragen structuur vergeleken kan worden.")]
    private GameObject nearbyBlueprint;

    [Tooltip("Geeft aan of een nieuwe structuur gespawned mag worden (wordt gereset na bouw).")]
    public bool canSpawn = true; // Deze is behouden hoewel de functionaliteit ervan niet duidelijk is in deze code.

    [Tooltip("Aantal voltooide bouwwerken in het huidige level.")]
    public int buildAmount = 0;

    [Header("Level Configuratie")]
    [Tooltip("Het vereiste aantal voltooide bouwwerken per level.")]
    // Index 0 = Level 1 (na LevelManager.Start), Index 1 = Level 2, etc.
    private readonly int[] structureAmountsLevels = { 5, 5 };
    // LET OP: LevelManager.level start bij 1 na NewLevel() in Start(), dus we gebruiken levelManager.currentLevel - 1.

    // -----------------------------------------------------------------------------------------------------------------

    #region Unity Lifecycle Methods

    /// <summary>
    /// Initialiseert variabelen en componenten.
    /// </summary>
    void Start()
    {
        // Initialiseer componenten
        audioSource = GetComponent<AudioSource>();

        // Zorg ervoor dat LevelManager op hetzelfde GameObject zit of gebruik GetComponentInParent/Children indien nodig
        // LET OP: De LevelManager zou idealiter op een Global/Game Manager object zitten, niet op de speler, maar de huidige implementatie wordt behouden.
        levelManager = GetComponent<LevelManager>();

        // Zoek de UI componenten
        GameObject canvas = GameObject.Find("Canvas");
        if (canvas != null)
        {
            ui = canvas.GetComponent<UIScript>();
        }

        // Initialiseer objecten op null en verberg UI
        heldStructure = null;
        tempPickupStructure = null;

        if (pickUpText != null) pickUpText.gameObject.SetActive(false);
        if (buildText != null) buildText.gameObject.SetActive(false);

        // Dubbele controle of LevelManager gevonden is
        if (levelManager == null)
        {
            Debug.LogError("LevelManager component niet gevonden op dit GameObject.");
        }
    }

    /// <summary>
    /// Wordt elke frame aangeroepen voor de spelerslogica.
    /// </summary>
    void Update()
    {
        // 1. Controleer of het level voltooid is en ga naar het volgende level.
        CheckLevelCompletion();

        // 2. Zoek naar objecten in de buurt
        CheckForNearbyStructure(); // Update tempPickupStructure
        CheckForNearbyBlueprint(); // Update nearbyBlueprint

        // 3. Verwerk input op basis van de staat van de speler
        if (heldStructure == null)
        {
            // Speler draagt niets: Logica voor oppakken
            HandlePickupLogic();
        }
        else
        {
            // Speler draagt iets: Logica voor plaatsen
            HandlePlaceLogic();
        }
    }

    #endregion

    // -----------------------------------------------------------------------------------------------------------------

    #region Private Logica

    /// <summary>
    /// Controleert of het vereiste aantal bouwwerken voor het huidige level is bereikt.
    /// </summary>
    private void CheckLevelCompletion()
    {
        // Zorg ervoor dat we binnen de grenzen van de array zijn
        // currentLevel start bij 0, en NewLevel() roept het op, dus bij de eerste check is levelManager.currentLevel = 1.
        int levelIndex = levelManager.currentLevel - 1;

        if (levelIndex >= 0 && levelIndex < structureAmountsLevels.Length)
        {
            if (buildAmount >= structureAmountsLevels[levelIndex])
            {
                buildAmount = 0;
                levelManager.NewLevel();

                // Update UI-doelstelling
                if (ui != null)
                {
                    ui.playerObjective++;
                }
            }
        }
        // Eventueel hier een else statement toevoegen voor als de levels op zijn.
    }

    /// <summary>
    /// Verwerkt de logica voor het oppakken van een structuur.
    /// </summary>
    private void HandlePickupLogic()
    {
        // Alleen doorgaan als er een oppakbare structuur in de buurt is
        if (tempPickupStructure == null)
        {
            pickUpText.gameObject.SetActive(false);
            return;
        }

        StructureScript script = tempPickupStructure.GetComponent<StructureScript>();

        // Controleer of de structuur oppakbaar is (canPickUp = true)
        if (script != null && script.canPickUp)
        {
            pickUpText.gameObject.SetActive(true);

            if (Input.GetKeyDown(KeyCode.E))
            {
                heldStructure = tempPickupStructure;
                PickUpStructure();

                // Update UI-doelstelling
                if (ui != null && ui.playerObjective == 2)
                {
                    ui.playerObjective++;
                }
            }
        }
        else
        {
            pickUpText.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Verwerkt de logica voor het plaatsen van een structuur.
    /// </summary>
    private void HandlePlaceLogic()
    {
        // Verberg altijd de Pick Up tekst wanneer de speler iets draagt
        pickUpText.gameObject.SetActive(false);

        // Haal de script-referentie op. Dit kan buiten de if/else blokken omdat heldStructure != null.
        StructureScript structureScript = heldStructure.GetComponent<StructureScript>();
        if (structureScript == null) return; // Vroege exit als het script mist

        if (nearbyBlueprint != null)
        {
            // Blueprint is in de buurt: Probeer uitlijning/vergelijking
            bool canBuild = structureScript.BlueprintComparer(nearbyBlueprint);

            if (canBuild)
            {
                // Uitleiding gelukt
                buildText.gameObject.SetActive(true);
                structureScript.followPlayer = false; // Stop met volgen, blijf uitgelijnd

                if (Input.GetKeyDown(KeyCode.B))
                {
                    PerformBuild(structureScript);
                }
            }
            else
            {
                // Kan niet bouwen (verkeerde structuur/blueprint match)
                structureScript.followPlayer = true; // Volg de speler weer
                buildText.gameObject.SetActive(false);
            }
        }
        else
        {
            // Geen blueprint in de buurt: Blijf volgen
            structureScript.followPlayer = true;
            buildText.gameObject.SetActive(false);

            // Speel geluid af als de speler probeert te bouwen zonder blueprint
            if (Input.GetKeyDown(KeyCode.B))
            {
                PlayErrorSound();
            }
        }
    }

    /// <summary>
    /// Zoekt naar nabije GameObjects met de tag "Structure".
    /// </summary>
    /// <returns>True als er een structuur binnen 2f afstand is, anders False.</returns>
    private bool CheckForNearbyStructure()
    {
        // Zoeken op tag is duur. Beter zou een Trigger Collider op de speler zijn.
        // Huidige logica wordt behouden: FindGameObjectsWithTag en afstand check.
        GameObject[] structures = GameObject.FindGameObjectsWithTag("Structure");

        float pickupRange = 2f;

        foreach (GameObject s in structures)
        {
            StructureScript script = s.GetComponent<StructureScript>();
            // Controleer of het object oppakbaar is en niet het object is dat we al dragen
            if (script != null && script.canPickUp && s != heldStructure)
            {
                float distance = Vector3.Distance(transform.position, s.transform.position);

                if (distance < pickupRange)
                {
                    tempPickupStructure = s;
                    return true;
                }
            }
        }

        tempPickupStructure = null; // Reset als er niets in de buurt is
        return false;
    }

    /// <summary>
    /// Zoekt naar nabije GameObjects met de tag "Blueprint".
    /// </summary>
    /// <returns>True als er een blauwdruk binnen 6f afstand is, anders False.</returns>
    private bool CheckForNearbyBlueprint()
    {
        GameObject[] blueprints = GameObject.FindGameObjectsWithTag("Blueprint");
        float blueprintRange = 6f; // Grotere afstand dan oppakken, voor bouwcomfort

        foreach (GameObject b in blueprints)
        {
            // We hoeven niet te controleren of de blueprint actief is,
            // want in LevelManager wordt de juiste al geactiveerd.
            float distance = Vector3.Distance(transform.position, b.transform.position);

            if (distance < blueprintRange)
            {
                nearbyBlueprint = b;
                return true;
            }
        }

        nearbyBlueprint = null; // Reset als er niets in de buurt is
        return false;
    }

    /// <summary>
    /// Voert de actie uit om een structuur op te pakken.
    /// </summary>
    private void PickUpStructure()
    {
        StructureScript structureScript = heldStructure.GetComponent<StructureScript>();

        structureScript.followPlayer = true;
        pickUpText.gameObject.SetActive(false);
    }

    /// <summary>
    /// Voert de daadwerkelijke bouwactie uit na succesvolle vergelijking.
    /// </summary>
    /// <param name="script">De StructureScript van de gedragen structuur.</param>
    private void PerformBuild(StructureScript script)
    {
        // Succesvolle plaatsing
        script.Build(nearbyBlueprint);
        buildText.gameObject.SetActive(false);

        canSpawn = true; // Maakt het mogelijk om een nieuwe structuur te spawnen
        buildAmount++;

        // Update UI-doelstelling
        if (ui != null && ui.playerObjective == 3)
        {
            ui.playerObjective++;
        }

        heldStructure = null; // Laat de structuur los
    }

    /// <summary>
    /// Speelt het foutgeluid af.
    /// </summary>
    private void PlayErrorSound()
    {
        if (audioSource != null && errorSound != null)
        {
            audioSource.PlayOneShot(errorSound);
        }
    }

    #endregion
}