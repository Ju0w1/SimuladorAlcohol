using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmKinematics : MonoBehaviour
{
    private Animator animator;
    public GameObject manoIK;
    public GameObject codoIK;
    public GameObject brazo;
    public GameObject antebrazo;
    public GameObject mano;

    public GameObject test_point;

    private float distance_brazo_antebrazo;
    private float distance_antebrazo_mano;
    private bool set_kinematics = false;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (set_kinematics)
        {
            Vector2[] intersections;
            Vector3 right = Vector3.Normalize(manoIK.transform.position - brazo.transform.position);
            Vector3 up = Vector3.Normalize(codoIK.transform.position - (brazo.transform.position + Vector3.Project(codoIK.transform.position - brazo.transform.position, right)));
            if (test_point)
            {
                Debug.Log(up);
                test_point.transform.position = brazo.transform.position + Vector3.Project(codoIK.transform.position - brazo.transform.position, right);
            }
            Intersect(
                new Vector2(0, 0),
                distance_brazo_antebrazo,
                new Vector2((manoIK.transform.position - brazo.transform.position).magnitude, 0),
                distance_antebrazo_mano,
                out intersections
                );
            Vector3 codo;
            if (intersections.Length == 0)
                codo = brazo.transform.position * 0.5f + manoIK.transform.position * 0.5f;
            else
                codo = brazo.transform.position + right * intersections[0].x + up * intersections[0].y;

            // if (test_point)
            //     test_point.transform.position = codo;
            brazo.transform.LookAt(codo);
            brazo.transform.Rotate(90, 0, 0, Space.Self);
            brazo.transform.Rotate(0, -90, 0, Space.Self);
            antebrazo.transform.LookAt(manoIK.transform.position);
            antebrazo.transform.Rotate(90, 0, 0, Space.Self);
            // antebrazo.transform.Rotate(0, -90, 0, Space.Self);
            // if plane = xy
            /*/
            Vector2[] intersections;
            Intersect(
                new Vector2(brazo.transform.position.x, brazo.transform.position.y),
                distance_brazo_antebrazo,
                new Vector2(manoIK.transform.position.x, manoIK.transform.position.y),
                distance_antebrazo_mano,
                out intersections
                );
            var codo = new Vector3(intersections[0].x, intersections[0].y, antebrazo.transform.position.z);
            if (test_point)
                test_point.transform.position = codo;
            brazo.transform.LookAt(codo);
            brazo.transform.Rotate(90, 0, 0, Space.Self);
            antebrazo.transform.LookAt(manoIK.transform.position);
            antebrazo.transform.Rotate(90, 0, 0, Space.Self);
            //*/
        }
        else if (Time.time > 0.01)
        {
            if (animator)
                animator.enabled = false;
            set_kinematics = true;
            distance_brazo_antebrazo = Vector3.Distance(brazo.transform.position, antebrazo.transform.position);
            distance_antebrazo_mano = Vector3.Distance(antebrazo.transform.position, mano.transform.position);
        }
    }

    public static int Intersect(Vector2       circleA,
                                float         radiusA,
                                Vector2       circleB,
                                float         radiusB,
                                out Vector2[] intersections) {

        float centerDx = circleA.x - circleB.x;
        float centerDy = circleB.y - circleB.y;
        float r = Mathf.Sqrt(centerDx * centerDx + centerDy * centerDy);

        // no intersection
        if (!(Mathf.Abs(radiusA - radiusB) <= r && r <= radiusA + radiusB)) {
            intersections = new Vector2[0];
            return 0;
        }

        float r2d = r * r;
        float r4d = r2d * r2d;
        float rASquared = radiusA * radiusA;
        float rBSquared = radiusB * radiusB;
        float a = (rASquared - rBSquared) / (2 * r2d);
        float r2r2 = (rASquared - rBSquared);
        float c = Mathf.Sqrt(2 * (rASquared + rBSquared) / r2d - (r2r2 * r2r2) / r4d - 1);

        float fx = (circleA.x + circleB.x) / 2 + a * (circleB.x - circleA.x);
        float gx = c * (circleB.y - circleA.y) / 2;
        float ix1 = fx + gx;
        float ix2 = fx - gx;

        float fy = (circleA.y + circleB.y) / 2 + a * (circleB.y - circleA.y);
        float gy = c * (circleA.x - circleB.x) / 2;
        float iy1 = fy + gy;
        float iy2 = fy - gy;

        // if gy == 0 and gx == 0 then the circles are tangent and there is only one solution
        if (Mathf.Abs(gx) < float.Epsilon && Mathf.Abs(gy) < float.Epsilon) {
            intersections = new [] {
                new Vector2(ix1, iy1)
            };
            return 1;
        }

        intersections = new [] {
            new Vector2(ix1, iy1),
            new Vector2(ix2, iy2),
        };
        return 2;
    }
}
