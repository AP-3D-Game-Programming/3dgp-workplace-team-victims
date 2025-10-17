using UnityEngine;

public class FollowPlayer_WallScript : MonoBehaviour
{
    public bool followPlayer = false;

    private Rigidbody rbWall;
    private GameObject player;

    // Wordt één keer aangeroepen bij het starten van het script
    private void Start()
    {
        rbWall = GetComponent<Rigidbody>();
        player = GameObject.Find("Player");
    }

    // Wordt elke frame aangeroepen
    private void Update()
    {
        if (followPlayer)
        {

            rbWall.useGravity = false;

            // Plaats de muur boven de speler
            Vector3 followPosition = player.transform.position + Vector3.up * 2f;
            transform.position = followPosition;

            // Rotatie vastzetten op 90 graden
            transform.rotation = Quaternion.Euler(0f, player.transform.eulerAngles.y, 90f);
        }
    }
}
