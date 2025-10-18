using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //player movement
    private float lateralInput;
    private float forwardlInput;
    public float speed = 50.0f;
    Rigidbody rb;
    public Transform orientation;
    Vector3 moveDirection;
    Vector3 velocity;
    private float gravity;

    //jumping
    private float jumpForce;
    private float jumpCooldown;
    private float airMultiplier;
    private bool isReadyToJump;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }
    
    // Update is called once per frame
    void Update()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        lateralInput = Input.GetAxis("Horizontal");
        forwardlInput = Input.GetAxis("Vertical");
        
        //makes the player only move on the x and z axis
        moveDirection = orientation.forward * forwardlInput + orientation.right * lateralInput;
        moveDirection.y = 0f;
        moveDirection.Normalize();
        transform.Translate(moveDirection * speed * Time.deltaTime, Space.World);

        //gravity = Physics.gravity * mass * Time.deltaTime;
        //velocity.y = 

        //transform.Translate(orientation.forward * forwardlInput * speed * Time.deltaTime);
        //transform.Translate(orientation.right * lateralInput * speed * Time.deltaTime);

        //Vector3 moveDirection = (orientation.forward * forwardlInput) + (orientation.right * lateralInput);
        //moveDirection.Normalize();
        //transform.Translate(moveDirection * speed * Time.deltaTime, Space.World);

        //transform.Rotate(orientation.forward * forwardlInput + orientation.right * lateralInput);

        //rb.AddForce(moveDirection.normalized * speed * 10f, ForceMode.Force);

        //rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
    }
}
