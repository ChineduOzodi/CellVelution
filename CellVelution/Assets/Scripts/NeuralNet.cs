using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NeuralNet
{
    internal int numInputs, numOutputs, numHiddenLayers, numNeuronHiddenLayer;

    private int numWeights;

    public NeuronLayer[] layers;

    public NeuralNet(int _numInputs, int _numOutputs, int _numHiddenLayers, int _numNeuronHiddenLayer)
    {
        numInputs = _numInputs;
        numOutputs = _numOutputs;
        numHiddenLayers = _numHiddenLayers;
        numNeuronHiddenLayer = _numNeuronHiddenLayer;

        CreateNet();
        GetNumWeights();
    }

    internal void CreateNet()
    {
        //Sum the weights and inputs
        layers = new NeuronLayer[numHiddenLayers + 1];

        if (numHiddenLayers > 0)
        {
            layers[0] = new NeuronLayer(numNeuronHiddenLayer, numInputs);

            for (int k = 1; k < numHiddenLayers; k++)
            {
                layers[k] = new NeuronLayer(numNeuronHiddenLayer, numNeuronHiddenLayer);
            }

            layers[numHiddenLayers] = new NeuronLayer(numOutputs, numNeuronHiddenLayer);
        }
        else
        {
            layers[0] = new NeuronLayer(numOutputs, numInputs);
        }
    }

    public double[] GetWeights()
    {
        double[] weights = new double[numWeights];

        int count = 0;

        for (int i = 0; i < numHiddenLayers + 1; i++)
        {
            for (int j = 0; j < layers[i].numNeurons; j++)
            {

                //For each Weight

                for (int k = 0; k < layers[i].neurons[j].weights.Length; k++)
                {
                    weights[count++] = layers[i].neurons[j].weights[k];
                }

            }

        }

        return weights;
    }

    public int GetNumWeights()
    {
        int count = 0;

        for (int i = 0; i < numHiddenLayers + 1; i++)
        {
            for (int j = 0; j < layers[i].numNeurons; j++)
            {

                //For each Weight

                for (int k = 0; k < layers[i].neurons[j].weights.Length; k++)
                {
                    count++;
                }

            }

        }

        numWeights = count;
        return count;
    }

    public void PutWeights(double[] weights)
    {
        int count = 0;

        for (int i = 0; i < numHiddenLayers + 1; i++)
        {
            for (int j = 0; j < layers[i].numNeurons; j++)
            {

                //For each Weight

                for (int k = 0; k < layers[i].neurons[j].weights.Length; k++)
                {
                    layers[i].neurons[j].weights[k] = weights[count++];
                }

            }

        }

        return;
    }

    public List<double> Update(List<double> inputs)
    {
        List<double> outputs = new List<double>();

        int weight = 0;

        if (inputs.Count != numInputs)
        {
            return outputs;
        }

        for (int i = 0; i < numHiddenLayers + 1; i++)
        {
            if (i > 0)
            {
                inputs = outputs;
            }

            outputs = new List<double>();
            weight = 0;

            for (int j = 0; j < layers[i].numNeurons; j++)
            {
                double netInput = 0;

                int nNumInputs = layers[i].neurons[j].numInputs;

                //For each Weight

                for (int k = 0; k < nNumInputs; k++)
                {
                    //Sum the weights and inputs

                    netInput += layers[i].neurons[j].weights[k] * inputs[weight++];
                }

                //Add bias

                netInput += layers[i].neurons[j].weights[nNumInputs] * -1;

                outputs.Add(Sigmoid(netInput, 10));

                weight = 0;
            }

        }
        return outputs;
    }




    public static double Sigmoid(double activation, double response)
    {
        return 1.0 / (1 + Mathf.Pow(Mathf.Epsilon, (float)(-activation / response)));
    }
}