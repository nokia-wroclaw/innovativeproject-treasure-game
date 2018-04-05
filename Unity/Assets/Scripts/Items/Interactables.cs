using System;
using UnityEngine;

public abstract class Interactables : MonoBehaviour
{
    protected bool interactable = false;

    protected virtual Func<bool> InteractCondition { get; set; } = () => true;

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
