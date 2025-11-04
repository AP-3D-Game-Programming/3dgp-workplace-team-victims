using UnityEngine;

/// <summary>
/// Beheert de staat en functionaliteit van een individueel bouwdeel in het spel,
/// inclusief het volgen van de speler, uitlijning met blauwdrukken en de uiteindelijke bouw.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class StructureScript : MonoBehaviour
{
    // -----------------------------------------------------------------------------------------------------------------

    #region Externe Verwijzingen & Configuratie

    [Header("Identificatie")]
    [Tooltip("De naam van de structuur (bijv. 'Wall'). Wordt ingesteld door de SpawnZone.")]
    public string structureName;

    [Tooltip("De AudioSource om bouwgeluiden af te spelen.")]
    private AudioSource audioSource;

    // De speler Transform die in Start() wordt gevonden.
    private GameObject player;

    #endregion

    // -----------------------------------------------------------------------------------------------------------------

    #region State Variabelen

    [Header("Status")]
    [Tooltip("Indien true, volgt de structuur de speler.")]
    public bool followPlayer = false;

    [Tooltip("Indien true, kan de speler dit object oppakken.")]
    public bool canPickUp = true;

    [Tooltip("Aangepaste rotatie (Euler hoeken) wanneer het object wordt gedragen.")]
    public Vector3 pickedUpRotation = Vector3.zero;

    #endregion

    // -----------------------------------------------------------------------------------------------------------------

    #region Unity Lifecycle Methods

    void Start()
    {
        // Zoek de speler
        player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Player object met tag 'Player' niet gevonden.");
        }

        // Haal de AudioSource component op
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (followPlayer)
        {
            HandlePlayerFollowing();
        }
    }

    #endregion

    // -----------------------------------------------------------------------------------------------------------------

    #region Beweging & Interactie

    /// <summary>
    /// Zorgt ervoor dat de structuur de speler volgt met soepele beweging (Lerp).
    /// </summary>
    private void HandlePlayerFollowing()
    {
        if (player == null) return;

        // Bepaal de doelpositie (voor de speler en iets omhoog)
        Vector3 targetPosition = player.transform.position +
                                 player.transform.forward * 1.5f +
                                 Vector3.up * 2f;

        // Maak de beweging soepel
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 10f);

        // Rotatie: Pas de custom rotatie toe op de rotatie van de speler
        Quaternion targetRotation = player.transform.rotation * Quaternion.Euler(pickedUpRotation);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
    }

    /// <summary>
    /// Controleert of de gedragen structuur overeenkomt met de nabije blauwdruk.
    /// Bij overeenkomst wordt de structuur direct uitgelijnd.
    /// </summary>
    /// <param name="blueprint">Het GameObject van de nabije blauwdruk.</param>
    /// <returns>True als de structuur bij de blauwdruk past, anders False.</returns>
    public bool BlueprintComparer(GameObject blueprint)
    {
        // 1. Validatie: Bestaat de blueprint en komt de naam overeen?
        // Let op: 'Contains' wordt gebruikt in de originele code, wat impliceert dat
        // de blueprint-naam de structuur-naam bevat (bv. "BlueprintWall" bevat "Wall").
        if (blueprint != null && blueprint.gameObject.name.Contains(this.structureName))
        {
            // 2. Uitlijning: Positioneer en roteer direct
            followPlayer = false; // Stop met volgen van de speler
            transform.position = blueprint.transform.position;
            transform.rotation = blueprint.transform.rotation;

            return true;
        }

        return false;
    }

    /// <summary>
    /// Finaliseert het bouwproces. Deactiveert de blauwdruk en maakt de structuur permanent.
    /// </summary>
    /// <param name="blueprint">Het GameObject van de blauwdruk om te deactiveren.</param>
    public void Build(GameObject blueprint)
    {
        // 1. Zet de staat vast
        followPlayer = false;
        canPickUp = false; // Kan niet meer worden opgepakt (essentieel voor game-logica)

        // 2. Deactiveer de blauwdruk
        if (blueprint != null)
        {
            blueprint.gameObject.SetActive(false);
        }

        // 3. Speel het bouwgeluid af
        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.Play();
        }
        else if (audioSource == null)
        {
            Debug.LogWarning("AudioSource mist op deze structuur.");
        }

        // Optioneel: Verwijder de "Structure" Tag hier als u wilt voorkomen dat de speler deze later nog vindt
        // gameObject.tag = "BuiltStructure"; 
    }

    #endregion
}