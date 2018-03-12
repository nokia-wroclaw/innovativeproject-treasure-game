using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerVisibility : MonoBehaviour
{
    public Transform Player;
    public float fieldOfViewDegrees;
    public float visibilityDistance;

    void Update()
    {
        if (PlayerVisible())
            //print("Visible");
        SwitchScene();
    }

    private bool PlayerVisible()
    {
        RaycastHit hit;
        Vector3 rayDirection = Player.transform.position - transform.position;
       // Debug.DrawRay(transform.position, rayDirection, Color.red);
        if ((Vector3.Angle(rayDirection, transform.forward)) <= fieldOfViewDegrees * 0.5f)
        {
            if (Physics.Raycast(transform.position, rayDirection, out hit, visibilityDistance))
            {
                return (hit.transform.CompareTag("Player"));
            }
        }

        return false;
    }

    private void SwitchScene()
    {
        SceneManager.LoadScene("MainMenu");
    }
}

