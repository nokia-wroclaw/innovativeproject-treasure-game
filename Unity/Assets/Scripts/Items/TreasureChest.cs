using UnityEngine.SceneManagement;

public class TreasureChest : Interactables
{
    protected override void Interact()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
