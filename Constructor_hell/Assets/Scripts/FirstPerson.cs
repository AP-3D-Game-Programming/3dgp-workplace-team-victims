using UnityEngine;

public class FirstPerson : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    private float horizontalInput;
    private float turnSpeed = 100;

    
    // Update is called once per frame
    void LateUpdate()
    {

        

        horizontalInput = Input.GetAxis("Horizontal");
        transform.Rotate(Vector3.up * Time.deltaTime * turnSpeed * horizontalInput);

    }
}
