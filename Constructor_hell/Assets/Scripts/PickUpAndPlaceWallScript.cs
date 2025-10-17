using TMPro;
using UnityEngine;

public class PickUpAndPlaceWallScript : MonoBehaviour
{
    public float checkRadius = 2.0f;
    public TextMeshProUGUI pickUpText;

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

        if (!hasWall)
        {
            pickUpText.gameObject.SetActive(wallNearby);
        } else
        {
            pickUpText.gameObject.SetActive(false);

        }

        if (wallNearby && Input.GetKeyDown(KeyCode.E))
        {
            PickUpWall();
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
}
