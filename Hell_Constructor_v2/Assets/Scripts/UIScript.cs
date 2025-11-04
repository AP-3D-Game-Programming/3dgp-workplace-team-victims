using TMPro;
using UnityEngine;

public class UIScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private GameObject player;
    private PlayerController playerController;
    private PlayerBuildScript playerBuildScript;
    private LevelManager levelManager;
    public TextMeshProUGUI tutorialText;
    public TextMeshProUGUI objectiveText;
    public TextMeshProUGUI magasinText;
    public TextMeshProUGUI bossText;


    public int playerObjective;
    void Start()
    {
        player = GameObject.Find("Player");
        playerController = player.GetComponent<PlayerController>();
        playerBuildScript = player.GetComponent<PlayerBuildScript>();
        levelManager = player.GetComponent<LevelManager>();
        tutorialText.gameObject.SetActive(true);
        objectiveText.gameObject.SetActive(true);
        magasinText.gameObject.SetActive(true);
        bossText.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        switch (playerController.playerUI)
        {
            case 0:
                tutorialText.text = @"Use mouse to look around
use wasd to move";
                break;
            case 1:
                tutorialText.text = "use shift to sprint";
                break;
            case 2:
                tutorialText.text = "use space to jump";
                break;
            default:
                tutorialText.gameObject.SetActive(false);
                break;
        }

        switch (playerObjective)
        {
            case 0:
                objectiveText.text = "objective: go to magasin";
                break;
            case 1:
                objectiveText.text = "objective: push button to drop off structure";
                break;
            case 2:
                objectiveText.text = "objective: pick up structure";
                break;
            case 3:
                objectiveText.text = "objective: place structure on matching blueprint";
                break;
            case 4:
                objectiveText.text = "objective: build the house";
                break;
            case 5:
                objectiveText.text = "objective: build the second house";
                break;
            default:
                tutorialText.gameObject.SetActive(false);
                break;
        }

        GameObject[] spawnZones = GameObject.FindGameObjectsWithTag("Spawn");
        int[] amounts = { 0, 0, 0 };
        foreach (var z in spawnZones)
        {
            SpawnZone zone = z.GetComponent<SpawnZone>();
            switch (zone.structureName)
            {
                case "Wall":
                    amounts[0] = zone.amount;
                    break;
                case "Floor":
                    amounts[1] = zone.amount;
                    break;
                case "Doorway":
                    amounts[2] = zone.amount;
                    break;
            }
        }

        magasinText.text = @$"Walls: {amounts[0]}   Floors: {amounts[1]}    Doorway: {amounts[2]}";

        if (levelManager.level == 2)
        {
            bossText.gameObject.SetActive(true);
            bossText.text = "Oops, there is a wall missing in the delivery. Do what you can and steal from your other house!";
        }


    }
}