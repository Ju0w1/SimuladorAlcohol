using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeatonController : MonoBehaviour
{
    public GameObject ragdoll_me;
    public float vel;
    private Rigidbody body;
    public float acera_y = 0.14f;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        body.velocity = transform.forward * vel;
        // RaycastHit hit;
        // Debug.DrawRay(transform.position + transform.forward * 2.0f + transform.right * 3, Vector3.down * 3, Color.green);
        // Debug.DrawRay(transform.position + transform.forward * 2.0f - transform.right * 3, Vector3.down * 3, Color.green);
        // if (!Physics.Raycast(transform.position + transform.forward * 2.0f + transform.right * 3, Vector3.down, out hit, 3) || hit.point.y < acera_y)
        //     transform.Rotate(new Vector3(0, -1, 0), Space.Self);
        // if (!Physics.Raycast(transform.position + transform.forward * 2.0f - transform.right, Vector3.down, out hit, 3) || hit.point.y < acera_y)
        //     transform.Rotate(new Vector3(0, 1, 0), Space.Self);
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
