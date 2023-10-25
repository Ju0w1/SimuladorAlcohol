using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeatonController : MonoBehaviour
{
    public GameObject ragdoll_me;
    public GameObject block_of_collision_prefab;
    public GameObject block_of_collision_2_prefab;
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
        int PeatonLayer = LayerMask.NameToLayer("PeatonLayer");
        int IgnoreRaycastLayer = LayerMask.NameToLayer("Ignore Raycast");
        int RoadObjectsLayer = LayerMask.NameToLayer("RoadObjectsLayer");
        Physics.IgnoreLayerCollision(PeatonLayer, PeatonLayer);
        Physics.IgnoreLayerCollision(PeatonLayer, IgnoreRaycastLayer);
        Physics.IgnoreLayerCollision(PeatonLayer, RoadObjectsLayer);

        // flip_direction = Random.Range(0.0f, 1.0f) > 0.5f;
    }

    private bool jumping = false;
    private (int, int) jump_destination;
    private bool waiting = false;
    private GameObject block_of_collision;
    private BlockOfCollisionController block_of_collision_controller;

    public Vector3 ray_offset;

    public float forever_height;

    [HideInInspector]
    public bool flip_direction;

    void Update()
    {
        transform.position = new Vector3(transform.position.x, Mathf.Min(transform.position.y, forever_height), transform.position.z);
        // if (!stop_things)
        // {
        //     if (transform.position.y > 0.1669993f)
        //     {
        //         Time.timeScale = 0;
        //         Debug.Log("what " + transform.position.y);
        //         stop_things = true;
        //     }
        // }
        // if (Input.GetKeyUp(KeyCode.Space))
        //     Time.timeScale = 1;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Moviendose para adelante
        body.velocity = transform.forward * vel;

        // Cambiando direccion de adelante
        // transform.forward = Vector3.Slerp(transform.forward, concealed_forward, 0.1f * Time.deltaTime);

        // Girando para evitar obstaculos
        // RaycastHit hit;
        // var dir_right = transform.forward * 3 - transform.right * 0.1f;
        // var dir_left = transform.forward * 3 + transform.right * 0.1f;
        // var origin_right = transform.position + transform.right * 0.1f + dir_right.normalized * 0.88f;
        // var origin_left = transform.position - transform.right * 0.1f + dir_left.normalized * 0.88f;
        // Debug.DrawRay(origin_right, dir_left, Color.magenta);
        // Debug.DrawRay(origin_left, dir_right, Color.magenta);
        // if (Physics.Raycast(origin_right, dir_right.normalized, out hit, 3))
        //     transform.Rotate(new Vector3(0, -10, 0), Space.Self);
        // if (Physics.Raycast(origin_left, dir_left.normalized, out hit, 3))
        //     transform.Rotate(new Vector3(0, 10, 0), Space.Self);

        // Cambiando direccion si es necesario
        if (waiting)
        {
            Debug.DrawRay(transform.position + ray_offset.z * transform.forward + ray_offset.y * transform.up + ray_offset.x * transform.right, transform.forward * 20, Color.magenta);
            RaycastHit hit;
            bool hitted_car = Physics.Raycast(transform.position + ray_offset.z * transform.forward + ray_offset.y * transform.up + ray_offset.x * transform.right, transform.forward, out hit, 20);
            if (hitted_car)
                hitted_car = hit.transform.gameObject.tag == "IaCar";

            body.velocity = Vector3.zero;
            if (block_of_collision_controller.IsCollidingWithStop() && !hitted_car)
            {
                block_of_collision_controller.SetTimer(0.0f);
                animator.SetBool("Idling", false);
                waiting = false;
                jumping = true;
                var new_block = Instantiate(block_of_collision_2_prefab, transform);
                new_block.transform.Rotate(new Vector3(0, -90, 0), Space.Self);
            }
        }
        else if (jumping)
        {
            Vector3 curr_cycle = peaton_system.CurrentCycle(this, peaton_cycle, peaton_cycle_point);
            Vector3 next_cycle = peaton_system.CurrentCycle(this, jump_destination.Item1, jump_destination.Item2);
            
            if ((next_cycle - curr_cycle).sqrMagnitude < (transform.position - curr_cycle).sqrMagnitude)
            {
                peaton_cycle = jump_destination.Item1;
                peaton_cycle_point = jump_destination.Item2;
                transform.forward = (peaton_system.NextCycle(this, peaton_cycle, peaton_cycle_point) - peaton_system.CurrentCycle(this, peaton_cycle, peaton_cycle_point)).normalized;
                // transform.forward = concealed_forward;
                jumping = false;
                // body.velocity = Vector3.zero;
            }
        }
        else
        {
            Vector3 curr_cycle = peaton_system.CurrentCycle(this, peaton_cycle, peaton_cycle_point);
            Vector3 next_cycle = peaton_system.NextCycle(this, peaton_cycle, peaton_cycle_point);
            Debug.DrawRay(curr_cycle, Vector3.up * 3, Color.red);
            Debug.DrawRay(next_cycle, Vector3.up * 3, Color.yellow);
            
            if ((next_cycle - curr_cycle).sqrMagnitude < (transform.position - curr_cycle).sqrMagnitude)
            {
                if (Random.Range(0.0f, 1.0f) > 0.1f)
                {
                    jump_destination = peaton_system.DestinoJump(this, peaton_cycle, peaton_system.NextPointIndex(this, peaton_cycle, peaton_cycle_point));
                    if (jump_destination.Item1 != -1)
                    {
                        waiting = true;
                        peaton_system.UpdatePeatonCycle(this);
                        transform.forward = (peaton_system.CurrentCycle(this, jump_destination.Item1, jump_destination.Item2) - peaton_system.CurrentCycle(this, peaton_cycle, peaton_cycle_point)).normalized;
                        // transform.forward = concealed_forward;
                        animator.SetBool("Idling", true);
                        block_of_collision = Instantiate(block_of_collision_prefab, transform.position, transform.rotation);
                        block_of_collision.transform.Rotate(new Vector3(0, -90, 0), Space.Self);
                        block_of_collision_controller = block_of_collision.GetComponent<BlockOfCollisionController>();
                        return;
                    }
                }
                peaton_system.UpdatePeatonCycle(this);
                transform.forward = (peaton_system.NextCycle(this, peaton_cycle, peaton_cycle_point) - peaton_system.CurrentCycle(this, peaton_cycle, peaton_cycle_point)).normalized;
                // transform.forward = concealed_forward;
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "MainCar")
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

    public void Destruir(float delay)
    {
        Destroy(gameObject, delay);
    }
}
