﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script is needed to demonstrate this asset.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class CarController : MonoBehaviour {

	[SerializeField] float MaxMotorTorque = 1500;
	[SerializeField] float MaxBrakeTorque = 500;
	[SerializeField] float AccelerationTorque = 1f;
	[SerializeField] float AccelerationBrakeTorque = 0.5f;
	[SerializeField] float AccelerationSteer = 10f;
	[SerializeField] GameObject COM;
	[SerializeField] List<WheelPreset> DrivingWheels = new List<WheelPreset>();
	[SerializeField] List<WheelPreset> SteeringWheels = new List<WheelPreset>();

	public Motor motor = new Motor(100, 1, 1);

	Rigidbody RB;
	HashSet<WheelPreset> AllWheels = new HashSet<WheelPreset>();
	public float CurrentAcceleration;
	float CurrentBrake;
	float CurrentSteer;

	public bool Enable { get; set; }

	LogitechGSDK.LogiControllerPropertiesData properties;

	public float volante, acelerador, freno, embriague;

	public bool activar = false;
	public bool activar2 = false;

    private void Awake () {
		Enable = false;
		RB = GetComponent<Rigidbody>();
		RB.centerOfMass = COM.transform.localPosition;
		RB.ResetInertiaTensor();
		foreach (var wheel in SteeringWheels) {
			AllWheels.Add(wheel);
		}
		foreach (var wheel in DrivingWheels) {
			AllWheels.Add(wheel);
		}
	}

	private void Update () {
		//Debug.Log("LogitechGSDK.LogiUpdate() " + LogitechGSDK.LogiUpdate());
        //Debug.Log("LogitechGSDK.LogiIsConnected(0) " + LogitechGSDK.LogiIsConnected(0));
        if (LogitechGSDK.LogiUpdate() && LogitechGSDK.LogiIsConnected(0))
		{
            LogitechGSDK.DIJOYSTATE2ENGINES rec;
			rec = LogitechGSDK.LogiGetStateUnity(0);

            volante = rec.lX / 32768f;

			if (rec.lY > 0)
			{
				acelerador = 0;
			}
			else if (rec.lY < 0)
			{
				acelerador = rec.lY / -32768f;
			}

			if (rec.lRz > 0)
			{
				freno = 0;
			}
			else if (rec.lRz < 0)
			{
				freno = rec.lRz / -32768f;
			}

            if (rec.rglSlider[0] > 0)
			{
                embriague = 0;
			}
			else if (rec.rglSlider[0] < 0)
			{
                embriague = rec.rglSlider[0] / -32768f;
			}



            for (int i = 12; i <= 18; i++)
			{
				if (rec.rgbButtons[i] == 128)
				{
					int nuevoCambio = i - 11;
					if (nuevoCambio > 6)
                        nuevoCambio = -1;
					motor.cambio = nuevoCambio;
                }
			}
				
			// Este codigo es solo si no se tiene palanca de cambios
			if (rec.rgbButtons[4] == 128 && !activar)
			{
				activar = true;
			}

			if (activar && rec.rgbButtons[4] == 0)
			{
				motor.cambio = Mathf.Min(motor.cambio + 1, 6);
				activar = false;
			}
				

			if (rec.rgbButtons[5] == 128 && !activar2)
			{
				activar2 = true;
			}

			if (rec.rgbButtons[5] == 0 && activar2)
			{
                motor.cambio = Mathf.Max(motor.cambio - 1, -1);
				activar2 = false;
			}
			
			motor.aceleracion = acelerador;
			motor.freno = freno;
			motor.embriague = embriague;

			// steering
			if (Enable)
				CurrentSteer = Mathf.MoveTowards(CurrentSteer, volante, AccelerationSteer * Time.deltaTime);
		}
		else
        {
			float targetAcceleration = Input.GetAxis("Vertical");
			float targetSteer = Input.GetAxis("Horizontal");

			if (Input.GetButton("Jump") || !Enable)
			{
				motor.freno = 1;
				motor.aceleracion = 0;
			}
			else
			{
				motor.freno = 0;
				motor.aceleracion = targetAcceleration;
			}

            // steering
            if (Enable)
				CurrentSteer = Mathf.MoveTowards(CurrentSteer, targetSteer, AccelerationSteer * Time.deltaTime);
		}
		 
	}

    private void FixedUpdate () {

		WheelCollider wheelCollider;
		for (int i = 0; i < DrivingWheels.Count; i++) {
			wheelCollider = DrivingWheels[i].WheelCollider;
			if (i == 0) // asegura que se haga una sola vez esto
				motor.update(wheelCollider.rpm, wheelCollider.radius);
			wheelCollider.motorTorque = motor.obtenerTorque(wheelCollider.rpm, wheelCollider.radius);
			wheelCollider.brakeTorque = DrivingWheels[i].BrakeTorque * motor.freno * motor.obtenerFreno(wheelCollider.rpm, wheelCollider.radius);
		}

		for (int i = 0; i < SteeringWheels.Count; i++) {
			wheelCollider = SteeringWheels[i].WheelCollider;
			wheelCollider.steerAngle = CurrentSteer * SteeringWheels[i].SteerAngle;
			wheelCollider.brakeTorque = DrivingWheels[i].BrakeTorque * motor.freno * MaxBrakeTorque;
		}
	}

	[System.Serializable]
	private class WheelPreset {
		public WheelCollider WheelCollider;
		public float BrakeTorque = 1;
		public float SteerAngle = 25;
	}

}
