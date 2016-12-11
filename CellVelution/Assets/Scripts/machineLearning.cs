using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class machineLearning : MonoBehaviour {

    public Text infoText;

    internal SpriteRenderer[] inputObj;
    internal SpriteRenderer[][] layersObj;
    internal LineRenderer[][][] weightObj;

    public GameObject lineInstance;
    public GameObject neuralNode;
    public GameObject spawn;
    public GameObject food;
    public Vector2 gameArea;

    public int numInputs = 2;
    public int numOutputs = 2;
    public int numHiddenLayers = 1;
    public int numNodeHiddenLayers = 5;
    public int populationTime = 10;
    public int population = 10;
    public int numFood = 3;
    public float speedAdj = .1f;

    internal GameObject[] spawns;
    internal GameObject[] foods;
    internal NeuralNet[] nNetwork;
    internal GenAlg genAlg;

    internal float[] distances;
    internal int[] closestFoodIndex;
    internal int updateTime;
    // Use this for initialization
    void Start() {
        updateTime = populationTime;
        spawns = new GameObject[population];
        foods = new GameObject[numFood];
        nNetwork = new NeuralNet[population];
        distances = new float[population];
        closestFoodIndex = new int[population];

        //lineInstance = Resources.Load("line") as GameObject;

        for (int i = 0; i < population; i++)
        {
            spawns[i] = Instantiate(spawn);
            spawns[i].transform.position = new Vector3(Random.Range(0, gameArea.x), Random.Range(0, gameArea.y));
            nNetwork[i] = new NeuralNet(numInputs, numOutputs, numHiddenLayers, numNodeHiddenLayers);
            distances[i] = 0;
            closestFoodIndex[i] = 0;
            if (i == 0)
            {
                genAlg = new GenAlg(population, .2, .7, nNetwork[0].GetNumWeights());
            }

            nNetwork[i].PutWeights(genAlg.population[i].weights);

            //genAlg.population[i] = new Genome(genAlg.population[i].weights, 1);
        }

        //Setup Nodes
        inputObj = new SpriteRenderer[numInputs];
        layersObj = new SpriteRenderer[numHiddenLayers + 1][];
        weightObj = new LineRenderer[numHiddenLayers + 1][][];


        weightObj[0] = new LineRenderer[numInputs][];

        int j = 5;

        for (int i = 0; i < numInputs; i++)
        {
            inputObj[i] = Instantiate(neuralNode).GetComponent<SpriteRenderer>();

            inputObj[i].transform.position = new Vector3(0, i);
        }


        for (int i = 0; i < numHiddenLayers; i++)
        {
            //Set Hidden Layers
            layersObj[i] = new SpriteRenderer[numNodeHiddenLayers];
            weightObj[i] = new LineRenderer[numNodeHiddenLayers][];

            for (int k = 0; k < numNodeHiddenLayers; k++)
            {
                layersObj[i][k] = Instantiate(neuralNode).GetComponent<SpriteRenderer>();

                layersObj[i][k].transform.position = new Vector3(j, k);

                //Set Weights

                if (0 == i)
                {
                    //Set first hidden Node
                    weightObj[i][k] = new LineRenderer[numInputs];

                    for (int m = 0; m < numInputs; m++)
                    {
                        weightObj[i][k][m] = Instantiate(lineInstance).GetComponent<LineRenderer>();

                        LineRenderer line = weightObj[i][k][m].GetComponent<LineRenderer>();
                        Vector3[] connection = new Vector3[2] { new Vector3(j, k), new Vector3(j - 5, m) };

                        line.SetPositions(connection);
                    }

                }
                else
                {
                    weightObj[i][k] = new LineRenderer[numNodeHiddenLayers];

                    for (int m = 0; m < numNodeHiddenLayers; m++)
                    {
                        weightObj[i][k][m] = Instantiate(lineInstance).GetComponent<LineRenderer>();

                        LineRenderer line = weightObj[i][k][m].GetComponent<LineRenderer>();
                        Vector3[] connection = new Vector3[2] { new Vector3(j, k), new Vector3(j - 5, m) };

                        line.SetPositions(connection);
                    }

                }
            }

            j += 5;
        }

        //Set output layer

        layersObj[numHiddenLayers] = new SpriteRenderer[numOutputs];
        weightObj[numHiddenLayers] = new LineRenderer[numOutputs][];

        for (int k = 0; k < numOutputs; k++)
        {
            layersObj[numHiddenLayers][k] = Instantiate(neuralNode).GetComponent<SpriteRenderer>();

            layersObj[numHiddenLayers][k].transform.position = new Vector3(j, k);

            // SetWeights
            weightObj[numHiddenLayers][k] = new LineRenderer[numNodeHiddenLayers];

            for (int m = 0; m < numNodeHiddenLayers; m++)
            {
                weightObj[numHiddenLayers][k][m] = Instantiate(lineInstance).GetComponent<LineRenderer>();

                LineRenderer line = weightObj[numHiddenLayers][k][m].GetComponent<LineRenderer>();
                Vector3[] connection = new Vector3[2] { new Vector3(j, k), new Vector3(j - 5, m) };

                line.SetPositions(connection);
            }
        }

        for (int i = 0; i < numFood; i++)
        {
            foods[i] = Instantiate(food);

            foods[i].transform.position = new Vector3(Random.Range(0, gameArea.x), Random.Range(0, gameArea.y));
        }

        infoText.text = "Generation: " + genAlg.generation;
    }

    // Update is called once per frame
    void Update() {

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

        for (int i = 0; i < population; i++)
        {
            GameObject closestFood = foods[closestFoodIndex[i]];
            float distance = Vector3.Distance(closestFood.transform.position, spawns[i].transform.position);
            SpriteRenderer sprite = spawns[i].GetComponent<SpriteRenderer>();
            sprite.color = Color.blue;
            bool newFood = false;

            for (int j = 0; j < numFood; j++)
            {
                float newDistance = Vector3.Distance(spawns[i].transform.position, foods[j].transform.position);
                if (newDistance < distance)
                {
                    distance = newDistance;
                    closestFood = foods[j];
                    closestFoodIndex[i] = j;
                    newFood = true;
                }
            }

            if (distances[i] == 0 || newFood)
            {
                distances[i] = distance;

            }
            else
            {
                float addFitness = - distance + distances[i];
                distances[i] = distance;
                if (genAlg.population[i].fitness + addFitness < 0)
                    genAlg.population[i] = new Genome(genAlg.population[i].weights, 0);
                else if (Mathf.Abs(addFitness) < 5)
                    genAlg.population[i] = new Genome(genAlg.population[i].weights, genAlg.population[i].fitness + addFitness - (.01 * distance));
            }
            
            Vector2 relPos = spawns[i].transform.position - closestFood.transform.position;
            
            relPos.Normalize();

            Vector2 relRotation = spawns[i].transform.up;

            List<double> input = new List<double> { relPos.x, relPos.y , relRotation.x, relRotation.y};
            List<double> output = nNetwork[i].Update(input);

            if (i == 0)
            {
                infoText.text = "Generation: " + genAlg.generation + "\nInputs: relPosX, relPosY, relRotaionX, relRotationY\nOutputs: rightRotation, leftRotation"
                    + "\nTime: " + Time.time.ToString("0")
                    + "\nTime Scale: " + Time.timeScale;
                bestFitness = genAlg.population[i].fitness;
                bestSpawn = i;
                sprite.color = Color.yellow;
                NodeUpdate(0,input);
            }
            else
            {
                //infoText.text += "\nFitness: " + genAlg.population[i].fitness+ "\nOutputs: ";
                //foreach (double d in output) { infoText.text += d + ", "; }

                //" Input/Ouput: [" + input[0].ToString("0.00") + "," + input[1].ToString("0.00") + "]/[" + output[0].ToString("0.00") + "," + output[1].ToString("0.00") + "]"

                if (bestFitness < genAlg.population[i].fitness)
                {
                    bestFitness = genAlg.population[i].fitness;
                    sprite = spawns[bestSpawn].GetComponent<SpriteRenderer>();
                    sprite.color = Color.blue;
                    bestSpawn = i;
                    sprite = spawns[i].GetComponent<SpriteRenderer>();
                    sprite.color = Color.yellow;

                }
            }

            //print(output);
            //Vector3 leftRight = Vector3.right * (float)(output[0] - output[1]) * speedAdj * Time.deltaTime;

            Rigidbody2D rgb = spawns[i].GetComponent<Rigidbody2D>();
            spawns[i].transform.Rotate(new Vector3(0,0,(float)(output[0] - output[1]) * Time.deltaTime * 1000));
            rgb.AddRelativeForce(Vector3.up *speedAdj * Time.deltaTime);
            layersObj[numHiddenLayers][0].color = Color.cyan * (float)(1 - output[0]);
            //layersObj[numHiddenLayers][1].color = Color.cyan * (float)(1 - output[1]);

            if (distance < .1f)
            {
                genAlg.population[i] = new Genome(genAlg.population[i].weights, genAlg.population[i].fitness + 10);

                closestFood.transform.position = new Vector3(Random.Range(0, gameArea.x), Random.Range(0, gameArea.y));
            }
            else
            {
                //genAlg.population[i] = new Genome(genAlg.population[i].weights, genAlg.population[i].fitness + 1f/distance);
            }

            //if (spawns[i].transform.position.y < 0)
            //    spawns[i].transform.position = new Vector3(spawns[i].transform.position.x, gameArea.y);
            //else if (spawns[i].transform.position.y > gameArea.y)
            //    spawns[i].transform.position = new Vector3(spawns[i].transform.position.x, 0);

            //if (spawns[i].transform.position.x < 0)
            //    spawns[i].transform.position = new Vector3(gameArea.x, spawns[i].transform.position.y);
            //else if (spawns[i].transform.position.x > gameArea.x)
            //    spawns[i].transform.position = new Vector3(0, spawns[i].transform.position.y);

        }

        infoText.text += "\n Best Fitness: " + bestFitness +"\nWeights: ";

        //foreach (double i in genAlg.population[bestSpawn].weights) { infoText.text += i + "\n"; }

        //set line widths and colors

        spawns[0].GetComponent<SpriteRenderer>().color = Color.cyan;

        if (Time.time > updateTime)
        {
            updateTime += populationTime;

            genAlg.Epoch(genAlg.population);

            for (int i = 0; i < population; i++)
            {
                spawns[i].transform.position = new Vector3(Random.Range(0, gameArea.x), Random.Range(0, gameArea.y));
                nNetwork[i].PutWeights(genAlg.population[i].weights);
                //genAlg.population[i] = new Genome(genAlg.population[i].weights, 1);
                SpriteRenderer sprite = spawns[i].GetComponent<SpriteRenderer>();
                sprite.color = Color.red;
            }
            for (int i = 0; i < numFood; i++)
            {
                foods[i].transform.position = new Vector3(Random.Range(0, gameArea.x), Random.Range(0, gameArea.y));
            }
        }
    }

    public List<double> NodeUpdate(int id, List<double> inputs)
    {
        List<double> outputs = new List<double>();

        int weight = 0;

        if (inputs.Count != numInputs)
        {
            return outputs;
        }

        //Set Input Nodes
        for (int i = 0; i < inputs.Count; i++)
        {
            if (inputs[i] > 0)
            {
                inputObj[i].color = Color.green * (float)(1 - inputs[i]);
                inputObj[i].transform.localScale = Vector3.one * (1 -(float)inputs[i]);
            }
            else
            {
                inputObj[i].color = Color.red * (float)(1 - inputs[i] * -1);
                inputObj[i].transform.localScale = Vector3.one * (1 -(float)inputs[i] * -1);
            }
        }

        for (int i = 0; i < numHiddenLayers + 1; i++)
        {
            if (i > 0)
            {
                inputs = outputs;
            }

            outputs = new List<double>();
            weight = 0;

            for (int j = 0; j <  nNetwork[id].layers[i].numNeurons; j++)
            {
                double netInput = 0;

                int nNumInputs = nNetwork[id].layers[i].neurons[j].numInputs;

                //For each Weight

                for (int k = 0; k < nNumInputs; k++)
                {
                    //Sum the weights and inputs

                    netInput += nNetwork[id].layers[i].neurons[j].weights[k] * inputs[weight];

                    //Set Lines
                    float line =(float)( nNetwork[id].layers[i].neurons[j].weights[k] * inputs[weight++]);

                    if (line > 0)
                    {
                        weightObj[i][j][k].SetColors(Color.green * line, Color.green * line);
                        weightObj[i][j][k].SetWidth(line * .25f, line * .25f);
                    }
                    else
                    {
                        weightObj[i][j][k].SetColors(Color.red * -line, Color.red * -line);
                        weightObj[i][j][k].SetWidth(-line * .25f, -line * .25f);
                    }
                    
                }

                //Add bias

                netInput += nNetwork[id].layers[i].neurons[j].weights[nNumInputs] * -1;

                double sig = NeuralNet.Sigmoid(netInput, 10);
                //Set Node Color
                layersObj[i][j].color = (Color.green * (float)sig);
                layersObj[i][j].transform.localScale = (Vector3.one * (float)sig);

                outputs.Add(sig);

                weight = 0;
            }

        }
        return outputs;
    }
}


