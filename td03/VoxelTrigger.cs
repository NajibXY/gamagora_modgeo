using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelTrigger : MonoBehaviour
{
    // Optional: You can have a unique identifier for each cube
    public string cubeName;

    public float potential;
    private float initialPotential;
    public bool touching;

    private bool destruct;
    private bool isRegen;

    void Start()
    {
        if (string.IsNullOrEmpty(cubeName))
        {
            cubeName = gameObject.name;  // Assign the GameObject name as default
        }
        initialPotential = UnityEngine.Random.Range(1.0f, 2.0f);
        potential = initialPotential;
        touching = false;
        destruct = true;
        isRegen = true;
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.F)) {
            destruct = !destruct;
            Debug.Log("destruct: " + destruct.ToString());
        }
        else if (Input.GetKeyUp(KeyCode.G)) {
            isRegen = !isRegen;
            Debug.Log("isRegen: " + isRegen.ToString());
        }

        if (!touching & isRegen)
        {
            if (potential != initialPotential)
            {
                potential += 0.2f;
            }
            if (potential > initialPotential)
            {
                potential = initialPotential;
            }
        }

        if (potential == 0.0f)
        {
            // Don't display the cube
            gameObject.GetComponent<MeshRenderer>().enabled = false;
        }
        else if (potential == initialPotential)
        {
            // Display the cube
            gameObject.GetComponent<MeshRenderer>().enabled = true;
        }
    }

    // Handle the trigger event
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Triggered!");
        if (other.TryGetComponent<TerraTool>(out _))
        {
            potential = destruct ? 0.0f : initialPotential;
            touching = true;
        }
    }

   
    void OnTriggerExit(Collider other)
    {
        Debug.Log("Untriggered!");

        if (other.TryGetComponent<TerraTool>(out _))
        {
            touching = false;
        }
    }
}
