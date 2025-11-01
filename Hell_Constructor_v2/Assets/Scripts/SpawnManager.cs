using UnityEngine;
using TMPro;

public class SpawnManager : MonoBehaviour
{
    public float interactDistance = 3f;
    public TextMeshProUGUI popupText;

    private Camera playerCamera;
    private SpawnZone currentZone;

    void Start()
    {
        playerCamera = Camera.main;
        popupText.gameObject.SetActive(false);
    }

    void Update()
    {
        HandleRaycast();
        HandleInput();
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

                    if (currentZone.HasStructure())
                        popupText.text = "A structure is already built here";
                    else
                        popupText.text = "Press F to build structure";
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
            if (!currentZone.HasStructure())
            {
                currentZone.SpawnStructure();
                popupText.text = "Structure built!";
            }
            else
            {
                popupText.text = "Already built here!";
            }
        }
    }
}
