using TMPro;
using UnityEngine;

/// <summary>
/// Beheert de gebruikersinterface (UI) elementen, inclusief tutorial stappen, 
/// speldoelen en de weergave van de resterende voorraad van bouwstructuren.
/// </summary>
public class UIScript : MonoBehaviour
{
    // -----------------------------------------------------------------------------------------------------------------

    #region UI Componenten

    [Header("UI Elementen")]
    [Tooltip("Veld voor tutorialteksten.")]
    public TextMeshProUGUI tutorialText;

    [Tooltip("Veld voor de huidige speldoelstelling.")]
    public TextMeshProUGUI objectiveText;

    [Tooltip("Veld voor de voorraadinformatie (magazijn).")]
    public TextMeshProUGUI magasinText;

    [Tooltip("Veld voor speciale level/boss meldingen.")]
    public TextMeshProUGUI bossText;

    #endregion

    // -----------------------------------------------------------------------------------------------------------------

    #region Component Verwijzingen

    // We hoeven alleen de componenten zelf op te slaan, niet het 'Player' GameObject.
    private PlayerController playerController;
    private PlayerBuildScript playerBuildScript; // Momenteel ongebruikt in de Update, maar behouden.
    private LevelManager levelManager;
    private SpawnZone[] allSpawnZones; // Array om alle SpawnZones te cachen.

    #endregion

    // -----------------------------------------------------------------------------------------------------------------

    #region State

    [Header("Spel Status")]
    [Tooltip("De index die het huidige speldoel bijhoudt.")]
    public int playerObjective;

    // Constanten voor de structuur namen (voor magazijn weergave)
    private const string WALL = "Wall";
    private const string DOORWAY = "Doorway";
    private const string FLOOR = "Floor";

    #endregion

    // -----------------------------------------------------------------------------------------------------------------

    #region Unity Lifecycle Methods

    void Start()
    {
        // Zoek het speler GameObject
        GameObject player = GameObject.Find("Player");

        if (player != null)
        {
            // Haal de benodigde componenten op (aangenomen dat ze op de Player zitten)
            playerController = player.GetComponent<PlayerController>();
            playerBuildScript = player.GetComponent<PlayerBuildScript>();
            levelManager = player.GetComponent<LevelManager>();
        }
        else
        {
            Debug.LogError("Player GameObject niet gevonden. UI-updates zullen falen.");
        }

        // Cache alle SpawnZones in de scène (Aangenomen tag "Spawn" of "SpawnZone")
        // De originele code gebruikte FindGameObjectsWithTag("Spawn").
        allSpawnZones = FindObjectsOfType<SpawnZone>();

        // Initialiseer de zichtbaarheid van de UI
        if (tutorialText != null) tutorialText.gameObject.SetActive(true);
        if (objectiveText != null) objectiveText.gameObject.SetActive(true);
        if (magasinText != null) magasinText.gameObject.SetActive(true);
        if (bossText != null) bossText.gameObject.SetActive(false);
    }

    void Update()
    {
        UpdateTutorialText();
        UpdateObjectiveText();
        UpdateMagasinText();
        CheckLevelSpecificMessages();
    }

    #endregion

    // -----------------------------------------------------------------------------------------------------------------

    #region UI Updatelogica

    /// <summary>
    /// Update de tutorialtekst op basis van de PlayerController.playerUI status.
    /// </summary>
    private void UpdateTutorialText()
    {
        if (playerController == null || tutorialText == null) return;

        switch (playerController.playerUI)
        {
            case 0:
                tutorialText.text = @"Gebruik **muis** om rond te kijken
gebruik **WASD** om te bewegen";
                break;
            case 1:
                tutorialText.text = "Gebruik **Shift** om te sprinten";
                break;
            case 2:
                tutorialText.text = "Gebruik **Spatiebalk** om te springen";
                break;
            default:
                tutorialText.gameObject.SetActive(false);
                break;
        }
    }

    /// <summary>
    /// Update de objective tekst op basis van de playerObjective status.
    /// </summary>
    private void UpdateObjectiveText()
    {
        if (objectiveText == null) return;

        switch (playerObjective)
        {
            case 0:
                objectiveText.text = "Doel: Ga naar het **Magazijn** (SpawnZone).";
                break;
            case 1:
                objectiveText.text = "Doel: Druk op **F** bij een knop om een structuur te spawnen.";
                break;
            case 2:
                objectiveText.text = "Doel: Druk op **E** om de gespawnde structuur op te pakken.";
                break;
            case 3:
                objectiveText.text = "Doel: Plaats de structuur met **B** op de bijpassende blauwdruk.";
                break;
            case 4:
                objectiveText.text = "Doel: Bouw het huis af.";
                break;
            case 5:
                objectiveText.text = "Doel: Bouw het tweede huis af.";
                break;
            default:
                objectiveText.text = "Doel: Voltooi het spel! (Alle huizen gebouwd)";
                // Optioneel: objectiveText.gameObject.SetActive(false);
                break;
        }
    }

    /// <summary>
    /// Update de magazijn voorraad tekst door alle SpawnZones te controleren.
    /// </summary>
    private void UpdateMagasinText()
    {
        if (magasinText == null) return;

        // Gebruik een dictionary om de hoeveelheden per structuur op te slaan.
        var amounts = new System.Collections.Generic.Dictionary<string, int>
        {
            { WALL, 0 }, { FLOOR, 0 }, { DOORWAY, 0 }
        };

        // Itereren over de gecachte SpawnZones is sneller dan elke frame FindGameObjectsWithTag te doen.
        foreach (var zone in allSpawnZones)
        {
            if (zone != null && amounts.ContainsKey(zone.structureName))
            {
                amounts[zone.structureName] = zone.amount;
            }
        }

        // Gebruik string interpolatie voor een duidelijke weergave
        magasinText.text =
            $"**Muur:** {amounts[WALL]}   **Vloer:** {amounts[FLOOR]}   **Deur:** {amounts[DOORWAY]}";
    }

    /// <summary>
    /// Toont speciale berichten op basis van het LevelManager.currentLevel.
    /// </summary>
    private void CheckLevelSpecificMessages()
    {
        if (levelManager == null || bossText == null) return;

        // Aangenomen: levelManager.level is inmiddels hernoemd naar currentLevel in LevelManager
        if (levelManager.currentLevel == 2)
        {
            bossText.gameObject.SetActive(true);
            bossText.text = "Oeps, er mist een muur in de levering. Doe wat je kunt en steel van je andere huis!";
        }
        else
        {
            bossText.gameObject.SetActive(false);
        }
    }

    #endregion
}