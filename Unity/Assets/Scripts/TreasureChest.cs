using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/TreasureChest")]
public class TreasureChest : Item {

	public override void Use()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
