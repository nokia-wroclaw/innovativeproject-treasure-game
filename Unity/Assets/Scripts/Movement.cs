using UnityEngine;

public abstract class Movement : MonoBehaviour
{
    public float maxMoveSpeed;

    protected Rigidbody rb;
    protected Animator anim;
    protected Vector3 moveVelocity;
    protected Vector3 lookToward;

    protected virtual float Smoothing
    {
        get
        {
            if (rb.velocity.magnitude < 5.5)
            {
                return 30.0f;
            }
            else
            {
                return 6f;
            }
        }
    }

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        lookToward = transform.forward;
    }

    protected abstract void Update();


    protected void FixedUpdate()
    {
        rb.velocity = moveVelocity;
    }

    protected virtual void Move(float xDir, float zDir, Vector3 forward)
    {
        var inputDirection = new Vector3(xDir, 0f, zDir);
        var directionReference = forward;
        directionReference.y = 0;
        var referenceRelativeRotation = Quaternion.FromToRotation(Vector3.forward, directionReference);

        lookToward = Vector3.Lerp(transform.forward, referenceRelativeRotation * inputDirection, Smoothing * Time.deltaTime);
        
        if (inputDirection.sqrMagnitude > 0)
        {
            var lookRay = new Ray(transform.position, lookToward);
            transform.LookAt(lookRay.GetPoint(1));
        }

        var rawMoveVelocity = transform.forward * maxMoveSpeed * inputDirection.magnitude;
        moveVelocity = Vector3.ClampMagnitude(rawMoveVelocity, maxMoveSpeed);
    }

    protected void Animating()
    {
        anim.SetFloat("blendSpeed", rb.velocity.magnitude);
    }
}
