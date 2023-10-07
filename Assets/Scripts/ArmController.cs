using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmController : MonoBehaviour
{
    public float animation_time = 0;
    public float animation_angle_multiplier = 1;
    private Animator animator;
    public SetRotationBySteerAngle steering_wheel_controller;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        animation_time = -steering_wheel_controller.GetSteerAngle() * animation_angle_multiplier;
        float time = Mathf.Repeat(Mathf.Abs(animation_time), 1);
        if (animation_time < 0)
            time = 1 - time;
        animator.Play("Brazo", 0, time);
    }
}
