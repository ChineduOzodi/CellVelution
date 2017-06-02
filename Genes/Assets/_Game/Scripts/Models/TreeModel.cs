using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;

public class TreeModel : Model {

    public string name;
    public int generation = 1;

    //-----mutatable data---------//
    public Color color;
    public float richness; //max 5
    public float maxHeight; //max 10
    public float seedThrowForce; //max 100
    public float growthRate; //max .2
    public float reproductionRate; //max .2
    public float roots; //max 5

    //-----Keep Track of Tree------//
    public float height;
    public float deathChance = 0;
    public float deathRate = .000001f;
    private float reproductionCounter;
    public Vector3 position;
    public bool seed;
    public bool die;

    public TreeModel() { }

    public TreeModel(string _name, Vector3 _position, float _maxHeight)
    {
        name = _name;
        position = _position;
        maxHeight = _maxHeight;
        growthRate = .1f;
        reproductionRate = .1f;
        richness = 1;
        roots = 2;
        color = Color.green;
    }

    public TreeModel(string _name, Vector3 _position)
    {

        name = _name;
        position = _position;
        maxHeight = Random.Range(0,10f);
        growthRate = Random.Range(0, .2f);
        reproductionRate = Random.Range(0, .2f);
        richness = Random.Range(0, 5f);
        roots = Random.Range(0, 5);
        color = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));

    }

    public TreeModel(string _name, Vector3 _position, int _generation, float _maxHeight, float _growthRate, float _reproductionRate, float _richness, float _roots, Color _color)
    {
        name = _name;
        generation = _generation;
        position = _position;
        maxHeight = _maxHeight;
        growthRate = _growthRate;
        reproductionRate = _reproductionRate;
        richness = _richness;
        roots = Cap(_roots, 0, 5);
        color = _color;
    }

    public void Grow(float deltaTime)
    {
        
        deathChance += deathRate * deltaTime;
        if (Random.Range(0,1f) < deathChance)
        {
            die = true;
        }

        if (height < maxHeight)
        {
            height += deltaTime * growthRate;
        }
        else
        {
            reproductionCounter += reproductionRate * deltaTime;
            if (reproductionCounter >= 1)
            {
                seed = true;
                reproductionCounter = 0;
            }
        }
    }

    private float Cap(float number, float min, float max)
    {
        if (number < min) { return min; }
        else if (number > max) return max;
        else return number;
    }
}
