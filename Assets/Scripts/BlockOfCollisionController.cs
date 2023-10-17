using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockOfCollisionController : MonoBehaviour
{
    public float birth_time;

    // Start is called before the first frame update
    void Start()
    {
        birth_time = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    int car_collision_counter = 0;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "IaCar")
            car_collision_counter++;
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "IaCar")
            car_collision_counter--;
    }

    public bool IsCollidingWithCar()
    {
        if (Time.time < birth_time + 2)
            return true;
        else
            return car_collision_counter != 0;
    }

    public void SetTimer(float offset)
    {
        Destroy(gameObject, offset);
    }
}
