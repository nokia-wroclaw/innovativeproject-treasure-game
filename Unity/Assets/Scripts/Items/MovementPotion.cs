using System;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Movement Potion")]
public class MovementPotion : Item
{
    public override IEnumerator Use(Action<bool> result)
    {
        var playerObject = GameObject.FindGameObjectWithTag("Player");
        var movementScript = playerObject.GetComponent<PlayerMovement>();

        if (movementScript.maxMoveSpeed < 14)
        {
            movementScript.maxMoveSpeed += 7;

            result(true);
            yield return new WaitForSeconds(2f);

            movementScript.maxMoveSpeed -= 7;
        }

        result(false);
        yield return null;
    }

}
