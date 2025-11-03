using UnityEngine;
using TMPro;

public class SpawnManager : MonoBehaviour
{
    public float interactDistance = 3f;
    public TextMeshProUGUI popupText;

    private Camera playerCamera;
    private SpawnZone currentZone;


    public GameObject player;
    private UIScript ui;

    void Start()
    {
        playerCamera = Camera.main;
        popupText.gameObject.SetActive(false);
        player = GameObject.Find("Player");
        ui = GameObject.Find("Canvas").GetComponent<UIScript>();
    }

    void Update()
    {
        HandleRaycast();
        HandleInput();
        if (currentZone != null && ui.playerObjective == 0)
        {
            ui.playerObjective++;
        }
    }



    void HandleRaycast()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            if (hit.collider.CompareTag("Button"))
            {
                currentZone = hit.collider.GetComponentInParent<SpawnZone>();

                if (currentZone != null)
                {
                    popupText.gameObject.SetActive(true);

                    if (currentZone.amount == 0)
                        popupText.text = $"There are no more left!";
                    else
                        popupText.text = "Press F to drop off structure";
                }
                return;
            }
        }

        currentZone = null;
        popupText.gameObject.SetActive(false);
    }

    void HandleInput()
    {
        if (currentZone == null) return;

        if (Input.GetKeyDown(KeyCode.F))
        {

                if (currentZone.amount != 0 && player.GetComponent<PlayerBuildScript>().canSpawn)
                {
                    currentZone.SpawnStructure();
                    popupText.text = "Structure built!";
                    currentZone.amount--;
                    player.GetComponent<PlayerBuildScript>().canSpawn = false;
                if (ui.playerObjective == 1)
                    ui.playerObjective++;
                }
            else
            {
                popupText.text = "Already built here!";
            }
        }
    }
}
