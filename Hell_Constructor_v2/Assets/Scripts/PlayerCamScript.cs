using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    [Header("Sensitivity Settings")]
    public float sensX = 100;
    public float sensY = 100;

    private float xRotation;
    private float yRotation;

    private Transform player;
    private float mouseX;
    private float mouseY;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        player = GameObject.Find("Player").transform;
    }

    void Update()
    {
        // alleen input lezen
        mouseX = Input.GetAxis("Mouse X") * sensX * Time.deltaTime;
        mouseY = Input.GetAxis("Mouse Y") * sensY * Time.deltaTime;

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
    }

    void LateUpdate()
    {
        // pas rotatie toe in LateUpdate (na physics)
        transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        player.rotation = Quaternion.Euler(0, yRotation, 0);
    }
}
