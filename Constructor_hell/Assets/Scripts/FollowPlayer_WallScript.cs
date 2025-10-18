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


            // Plaats de muur boven de speler
            Vector3 followPosition = player.transform.position + Vector3.up * 5f;
            transform.position = followPosition;

            // Rotatie vastzetten op 90 graden
            transform.rotation = Quaternion.Euler(0f, player.transform.eulerAngles.y, 90f);
        }
    }

    // zet muur neer
    public void Build()
    {
        Collider col = GetComponentInChildren<Collider>();
        if (col == null) return;

        Vector3 buildPos = player.transform.position + player.transform.forward * 20f;

        if (Physics.Raycast(buildPos + Vector3.up * 5f, Vector3.down, out RaycastHit hit, 20f))
        {
            buildPos.y = hit.point.y + 100;
        }

        transform.position = buildPos;
        transform.rotation = Quaternion.Euler(0f, player.transform.eulerAngles.y - 90f, 0f);
    }

}
