using UnityEngine;
using System.Collections;

public struct NeuronLayer
{
    internal int numNeurons;

    internal Neuron[] neurons;

    public NeuronLayer(int _numNeurons, int numInputsPerNeuron)
    {
        numNeurons = _numNeurons;

        neurons = new Neuron[numNeurons];

        for (int i = 0; i < numNeurons; i++)
        {
            neurons[i] = new Neuron(numInputsPerNeuron);
        }
    }
}
