using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    float horizontalInput;
    float verticalInput;

    Rigidbody rb;
    public Camera playerCamera; // Sleep hier je main camera in in de inspector
    public float moveSpeed = 5f;

    private Vector3 moveDirection;
    private Vector3 normalizedMoveDirection;
    // Footstep sound system
    private AudioSource audioSource;
    public AudioClip step1;
    public AudioClip step2;
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
    }

    void FixedUpdate()
    {
        // Beweging toepassen

        rb.MovePosition(rb.position + normalizedMoveDirection * moveSpeed * Time.fixedDeltaTime);


        //rb.AddForce(moveDirection.normalized * moveSpeed * 10f);

        HandleFootsteps();

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

        // Alternate between the two clips
        AudioClip clipToPlay = playFirstStep ? step1 : step2;
        audioSource.PlayOneShot(clipToPlay);
        playFirstStep = !playFirstStep;

        yield return new WaitForSeconds(stepDelay);
        isStepping = false;
    }

    void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        //limit velocity if needed
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
        //reset y velocity
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.linearVelocity += Vector3.up * jumpForce;
        //rb.MovePosition(rb.position + transform.up * jumpForce * airMultiplier * Time.fixedDeltaTime);

        //rb.AddForce(transform.up * jumpCooldown, ForceMode.Impulse);
    }

    void ResetJump()
    {
        isReadyToJump = true;
    }
}
