using TMPro;
using UnityEditor.PackageManager;
using UnityEngine;

public class PlayerBuildScript : MonoBehaviour
{
    private GameObject structure;
    public TextMeshProUGUI pickUpText;
    public TextMeshProUGUI buildText;
    private GameObject tempStructure;
    private GameObject blueprint;

    public bool canSpawn = true;

    private AudioSource aS;
    public AudioClip errorSound;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        structure = null;
        aS = gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        bool StructureNearby = CheckForNearbyStructure();

        if (structure == null)
        {
            if (tempStructure.gameObject.GetComponent<StructureScript>().canPickUp)
            {
                pickUpText.gameObject.SetActive(StructureNearby);
                if (Input.GetKeyDown(KeyCode.E) && structure == null)
                {
                    structure = tempStructure;
                    PickUpStructure();
                }
            }
        }
        else
        {
            pickUpText.gameObject.SetActive(false);

        }

        bool blueprintNearby = CheckForNearbyBlueprint();


        if (blueprintNearby && structure != null)
        {
            StructureScript structureScript = structure.GetComponent<StructureScript>();
            bool canBuild = structureScript.BlueprintComparer(blueprint);
            if (canBuild)
            {
                buildText.gameObject.SetActive(true);
                if (Input.GetKeyDown(KeyCode.B))
                {
                    structureScript.Build(blueprint);
                    structure = null;
                }
            }
        }
        else if (!blueprintNearby && structure != null)
        {
            StructureScript structureScript = structure.GetComponent<StructureScript>();
            structureScript.followPlayer = true;
            buildText.gameObject.SetActive(false);

        } else
        {
            buildText.gameObject.SetActive(false);

        }

        if (!blueprintNearby && structure != null && Input.GetKeyDown(KeyCode.B))
        {
            aS.PlayOneShot(errorSound);
        }


    }

    private bool CheckForNearbyStructure()
    {
        GameObject[] structures = GameObject.FindGameObjectsWithTag("Structure");

        foreach (GameObject s in structures)
        {
            float distance = Vector3.Distance(transform.position, s.transform.position);

            if (distance < 2f)
            {
                tempStructure = s;
                return true;
            }
        }

        return false;
    }

    private bool CheckForNearbyBlueprint()
    {
        GameObject[] blueprints = GameObject.FindGameObjectsWithTag("Blueprint");

        foreach (GameObject b in blueprints)
        {
            float distance = Vector3.Distance(transform.position, b.transform.position);

            if (distance < 6f)
            {
                blueprint = b;
                return true;
            }
        }

        return false;
    }

    private void PickUpStructure()
    {
        StructureScript structureScript = structure.GetComponent<StructureScript>();

        structureScript.followPlayer = true;
        pickUpText.gameObject.SetActive(false);
        canSpawn = true;
    }
}
