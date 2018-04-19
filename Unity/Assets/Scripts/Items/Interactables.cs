using System;
using UnityEngine;

public abstract class Interactables : MonoBehaviour
{
    protected abstract Func<bool> InteractCondition { get; }

    protected abstract void Interact();

    private bool _interactable = false;

    private void Update()
    {
        if(_interactable && InteractCondition())
        {
            Interact();
        }
    }
    
    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            _interactable = true;
        }
    }
    protected virtual void OnTriggerExit(Collider other)
    {

        if (other.gameObject.tag == "Player")
        {
            _interactable = false;
        }
    }
    
}
