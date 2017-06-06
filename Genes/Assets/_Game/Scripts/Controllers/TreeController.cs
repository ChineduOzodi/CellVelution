using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;
using System;

public class TreeController : Controller<TreeModel> {

    public Transform launch;
    public GameObject pollen;
    public GameObject seed;
    public SphereCollider germTermRange;
    public MeshRenderer meshRender;

    protected override void OnInitialize()
    {
        name = model.name + " - " + model.generation;
        transform.position = model.position;
        germTermRange.radius = model.roots * model.richness;
        meshRender.material.color = model.treeColor.phenotype;
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
            if (model.pollen) { ThrowPollen(); }
        }
    }

    

    private void ThrowPollen()
    {
        float direction = UnityEngine.Random.Range(0, 360f);
        launch.transform.eulerAngles = new Vector3(0, direction, 0);
        PollenModel pollen = new PollenModel(launch.position, launch.transform.rotation, model.name, model.generation, model.maxHeight, model.growthRate, model.reproductionRate, model.richness, model.roots, model.color, model.treeColor);
        Controller.Instantiate<PollenController>("pollen", pollen, GameManager.instance.seeds.transform);

        model.pollen = false;
    }

    public void CreateSeed(PollenModel pollen)
    {
        
        float direction = UnityEngine.Random.Range(0, 360f);
        launch.transform.eulerAngles = new Vector3(0, direction, 0);
        SeedModel seed = Pollinate(pollen);
        if (seed != null)
        Controller.Instantiate<SeedController>("seed", seed, GameManager.instance.seeds.transform);

        model.pollen = false;
    }
    /// <summary>
    /// Randomly mixes the genes of the pollen and the tree
    /// </summary>
    /// <param name="pollen"></param>
    /// <returns></returns>
    private SeedModel Pollinate(PollenModel pollen)
    {
        if (UnityEngine.Random.Range(0, 2) == 1)
        {
            float maxHeight = RandomChoice(pollen.maxHeight, model.maxHeight);
            float growthRate = RandomChoice(pollen.growthRate, model.growthRate);
            float reproductionRate = RandomChoice(pollen.reproductionRate, model.reproductionRate);
            float richness = RandomChoice(pollen.richness, model.richness);
            float roots = RandomChoice(pollen.roots, model.roots);
            Color color = RandomChoice(pollen.color, model.color);
            Genes<Color> treeColor = model.treeColor.Child(model.treeColor, pollen.treeColor);
            return new SeedModel(launch.position, launch.transform.rotation, model.name, model.generation, maxHeight, growthRate, reproductionRate, richness, roots, color, treeColor);
        }
        else return null;
        
    }

    private T RandomChoice<T>(T choice1, T choice2)
    {
        if(UnityEngine.Random.Range(0,2) == 1)
        {
            return choice1;
        }
        else
        {
            return choice2;
        }
    }

    public string GetInfo()
    {
        return String.Format("Name: {0}\nPhenotype: {1}\nGenotype: {2}", model.name + " - " + model.generation, model.treeColor.phenotype, model.treeColor.genotype);
    }
}
