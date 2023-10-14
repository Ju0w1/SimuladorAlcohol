using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmParentController : MonoBehaviour
{
    public GameObject _camera;

    // Start is called before the first frame update
    void Start()
    {
        //
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = _camera.transform.position + (new Vector3(-38.91128158569336f, 1.5529999732971192f, -26.66899871826172f) - new Vector3(-38.90937423706055f, 1.7009999752044678f, -26.675952911376954f));
    }
}
