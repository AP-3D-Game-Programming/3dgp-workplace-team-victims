


using TMPro;
using UnityEngine;

public class PlayerBuildScript : MonoBehaviour
{
    private GameObject structure;
    public TextMeshProUGUI pickUpText;
    public TextMeshProUGUI buildText;
    private GameObject tempStructure; // Wordt ingesteld door CheckForNearbyStructure()
    private GameObject blueprint;
    private LevelManager levelManager;
    public bool isInMagazine;

    public bool canSpawn = true;
    private int[] structureAmountsLevels = { 5, 5 };
    public int buildAmount = 0;

    private AudioSource aS;
    public AudioClip errorSound;

    private UIScript ui;
    void Start()
    {
        structure = null;
        tempStructure = null; // Zorg ervoor dat deze ook null is bij de start
        aS = gameObject.GetComponent<AudioSource>();
        if (pickUpText != null) pickUpText.gameObject.SetActive(false);
        if (buildText != null) buildText.gameObject.SetActive(false);
        levelManager = gameObject.GetComponent<LevelManager>();
        ui = GameObject.Find("Canvas").GetComponent<UIScript>();
        isInMagazine = false;
    }

    void Update()
    {
        if (buildAmount == structureAmountsLevels[levelManager.level - 1])
        {
            buildAmount = 0;
            levelManager.newLevel();
            ui.playerObjective++;
        }
        // 1. Zorg ervoor dat tempStructure bijgewerkt wordt.
        bool structureNearby = CheckForNearbyStructure();

        // ------------------
        // LOGICA VOOR OPPAKKEN (structure == null)
        // ------------------
        if (structure == null)
        {
            // Belangrijke FIX: Controleer of een structuur gevonden is (tempStructure != null)
            if (structureNearby && tempStructure != null)
            {
                // Dubbele controle om NullReferenceException te voorkomen
                StructureScript script = tempStructure.GetComponent<StructureScript>();

                if (script != null && script.canPickUp)
                {
                    pickUpText.gameObject.SetActive(true);

                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        structure = tempStructure;
                        PickUpStructure();
                        if (ui.playerObjective == 2)
                            ui.playerObjective++;
                    }
                }
                else
                {
                    pickUpText.gameObject.SetActive(false);
                }
            }
            else
            {
                pickUpText.gameObject.SetActive(false);
            }
        }
        else // structure != null (We dragen een structuur)
        {
            pickUpText.gameObject.SetActive(false);

            // ------------------
            // LOGICA VOOR PLAATSEN (structure != null)
            // ------------------
            bool blueprintNearby = CheckForNearbyBlueprint();
            StructureScript structureScript = structure.GetComponent<StructureScript>();

            if (blueprintNearby)
            {
                // Blueprint is in de buurt, probeer uit te lijnen
                bool canBuild = structureScript.BlueprintComparer(blueprint);

                if (canBuild)
                {
                    // Uitleiding gelukt
                    buildText.gameObject.SetActive(true);
                    structureScript.followPlayer = false; // Blijf uitgelijnd

                    if (Input.GetKeyDown(KeyCode.B))
                    {
                        // Succesvolle plaatsing
                        structureScript.Build(blueprint);
                        buildText.gameObject.SetActive(false);
                        canSpawn = true;
                        if (!isInMagazine)
                            buildAmount++;
                        if (ui.playerObjective == 3)
                            ui.playerObjective++;
                        structure = null; // Laat de structuur los
                        // Na succesvolle bouw zal het geplaatste object in de volgende frame 
                        // niet opnieuw worden opgepakt omdat canPickUp = false is.
                    }
                }
                else
                {
                    // Kan niet bouwen (verkeerde blueprint/structuur match)
                    structureScript.followPlayer = true; // Volg de speler weer
                    buildText.gameObject.SetActive(false);
                }
            }
            else // Geen blueprint in de buurt
            {
                structureScript.followPlayer = true; // Blijf de speler volgen
                buildText.gameObject.SetActive(false);

                // Speel geluid af als de speler probeert te bouwen zonder blueprint
                if (Input.GetKeyDown(KeyCode.B))
                {
                    if (aS != null && errorSound != null)
                    {
                        aS.PlayOneShot(errorSound);
                    }
                }
            }
        }
    }

    private bool CheckForNearbyStructure()
    {
        GameObject[] structures = GameObject.FindGameObjectsWithTag("Structure");

        foreach (GameObject s in structures)
        {
            // Zorg ervoor dat de structuur oppakbaar is voordat we de afstand controleren
            StructureScript script = s.GetComponent<StructureScript>();
            if (script != null && script.canPickUp)
            {
                float distance = Vector3.Distance(transform.position, s.transform.position);

                if (distance < 2f)
                {
                    tempStructure = s;
                    return true;
                }
            }
        }

        tempStructure = null; // FIX: Reset tempStructure als er niets in de buurt is
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

        blueprint = null; // Reset blueprint als er niets in de buurt is
        return false;
    }

    private void PickUpStructure()
    {
        StructureScript structureScript = structure.GetComponent<StructureScript>();

        structureScript.followPlayer = true;
        pickUpText.gameObject.SetActive(false);
        // canSpawn = true; // Niet echt gebruikt, maar kan blijven staan.
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("temp"))
        {
            isInMagazine = true;
        }
    }

    public void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("temp"))
        {
            isInMagazine = false;
        }
    }
}