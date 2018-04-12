using UnityEngine;

public class TrapController : PickupItem
{
    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        if(gameObject.tag == "DroppedItem" &&  other.gameObject.tag == "Enemy")
        {
            var patroller = other.GetComponent<Patroller>();
            patroller.StunEnemy();
            Destroy(gameObject);
        }
    }
}
