using CodeControl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SeedController : Controller<SeedModel> {

    
    Rigidbody rgb;
    bool inAir;
    private Vector3 windForce;
    private Vector3 sphereDrag;

    protected override void OnModelChanged()
    {
        if (model.die)
        {
            model.Delete();
        }
        if (model.germinate)
        {
            TreeModel tree = new TreeModel("Tree" + GameManager.instance.trees.Count + 1, transform.position, GameManager.instance.date, model.generation,model.maxHeight, model.growthRate, model.reproductionRate, model.richness, model.roots, model.color, model.treeColor);
            GameManager.instance.trees.Add(tree);
            Controller.Instantiate<TreeController>("tree", tree);
            model.Delete();
        }

    }

    protected override void OnInitialize()
    {
        rgb = GetComponent<Rigidbody>();
        transform.position = model.position;
        transform.rotation = model.rotation;
        rgb.AddRelativeForce(new Vector3(model.seedForce, 0));
    }

    private void Update()
    {
        model.germination += model.germinationChance * Time.deltaTime;
        if(model.germination > 1)
        {
            model.germinate = true;
            model.NotifyChange();
        }

        if (inAir)
        {
            windForce = GameManager.instance.WindForce(transform.position);
            sphereDrag = GameManager.instance.SphereDrag(gameObject);
            rgb.AddForce(windForce + sphereDrag);
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
    public void Delete()
    {
        model.die = true;
        model.NotifyChange();
    }
}
