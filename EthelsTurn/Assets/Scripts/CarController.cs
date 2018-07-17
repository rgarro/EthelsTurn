using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class CarController : MonoBehaviour {

	public WheelCollider[] WColForward;
	public WheelCollider[] WColBack;

	public Transform[] wheelsF; //1
	public Transform[] wheelsB; //1

	public float wheelOffset = 0.1f; //2
	public float wheelRadius = 0.13f; //2

	public float maxSteer = 30;
	public float maxAccel = 25;
	public float maxBrake = 50;

	public Transform COM;



	protected WheelData[] wheels; //8
	private WheelData wd;

//	protected WheelData[] wheels; //8
	public Rigidbody rb;

	// Use this for initialization
	void Start () {
		//COM.GetComponent<RigidBody3D>
		//rigidbody = GetComponent<RigidBody3D> ();
		rb = GetComponent<Rigidbody>();
		rb.centerOfMass = COM.GetComponent<Rigidbody>().centerOfMass;

		wheels = new WheelData[WColForward.Length+WColBack.Length]; //8

		for (int i = 0; i<WColForward.Length; i++){ //9
			//wheels[i] = SetupWheels(wheelsF[i],WColForward[i]); //9
		}

		for (int i = 0; i<WColBack.Length; i++){ //9
			//wheels[i+WColForward.Length] = SetupWheels(wheelsB[i],WColBack[i]); //9
		}
	}



	// Update is called once per frame
	void Update () {
		
	}

	private WheelData SetupWheels(Transform wheel, WheelCollider col){ //10
		WheelData result = new WheelData(); 

		result.wheelTransform = wheel; //10
		result.col = col; //10
		result.wheelStartPos = wheel.transform.localPosition; //10

		return result; //10
	}


	void FixedUpdate () {

		float accel = 0;
		float steer = 0;

		accel = Input.GetAxis("Vertical");  //4
		steer = Input.GetAxis("Horizontal");	 //4	

		CarMove(accel,steer);
		UpdateWheels();
	}

	private void UpdateWheels(){ //11
		float delta = Time.fixedDeltaTime; //12


		/*foreach (WheelData w in wheels){ //13
			WheelHit hit; //14
			Debug.Log(w);
			Vector3 lp = w.wheelTransform.localPosition; //15
			if(w.col.GetGroundHit(out hit)){ //16
				lp.y -= Vector3.Dot(w.wheelTransform.position - hit.point, transform.up) - wheelRadius; //17
			}else{ //18

				lp.y = w.wheelStartPos.y - wheelOffset; //18
			}
			w.wheelTransform.localPosition = lp; //19


			w.rotation = Mathf.Repeat(w.rotation + delta * w.col.rpm * 360.0f / 60.0f, 360.0f); //20
			w.wheelTransform.localRotation = Quaternion.Euler(w.rotation, w.col.steerAngle, 90.0f); //21
		}*/	

	}

	private void CarMove(float accel,float steer){ //5

		foreach(WheelCollider col in WColForward){ //6
			col.steerAngle = steer*maxSteer; //6
		}

		if(accel == 0){ //7
			foreach(WheelCollider col in WColBack){  //7
				col.brakeTorque = maxBrake; //7
			}	

		}else{ //8

			foreach(WheelCollider col in WColBack){ //8
				col.brakeTorque = 0; //8
				col.motorTorque = accel*maxAccel; //8
			}	

		}



	}


}
