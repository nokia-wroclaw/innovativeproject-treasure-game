using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : Movement
{

    public GameObject pointA;
    public GameObject pointB;
    private bool reverseMove = false;
    private Vector3 position;

    /*protected override float Smoothing
    {
        get
        {   
            return 6.0f;
        }
    }

    */
    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        MovementController();
        Animating();
    }

    private void MovementController()
    {

        if (reverseMove)
        {
            position = new Vector3(0, 0, -1);
        }
        else
        {
            position = new Vector3(0, 0, 1);

        }

        Move(position.x, position.z, pointB.transform);
        // print(Vector3.Distance(this.transform.position, pointB.transform.position));

        if ((Vector3.Distance(this.transform.position, pointB.transform.position) < 2.0f || Vector3.Distance(this.transform.position, pointA.transform.position) < 2.0f))
            reverseMove = !reverseMove;

        print(rb.velocity.magnitude);
    }

   
}
