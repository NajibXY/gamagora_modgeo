using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerraTool : MonoBehaviour
{
    public float customScale;
    public float customSpeed;
    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = new Vector3(customScale, customScale, customScale);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(customSpeed * Vector3.forward * Time.deltaTime);
            Debug.Log("W key was pressed");
        }
        else if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(customSpeed * Vector3.back * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(customSpeed * Vector3.left * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(customSpeed * Vector3.right * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.Translate(customSpeed* Vector3.up * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.Translate(customSpeed * Vector3.down * Time.deltaTime);
        }
    }
}
