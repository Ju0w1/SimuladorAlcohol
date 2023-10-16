using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightArmController : MonoBehaviour
{
    public GameObject elbow;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        elbow.transform.position = new Vector3(0.00540000014f, -0.0109999999f, -0.0138999997f);
    }
}
