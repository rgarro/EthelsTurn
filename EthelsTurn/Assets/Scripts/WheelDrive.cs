/**
 * Darkness in a blink - Mantra engine roaring bliss - sudden noise absence
 *       ____                     _____________         _____________
 *  ____//_]|________        ____//__][__][___|    ____//__][______||
 * (o _ |  -|   _  o|       (o  _|  -|     _ o|   (o _ |  -|   _   o|
 *  `(_)-------(_)--'        `-(_)--------(_)-'    `(_)-------(_)---'
 *
 * @author Rolando <rgarro@gmail.com>
 */
﻿using UnityEngine;
using System;
using emptyLibUnity.UI.Util;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public enum DriveType
{
	RearWheelDrive,
	FrontWheelDrive,
	AllWheelDrive
}

public class WheelDrive : MonoBehaviour
{
  [Tooltip("Maximum steering angle of the wheels")]
	public float maxAngle = 30f;
	[Tooltip("Maximum torque applied to the driving wheels")]
	public float maxTorque = 300f;
	[Tooltip("Maximum brake torque applied to the driving wheels")]
	public float brakeTorque = 30000f;
	[Tooltip("If you need the visual wheels to be attached automatically, drag the wheel shape here.")]
	public GameObject wheelShape;

	[Tooltip("The vehicle's speed when the physics engine can use different amount of sub-steps (in m/s).")]
	public float criticalSpeed = 5f;
	[Tooltip("Simulation sub-steps when the speed is above critical.")]
	public int stepsBelow = 5;
	[Tooltip("Simulation sub-steps when the speed is below critical.")]
	public int stepsAbove = 1;

	[Tooltip("The vehicle's drive type: rear-wheels drive, front-wheels drive or all-wheels drive.")]
	public DriveType driveType;

	public SimpleGaugeNeedle speedNeedle;
	public SimpleGaugeNeedle mphNeedle;
	public Image NeedleIm;
	public Image NeedleMph;
  private WheelCollider[] m_Wheels;

	public double speedKph = 0.0F;
	public double speedMph = 0.0F;
	private Rigidbody rb;
	private GameObject go;
	public float timerSpeed = 1.0f;
	private float secondsDriving = 0;
	private double distanceRunnedMetres = 0;
	private double distanceRunnedKm = 0;

    // Find all the WheelColliders down in the hierarchy.
	void Start()
	{
		StartCoroutine(this.drivingTimer());
		this.rb = GetComponent<Rigidbody>();
		this.go = GetComponent<GameObject>();
		this.startDashItems();
		m_Wheels = GetComponentsInChildren<WheelCollider>();

		for (int i = 0; i < m_Wheels.Length; ++i)
		{
			var wheel = m_Wheels [i];

			// Create wheel shapes only when needed.
			if (wheelShape != null)
			{
				var ws = Instantiate (wheelShape);
				ws.transform.parent = wheel.transform;
			}
		}
	}

 private IEnumerator drivingTimer(){
	 while(true){
		 this.secondsDriving = this.secondsDriving + this.timerSpeed;
			this.distanceRunnedMetres = this.distanceRunnedMetres + (this.speedKph/3.6);
			this.distanceRunnedKm = this.distanceRunnedMetres / 1000;
Debug.Log("runnedKm " + this.distanceRunnedKm);
		 yield return new WaitForSeconds(this.timerSpeed);
	 }
 }

	void startDashItems(){
		this.speedNeedle = new SimpleGaugeNeedle();
		this.speedNeedle.Needle = this.NeedleIm;
		this.mphNeedle = new SimpleGaugeNeedle();
		this.mphNeedle.Needle = this.NeedleMph;
	}

	void setSpeedKph(){
		this.speedKph = this.rb.velocity.magnitude*3.6;
		this.speedNeedle.getTilter(this.speedKph);//fractals are the far end of the needle speed oscilation ...
		this.speedNeedle.tiltNeedle();
	}

	protected void setSpeedMph(){
		this.speedMph = this.speedKph * 0.621371;
		//Debug.Log(this.speedMph);
		this.mphNeedle.getTilter(this.speedMph);
		this.mphNeedle.tiltNeedle();
	}

	// This is a really simple approach to updating wheels.
	// We simulate a rear wheel drive car and assume that the car is perfectly symmetric at local zero.
	// This helps us to figure our which wheels are front ones and which are rear.
	void Update()
	{
		this.setSpeedKph();
		this.setSpeedMph();
		m_Wheels[0].ConfigureVehicleSubsteps(criticalSpeed, stepsBelow, stepsAbove);

		float angle = maxAngle * Input.GetAxis("Horizontal");
		float torque = maxTorque * Input.GetAxis("Vertical");

		float handBrake = Input.GetKey(KeyCode.X) ? brakeTorque : 0;

		foreach (WheelCollider wheel in m_Wheels)
		{
			// A simple car where front wheels steer while rear ones drive.
			if (wheel.transform.localPosition.z > 0)
				wheel.steerAngle = angle;

			if (wheel.transform.localPosition.z < 0)
			{
				wheel.brakeTorque = handBrake;
			}

			if (wheel.transform.localPosition.z < 0 && driveType != DriveType.FrontWheelDrive)
			{
				wheel.motorTorque = torque;
			}

			if (wheel.transform.localPosition.z >= 0 && driveType != DriveType.RearWheelDrive)
			{
				wheel.motorTorque = torque;
			}

			// Update visual wheels if any.pw
			if (wheelShape)
			{
				Quaternion q;
				Vector3 p;
				wheel.GetWorldPose (out p, out q);

				// Assume that the only child of the wheelcollider is the wheel shape.
				Transform shapeTransform = wheel.transform.GetChild (0);
				shapeTransform.position = p;
				shapeTransform.rotation = q;
			}
		}
	}
}
