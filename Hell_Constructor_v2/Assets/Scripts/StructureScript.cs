using UnityEngine;

public class StructureScript : MonoBehaviour
{
    public bool followPlayer = false;
    private GameObject player;
    // Gebruik een Vector3 voor rotatie (Euler hoeken) om het makkelijker te maken in de Inspector
    public Vector3 pickedUpRotation = Vector3.zero;
    public bool canPickUp = true;
    private AudioSource aS;
    public string structureName;

    void Start()
    {
        // Zoek de speler met Find, dit is prima voor Start()
        player = GameObject.Find("Player");
        // Optioneel: Controleer of de AudioSource bestaat
        aS = gameObject.GetComponent<AudioSource>();
    }

    void Update()
    {
        if (followPlayer)
        {
            // Plaats de structuur voor/boven de speler (voor een beter zicht)
            // Dit is een simpele manier om het voor de speler te houden
            Vector3 targetPosition = player.transform.position + player.transform.forward * 1.5f + Vector3.up * 2f;

            // Gebruik Lerp om de beweging soepeler te maken
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 10f);

            // Rotatie: Pas de custom rotatie toe op de rotatie van de speler
            Quaternion targetRotation = player.transform.rotation * Quaternion.Euler(pickedUpRotation);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }

    public bool BlueprintComparer(GameObject blueprint)
    {
        // Controleer of de blueprint bestaat en of de namen overeenkomen
        if (blueprint != null && blueprint.gameObject.name.Contains(this.structureName))
        {
            // Lijn direct uit met de blueprint
            // followPlayer wordt in de PlayerBuildScript aan/uit gezet, 
            // maar we zetten het hier uit voor directe alignment.
            followPlayer = false;
            transform.position = blueprint.transform.position;
            transform.rotation = blueprint.transform.rotation;

            return true;
        }
        return false;
    }

    public void Build(GameObject blueprint)
    {
        // Eenmaal gebouwd:
        followPlayer = false;
        if (blueprint != null)
        {
            blueprint.gameObject.SetActive(false);
        }

        // Dit is CRUCIAAL voor de 'bounce back' fix: 
        // De geplaatste structuur kan NIET meer worden opgepakt.
        canPickUp = false;

        if (aS != null)
        {
            aS.Play();
        }


        // Optioneel: Verwijder de "Structure" Tag hier als je wilt voorkomen dat de speler hem later nog vindt
        // gameObject.tag = "BuiltStructure"; 
    }
}