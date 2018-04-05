using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerVisibility : MonoBehaviour
{   
    public bool isStuned = false;
    public float fieldOfViewDegrees;
    public float visibilityDistance;
    public float endGameDistance;
    public bool Chasing { get; private set; }
    public GameObject Player { get; private set; }

    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(PlayerVisibilityCheck());
    }

    private IEnumerator PlayerVisibilityCheck()
    {
        RaycastHit hit;
        Vector3 rayDirection;
        
        while (true)
        {
            rayDirection = Player.transform.position - transform.position;
            Debug.DrawRay(transform.position, rayDirection, Color.red);

            Chasing = false;

            if (Vector3.Angle(rayDirection, transform.forward) <= fieldOfViewDegrees * 0.5f)
            {
                if (Physics.Raycast(transform.position, rayDirection, out hit))
                {
                    if (hit.transform.CompareTag("Player"))
                    {
                        if (hit.distance <= endGameDistance)
                            if(isStuned)
                            {
                                yield return new WaitForSeconds(10.0f);
                                isStuned = false;
                            }
                            else
                                break;

                        if (hit.distance <= visibilityDistance)
                            Chasing = true;
                    }
                }
            }

            yield return new WaitForSeconds(0.2f);
        }
        SwitchScene();
    }

    private void SwitchScene()
    {
        SceneManager.LoadScene("MainMenu");
    }
}

