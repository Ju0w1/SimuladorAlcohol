using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeatonController : MonoBehaviour
{
    public GameObject ragdoll_me;
    public GameObject block_of_collision_prefab;
    public float vel;
    private Rigidbody body;
    private Animator animator;
    public float acera_y = 0.14f;

    public PeatonSystem peaton_system;

    public int peaton_cycle;
    public int peaton_cycle_point;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    private bool jumping = false;
    private (int, int) jump_destination;
    private bool waiting = false;
    private GameObject block_of_collision;
    private BlockOfCollisionController block_of_collision_controller;

    // Update is called once per frame
    void FixedUpdate()
    {
        // Moviendose para adelante
        body.velocity = transform.forward * vel;

        // Cambiando direccion si es necesario
        if (waiting)
        {
            body.velocity = Vector3.zero;
            if (!block_of_collision_controller.IsCollidingWithCar())
            {
                block_of_collision_controller.SetTimer(5.0f);
                waiting = false;
                jumping = true;
            }
        }
        else if (jumping)
        {
            Vector3 curr_cycle = peaton_system.CurrentCycle(peaton_cycle, peaton_cycle_point);
            Vector3 next_cycle = peaton_system.CurrentCycle(jump_destination.Item1, jump_destination.Item2);
            
            if ((next_cycle - curr_cycle).sqrMagnitude < (transform.position - curr_cycle).sqrMagnitude)
            {
                peaton_cycle = jump_destination.Item1;
                peaton_cycle_point = jump_destination.Item2;
                transform.forward = (peaton_system.NextCycle(peaton_cycle, peaton_cycle_point) - peaton_system.CurrentCycle(peaton_cycle, peaton_cycle_point)).normalized;
                jumping = false;
                // body.velocity = Vector3.zero;
            }
        }
        else
        {
            Vector3 curr_cycle = peaton_system.CurrentCycle(peaton_cycle, peaton_cycle_point);
            Vector3 next_cycle = peaton_system.NextCycle(peaton_cycle, peaton_cycle_point);
            
            if ((next_cycle - curr_cycle).sqrMagnitude < (transform.position - curr_cycle).sqrMagnitude)
            {
                if (Random.Range(0.0f, 1.0f) > 0.1f)
                {
                    jump_destination = peaton_system.DestinoJump(peaton_cycle, peaton_system.NextPointIndex(peaton_cycle, peaton_cycle_point));
                    if (jump_destination.Item1 != -1)
                    {
                        waiting = true;
                        peaton_system.UpdatePeatonCycle(this);
                        transform.forward = (peaton_system.CurrentCycle(jump_destination.Item1, jump_destination.Item2) - peaton_system.CurrentCycle(peaton_cycle, peaton_cycle_point)).normalized;
                        animator.SetBool("Idling", true);
                        block_of_collision = Instantiate(block_of_collision_prefab, transform.position, transform.rotation);
                        block_of_collision.transform.Rotate(new Vector3(0, -90, 0), Space.Self);
                        block_of_collision_controller = block_of_collision.GetComponent<BlockOfCollisionController>();
                        return;
                    }
                }
                peaton_system.UpdatePeatonCycle(this);
                transform.forward = (peaton_system.NextCycle(peaton_cycle, peaton_cycle_point) - peaton_system.CurrentCycle(peaton_cycle, peaton_cycle_point)).normalized;
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "IaCar")
        {
            var obj = Instantiate(ragdoll_me, transform.position, transform.rotation);
            CopiarRotaciones(obj, gameObject);
            Destroy(gameObject);
        }
    }

    void CopiarRotaciones(GameObject src, GameObject dst, int level = 0)
    {
        src.transform.localRotation = dst.transform.localRotation;
        foreach (Transform src_child in src.transform)
        {
            foreach (Transform dst_child in dst.transform)
            {
                if (src_child.gameObject.name == dst_child.gameObject.name)
                    CopiarRotaciones(src_child.gameObject, dst_child.gameObject, level + 1);
            }
        }
    }
}
