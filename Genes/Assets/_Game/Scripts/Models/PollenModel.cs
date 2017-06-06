using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;

public class PollenModel : Model{

    //-----mutatable data---------//
    public float pollenForce = 10;
    public float germinationRate = .1f;

    //-----Tree Genes---------//
    public Genes<Color> treeColor;

    //-----Tree data -----//
    public string name;
    public int generation;
    public Color color;
    public float richness; //max 5
    public float maxHeight; //max 10
    public float seedThrowForce; //max 100
    public float growthRate; //max .2
    public float reproductionRate; //max .2
    public float roots; //max 5

    //-----Keep Track of Tree------//
    public Vector3 position;
    public Quaternion rotation;
    public float germination;
    public bool die;
    public bool germinate;

    //------------------Mutation Rates--------------//
    private float heightMutation = .1f;
    private float growthMutation = .001f;
    private float reprMutation = .0005f;
    private float richMutation = .05f;
    private float rootMutation = .05f;
    private float colorMutation = .001f;

    public PollenModel() { }

    public PollenModel(Vector3 position, Quaternion rotation, string name, int generation, float maxHeight, float growthRate, float reproductionRate, float richness, float roots, Color color, Genes<Color> treeColor)
    {
        this.position = position;
        this.rotation = rotation;
        this.name = name;
        this.generation = generation;
        this.maxHeight = maxHeight;
        this.growthRate = growthRate;
        this.reproductionRate = reproductionRate;
        this.richness = richness;
        this.roots = roots;
        this.color = color;
        this.treeColor = treeColor;


    }
}
