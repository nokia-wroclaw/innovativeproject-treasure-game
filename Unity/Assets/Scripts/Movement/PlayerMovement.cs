using UnityEngine;

public class PlayerMovement : Movement
{
    protected override void Update()
    {
        float lh = Input.GetAxis("Horizontal");
        float lv = Input.GetAxis("Vertical");

        Move(lh, lv, Camera.main.transform.forward);

        Animating();
    }
}
