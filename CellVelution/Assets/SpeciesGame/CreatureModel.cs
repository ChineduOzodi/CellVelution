using UnityEngine;
using System.Collections;
using CodeControl;
using System.Collections.Generic;
using NeuralNetwork;

public class CreatureModel : Model {

    public string name;
    public int generation = 1;
    public double age;
    public float nutrition = 10;

    public Genes genes;
    public List<string> geneInterpret;

    public string sex;
    public FoodType foodPref;
    public float reactionTime = 1;
    public float moveSpeed = 0;
    public int numLegs = 0;
    public float mass = 50;
    public float metabolism = 100;
    public float size;
    public float sightDistance = 0;
    public float numEyes = 0;

    public int herb = 0;
    public int omni = 0;
    public int carni = 0;

    //Neural Stuff
    public int numInputs;
    public int numOutputs;
    public int numHiddenLayers;
    public int numNodeHiddenLayers;

    public NeuralNet nNetwork;
    public List<double> inputs;
    public List<double> ouputs;
}
