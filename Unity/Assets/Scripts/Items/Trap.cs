using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Trap")]
public class Trap : Item
{
    [SerializeField]
    private GameObject itemToDrop;

    public override IEnumerator Use()
    {   
        var playerObject = GameObject.FindGameObjectWithTag("Player");
        var trapPosition = playerObject.transform.position + new Vector3(0, playerObject.transform.localScale.y/2, 0);
        Instantiate(itemToDrop, trapPosition, playerObject.transform.rotation);
        yield return null;
    }
}
