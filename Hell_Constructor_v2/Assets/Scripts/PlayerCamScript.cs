using UnityEngine;

/// <summary>
/// Beheert de first-person camera beweging (look-around) en de spelerrotatie
/// op basis van muis-input.
/// </summary>
public class PlayerCam : MonoBehaviour
{
    // -----------------------------------------------------------------------------------------------------------------

    #region Instelbare Camera Parameters

    [Header("Gevoeligheidsinstellingen")]
    [Tooltip("De horizontale gevoeligheid van de muis (Mouse X).")]
    public float sensX = 100f;

    [Tooltip("De verticale gevoeligheid van de muis (Mouse Y).")]
    public float sensY = 100f;

    [Tooltip("De Transform van het Player-object. Wordt ingesteld in Start().")]
    [SerializeField] // Zorg ervoor dat deze zichtbaar is in de Inspector, maar niet publiekelijk wijzigbaar van buitenaf
    private Transform playerBody;

    #endregion

    // -----------------------------------------------------------------------------------------------------------------

    #region Interne Rotatie State

    private float xRotation; // Huidige verticale rotatie (pitch) van de camera
    private float yRotation; // Huidige horizontale rotatie (yaw) van de speler

    #endregion

    // -----------------------------------------------------------------------------------------------------------------

    #region Unity Lifecycle Methods

    /// <summary>
    /// Wordt aangeroepen bij de start van het script.
    /// Vergrendelt de cursor en vindt de speler-transform.
    /// </summary>
    void Start()
    {
        // 1. Cursor vergrendelen en verbergen voor FPS-besturing
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // 2. Zoek de Transform van het speler-object (wordt gebruikt voor horizontale rotatie)
        // Ervan uitgaande dat de camera een kind is van de speler of dat de speler de tag "Player" heeft.
        // Aangezien de originele code 'GameObject.Find("Player")' gebruikt, wordt dit behouden.
        GameObject playerObject = GameObject.Find("Player");
        if (playerObject != null)
        {
            playerBody = playerObject.transform;
        }
        else
        {
            Debug.LogError("GameObject genaamd 'Player' niet gevonden! Camera-besturing werkt mogelijk niet correct.");
        }
    }

    /// <summary>
    /// Wordt elke frame aangeroepen.
    /// Leest de muis-input en berekent de nieuwe rotatiewaarden.
    /// </summary>
    void Update()
    {
        // 1. Lees ruwe muis-input
        float mouseX = Input.GetAxis("Mouse X") * sensX * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensY * Time.deltaTime;

        // 2. Bereken de horizontale rotatie (Yaw - beïnvloedt de speler)
        yRotation += mouseX;

        // 3. Bereken de verticale rotatie (Pitch - beïnvloedt de camera)
        xRotation -= mouseY;

        // 4. Beperk de verticale rotatie om overstrekking te voorkomen (bv. 90 graden omhoog/omlaag)
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
    }

    /// <summary>
    /// Wordt na Update() aangeroepen.
    /// Past de berekende rotaties toe op de camera en de speler.
    /// </summary>
    void LateUpdate()
    {
        if (playerBody == null) return; // Vroege exit als de speler niet gevonden is

        // Camera Rotatie (alleen verticaal - Pitch)
        // De camera zelf roteert alleen lokaal op de X-as.
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Speler Rotatie (alleen horizontaal - Yaw)
        // De speler roteert horizontaal, waardoor de camera meedraait.
        playerBody.rotation = Quaternion.Euler(0f, yRotation, 0f);
    }

    #endregion
}