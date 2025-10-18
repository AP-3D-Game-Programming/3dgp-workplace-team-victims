using TMPro;
using UnityEngine;

public class PickUpAndPlaceWallScript : MonoBehaviour
{
    public float checkRadius = 2.0f;
    public TextMeshProUGUI pickUpText;
    public TextMeshProUGUI buildText;

    private GameObject wall;
    private bool hasWall = false;

    // Called once before the first execution of Update
    private void Start()
    {
        
    }

    // Called once per frame
    private void Update()
    {
        bool wallNearby = CheckForNearbyWall();

        // show text
        if (!hasWall)
        {
            pickUpText.gameObject.SetActive(wallNearby);
            buildText.gameObject.SetActive(false);
        } else
        {
            pickUpText.gameObject.SetActive(false);
            buildText.gameObject.SetActive(true);

        }

        if (wallNearby && Input.GetKeyDown(KeyCode.E) && !hasWall)
        {
            PickUpWall();
        }

        if (hasWall && Input.GetKeyDown(KeyCode.B))
        {
            BuildWall();
        }
    }

    // Checks for nearby wall objects within the checkRadius
    private bool CheckForNearbyWall()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, checkRadius);

        foreach (Collider hit in hitColliders)
        {
            if (hit.CompareTag("Wall"))
            {
                wall = hit.gameObject;
                return true;
            }
        }

        return false;
    }

    // Activates the wall's follow behavior
    private void PickUpWall()
    {
        hasWall = true;
        FollowPlayer_WallScript wallScript = wall.GetComponent<FollowPlayer_WallScript>();
        wallScript.followPlayer = true;
    }

    // place down wall and deactivate the wall's follow behavior
    private void BuildWall()
    {
        FollowPlayer_WallScript wallScript = wall.GetComponent<FollowPlayer_WallScript>();
        wallScript.followPlayer = false;
        wallScript.Build();
        hasWall = false;


    }
}
