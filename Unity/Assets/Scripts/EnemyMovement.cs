using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : Movement
{
    public GameObject pointA;
    public GameObject pointB;
    private Vector3 direction;

    /*protected override float Smoothing
    {
        get
        {   
            return 5.5f;
        }
    }*/

    protected override void Start()
    {
        base.Start();
        if(Vector3.Distance(this.transform.position, pointA.transform.position) > Vector3.Distance(this.transform.position, pointB.transform.position))
            direction = pointA.transform.position - transform.position;
        else
            direction = pointB.transform.position - transform.position;
    }

    protected override void Update()
    {
        MovementController();
        Animating();
    }

    private void MovementController()
    {
        Move(direction.x, direction.z, Vector3.forward);

        if (Vector3.Distance(this.transform.position, pointB.transform.position) < 3.0f) 
        {
            direction = pointA.transform.position - transform.position;
        }

        if (Vector3.Distance(this.transform.position, pointA.transform.position) < 3.0f) 
        {
            direction = pointB.transform.position - transform.position;
        }

    } 
}
