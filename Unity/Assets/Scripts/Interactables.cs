using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Interactables : MonoBehaviour
{
    public bool interactable = false;

    public virtual void Interact()
    {
        SceneManager.LoadScene("MainMenu");
        print("Interacting");
    }

    void Update()
    {
        if (interactable)
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
