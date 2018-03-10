using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float maxMoveSpeed;
    private float smoothing;
    private Rigidbody rb;
    private Animator anim;

    private Vector3 inputDirection;
    private Vector3 moveVelocity;
    private Vector3 lookToward;
    private Camera mainCamera;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        mainCamera = Camera.main;
        anim = GetComponent<Animator>();
        lookToward = transform.forward;
    }

    void Update()
    {
        float lh = Input.GetAxis("Horizontal");
        float lv = Input.GetAxis("Vertical");

        inputDirection = new Vector3(lh, 0f, lv);
        Vector3 cameraForward = mainCamera.transform.forward;
        cameraForward.y = 0;
        Quaternion cameraRelativeRotation = Quaternion.FromToRotation(Vector3.forward, cameraForward);

        if(rb.velocity.magnitude < 4.5)
        {
            smoothing = 30.0f;
        }
        else
        {
            smoothing = 6f;
        }

        lookToward = Vector3.Lerp (transform.forward, cameraRelativeRotation * inputDirection, smoothing*Time.deltaTime);

        if (inputDirection.sqrMagnitude > 0)
        {
            Ray lookRay = new Ray(transform.position, lookToward);
            transform.LookAt(lookRay.GetPoint(1));
        }
        
        moveVelocity = transform.forward* maxMoveSpeed * inputDirection.sqrMagnitude;
        moveVelocity = Vector3.ClampMagnitude(moveVelocity, maxMoveSpeed);

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
