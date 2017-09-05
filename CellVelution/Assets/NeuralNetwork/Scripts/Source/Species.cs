using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NeuralNetwork
{
    public class Species
    {

        /// <summary>
        /// List of the populations genomes/neural weights
        /// </summary>
        public List<Genome> population = new List<Genome>();

        public int popSize;

        private double totalFitness = 0;

        public double oldBestFitness = 0;

        public double bestFitness, averageFitness = 0;
        public double worstFitness = 9999999999999999;

        public int fittestGenome = 0;

        /// <summary>
        /// The generation count
        /// </summary>
        public int generation = 1;

        public int generationStagnation = 0;
        public int generationStagnationLimit = 15;

        public Color color;

        public Species()
        {
            generation = 1;
            oldBestFitness = 0;
            color = Random.ColorHSV(.5f,1,.5f,1);
        }

        public Species(int _generation, double _oldBestFitness, Color _color)
        {
            generation = _generation;
            oldBestFitness = _oldBestFitness;
            color = _color;
        }

        public void CalculateBestWorstAvTot()
        {
            totalFitness = 0;

            double highestSoFar = 0;
            double lowestSoFar = 99999999999;

            for (int i = 0; i < population.Count; i++)
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

            averageFitness = totalFitness / population.Count;

            if (bestFitness > oldBestFitness)
            {
                oldBestFitness = bestFitness;
                generationStagnation = 0;
            }

        }

        /// <summary>
        /// Returns a random genome weighted by fitness.
        /// </summary>
        /// <returns></returns>
        public Genome GetChromoRoulette()
        {
            double slice = Random.value * totalFitness;

            Genome chosen = null;

            if (totalFitness <= 0)
            {
                throw new System.Exception("Total fitness less than 0.");
            }

            double fitnessSoFar = 0;

            for (int i = 0; i < population.Count; i++)
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

    }
}


