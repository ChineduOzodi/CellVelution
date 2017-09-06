/// <copyright file="machinLearning.cs">Copyright (c) 2016 All Rights Reserved</copyright>
/// <author>Chinedu Ozodi</author>
/// <date>12/12/2016</date>

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using NeuralNetwork;

public class machineLearning2 : MonoBehaviour {

    public Text infoText;
    public NeuralMap graph;
    public bool updateGraph;
    public FitnessGraph fitnessGraph;
    public SpeciesGraph speciesGraph;

    public GameObject spawn;
    public GameObject food;
    public Vector2 gameArea;

    public int numInputs = 2;
    public int numOutputs = 2;
    public int numHiddenLayers = 1;
    public int numNodeHiddenLayers = 5;
    public int populationResetTime = 10;
    public int population = 10;
    public int numFood = 3;
    public float speedAdj = .1f;

    int randomWatchSpecies = 0;
    int randomWatchGenome = 0;
    int randomWatchSpawn = 0;

    internal GameObject[] spawns;
    internal GameObject[] foods;
    internal GeneticAlgorithm genAlg;

    internal int updateTime;

    // Use this for initialization
    void Start() {
        //-------------Basic Visual Representation of Neural Network---------------------------//
        //nView = GetComponent<NeuralNetworkView>();
        //nView.CreateNetwork(numInputs, numOutputs, numHiddenLayers, numNodeHiddenLayers);

        spawns = new GameObject[population];
        foods = new GameObject[numFood];

        updateTime = populationResetTime;

        GameObject spawnParent = new GameObject("Spawn Parent");

        //Intiation of neural network array for the population
        //nNetwork = new NeuralNet[population];

        //For Loop to create all the neccessary spawn defintions
        for (int i = 0; i < population; i++)
        {
            spawns[i] = Instantiate(spawn, spawnParent.transform) as GameObject;
            spawns[i].transform.position = new Vector3(Random.Range(0, gameArea.x), Random.Range(0, gameArea.y)); //Set spawn in random map location

            //-------Create neural network with defined inputs and ouputs for each spawn----------//
            //nNetwork[i] = new NeuralNet(numInputs, numOutputs, numHiddenLayers, numNodeHiddenLayers); 

            //-------Creates on Genetic algorithm to handle the whole popluation-----------------//
            if (i == 0)
            {
                genAlg = new GeneticAlgorithm(population,numInputs,numOutputs);
                randomWatchSpecies = Random.Range(0, genAlg.species.Count);
                randomWatchGenome = Random.Range(0, genAlg.species[randomWatchSpecies].population.Count);
                graph.CreateGraph(genAlg.species[randomWatchSpecies].population[randomWatchGenome]);
                speciesGraph.UpdateGraph(genAlg.species,genAlg.generation);
            }
            //--------Set the random weights generated in genAlg into 
            //nNetwork[i].PutWeights(genAlg.population[i].weights);
        }

        //Setup food
        for (int i = 0; i < numFood; i++)
        {
            foods[i] = Instantiate(food);

            foods[i].transform.position = new Vector3(Random.Range(0, gameArea.x), Random.Range(0, gameArea.y));
        }

        //Infotext
        infoText.text = "Generation: " + genAlg.generation;
    }

