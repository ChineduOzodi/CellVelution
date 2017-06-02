using CodeControl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SeedController : Controller<SeedModel> {

    Rigidbody rgb;

    private void Start()
    {
        
    }

    protected override void OnModelChanged()
    {
        if (model.die)
        {
            model.Delete();
        }
        if (model.germinate)
        {
            TreeModel tree = new TreeModel("Tree" + GameManager.instance.trees.Count + 1, transform.position, model.generation,model.maxHeight, model.growthRate, model.reproductionRate, model.richness, model.roots, model.color);
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
        model.germination += model.germinationRate * Time.deltaTime;
        if(model.germination > 1)
        {
            model.germinate = true;
            model.NotifyChange();
        }
    }

    public void Delete()
    {
        model.die = true;
        model.NotifyChange();
    }
}
