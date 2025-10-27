using TMPro;
using UnityEngine;

public class PlayerBuildScript : MonoBehaviour
{

    public enum Structures
    {
        wall
    }

    public TextMeshProUGUI pickUpText;
    public TextMeshProUGUI buildText;

    private GameObject Structure;
    private bool hasStructure = false;
    private bool inConstructionZone = false;

    public bool pickedUpStructure = false;
    public bool placedStructure = false;
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
        bool StructureNearby = CheckForNearbyStructure();

        // show text
        if (!hasStructure)
        {
            pickUpText.gameObject.SetActive(StructureNearby);
            buildText.gameObject.SetActive(false);
        }
        else
        {
            pickUpText.gameObject.SetActive(false);

        }

        if (StructureNearby && Input.GetKeyDown(KeyCode.E) && !hasStructure)
        {
            PickUpStructure();
        }




    }

    // Checks for nearby wall objects within the checkRadius
    private bool CheckForNearbyStructure()
    {
        GameObject[] structures = GameObject.FindGameObjectsWithTag("Structure");

        foreach (GameObject s in structures)
        {
            float distance = Vector3.Distance(transform.position, s.transform.position);

            if (distance < 2f)
            {
                Structure = s;
                return true;
            }
        }

        return false;
    }


    // Activates the wall's follow behavior
    private void PickUpStructure()
    {
        hasStructure = true;
        WallScript wallScript = Structure.GetComponent<WallScript>();
        wallScript.followPlayer = true;
        pickedUpStructure = true;
    }

    // place down wall and deactivate the wall's follow behavior
    private void BuildWall()
    {
        WallScript wallScript = Structure.GetComponent<WallScript>();
        wallScript.followPlayer = false;
        wallScript.Build();
        hasStructure = false;
        placedStructure = true;


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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Blueprint"))
        {

        }
    }
}
