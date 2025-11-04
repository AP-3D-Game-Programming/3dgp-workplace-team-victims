using UnityEngine;
using TMPro;

/// <summary>
/// Beheert de interactie van de speler met SpawnZones. 
/// Gebruikt Raycasting om te detecteren of de speler naar een SpawnZone kijkt 
/// en triggert het spawnen van een nieuwe structuur bij input.
/// </summary>
public class SpawnManager : MonoBehaviour
{
    // -----------------------------------------------------------------------------------------------------------------

    #region Externe Verwijzingen & Configuratie

    [Header("Interactie Instellingen")]
    [Tooltip("Maximale afstand waarbinnen de speler kan interageren met een SpawnZone.")]
    public float interactDistance = 3f;

    [Header("UI & Componenten")]
    [Tooltip("De TextMeshProUGUI die de interactie prompt weergeeft.")]
    public TextMeshProUGUI popupText;

    // Deze moeten in Start() worden gevonden, dus we maken ze private.
    private Camera playerCamera;
    private GameObject playerBody;
    private UIScript ui;

    #endregion

    // -----------------------------------------------------------------------------------------------------------------

    #region Interne State

    // De momenteel gedetecteerde SpawnZone. Null als de speler niet naar een kijkt.
    private SpawnZone currentZone;

    #endregion

    // -----------------------------------------------------------------------------------------------------------------

    #region Unity Lifecycle Methods

    void Start()
    {
        // Zoek componenten en initialisatie
        playerCamera = Camera.main;

        // Zoek speler en UI (aangenomen dat ze in de scene bestaan)
        playerBody = GameObject.FindWithTag("Player"); // Gebruik FindWithTag voor robuustheid, of GameObject.Find("Player")
        GameObject canvas = GameObject.Find("Canvas");

        if (canvas != null)
        {
            ui = canvas.GetComponent<UIScript>();
        }

        if (popupText != null)
        {
            popupText.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        // 1. Controleer of de speler naar een SpawnZone kijkt
        CheckForSpawnZone();

        // 2. Verwerk de input (spawnen)
        HandleSpawnInput();

        // 3. UI/Tutorial progressie
        CheckUIObjectiveProgress();
    }

    #endregion

    // -----------------------------------------------------------------------------------------------------------------

    #region Interactie & Logica

    /// <summary>
    /// Gebruikt Raycasting om te bepalen of de speler naar een SpawnZone kijkt.
    /// Update de 'currentZone' en 'popupText'.
    /// </summary>
    void CheckForSpawnZone()
    {
        // Raycast van de camera uit
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
        {
            // Controleer of het object de "Button" tag heeft
            if (hit.collider.CompareTag("Button"))
            {
                // De SpawnZone component bevindt zich op de parent van de collider (de knop)
                SpawnZone detectedZone = hit.collider.GetComponentInParent<SpawnZone>();

                if (detectedZone != null)
                {
                    currentZone = detectedZone;
                    UpdatePopupText();
                    return; // Stop verdere checks in deze frame
                }
            }
        }

        // Als Raycast niets raakt, of het is geen geldige zone: Reset state
        currentZone = null;
        if (popupText != null)
        {
            popupText.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Stelt de tekst van de pop-up in op basis van de status van de SpawnZone.
    /// </summary>
    private void UpdatePopupText()
    {
        if (popupText == null) return;

        popupText.gameObject.SetActive(true);

        // Geef feedback over de resterende voorraad
        if (currentZone.amount <= 0)
        {
            popupText.text = $"Er zijn geen **{currentZone.structureName}** meer over!";
        }
        else
        {
            // Controleer of de speler een structuur kan spawnen
            PlayerBuildScript playerBuildScript = playerBody.GetComponent<PlayerBuildScript>();
            if (playerBuildScript != null && playerBuildScript.canSpawn)
            {
                popupText.text = $"Druk op **F** om een **{currentZone.structureName}** te spawnen.";
            }
            else
            {
                popupText.text = "Je draagt al een structuur!";
            }
        }
    }

    /// <summary>
    /// Verwerkt de 'F'-toets om een structuur te spawnen.
    /// </summary>
    void HandleSpawnInput()
    {
        if (currentZone == null || !Input.GetKeyDown(KeyCode.F)) return;

        // Haal de PlayerBuildScript op voor de 'canSpawn' state
        PlayerBuildScript playerBuildScript = playerBody.GetComponent<PlayerBuildScript>();

        // Alleen spawnen als er voorraad is EN de speler momenteel niets draagt
        if (currentZone.amount > 0 && playerBuildScript != null && playerBuildScript.canSpawn)
        {
            // 1. Spawn de structuur
            currentZone.SpawnStructure();

            // 2. Update de staat
            currentZone.amount--;
            playerBuildScript.canSpawn = false;

            // 3. Update UI
            if (popupText != null)
            {
                popupText.text = $"**{currentZone.structureName}** gespawned!";
            }

            // 4. Update UI Objective
            if (ui != null && ui.playerObjective == 1)
            {
                ui.playerObjective++;
            }
        }
        else if (currentZone.amount <= 0)
        {
            // Geen voorraad
            if (popupText != null)
            {
                popupText.text = "Er zijn geen meer over!";
            }
        }
        else // Speler draagt al iets (canSpawn == false)
        {
            if (popupText != null)
            {
                popupText.text = "Je draagt al een structuur!";
            }
        }
    }

    /// <summary>
    /// Update de UI objective als de speler een SpawnZone detecteert.
    /// </summary>
    private void CheckUIObjectiveProgress()
    {
        if (currentZone != null && ui != null && ui.playerObjective == 0)
        {
            ui.playerObjective++;
        }
    }

    #endregion
}