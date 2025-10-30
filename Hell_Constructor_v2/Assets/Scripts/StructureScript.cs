using UnityEngine;

public class StructureScript : MonoBehaviour
{
    public bool followPlayer = false;
    private GameObject player;
    public float pickedUpRotation;
    public bool canPickUp = true;
    private AudioSource aS;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.Find("Player");
        aS = gameObject.GetComponent<AudioSource>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (followPlayer)
        {

         // Plaats de muur boven de speler
            Vector3 followPosition = player.transform.position + Vector3.up * 2f;
            transform.position = followPosition;

            // Rotatie: zelfde richting als speler
            transform.rotation = player.transform.rotation * Quaternion.Euler(pickedUpRotation, 0f, 0f);
        }
        
    }

    public bool BlueprintComparer(GameObject blueprint)
    {
        if (blueprint.gameObject.name.Contains(gameObject.name))
        {
            followPlayer = false;
            transform.position = blueprint.transform.position;
            transform.rotation = blueprint.transform.rotation;
            
            return true;
        }
        return false;
    }

    public void Build(GameObject blueprint)
    {
        followPlayer = false;
        blueprint.gameObject.SetActive(false);
        canPickUp = false;
        aS.Play();
    }
}
