using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;

public class TreeModel : Model {

    public string name;
    public int generation = 1;
    public Date birthDate;

    //-----genes------------------//
    public Genes<Color> treeColor;

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
    public float deathAge = Date.secondsInDay;
    private float reproductionCounter;
    public Vector3 position;
    public bool pollen;
    public bool die;
    public float lastUpdated;

    public Date age
    {
        get
        {
            return new Date(lastUpdated - birthDate.time);
        }
    }

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

    public TreeModel(string _name, Vector3 _position, Date _birthDate)
    {

        name = _name;
        position = _position;
        birthDate = _birthDate;
        maxHeight = Random.Range(0,10f);
        growthRate = Random.Range(0, .2f);
        reproductionRate = Random.Range(0, .2f);
        richness = Random.Range(0, 5f);
        roots = Random.Range(0, 5);
        color = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));

        Dictionary<Color, string[]> phenotypes = new Dictionary<Color, string[]>();
        phenotypes.Add(Color.white, new string[] { "bb" });
        phenotypes.Add(Color.red, new string[] { "tt", "tb", "bt" });
        phenotypes.Add(Color.green, new string[] { "TT", "Tb", "bT", "Tt", "tT", "TB", "BT" });
        phenotypes.Add(Color.blue, new string[] { "BB", "Bb", "bB", "Bt", "tB" });

        treeColor = new Genes<Color>(2, "TtBb", phenotypes);

        lastUpdated = birthDate.time;

    }

    public TreeModel(string _name, Vector3 _position, Date _birthDate, int _generation, float _maxHeight, float _growthRate, float _reproductionRate, float _richness, float _roots, Color _color, Genes<Color> _treeColor)
    {
        name = _name;
        generation = _generation;
        birthDate = _birthDate;
        position = _position;
        maxHeight = _maxHeight;
        growthRate = _growthRate;
        reproductionRate = _reproductionRate;
        richness = _richness;
        roots = Cap(_roots, 0, 5);
        color = _color;
        treeColor = _treeColor;

        lastUpdated = birthDate.time;

        
    }

    public void Grow(float deltaTime)
    {
        
        deathChance += deathAge * deltaTime;
        if (age.Hour > maxHeight)
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
                if (Random.Range(0,2) == 1)
                {
                    pollen = true;
                }
                reproductionCounter = 0;
            }
        }

        lastUpdated += deltaTime;
    }

    private float Cap(float number, float min, float max)
    {
        if (number < min) { return min; }
        else if (number > max) return max;
        else return number;
    }
}
