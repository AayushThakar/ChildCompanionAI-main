using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParkManager : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject treePrefab;
    public GameObject sidewalkPrefab;
    public GameObject childPrefab;
    public GameObject strangerPrefab;
    public GameObject dogPrefab;

    [Header("Park Layout Settings")]
    public int numberOfTrees = 10;
    public float parkWidth = 20f;
    public float parkLength = 20f;
    public float sidewalkWidth = 2f;

    [Header("Child & Stranger Settings")]
    public Vector3 childPosition = new Vector3(0, 0, 0);
    public int numberOfStrangers = 3;
    public float strangerRadius = 5f;

    private GameObject child;
    private GameObject dog;
    private List<GameObject> strangers = new List<GameObject>();

    void Start()
    {
        CreateSidewalk();
        PlaceTrees();
        PlaceChildAndDog();
        PlaceStrangers();
    }

    void CreateSidewalk()
    {
        GameObject sidewalk = Instantiate(sidewalkPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        sidewalk.transform.localScale = new Vector3(parkWidth, 1, sidewalkWidth);
        sidewalk.transform.position = new Vector3(0, 0.1f, -parkLength / 2 + sidewalkWidth / 2);
    }

    void PlaceTrees()
    {
        for (int i = 0; i < numberOfTrees; i++)
        {
            float x = Random.Range(-parkWidth / 2, parkWidth / 2);
            float z = Random.Range(-parkLength / 2, parkLength / 2);

            Vector3 position = new Vector3(x, 0, z);
            Instantiate(treePrefab, position, Quaternion.identity);
        }
    }

    void PlaceChildAndDog()
    {
        // Instantiate the child
        child = Instantiate(childPrefab, childPosition, Quaternion.identity);

        // Instantiate the dog at the offset position
        Vector3 initialDogPosition = childPosition + new Vector3(-1f, 0, -1f); // Offset for dog position
        dog = Instantiate(dogPrefab, initialDogPosition, Quaternion.identity);

        // Get the DogController component and initialize it
        DogController dogController = dog.GetComponent<DogController>();
        if (dogController != null)
        {
            dogController.Initialize(child);
        }
        else
        {
            Debug.LogError("DogController component not found on dogPrefab.");
        }
    }

    void PlaceStrangers()
    {
        for (int i = 0; i < numberOfStrangers; i++)
        {
            float angle = i * Mathf.PI * 2 / numberOfStrangers;
            float x = Mathf.Cos(angle) * strangerRadius + childPosition.x;
            float z = Mathf.Sin(angle) * strangerRadius + childPosition.z;

            Vector3 position = new Vector3(x, 0, z);
            GameObject stranger = Instantiate(strangerPrefab, position, Quaternion.identity);

            // Get the StrangerController component and initialize it
            StrangerController strangerController = stranger.GetComponent<StrangerController>();
            if (strangerController != null)
            {
                strangerController.Initialize(child, strangerRadius);
            }
            else
            {
                Debug.LogError("StrangerController component not found on strangerPrefab.");
            }

            strangers.Add(stranger);
        }
    }
}