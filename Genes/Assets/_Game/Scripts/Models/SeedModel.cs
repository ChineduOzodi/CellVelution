using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;

public class SeedModel : Model {

    //-----mutatable data---------//
    public float seedForce = 10;
    public float germinationChance = .5f;

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

    public SeedModel() {     
    }

    public SeedModel(Vector3 _position, Quaternion _rotation)
    {

        position = _position;
        rotation = _rotation;

        name = "Random Tree";
        position = _position;
        maxHeight = Random.Range(0, 10f);
        growthRate = Random.Range(0, .2f);
        reproductionRate = Random.Range(0, .2f);
        richness = Random.Range(0, 5f);
        color = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));

        
    }

    public SeedModel(Vector3 _position, Quaternion _rotation, string _name, int _generation, float _maxHeight, float _growthRate, float _reproductionRate, float _richness, float _roots, Color _color, Genes<Color> _treeColor)
    {
        germinationChance = .1f;
        seedForce = 100f;

        position = _position;
        rotation = _rotation;

        name = _name;
        generation = _generation + 1;
        maxHeight = Cap(Random.Range(_maxHeight - 1, _maxHeight + 1),0,10);

        growthRate = Cap(Random.Range(_growthRate - growthMutation, _growthRate + growthMutation),0,.2f);
        reproductionRate = Cap(Random.Range(_reproductionRate - reprMutation,_reproductionRate + reprMutation), 0, .2f);
        richness = Cap(Random.Range(_richness - richMutation, richness + richMutation),0.1f, 5f);
        roots = Random.Range(_roots - rootMutation, _roots + rootMutation);
        color = new Color(Cap(Random.Range(_color.r - colorMutation, color.r + colorMutation), 0, 1f),
            Cap(Random.Range(_color.g - colorMutation, color.g + colorMutation), 0, 1f),
           Cap(Random.Range(_color.b - colorMutation, color.b + colorMutation), 0, 1f));
        treeColor = _treeColor;

    }

    private float Cap(float number, float min, float max)
    {
        if (number < min) { return min; }
        else if (number > max) return max;
        else return number;
    }
}
