using UnityEngine;
using UnityEngine.SceneManagement;

public class Interactables : MonoBehaviour
{
    private bool interactable = false;

    public virtual void Interact()
    {
    }

    void Update()
    {
        if(interactable && Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            interactable = true;
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            interactable = false;
        }
    }
    
}
