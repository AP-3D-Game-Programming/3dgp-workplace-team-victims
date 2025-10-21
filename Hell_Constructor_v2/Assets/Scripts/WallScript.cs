using UnityEngine;

public class WallScript : MonoBehaviour
{
    public bool followPlayer = false;

    private Rigidbody rbWall;
    private GameObject player;

    // Sound
    AudioSource aS;

    // Wordt één keer aangeroepen bij het starten van het script
    private void Start()
    {
        rbWall = GetComponent<Rigidbody>();
        player = GameObject.Find("Player");
        aS = GetComponent<AudioSource>();
    }

    // Wordt elke frame aangeroepen
    private void Update()
    {
        if (followPlayer)
        {


            // Plaats de muur boven de speler
            Vector3 followPosition = player.transform.position + Vector3.up * 2f;
            transform.position = followPosition;

            // Rotatie: zelfde richting als speler, maar gekanteld 90° om de X-as
            transform.rotation = player.transform.rotation * Quaternion.Euler(90f, 0f, 0f);

        }
    }

    // zet muur neer
    public void Build()
    {
        Collider col = GetComponentInChildren<Collider>();
        if (col == null) return;

        Vector3 buildPos = player.transform.position + player.transform.forward * 4f;

        if (Physics.Raycast(buildPos + Vector3.up * 5f, Vector3.down, out RaycastHit hit, 20f))
        {
            buildPos.y = hit.point.y + 2;
        }

        transform.position = buildPos;
        transform.rotation = Quaternion.Euler(0f, player.transform.eulerAngles.y, 0f);

        // Place sound
        aS.Play();
    }

}
