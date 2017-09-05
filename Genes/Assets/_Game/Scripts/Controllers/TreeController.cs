using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;
using System;

public class TreeController : Controller<TreeModel> {

    public Transform seedLaunch;
    public GameObject seed;
    public SphereCollider germTermRange;
    public MeshRenderer meshRender;

    protected override void OnInitialize()
    {
        name = model.name + " g " + model.generation;
        transform.position = model.position;
        germTermRange.radius = model.roots * model.richness;
        meshRender.material.color = model.color;
    }

    internal void IncreaseDeath()
    {
        model.deathChance += (1 - Mathf.Pow(model.deathChance, 10))/100000 * Time.deltaTime;
    }

    // Update is called once per frame
    void Update () {


		
	}

    protected override void OnModelChanged()
    {
        if (model.die)
        {
            model.Delete();
        }
        else
        {
            transform.localScale = new Vector3(model.richness, model.height, model.richness);
            if (model.seed) { ThrowSeed(); }
        }
    }

    private void ThrowSeed()
    {
        float direction = UnityEngine.Random.Range(0, 360f);
        seedLaunch.transform.eulerAngles = new Vector3(0, direction, 0);
        SeedModel seed = new SeedModel(seedLaunch.position, seedLaunch.transform.rotation, model.name, model.generation, model.maxHeight, model.growthRate, model.reproductionRate, model.richness, model.roots, model.color);
        Controller.Instantiate<SeedController>("seed",seed,GenesGameManager.instance.seeds.transform);

        model.seed = false;
    }
}
