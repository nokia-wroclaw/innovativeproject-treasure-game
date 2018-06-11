using UnityEngine;

public class PlayerMovement : Movement
{
    protected void FixedUpdate()
    {
        float lh = Input.GetAxis("Horizontal");
        float lv = Input.GetAxis("Vertical");

        Move(lh, lv, Camera.main.transform.forward);

        Animating();
    }
}
