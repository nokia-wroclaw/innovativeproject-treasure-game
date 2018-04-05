using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapController : MonoBehaviour {

	//GameObject Object;
	void OnTriggerEnter(Collider other)
    {
		
        if (other.gameObject.tag == "Enemy")
        {
            var patroller = other.GetComponent<Patroller>();
			patroller.StunEnemy();			
			Destroy(gameObject);
        }
    }
}
