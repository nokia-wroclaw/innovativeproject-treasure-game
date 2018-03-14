using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Interactables : MonoBehaviour
{
    public void Interact()
    {
        SceneManager.LoadScene("MainMenu");
        print("Interacting");
    }

    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Interact();
        }
    }
}
