using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.TerrainTools;
using UnityEngine;

/// <summary>
/// This script is needed to demonstrate this asset.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class CarController : MonoBehaviour {

	//[SerializeField] float MaxMotorTorque = 1500;
	[SerializeField] float MaxBrakeTorque = 500;
	//[SerializeField] float AccelerationTorque = 1f;
	//[SerializeField] float AccelerationBrakeTorque = 0.5f;
	[SerializeField] float AccelerationSteer = 0.1f;//10f;
	[SerializeField] GameObject COM;
	[SerializeField] List<WheelPreset> DrivingWheels = new List<WheelPreset>();
	[SerializeField] List<WheelPreset> SteeringWheels = new List<WheelPreset>();

	public Motor motor = new Motor(1, 2000, 1);

	public Rigidbody RB;
	AudioSource audiosource;
	HashSet<WheelPreset> AllWheels = new HashSet<WheelPreset>();
	public float CurrentAcceleration;
	float CurrentBrake;
	float CurrentSteer;

	public AgujaController aguja_rpm_controller;
	public AgujaController aguja_velocidad_controller;

	public bool CentrarVolante;

	public bool Enable { get; set; }

	LogitechGSDK.LogiControllerPropertiesData properties;

	public float volante, acelerador, freno, embriague;

	public bool activar = false;
	public bool activar2 = false;

	public int cantidadDeChoques = 0;

	public bool cambioH;

	void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.tag == "IaCar" || collision.gameObject.tag == "Colisionable")
		{
			cantidadDeChoques++;

			LogitechGSDK.LogiPlayFrontalCollisionForce(0, 70);	

		}
		Debug.Log("Entered collision with " + collision.gameObject.name);
	}

	private void Awake () {
		Enable = false;
		audiosource = GetComponent<AudioSource>();
		RB = GetComponent<Rigidbody>();
		RB.centerOfMass = COM.transform.localPosition;
		RB.ResetInertiaTensor();
		foreach (var wheel in SteeringWheels) {
			AllWheels.Add(wheel);
		}
		foreach (var wheel in DrivingWheels) {
			AllWheels.Add(wheel);
		}
		CentrarVolante = true;
	}

	private void Update () {
        if (Input.GetKeyUp(KeyCode.L))
		{
			System.Random random = new System.Random();
			float factor = 5;// + (float)random.NextDouble() * 1;
			Time.timeScale = factor;
			//Time.fixedDeltaTime = 0.02f * factor;
		}

		// Cambiando panel de la cabina
		aguja_rpm_controller.SetValue(motor.rpm / motor.max_rpm);
		aguja_velocidad_controller.SetValue(RB.velocity.magnitude * 3.6f / 120);

        // Controlando audio basandonos en los rpm
        if (motor.rpm >= motor.min_rpm)
		{
			audiosource.pitch = 1 + (motor.rpm - motor.min_rpm) / (motor.max_rpm - motor.min_rpm);
			audiosource.volume = 1;
		}
		else
            audiosource.volume = 0;
		// Cambiando cambios basado en numeros del teclado
		if (Input.GetKeyUp(KeyCode.Alpha1))
			motor.cambio = 1;
        if (Input.GetKeyUp(KeyCode.Alpha2))
            motor.cambio = 2;
        if (Input.GetKeyUp(KeyCode.Alpha3))
            motor.cambio = 3;
        if (Input.GetKeyUp(KeyCode.Alpha4))
            motor.cambio = 4;
        if (Input.GetKeyUp(KeyCode.Alpha5))
            motor.cambio = 5;
        if (Input.GetKeyUp(KeyCode.Alpha6))
            motor.cambio = 6;
        if (Input.GetKeyUp(KeyCode.Alpha0))
            motor.cambio = 0;
        if (Input.GetKeyUp(KeyCode.R))
            motor.cambio = -1;
        if (Input.GetKeyUp(KeyCode.Return) && !motor.encendido())
		{
            motor.encender();
            audiosource.time = 0;
		}
        // Controlando el auto a travez del volante
        if (LogitechGSDK.LogiUpdate() && LogitechGSDK.LogiIsConnected(0))
		{
            LogitechGSDK.DIJOYSTATE2ENGINES rec;
			rec = LogitechGSDK.LogiGetStateUnity(0);


			// fuerza del movimiento del volante para centrarlo
			//Spring Force -> S
			if (CentrarVolante)
			{
				if (LogitechGSDK.LogiIsPlaying(0, LogitechGSDK.LOGI_FORCE_SPRING))
				{
					LogitechGSDK.LogiStopSpringForce(0);
					//activeForceAndEffect[0] = "";
				}
				else
				{
					LogitechGSDK.LogiPlaySpringForce(0, 0, 30, 50);
					//activeForceAndEffect[0] = "Spring Force\n ";
				}
				CentrarVolante = !CentrarVolante;
			}

			// Tecla de prender el motor
			if (rec.rgbButtons[23] == 128 && !motor.encendido())
			{
				motor.encender();
                audiosource.time = 0;
            }

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

            if (cambioH)
            {
				bool cambio_pressed = false;
				for (int i = 12; i <= 18; i++)
				{
					if (rec.rgbButtons[i] == 128)
					{
						int nuevoCambio = i - 11;
						if (nuevoCambio > 6)
							nuevoCambio = -1;
						motor.cambio = nuevoCambio;
						cambio_pressed = true;
					}
				}
				if (!cambio_pressed)
					motor.cambio = 0;
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
			motor.embrague = embriague;

			// steering
			if (Enable)
				CurrentSteer = Mathf.MoveTowards(CurrentSteer, volante, AccelerationSteer * Time.deltaTime);
		}
		else // controlando el auto a travez del teclado
        {
			float targetAcceleration = Input.GetAxis("Vertical");
			float targetSteer = Input.GetAxis("Horizontal");
            motor.embrague = Input.GetAxis("Cancel");

            if (Input.GetButton("Jump") || !Enable)
			{
				motor.freno = 1;
				motor.aceleracion = 0;
			}
			else
			{
				motor.freno = 0;
                motor.aceleracion = Mathf.Max(targetAcceleration, 0);
				motor.freno = Mathf.Max(-targetAcceleration, 0);
            }

            // steering
            if (Enable)
				CurrentSteer = Mathf.MoveTowards(CurrentSteer, targetSteer, AccelerationSteer * Time.deltaTime);
		}
		 
	}

	// Obtiene el rpm de las dos ruedas, incluso cuando una va mas rapido que otra XD
	public float obtener_rpm()
	{
		// esto puede estar todo mal
		float factor = Vector3.Dot(DrivingWheels[0].WheelCollider.transform.forward, RB.velocity);
        return RB.velocity.magnitude * factor / (Mathf.PI * DrivingWheels[0].WheelCollider.radius * 60f) * 800;
    }

    private void FixedUpdate () {
		float rpm = obtener_rpm();
		
        WheelCollider wheelCollider;
		for (int i = 0; i < DrivingWheels.Count; i++) {
			wheelCollider = DrivingWheels[i].WheelCollider;
			if (i == 0) // asegura que se haga una sola vez esto
			{
				motor.update(rpm, wheelCollider.radius);
			}
			float value = motor.obtenerTorque(rpm, wheelCollider.radius);
			//Debug.Log(value);
			wheelCollider.motorTorque = value;
			wheelCollider.brakeTorque = DrivingWheels[i].BrakeTorque * motor.freno * motor.obtenerFreno(wheelCollider.rpm, wheelCollider.radius);
		}
		//float vel = Mathf.MoveTowards(RB.velocity.magnitude, motor.obtener_rpm_objetivo_rueda() / 80.0f, motor.efecto_embrague() * Time.deltaTime);
		//if (motor.embrague == 0)
		//	RB.velocity = transform.forward * motor.obtener_rpm_objetivo_rueda() / 80.0f;

		for (int i = 0; i < SteeringWheels.Count; i++) {
			wheelCollider = SteeringWheels[i].WheelCollider;
			wheelCollider.steerAngle = CurrentSteer * SteeringWheels[i].SteerAngle;
			wheelCollider.brakeTorque = DrivingWheels[i].BrakeTorque * motor.freno * MaxBrakeTorque;
		}
	}

	public float GetWheelGroundRPM(WheelCollider collider)
	{
		WheelHit hit;

		if (collider.GetGroundHit(out hit))
		{
            float RadsToRevs = Mathf.PI * 2;
            float SlipInDirection = hit.forwardSlip / (RadsToRevs * collider.radius) / RadsToRevs * 60 / Time.deltaTime;
			return collider.rpm - SlipInDirection;
		}
		else
			return collider.rpm;
	}

    [System.Serializable]
	private class WheelPreset {
		public WheelCollider WheelCollider;
		public float BrakeTorque = 1;
		public float SteerAngle = 25;
	}

}
