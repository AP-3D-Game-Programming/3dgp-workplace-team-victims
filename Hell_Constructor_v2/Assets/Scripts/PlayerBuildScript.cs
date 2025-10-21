using TMPro;
using UnityEngine;

public class PlayerBuildScript : MonoBehaviour
{

    public TextMeshProUGUI pickUpText;
    public TextMeshProUGUI buildText;

    private GameObject wall;
    private bool hasWall = false;
    private bool inConstructionZone = false;

    public bool pickedUpWall = false;
    public bool placedWall = false;
    public bool gotToConstruction = false;

    // Sound
    private AudioSource aS;
    public AudioClip errorSound;

    // Called once before the first execution of Update
    private void Start()
    {
        aS = GetComponent<AudioSource>();
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
        }
        else
        {
            pickUpText.gameObject.SetActive(false);

        }

        if (wallNearby && Input.GetKeyDown(KeyCode.E) && !hasWall)
        {
            PickUpWall();
        }

        if (hasWall && Input.GetKeyDown(KeyCode.B) && inConstructionZone)
        {
            BuildWall();
        }

        if (hasWall && Input.GetKeyDown(KeyCode.B) && !inConstructionZone)
        {
            aS.PlayOneShot(errorSound);
        }



    }

    // Checks for nearby wall objects within the checkRadius
    private bool CheckForNearbyWall()
    {
        GameObject[] walls = GameObject.FindGameObjectsWithTag("Wall");

        foreach (GameObject w in walls)
        {
            float distance = Vector3.Distance(transform.position, w.transform.position);

            if (distance < 2f)
            {
                wall = w;
                return true;
            }
        }

        return false;
    }


    // Activates the wall's follow behavior
    private void PickUpWall()
    {
        hasWall = true;
        WallScript wallScript = wall.GetComponent<WallScript>();
        wallScript.followPlayer = true;
        pickedUpWall = true;
    }

    // place down wall and deactivate the wall's follow behavior
    private void BuildWall()
    {
        WallScript wallScript = wall.GetComponent<WallScript>();
        wallScript.followPlayer = false;
        wallScript.Build();
        hasWall = false;
        placedWall = true;


    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            buildText.gameObject.SetActive(true);
            gotToConstruction = true;
            inConstructionZone = true;


        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            buildText.gameObject.SetActive(false);
            inConstructionZone = false;
        }
    }
}
