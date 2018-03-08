using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed;
    private Rigidbody rb;
    private Animator anim;

    private Vector3 inputDirection;
    private Vector3 moveVelocity;

    private Camera mainCamera;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        mainCamera = Camera.main;
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        float lh = Input.GetAxis("Horizontal");
        float lv = Input.GetAxis("Vertical");

        inputDirection = new Vector3(lh, 0f, lv);
        Vector3 cameraForward = mainCamera.transform.forward;
        cameraForward.y = 0;
        Quaternion cameraRelativeRotation = Quaternion.FromToRotation(Vector3.forward, cameraForward);
        Vector3 lookToward = cameraRelativeRotation * inputDirection;

        if (inputDirection.sqrMagnitude > 0)
        {
            Ray lookRay = new Ray(transform.position, lookToward);
            transform.LookAt(lookRay.GetPoint(1));
        }

        moveVelocity = transform.forward* moveSpeed * inputDirection.sqrMagnitude;
        moveVelocity = Vector3.ClampMagnitude(moveVelocity, moveSpeed);
        Animating();
    }

    void FixedUpdate()
    {
        rb.velocity = moveVelocity;
    }

    void Animating()
    {
        anim.SetFloat("blendSpeed", rb.velocity.magnitude);
    }
 }
