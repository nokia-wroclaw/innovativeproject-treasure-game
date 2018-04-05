using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Trap")]
public class Trap : Item {

    public GameObject trap;
	public override void Use()
    {
        var playerObject = GameObject.FindGameObjectsWithTag("Player");
        var trapPosition = playerObject[0].transform.position + new Vector3(0, playerObject[0].transform.localScale.y/2, 0);
        Instantiate(trap, trapPosition, playerObject[0].transform.rotation);
    }
}
