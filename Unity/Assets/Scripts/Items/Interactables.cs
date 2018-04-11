using System;
using UnityEngine;

public abstract class Interactables : MonoBehaviour
{
    protected bool interactable = false;

    protected abstract Func<bool> InteractCondition { get; }

    protected abstract void Interact();

    protected void Update()
    {
        if(interactable && InteractCondition())
        {
            Interact();
        }
    }
    
    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            interactable = true;
        }
    }
    protected virtual void OnTriggerExit(Collider other)
    {

        if (other.gameObject.tag == "Player")
        {
            interactable = false;
        }
    }
    
}
