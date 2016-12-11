using UnityEngine;
using System.Collections;

public struct Neuron
{
    public int numInputs;

    public double[] weights;

    public Neuron(int _numInputs)
    {
        numInputs = _numInputs;

        weights = new double[numInputs + 1];

        for (int i = 0; i < numInputs + 1; i++)
        {
            weights[i] = Random.Range(-1.0f, 1.0f);
        }
    }
}
