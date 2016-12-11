using UnityEngine;
using System.Collections;

public struct Genome
{
    internal double[] weights;

    public double fitness;

    public Genome(double[] _weights, double _fitness = 0)
    {
        fitness = _fitness;
        weights = _weights;
    }

    public static bool operator <(Genome g1, Genome g2)
    {
        return (g1.fitness < g2.fitness);
    }
    public static bool operator >(Genome g1, Genome g2)
    {
        return (g1.fitness > g2.fitness);
    }

}
