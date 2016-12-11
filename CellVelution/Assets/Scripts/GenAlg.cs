using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GenAlg
{
    public List<Genome> population;

    private int popSize;
    private int chromoLength;
    private double totalFitness = 0;

    private double bestFitness, averageFitness = 0;
    private double worstFitness = 9999999999999999;

    private int fittestGenome = 0;

    public static double mutationRate = .2;
    public static double crossOverRate = .7;

    public int generation;

    public GenAlg(int _popSize, double mutRat, double crossRate, int numWeights)
    {
        mutationRate = mutRat;
        crossOverRate = crossRate;
        popSize = _popSize;
        chromoLength = numWeights;

        population = new List<Genome>();

        for (int i = 0; i < popSize; i++)
        {
            population.Add(new Genome(new double[chromoLength]));

            for (int j = 0; j < chromoLength; j++)
            {
                population[i].weights[j] = Random.Range(-1f, 1f);


            }
        }
    }
    public static void Crossover(double[] mum, double[] dad, ref double[] baby1, ref double[] baby2)
    {
        if (Random.Range(0f, 1f) > crossOverRate || mum == dad)
        {
            baby1 = mum;
            baby2 = dad;

            return;
        }

        int cp = Random.Range(0, mum.Length - 1);
        //print("crossover at " + cp);
        //Create the offspring

        for (int i = 0; i < cp; i++)
        {
            baby1[i] = mum[i];
            baby2[i] = dad[i];
        }
        for (int i = cp; i < mum.Length; i++)
        {
            baby1[i] = dad[i];
            baby2[i] = mum[i];
        }

        return;
    }

    public static void Mutate(ref double[] chromo)
    {
        for (int i = 0; i < chromo.Length; i++)
        {
            float randFloat = Random.Range(0f, 1f);
            if (randFloat < mutationRate)
            {
                randFloat = Random.Range(-.1f, .1f);
                //print("mutation");
                chromo[i] += randFloat;

                if (chromo[i] > 1)
                    chromo[i] = 1;
                else if (chromo[i] < 0)
                    chromo[i] = 0;
            }
        }
    }

    private Genome GetChromoRoulette()
    {
        double slice = Random.Range(0f, 1f) * totalFitness;

        Genome chosen = new Genome();

        if (totalFitness <= 0)
        {
            int index = Random.Range(0, population.Count);

            chosen = new Genome(new double[chromoLength]);

            for (int j = 0; j < chromoLength; j++)
            {
                chosen.weights[j] = Random.Range(-1f, 1f);


            }

            System.Console.Write("No winners");
            return chosen;
            
        }

        

        double fitnessSoFar = 0;

        for (int i = 0; i < popSize; i++)
        {
            fitnessSoFar += population[i].fitness;

            if (fitnessSoFar >= slice)
            {
                chosen = population[i];
                break;
            }
        }

        return chosen;
    }

    private void CalculateBestWorstAvTot()
    {
        totalFitness = 0;

        double highestSoFar = 0;
        double lowestSoFar = 99999999999;

        for (int i = 0; i < popSize; i++)
        {
            //updates fittest if necessary
            if (population[i].fitness > highestSoFar)
            {
                highestSoFar = population[i].fitness;

                fittestGenome = i;

                bestFitness = highestSoFar;
            }

            //update Worst
            if (population[i].fitness < lowestSoFar)
            {
                lowestSoFar = population[i].fitness;

                worstFitness = lowestSoFar;
            }

            totalFitness += population[i].fitness;
        }

        averageFitness = totalFitness / popSize;

    }

    private void Reset()
    {
        totalFitness = 0;
        bestFitness = 0;
        worstFitness = 9999999999;
        averageFitness = 0;
    }

    

    public List<Genome> Epoch(List<Genome> oldPop)
    {

        population = oldPop;

        
        CalculateBestWorstAvTot();

        //population.Sort();

        List<Genome> newPop = new List<Genome>();

        while (newPop.Count < popSize)
        {
            Genome mum = GetChromoRoulette();
            Genome dad = GetChromoRoulette();

            double[] baby1 = new double[chromoLength];
            double[] baby2 = new double[chromoLength];

            Crossover(mum.weights, dad.weights, ref baby1, ref baby2);

            Mutate(ref baby1);
            Mutate(ref baby2);

            newPop.Add(new Genome(baby1));
            newPop.Add(new Genome(baby2));

        }
        Reset();
        population = newPop;
        generation++;
        return population;
    }

}