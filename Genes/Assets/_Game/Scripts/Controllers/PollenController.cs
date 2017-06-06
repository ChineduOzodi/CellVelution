using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;
using System;

public class PollenController : Controller<PollenModel> {

    Rigidbody rgb;
    float timer;

    bool inAir = true;

    private Vector3 windForce;
    private Vector3 sphereDrag;

    protected override void OnInitialize()
    {
        rgb = GetComponent<Rigidbody>();
        transform.position = model.position;
        transform.rotation = model.rotation;
        rgb.AddRelativeForce(new Vector3(model.pollenForce, 0));
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (inAir)
        {
            windForce = GameManager.instance.WindForce(transform.position);
            sphereDrag = GameManager.instance.SphereDrag(gameObject);
            rgb.AddForce(windForce + sphereDrag);
        }
        timer += Time.deltaTime;
        if (timer > 80)
        {
            model.Delete();
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        inAir = false;
    }

    private void OnCollisionExit(Collision collision)
    {
        inAir = true;
    }

    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "tree" && col.gameObject.GetComponentInParent<Transform>().name != model.name + " - " + model.generation)
        {
            col.gameObject.GetComponentInParent<TreeController>().CreateSeed(model);
            model.Delete();
        }
    }
    //private void OnGUI()
    //{
    //    GUI.TextArea(new Rect(10, 120, 150, 100), String.Format("WindForce: {0}\nSphereDrag{1}",windForce,sphereDrag));
    //    if (GUI.Button(new Rect( 10, 230, 50, 20), "Reset"))
    //    {
    //        transform.position = model.position;
    //        transform.rotation = model.rotation;
    //    }
    //}

    protected override void OnModelChanged()
    {
        transform.position = model.position;
        transform.rotation = model.rotation;
    }

    
}
