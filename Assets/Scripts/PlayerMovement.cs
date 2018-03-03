using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
	
	public float moveSpeed;
	private Vector3 velocity;
	Rigidbody rb;
	Camera mainCamera;
	Animator myAnim;
	float inputVelocity;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody>();
		mainCamera = Camera.main;
		myAnim = GetComponent<Animator> ();
	}
	
	// Update is called once per frame
	void Update () {
		float horizontalInput = Input.GetAxis ("Horizontal");
		float verticalInput = Input.GetAxis ("Vertical");
		Vector3 inputDirection = new Vector3 (horizontalInput, 0, verticalInput);
		inputVelocity = inputDirection.sqrMagnitude;

		velocity = transform.forward * moveSpeed * inputVelocity;
		if (inputVelocity > 0) {
			Vector3 cameraForward = mainCamera.transform.forward;
			cameraForward.y = 0;
			Quaternion cameraRelativeRotation = Quaternion.FromToRotation (Vector3.forward, cameraForward);
			Vector3 lookToward =  cameraRelativeRotation * inputDirection;
			Ray characterForwardRay = new Ray (transform.position, lookToward);
			transform.LookAt (characterForwardRay.GetPoint (1));
		}
	}

	void FixedUpdate() {
		rb.velocity = velocity;
		myAnim.SetFloat ("BlendSpeed", rb.velocity.magnitude);
	}
}
