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
        int customLayer = LayerMask.NameToLayer("BlockOfCollisionLayer");

        // Disable collision between Default layer and the custom layer
        Physics.IgnoreLayerCollision(0, customLayer, true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // int car_collision_counter = 0;
    bool is_colliding_with_stop = false;

    void OnTriggerEnter(Collider collider)
    {
        // if (collision.gameObject.tag == "IaCar")
        //     car_collision_counter++;
        // Debug.Log(collider.name);
        if (collider.name == "Stop")
            is_colliding_with_stop = true;
    }

    // void OnCollisionExit(Collision collision)
    // {
    //     // if (collision.gameObject.tag == "IaCar")
    //     //     car_collision_counter--;
    //     Debug.Log(collision.gameObject.name);
    //     if (collision.gameObject.name == "Stop")
    //         is_colliding_with_stop = false;
    // }

    public bool IsCollidingWithStop()
    {
        if (Time.time < birth_time + 2)
            return false;
        else if (Time.time > birth_time + 122)
            return true;
        else
            // return car_collision_counter != 0;
            return is_colliding_with_stop;
    }

    public void SetTimer(float offset)
    {
        Destroy(gameObject, offset);
    }
}
