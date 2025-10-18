using TMPro;
using UnityEngine;

public class ShowObjectives : MonoBehaviour
{
    private PickUpAndPlaceWallScript buildScript;
    public TextMeshProUGUI objectiveText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        buildScript = gameObject.GetComponent<PickUpAndPlaceWallScript>();
        objectiveText.text = "Objective: Pick up wall.";
    }

    // Update is called once per frame
    void Update()
    {
        if (buildScript.pickedUpWall && !buildScript.gotToConstruction && !buildScript.placedWall)
        {
            objectiveText.text = "Objective: Go to construction Zone. (light grey)";
        }
        else if (buildScript.pickedUpWall && buildScript.gotToConstruction && !buildScript.placedWall)
        {
            objectiveText.text = "Objective: Build wall.";
        } 
        else if (buildScript.pickedUpWall && buildScript.gotToConstruction && buildScript.placedWall)
        {
            objectiveText.text = "Objective: Have fun!";

        }
    }
}
