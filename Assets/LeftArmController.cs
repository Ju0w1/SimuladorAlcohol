using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftArmController : MonoBehaviour
{
    public GameObject mi_mano;
    public GameObject mi_IKmano;
    public GameObject su_mano;
    public GameObject su_IKmano;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        mi_IKmano.transform.position = su_IKmano.transform.position;
        // mi_mano.transform.rotation = su_mano.transform.rotation;
        CopiarRotaciones(mi_mano, su_mano);
        // mi_mano.transform.Rotate(90, 0, 0, Space.Self);
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
