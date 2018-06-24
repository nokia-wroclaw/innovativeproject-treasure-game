using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TreasureChestController : Interactables
{
    protected override Func<bool> InteractCondition => () => true;
    private  GameplayManager _gameplayManager;
    void Start()
    {
        _gameplayManager = GameObject.FindGameObjectWithTag("GameplayManager").GetComponent<GameplayManager>();
    }
    

    protected override void Interact()
    {
        _gameplayManager.Win();
    }
}
