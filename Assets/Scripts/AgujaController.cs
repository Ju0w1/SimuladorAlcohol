using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgujaController : MonoBehaviour
{
    public float min_angle = 0;
    public float max_angle = 100;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetValue(float percent)
    {
        transform.localRotation = Quaternion.Euler(-67.73f, 0, percent * (max_angle - min_angle) + min_angle);
    }
}
