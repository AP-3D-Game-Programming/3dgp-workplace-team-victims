using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    // Movement
    private float lateralInput;
    private float forwardInput;
    public float speed = 5f;
    public Transform orientation;
    private Rigidbody rb;
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

        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        lateralInput = Input.GetAxis("Horizontal");
        forwardInput = Input.GetAxis("Vertical");

        moveDirection = orientation.forward * forwardInput + orientation.right * lateralInput;
        moveDirection.y = 0f;
        moveDirection.Normalize();

        transform.Translate(moveDirection * speed * Time.deltaTime, Space.World);

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
