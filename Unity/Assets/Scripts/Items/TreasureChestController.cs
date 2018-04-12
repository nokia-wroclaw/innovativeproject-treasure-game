using System;
using UnityEngine.SceneManagement;

public class TreasureChestController : Interactables
{
    protected override Func<bool> InteractCondition => () => true;
    protected override void Interact()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
