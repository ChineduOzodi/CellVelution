using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeGermControl : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

    private void OnTriggerStay(Collider other)
    {
        //print("Triggered by " + other.gameObject.name);
        if(other.gameObject.tag == "tree" && other.GetComponentInParent<Transform>().name != GetComponentInParent<Transform>().name)
        {
            other.GetComponentInParent<TreeController>().IncreaseDeath();
        }
    }
}
