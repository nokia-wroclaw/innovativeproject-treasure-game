using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Movement Potion")]
public class MovementPotion : Item
{
    public override IEnumerator Use()
    {
        var playerObject = GameObject.FindGameObjectWithTag("Player");
        var movementScript = playerObject.GetComponent<PlayerMovement>();

        if (movementScript.maxMoveSpeed < 14)
        {
            movementScript.maxMoveSpeed += 7;

            yield return new WaitForSeconds(2f);

            movementScript.maxMoveSpeed -= 7;
        }

        yield return null;
    }

}