    // Update is called once per frame
    void Update() {

        //Used to track the best performing spawn
        double bestFitness = 0;
        int bestSpawn = 0;


        //TimeScale
        if (Input.GetKeyDown(KeyCode.Comma))
        {
            Time.timeScale *= .5f;
        }
        if (Input.GetKeyDown(KeyCode.Period))
        {
            Time.timeScale *= 2;
        }
        int species = 0;
        int genome = 0;
        //Update each spawn in the population, used to increase their fitness as needed and have them respond to the inputs
        for (int i = 0; i < population; i++)
        {
            GameObject closestFood = foods[0];
            float distance = Vector3.Distance(closestFood.transform.position, spawns[i].transform.position); //distance to closest food
            SpriteRenderer sprite = spawns[i].GetComponent<SpriteRenderer>();//get spawn sprite
            sprite.color = genAlg.species[species].color;

            for (int j = 0; j < numFood; j++) //Finds the closest food item
            {
                float newDistance = Vector3.Distance(spawns[i].transform.position, foods[j].transform.position);
                if (newDistance < distance)
                {
                    distance = newDistance;
                    closestFood = foods[j];
                }
            }
            
            //relative position of spawn to closest food
            Vector2 relPos = spawns[i].transform.position - closestFood.transform.position;
            relPos.Normalize();

            Vector2 relRotation = spawns[i].transform.up;
            relRotation.Normalize();

            //---------------Create the input list--------------------------------------------\\
            List<double> input = new List<double> { relPos.x, relPos.y , relRotation.x, relRotation.y, 1};

            
            //---------------Get the ouput list by inputing the input list--------------------\\
            List<double> output = genAlg.species[species].population[genome].Run(input);
            

            
            //----------------Applying the output to te spawn---------------------------------\\
            spawns[i].transform.Rotate(new Vector3(0,0,(float)(output[0] - output[1]) * Time.deltaTime * 100));
            spawns[i].transform.Translate( Vector2.up * speedAdj * Time.deltaTime);

            //---------------Increasing Fitness-------------------------------------------------\\
            if (distance < 1f)
            {
                genAlg.species[species].population[genome].fitness += genAlg.species[species].population[genome].fitness * .1F + 1;

                //move food particle
                closestFood.transform.position = new Vector3(Random.Range(0, gameArea.x), Random.Range(0, gameArea.y));
            }


            //Some informational text, and allowing for the search of the most fit spawn
            if (i == 0)
            {
                infoText.text = "Generation: " + genAlg.generation + "\nInputs: relPosX, relPosY, relRotaionX, relRotationY\nOutputs: rightRotation, leftRotation"
                    + "\nTime: " + Time.time.ToString("0")
                    + "\nTime Scale: " + Time.timeScale
                    + "\nSpecies Count: " + genAlg.species.Count;
                bestFitness = genAlg.species[species].population[genome].fitness;
                bestSpawn = i;


                //----------------Use the first spawn as the selected object in visual representation--------------//
                if (updateGraph)
                    graph.NodeUpdate(genAlg.species[randomWatchSpecies].population[randomWatchGenome]);
            }
            else
            {
                if (bestFitness < genAlg.species[species].population[genome].fitness)
                {
                    bestFitness = genAlg.species[species].population[genome].fitness;
                    sprite = spawns[bestSpawn].GetComponent<SpriteRenderer>();
                    sprite.color = genAlg.species[species].color;
                    bestSpawn = i;
                    sprite = spawns[i].GetComponent<SpriteRenderer>();
                    sprite.color = Color.yellow;

                }
            }

            if (species == randomWatchSpecies && genome == randomWatchGenome)
                randomWatchSpawn = i;
            
            genome++;
            if (genome >= genAlg.species[species].population.Count)
            {
                species++;
                genome = 0;
            }

            //Border control
            if (spawns[i].transform.position.y < 0)
                spawns[i].transform.position = new Vector3(spawns[i].transform.position.x, gameArea.y);
            else if (spawns[i].transform.position.y > gameArea.y)
                spawns[i].transform.position = new Vector3(spawns[i].transform.position.x, 0);

            if (spawns[i].transform.position.x < 0)
                spawns[i].transform.position = new Vector3(gameArea.x, spawns[i].transform.position.y);
            else if (spawns[i].transform.position.x > gameArea.x)
                spawns[i].transform.position = new Vector3(0, spawns[i].transform.position.y);

        }

        //More Stats
        genAlg.CalculateBestWorstAvTot();
        infoText.text += "\nBest Fitness: " + genAlg.bestFitness + "\nAverage Fitness: " + genAlg.averageFitness + "\nLowest Fitness: " + genAlg.worstFitness;
        fitnessGraph.UpdateGraph(genAlg.generation, genAlg.bestFitness, genAlg.averageFitness);

        //Change first spawn's color so it is recognizable
        spawns[randomWatchSpawn].GetComponent<SpriteRenderer>().color = Color.cyan;


        //----------------Creates the New Population using the fitness levels of the old one----------------//
        if (Time.time > updateTime)
        {
            updateTime += populationResetTime;

            //--------------The main function that creates the new population into the genAlg class--------//
            genAlg.Epoch();

            for (int i = 0; i < population; i++)
            {
                //change all of the spawns positions
                spawns[i].transform.position = new Vector3(Random.Range(0, gameArea.x), Random.Range(0, gameArea.y));

                //Update the spawns neural networks with the new weights
                //nNetwork[i].PutWeights(genAlg.population[i].weights);
            }

            //change the location of all the food
            for (int i = 0; i < numFood; i++)
            {
                foods[i].transform.position = new Vector3(Random.Range(0, gameArea.x), Random.Range(0, gameArea.y));
            }
            randomWatchSpecies = Random.Range(0, genAlg.species.Count);
            randomWatchGenome = Random.Range(0, genAlg.species[randomWatchSpecies].population.Count);
            graph.CreateGraph(genAlg.species[randomWatchSpecies].population[randomWatchGenome]);
            speciesGraph.UpdateGraph(genAlg.species, genAlg.generation);
        }
    }
}


