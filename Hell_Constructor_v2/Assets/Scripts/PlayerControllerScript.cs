using System.Collections;
using UnityEngine;

/// <summary>
/// Beheert de beweging, sprongen, snelheid, voetstappen en val-respawn logica van de speler.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class PlayerController : MonoBehaviour
{
    // -----------------------------------------------------------------------------------------------------------------

    #region Componenten & Externe Verwijzingen

    [Header("Componenten & Verwijzingen")]
    [Tooltip("De Rigidbody component van de speler.")]
    private Rigidbody rb;

    [Tooltip("De camera die de speler volgt (wordt gebruikt voor richtingsberekening).")]
    public Camera playerCamera;

    [Tooltip("De AudioSource component van de speler.")]
    private AudioSource audioSource;

    [Tooltip("Het punt waar de speler na een val respawnt. Optioneel in te stellen.")]
    public Transform respawnPoint;

    #endregion

    // -----------------------------------------------------------------------------------------------------------------

    #region Bewegingsinstellingen

    [Header("Bewegingsinstellingen")]
    [Tooltip("De basissnelheid van de speler.")]
    public float moveSpeed = 5f;

    [Tooltip("Multiplier voor de sprintsnelheid.")]
    public float sprintMultiplier = 1.5f;

    [Tooltip("De toets die gebruikt wordt om te sprinten.")]
    public KeyCode sprintKey = KeyCode.LeftShift;

    // Jumping
    [Header("Springinstellingen")]
    public float jumpForce = 7f;
    public float jumpCooldown = 0.25f;
    public float airMultiplier = 0.4f; // Wordt momenteel niet volledig gebruikt in FixedUpdate/MovePlayer
    public KeyCode jumpKey = KeyCode.Space;

    // Gronddetectie & Drag
    [Header("Gronddetectie & Fysica")]
    [Tooltip("De hoogte van de speler (gebruikt voor Raycast).")]
    public float playerHeight = 2f;

    [Tooltip("De LayerMask die grondobjecten definieert.")]
    public LayerMask whatIsGround;

    [Tooltip("De lineaire demping (drag) op de grond.")]
    public float groundDrag = 5f;

    [Tooltip("Drempelwaarde voor de Y-positie om de geheime audio te triggeren en te respawnen.")]
    public float fallThreshold = -10f;

    #endregion

    // -----------------------------------------------------------------------------------------------------------------

    #region Audio Instellingen

    [Header("Audio Instellingen")]
    public AudioClip step1;
    public AudioClip step2;
    public AudioClip superSecret;
    public float stepDelay = 0.5f;

    #endregion

    // -----------------------------------------------------------------------------------------------------------------

    #region Interne State Variabelen

    // Input & Beweging
    private float horizontalInput;
    private float verticalInput;
    private Vector3 moveDirection;
    private Vector3 normalizedMoveDirection;
    private bool isSprinting = false;

    // Jumping
    private bool isGrounded;
    private bool isReadyToJump = true;

    // Footsteps
    private bool isStepping = false;
    private bool playFirstStep = true;

    // Secret & UI
    private bool hasPlayedSecret = false;

    [Tooltip("Tracker voor UI/Tutorial progressie.")]
    public int playerUI = 0;

    #endregion

    // -----------------------------------------------------------------------------------------------------------------

    #region Unity Lifecycle Methods

    void Start()
    {
        // Initialiseer componenten
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();

        // Zorg ervoor dat de speler niet roteert door fysica
        rb.freezeRotation = true;
    }

    void Update()
    {
        // 1. Gronddetectie (Raycast)
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.5f, whatIsGround);

        // 2. Input verwerken
        HandleInput();

        // 3. Drag en Jump logica
        HandleDrag();
        HandleJumpInput();

        // 4. UI Progressie (specifieke input)
        HandleUIProgress();
    }

    void FixedUpdate()
    {
        // 1. Beweging toepassen (Fysica)
        MovePlayer();

        // 2. Snelheid en limieten controleren (Fysica)
        SpeedControl();

        // 3. Audio & Geheime logica
        HandleFootsteps();
        HandleSuperSecret();
    }

    #endregion

    // -----------------------------------------------------------------------------------------------------------------

    #region Bewegingslogica

    /// <summary>
    /// Leest de bewegings- en sprint-input en berekent de bewegingsrichting.
    /// </summary>
    private void HandleInput()
    {
        // Bewegings-input
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // Sprint-input
        isSprinting = Input.GetKey(sprintKey);

        // Camera richtingen
        Vector3 forward = playerCamera.transform.forward;
        Vector3 right = playerCamera.transform.right;

        // Maak Y-component nul voor beweging op het platte vlak
        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        // Finale bewegingsrichting
        moveDirection = forward * verticalInput + right * horizontalInput;
        normalizedMoveDirection = moveDirection.normalized;
    }

    /// <summary>
    /// Past de daadwerkelijke beweging toe via Rigidbody.MovePosition.
    /// </summary>
    private void MovePlayer()
    {
        // Bereken de huidige snelheid inclusief sprint-multiplier
        float currentSpeed = isSprinting ? moveSpeed * sprintMultiplier : moveSpeed;

        // Update UI als de speler sprint
        if (isSprinting && playerUI == 1)
        {
            playerUI++;
        }

        // Pas beweging toe
        rb.MovePosition(rb.position + normalizedMoveDirection * currentSpeed * Time.fixedDeltaTime);
    }

    /// <summary>
    /// Controleert of de speler springt en triggert de sprong.
    /// </summary>
    private void HandleJumpInput()
    {
        if (Input.GetKeyDown(jumpKey) && isReadyToJump && isGrounded)
        {
            isReadyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    /// <summary>
    /// Voegt sprongkracht toe aan de Rigidbody.
    /// </summary>
    private void Jump()
    {
        // Reset verticale snelheid om consistente sprongen te garanderen
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        // Voeg de sprongkracht toe
        rb.linearVelocity += Vector3.up * jumpForce;

        // Update UI
        if (playerUI == 2)
            playerUI++;
    }

    /// <summary>
    /// Reset de sprong-cooldown.
    /// </summary>
    private void ResetJump()
    {
        isReadyToJump = true;
    }

    /// <summary>
    /// Stelt de groundDrag in afhankelijk van of de speler de grond raakt.
    /// </summary>
    private void HandleDrag()
    {
        // Pas ground drag toe als de speler op de grond staat
        rb.linearDamping = isGrounded ? groundDrag : 0f;
    }

    /// <summary>
    /// Beperkt de snelheid van de speler tot moveSpeed (of sprintSpeed).
    /// Let op: deze logica werkt mogelijk niet optimaal met Rigidbody.MovePosition().
    /// </summary>
    private void SpeedControl()
    {
        // LET OP: Met Rigidbody.MovePosition() wordt de snelheid niet door de fysica gemanipuleerd.
        // Deze methode is over het algemeen meer geschikt voor AddForce() beweging.

        // Bereken de maximale toegestane snelheid
        float maxSpeed = isSprinting ? moveSpeed * sprintMultiplier : moveSpeed;

        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        // Limiteer de snelheid op het platte vlak
        if (flatVel.magnitude > maxSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * maxSpeed;
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }

        // De tweede 'if' (if (isGrounded && rb.linearVelocity.y > 0f)) lijkt een overblijfsel en is verwarrend.
        // Dit deel wordt genegeerd voor een zuivere snelheidscontrole.
    }

    #endregion

    // -----------------------------------------------------------------------------------------------------------------

    #region Audio & Secret Logic

    /// <summary>
    /// Start de Coroutine voor voetstappen als de speler beweegt en op de grond staat.
    /// </summary>
    private void HandleFootsteps()
    {
        bool isMoving = moveDirection.magnitude > 0.1f; // Controleer of er input is

        // Voeg controle toe om alleen stappen af te spelen op de grond
        if (isMoving && isGrounded && !isStepping)
        {
            StartCoroutine(PlayStep());
        }
    }

    /// <summary>
    /// Speelt een voetstapgeluid af en wacht de ingestelde vertraging.
    /// </summary>
    private IEnumerator PlayStep()
    {
        isStepping = true;

        // Wissel tussen step1 en step2
        AudioClip clipToPlay = playFirstStep ? step1 : step2;
        if (audioSource != null && clipToPlay != null)
        {
            audioSource.PlayOneShot(clipToPlay);
        }
        playFirstStep = !playFirstStep;

        // Verkort de vertraging tijdens het sprinten
        float currentStepDelay = isSprinting ? stepDelay * 0.7f : stepDelay;

        yield return new WaitForSeconds(currentStepDelay);
        isStepping = false;
    }

    /// <summary>
    /// Controleert de Y-positie van de speler om het geheime geluid en respawn te triggeren.
    /// </summary>
    private void HandleSuperSecret()
    {
        if (transform.position.y < fallThreshold && !hasPlayedSecret)
        {
            hasPlayedSecret = true;
            StartCoroutine(PlaySuperSecret());
        }

        // Reset de trigger als de speler terug boven de drempel komt
        if (transform.position.y >= fallThreshold && hasPlayedSecret)
        {
            hasPlayedSecret = false;
        }
    }

    /// <summary>
    /// Speelt het geheime geluid af en teleporteert de speler daarna naar het respawn punt.
    /// </summary>
    private IEnumerator PlaySuperSecret()
    {
        if (audioSource != null && superSecret != null)
        {
            audioSource.PlayOneShot(superSecret);
            // Wacht tot het geluid is afgelopen voordat we teleporteren
            yield return new WaitForSeconds(superSecret.length);
        }

        // Bereken de respawn positie
        Vector3 respawnPos;
        if (respawnPoint != null)
        {
            respawnPos = respawnPoint.position + Vector3.up * 2f; // Iets boven het respawn punt
        }
        else
        {
            // Fallback: teleport naar (0, 5, 0)
            respawnPos = new Vector3(0, 5f, 0);
        }

        // Reset de fysica en verplaats de speler
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.position = respawnPos;
    }

    #endregion

    // -----------------------------------------------------------------------------------------------------------------

    #region UI/Tutorial Logic

    /// <summary>
    /// Verwerkt de stappen in de UI/Tutorial progressie.
    /// </summary>
    private void HandleUIProgress()
    {
        // Eerste UI stap (W-toets)
        if (Input.GetKeyDown(KeyCode.W) && playerUI == 0)
        {
            playerUI++;
        }
        // Sprint (playerUI == 1) en Springen (playerUI == 2) worden al in MovePlayer() en Jump() afgehandeld.
    }

    #endregion
}