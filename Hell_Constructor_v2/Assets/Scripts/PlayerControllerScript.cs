using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    float horizontalInput;
    float verticalInput;

    Rigidbody rb;
    public Camera playerCamera;
    public float moveSpeed = 5f;

    // ADDED
    public float sprintMultiplier = 1.5f; // Sprint speed multiplier
    public KeyCode sprintKey = KeyCode.LeftShift; // Sprint key
    private bool isSprinting = false; // Track sprint state

    public int playerUI = 0;

    private Vector3 moveDirection;
    private Vector3 normalizedMoveDirection;

    // Footstep sound system
    private AudioSource audioSource;
    public AudioClip step1;
    public AudioClip step2;
    public AudioClip superSecret;
    public float stepDelay = 0.5f;

    private bool isStepping = false;
    private bool playFirstStep = true;

    //playerDrag
    public float playerHeight;
    public LayerMask whatIsGround;
    bool isGrounded;
    public float groundDrag;

    //jumping
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool isReadyToJump = true;
    public KeyCode jumpKey = KeyCode.Space;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        SpeedControl();
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.5f, whatIsGround);

        // Input ophalen
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // Sprint input // ADDED
        isSprinting = Input.GetKey(sprintKey);

        // Camera forward en right vector
        Vector3 forward = playerCamera.transform.forward;
        Vector3 right = playerCamera.transform.right;

        // Y-component nul maken
        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        // Beweging berekenen
        moveDirection = forward * verticalInput + right * horizontalInput;
        normalizedMoveDirection = moveDirection.normalized;

        //when to jump
        if (Input.GetKeyDown(jumpKey) && isReadyToJump && isGrounded)
        {
            isReadyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }

        //handle drag
        if (isGrounded)
        {
            rb.linearDamping = groundDrag;
        }
        else
        {
            rb.linearDamping = 0;
        }

        if (Input.GetKeyDown(KeyCode.W) && playerUI == 0)
        {
            playerUI++;
        }
    }

    void FixedUpdate()
    {
        // ADDED: adjust movement speed based on sprint state
        float currentSpeed;

        if (isSprinting)
        {
            currentSpeed = moveSpeed * sprintMultiplier;
            if (playerUI == 1)
            {
                playerUI++;
            }
        } else
        {
            currentSpeed = moveSpeed;
        }

        rb.MovePosition(rb.position + normalizedMoveDirection * currentSpeed * Time.fixedDeltaTime);

        HandleFootsteps();
        HandleSuperSecret();
    }

    void HandleFootsteps()
    {
        bool isMoving = moveDirection.magnitude > 0.1f;

        if (isMoving && !isStepping)
        {
            StartCoroutine(PlayStep());
        }
    }

    IEnumerator PlayStep()
    {
        isStepping = true;

        AudioClip clipToPlay = playFirstStep ? step1 : step2;
        audioSource.PlayOneShot(clipToPlay);
        playFirstStep = !playFirstStep;

        // Optional: shorten footstep delay while sprinting // ADDED
        float currentStepDelay = isSprinting ? stepDelay * 0.7f : stepDelay;

        yield return new WaitForSeconds(currentStepDelay);
        isStepping = false;
    }

    void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }

        if (isGrounded && rb.linearVelocity.y > 0f)
        {
            flatVel = rb.linearVelocity;
        }
    }

    void Jump()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.linearVelocity += Vector3.up * jumpForce;
        if (playerUI == 2)
            playerUI++;
    }

    void ResetJump()
    {
        isReadyToJump = true;
    }

    bool hasPlayedSecret = false;
    public Transform respawnPoint; // assign in Inspector or use code fallback

    void HandleSuperSecret()
    {
        float fallThreshold = -10f; // adjust for your map

        if (transform.position.y < fallThreshold && !hasPlayedSecret)
        {
            hasPlayedSecret = true;
            StartCoroutine(PlaySuperSecret());
        }

        if (transform.position.y >= fallThreshold && hasPlayedSecret)
        {
            hasPlayedSecret = false;
        }
    }

    IEnumerator PlaySuperSecret()
    {

        audioSource.PlayOneShot(superSecret);

        // Wait for the sound to finish before teleporting
        yield return new WaitForSeconds(superSecret.length);

        // Teleport player slightly above ground
        Vector3 respawnPos;

        if (respawnPoint != null)
        {
            respawnPos = respawnPoint.position + Vector3.up * 2f; // a bit above the respawn point
        }
        else
        {
            // fallback: teleport to (0, 5, 0)
            respawnPos = new Vector3(0, 5f, 0);
        }

        rb.linearVelocity = Vector3.zero; // stop falling fast
        rb.angularVelocity = Vector3.zero;
        transform.position = respawnPos;


    }



}
