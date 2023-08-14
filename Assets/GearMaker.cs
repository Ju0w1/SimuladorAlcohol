using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GearMaker : MonoBehaviour
{
    private readonly float DISTANCE_TO_CIRCUMFERENCE = 0.06f;
    public GameObject circle_prefab;
    public GameObject tooth_prefab;

    public GameObject MakeGear(Vector3 position, float radius, int numberOfTeeth)
    {
        radius *= 2.0f;
        GameObject gear = new GameObject();
        gear.transform.position = position;

        GameObject circle = Instantiate(circle_prefab);
        circle.transform.parent = gear.transform;
        circle.transform.position = position;
        circle.transform.localScale = new Vector3(radius, radius, 1);

        float angleBetweenTeeth = Mathf.PI * 2 / numberOfTeeth;
        for (int i = 0; i < numberOfTeeth; i++)
        {
            GameObject tooth = Instantiate(tooth_prefab, position, Quaternion.identity);
            tooth.transform.Rotate(Vector3.right, -90f);
            tooth.transform.Rotate(Vector3.up, -90f);
            tooth.transform.parent = gear.transform;
            float separation = radius / 2.0f + DISTANCE_TO_CIRCUMFERENCE;
            float angle = i * angleBetweenTeeth;
            tooth.transform.localPosition = -new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * separation;
            tooth.transform.Rotate(tooth.transform.right, -angle * 180 / Mathf.PI);
        }
        gear.AddComponent<Rigidbody>();
        gear.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
        return gear;
    }

    public float DistanceBetweenGears(float radius1, float radius2)
    {
        return radius1 + radius2 + DISTANCE_TO_CIRCUMFERENCE * 3f;
    }

    private GameObject gear1;
    // Start is called before the first frame update
    void Start()
    {
        print("hello");
        gear1 = MakeGear(new Vector3(0, 1.4f, 0), 0.5f, 15);
        GameObject gear2 = MakeGear(gear1.transform.position - Vector3.right * DistanceBetweenGears(0.5f, 0.5f), 0.5f, 15);
        GameObject gear3 = MakeGear(gear2.transform.position - Vector3.right * DistanceBetweenGears(0.5f, 0.5f), 0.5f, 15);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            print("clicking");
            gear1.GetComponent<Rigidbody>().AddTorque(new Vector3(0, 0, 1.0f));
        }
    }
}
