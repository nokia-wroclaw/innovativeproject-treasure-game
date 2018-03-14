using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerVisibility : MonoBehaviour
{
    public Transform Player;
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
        // Debug.DrawRay(transform.position, rayDirection, Color.red);
        while (true)
        {
            rayDirection = Player.transform.position - transform.position;

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

