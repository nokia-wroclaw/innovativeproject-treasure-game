using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerVisibility : MonoBehaviour
{
    public Transform player;
    public float fieldOfViewDegrees;
    public float visibilityDistance;

    void Start()
    {
        StartCoroutine(PlayerVisibilityCheck());
    }

    private IEnumerator PlayerVisibilityCheck()
    {
        RaycastHit hit;
        Vector3 rayDirection;
        
        while (true)
        {
            rayDirection = player.transform.position - transform.position;
            Debug.DrawRay(transform.position, rayDirection, Color.red);

            if (Vector3.Angle(rayDirection, transform.forward) <= fieldOfViewDegrees * 0.5f)
            {
                if (Physics.Raycast(transform.position, rayDirection, out hit, visibilityDistance))
                {
                    if(hit.transform.CompareTag("Player"))
                    {
                        break;
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

