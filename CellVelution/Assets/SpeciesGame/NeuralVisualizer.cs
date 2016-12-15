using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

namespace NeuralNetwork
{
    public class NeuralVisualizer : MonoBehaviour
    {
        public bool cameraCenterOnSelectedObject = true;
        public Text infoText;

        internal SpriteRenderer[] inputObj;
        internal SpriteRenderer[][] layersObj;
        internal LineRenderer[][][] weightObj;

        public GameObject lineInstance;
        public GameObject neuralNode;
        public GameObject selectionObject;

        public Creature selectedNeuralObject;


        SpeciesGame species;

        internal int numInputs, numOutputs, numHiddenLayers, numNodeHiddenLayers;

        // Use this for initialization
        void Start()
        {

            species = GetComponent<SpeciesGame>();
            GameObject parent = new GameObject("Visual Neural Network");
            numInputs = species.numInputs;
            numOutputs = species.numOutputs;
            numHiddenLayers = species.numHiddenLayers;
            numNodeHiddenLayers = species.numNodeHiddenLayers;

            //Setup Nodes
            inputObj = new SpriteRenderer[numInputs];
            layersObj = new SpriteRenderer[numHiddenLayers + 1][];
            weightObj = new LineRenderer[numHiddenLayers + 1][][];


            weightObj[0] = new LineRenderer[numInputs][];

            int j = 5;

            for (int i = 0; i < numInputs; i++)
            {
                inputObj[i] = (Instantiate(neuralNode, parent.transform) as GameObject).GetComponent<SpriteRenderer>();

                inputObj[i].transform.position = new Vector3(0, i);
            }


            for (int i = 0; i < numHiddenLayers; i++)
            {
                //Set Hidden Layers
                layersObj[i] = new SpriteRenderer[numNodeHiddenLayers];
                weightObj[i] = new LineRenderer[numNodeHiddenLayers][];

                for (int k = 0; k < numNodeHiddenLayers; k++)
                {
                    layersObj[i][k] = (Instantiate(neuralNode, parent.transform) as GameObject).GetComponent<SpriteRenderer>();

                    layersObj[i][k].transform.position = new Vector3(j, k);

                    //Set Weights

                    if (0 == i)
                    {
                        //Set first hidden Node
                        weightObj[i][k] = new LineRenderer[numInputs];

                        for (int m = 0; m < numInputs; m++)
                        {
                            weightObj[i][k][m] = (Instantiate(lineInstance, parent.transform) as GameObject).GetComponent<LineRenderer>();

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
                            weightObj[i][k][m] = (Instantiate(lineInstance, parent.transform) as GameObject).GetComponent<LineRenderer>();

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
                layersObj[numHiddenLayers][k] = (Instantiate(neuralNode, parent.transform) as GameObject).GetComponent<SpriteRenderer>();

                layersObj[numHiddenLayers][k].transform.position = new Vector3(j, k);

                // SetWeights
                weightObj[numHiddenLayers][k] = new LineRenderer[numNodeHiddenLayers];

                for (int m = 0; m < numNodeHiddenLayers; m++)
                {
                    weightObj[numHiddenLayers][k][m] = (Instantiate(lineInstance, parent.transform) as GameObject).GetComponent<LineRenderer>();

                    LineRenderer line = weightObj[numHiddenLayers][k][m].GetComponent<LineRenderer>();
                    Vector3[] connection = new Vector3[2] { new Vector3(j, k), new Vector3(j - 5, m) };

                    line.SetPositions(connection);
                }
            }

        }

        // Update is called once per frame
        void Update()
        {

            if (selectedNeuralObject)
            {
                selectionObject.transform.position = selectedNeuralObject.transform.position;
                selectionObject.transform.localScale = 1.1f * selectedNeuralObject.transform.localScale;
                NodeUpdate(selectedNeuralObject.Model.inputs);
                infoText.text = selectedNeuralObject.GetInfo();

                if (cameraCenterOnSelectedObject)
                    transform.position = selectedNeuralObject.transform.position;
            }
            else
            {
                selectionObject.transform.position = transform.position;
            }

        }

        public List<double> NodeUpdate(List<double> inputs)
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
                    inputObj[i].transform.localScale = Vector3.one * (1 - (float)inputs[i]);
                }
                else
                {
                    inputObj[i].color = Color.red * (float)(1 - inputs[i] * -1);
                    inputObj[i].transform.localScale = Vector3.one * (1 - (float)inputs[i] * -1);
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

                for (int j = 0; j < selectedNeuralObject.Model.nNetwork.layers[i].numNeurons; j++)
                {
                    double netInput = 0;

                    int nNumInputs = selectedNeuralObject.Model.nNetwork.layers[i].neurons[j].numInputs;

                    //For each Weight

                    for (int k = 0; k < nNumInputs; k++)
                    {
                        //Sum the weights and inputs

                        netInput += selectedNeuralObject.Model.nNetwork.layers[i].neurons[j].weights[k] * inputs[weight];

                        //Set Lines
                        float line = (float)(selectedNeuralObject.Model.nNetwork.layers[i].neurons[j].weights[k] * inputs[weight++]);

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

                    netInput += selectedNeuralObject.Model.nNetwork.layers[i].neurons[j].weights[nNumInputs] * -1;

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



}
