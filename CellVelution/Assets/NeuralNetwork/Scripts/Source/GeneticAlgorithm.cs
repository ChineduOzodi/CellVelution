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
        public List<Species> species;

        private int popSize;
        private int chromoLength;
        private double totalBestFitness = 0;

        public double bestFitness, averageFitness = 0;
        public double worstFitness = 9999999999999999;

        private int fittestSpecies = 0;

        double compatibilityThreshold = 3;
        

        /// <summary>
        /// The generation count
        /// </summary>
        public int generation = 1;

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
            species = new List<Species>();

            species.Add(new Species());
            species[0].population = new List<Genome>();
            species[0].population.Add(new Genome(numInputs, numOutputs));

            for (int i = 1; i < popSize; i++)
            {
                var genome = new Genome(numInputs, numOutputs);

                bool fitFound = false;

                for (int b = 0; b < species.Count; b++) //TODO: makes speciation front heavy, shold fix to goto the one with greatest compatibility
                {
                    if (Genome.CompatibilityDistance(species[i].population[0],genome) < compatibilityThreshold)
                    {
                        species[i].population.Add(genome);
                        fitFound = true;
                        break;
                    }
                }

                if (!fitFound)
                {
                    species.Add(new Species());
                    species[i].population = new List<Genome>();
                    species[i].population.Add(genome);
                }
                
            }
        }

        

        public void CalculateBestWorstAvTot()
        {
            totalBestFitness = 0;

            double highestSoFar = 0;
            double lowestSoFar = 99999999999;

            for (int i = 0; i < species.Count; i++)
            {
                species[i].CalculateBestWorstAvTot();

                //updates fittest if necessary
                if (species[i].bestFitness > highestSoFar)
                {
                    highestSoFar = species[i].bestFitness;

                    fittestSpecies = i;

                    bestFitness = highestSoFar;
                }

                //update Worst
                if (species[i].bestFitness < lowestSoFar)
                {
                    lowestSoFar = species[i].bestFitness;

                    worstFitness = lowestSoFar;
                }

                totalBestFitness += species[i].bestFitness;
            }

            averageFitness = totalBestFitness / species.Count;

        }

        private void Reset()
        {
            totalBestFitness = 0;
            bestFitness = 0;
            worstFitness = 9999999999;
            averageFitness = 0;
        }


        /// <summary>
        /// Creates a new generation population with the old generation population, running crossovers and mutations randomly. Make sure each genome has a 0 to positive fitness score.
        /// </summary>
        /// <param name="oldPop"></param>
        /// <returns>new population genome</returns>
        public void Epoch()
        {
            CalculateBestWorstAvTot();
            var oldPop = new List<Species>(species);

            //sorts population by fitness level
            //population.Sort();

            species = new List<Species>();

            int currentPopSize = 0;

            foreach (Species sp in oldPop)
            {
                if (sp.generationStagnation < sp.generationStagnationLimit)
                {
                    sp.generationStagnation++;

                    species.Add(new Species(sp.generation + 1, sp.oldBestFitness, sp.color));
                    species[species.Count - 1].generationStagnation = sp.generationStagnation;
                    species[species.Count - 1].population.Add(sp.population[sp.fittestGenome]);

                    species[species.Count - 1].popSize = Mathf.RoundToInt((float)(totalBestFitness / sp.bestFitness));

                    currentPopSize++;

                    while (species[species.Count - 1].population.Count < species[species.Count - 1].popSize)
                    {
                        Genome mum = Genome.DeepCopy(sp.GetChromoRoulette());
                        Genome dad = Genome.DeepCopy(sp.GetChromoRoulette());

                        Genome childA = Genome.Mate(mum, dad);

                        species[species.Count - 1].population.Add(childA);
                        currentPopSize++;

                    }

                }
            }
            
            while (currentPopSize  < popSize)
            {
                int spI = Random.Range(0, species.Count);

                Genome mum = Genome.DeepCopy(species[spI].GetChromoRoulette());
                Genome dad = Genome.DeepCopy(species[spI].GetChromoRoulette());

                Genome childA = Genome.Mate(mum, dad);

                species[spI].population.Add(childA);
                currentPopSize++;
            }
            Reset();
            generation++;
        }

    }
}
