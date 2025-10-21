using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    float horizontalInput;
    float verticalInput;

    private Rigidbody rb;
    public Camera playerCamera; // Sleep hier je main camera in in de inspector
    public float moveSpeed = 5f;

    private Vector3 moveDirection;

    // Footstep sound system
    private AudioSource audioSource;
    public AudioClip step1;
    public AudioClip step2;
    public float stepDelay = 0.5f;

    private bool isStepping = false;
    private bool playFirstStep = true;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        // Input ophalen
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

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


    }

    void FixedUpdate()
    {
        // Beweging toepassen
        rb.MovePosition(rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime);
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
}
