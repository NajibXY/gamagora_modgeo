using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public TerraTool tool;
    private float customSpeed;
    private Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        offset = new Vector3 (0.25f,0.5f,0.5f);
        customSpeed = tool.customSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = tool.transform.position + offset ;
        transform.LookAt(tool.transform.position);
/*        if (Input.GetKey(KeyCode.W))
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
            transform.Translate(customSpeed * Vector3.up * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.Translate(customSpeed * Vector3.down * Time.deltaTime);
        }*/
    }
}
