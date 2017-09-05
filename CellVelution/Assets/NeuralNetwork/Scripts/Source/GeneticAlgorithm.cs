/// <copyright file="GeneticAlgorithm.cs">Copyright (c) 2016 All Rights Reserved</copyright>
/// <author>Chinedu Ozodi</author>
/// <date>12/12/2016</date>

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace NeuralNetwork
{
    /// <summary>
    /// The genetic algorythm that mutates the weights in the neural network
    /// </summary>
    public class GeneticAlgorithm
    {
        /// <summary>
        /// List of the populations genomes/neural weights
        /// </summary>
        public List<Genome> population;

        private int popSize;
        private int chromoLength;
        private double totalFitness = 0;

        public double bestFitness, averageFitness = 0;
        public double worstFitness = 9999999999999999;

        private int fittestGenome = 0;

        double compatibilityThreshold = 3;
        

        /// <summary>
        /// The generation count
        /// </summary>
        public int generation;

        int numInputs;
        int numOutputs;

        /// <summary>
        /// Creates the gene alg class with desired population size, mutation rate, crossover rate, and number of weight
        /// </summary>
        /// <param name="_popSize">Size of population to test</param>
        /// <param name="mutRat"> mutation rate</param>
        /// <param name="crossRate">crossover rate</param>
        /// <param name="numWeights">number of weights in gene, gotten from the GetNumWeights function in the NeuralNet class</param>
        public GeneticAlgorithm(int _popSize, int _numInputs, int _numOutputs)
        {
            popSize = _popSize;
            numInputs = _numInputs;
            numOutputs = _numOutputs;
            population = new List<Genome>();

            for (int i = 0; i < popSize; i++)
            {
                population.Add(new Genome(numInputs,numOutputs));
            }
        }

        private Genome GetChromoRoulette()
        {
            double slice = Random.Range(0f, 1f) * totalFitness;

            Genome chosen = new Genome(numInputs,numOutputs);

            if (totalFitness <= 0)
            {
                int index = Random.Range(0, population.Count);

                chosen = new Genome(numInputs,numOutputs);

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

        public void CalculateBestWorstAvTot()
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


        /// <summary>
        /// Creates a new generation population with the old generation population, running crossovers and mutations randomly. Make sure each genome has a 0 to positive fitness score.
        /// </summary>
        /// <param name="oldPop"></param>
        /// <returns>new population genome</returns>
        public List<Genome> Epoch(List<Genome> oldPop)
        {

            population = oldPop;


            CalculateBestWorstAvTot();

            //sorts population by fitness level
            //population.Sort();

            List<Genome> newPop = new List<Genome>();

            while (newPop.Count < popSize)
            {
                Genome mum = GetChromoRoulette();
                Genome dad = GetChromoRoulette();

                Genome childA = Genome.Mate(mum, dad);
                Genome childB = Genome.Mate(mum, dad);

                newPop.Add(childA);
                newPop.Add(childB);

            }
            Reset();
            population = newPop;
            generation++;
            return population;
        }

    }
}
