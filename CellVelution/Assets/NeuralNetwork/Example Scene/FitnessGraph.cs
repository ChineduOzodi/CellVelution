using NeuralNetwork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FitnessGraph : MonoBehaviour {

    public GameObject graphPrefab;

    GameObject graphObj;

    WMG_Axis_Graph graph;

    WMG_Series bestFitnessSeries;
    WMG_Series averageFitnessSeries;

    WMG_List<Vector2> bestFitnessList = new WMG_List<Vector2>() { Vector2.zero};
    WMG_List<Vector2> averageFitnessList = new WMG_List<Vector2>() { Vector2.zero };

    List<WMG_Series> speciesSeries = new List<WMG_Series>();

    List<SeriesList<Vector2>> speciesPopulationList = new List<SeriesList<Vector2>>();

    int currentGeneration;

    public int generationLimit = 21;

    private GraphDisplay graphDisplay = GraphDisplay.Fitness;

    // Use this for initialization
    void Start () {

        graphObj = Instantiate(graphPrefab, transform);
        
        graph = graphObj.GetComponent<WMG_Axis_Graph>();

        SetFitnessGraph();
		
	}
	
	// Update is called once per frame
	public void UpdateFitnessGraph(int generation, double bestFitness, double averageFitness) {

        if(generation == currentGeneration)
        {
            bestFitnessList[bestFitnessList.Count-1] = new Vector2(generation, (float)bestFitness);
            averageFitnessList[averageFitnessList.Count-1] = new Vector2(generation, (float)averageFitness);
        }
        else
        {
            bestFitnessList.Add(new Vector2(generation, (float)bestFitness));
            averageFitnessList.Add((new Vector2(generation, (float)averageFitness)));
            
            currentGeneration = generation;

        }

        if (bestFitnessList.Count > generationLimit)
        {
            bestFitnessList.RemoveAt(0);
            averageFitnessList.RemoveAt(0);
        }

        if (graphDisplay == GraphDisplay.Fitness)
        {
            bestFitnessSeries.pointValues = bestFitnessList;

            averageFitnessSeries.pointValues = averageFitnessList;

            bestFitnessSeries.pointValuesCountChanged();
            averageFitnessSeries.pointValuesCountChanged();
        }

        //bestFitnessSeries.RealTimeUpdate();
        //averageFitnessSeries.RealTimeUpdate();
        //graph.Refresh();
    }
    private void DeletAllSeries()
    {
        int seriesCount = graph.lineSeries.Count;
        for (int i =0; i < seriesCount; i++)
        {
            graph.deleteSeries();
        }
    }
    private void SetFitnessGraph()
    {
        DeletAllSeries();

        bestFitnessSeries = graph.addSeries();
        averageFitnessSeries = graph.addSeries();

        graph.yAxis.AxisTitleString = "Fitness";
        graph.xAxis.AxisTitleString = "Generations";

        graph.yAxis.MaxAutoGrow = true;
        graph.yAxis.MaxAutoShrink = true;
        graph.xAxis.MaxAutoGrow = true;
        graph.xAxis.MaxAutoShrink = true;
        graph.xAxis.MinAutoGrow = true;
        graph.xAxis.MinAutoShrink = true;

        graph.xAxis.SetLabelsUsingMaxMin = true;
        graph.xAxis.LabelType = WMG_Axis.labelTypes.ticks;

        bestFitnessSeries.seriesName = "Best Fitness";
        averageFitnessSeries.seriesName = "Average Fitness";

        bestFitnessSeries.pointColor = Color.green;
        averageFitnessSeries.pointColor = Color.yellow;

        graph.legend.hideLegend = false;

        bestFitnessSeries.pointValues = new WMG_List<Vector2>();
        averageFitnessSeries.pointValues = new WMG_List<Vector2>();
    }

    public void SetGraphToFitness()
    {
        SetFitnessGraph();
        graphDisplay = GraphDisplay.Fitness;

        bestFitnessSeries.pointValues = bestFitnessList;
        averageFitnessSeries.pointValues = averageFitnessList;

        bestFitnessSeries.pointValuesCountChanged();
        averageFitnessSeries.pointValuesCountChanged();
    }

    private void SetSpeciesPopulationGraph()
    {
        DeletAllSeries();

        graph.yAxis.AxisTitleString = "Population";
        graph.xAxis.AxisTitleString = "Generations";

        graph.yAxis.MaxAutoGrow = true;
        graph.yAxis.MaxAutoShrink = true;
        graph.xAxis.MaxAutoGrow = true;
        graph.xAxis.MaxAutoShrink = true;
        graph.xAxis.MinAutoGrow = true;
        graph.xAxis.MinAutoShrink = true;
        //graph.graphType = WMG_Axis_Graph.graphTypes.line_stacked;

        graph.xAxis.SetLabelsUsingMaxMin = true;
        graph.xAxis.LabelType = WMG_Axis.labelTypes.ticks;

        graph.legend.hideLegend = true;

        //var rect = graphObj.GetComponent<RectTransform>();
        //rect.anchorMax = new Vector2(.5f, 0);
        //rect.anchorMin = new Vector2(.5f, 0);
        //rect.pivot = new Vector2(.5f, 0);
    }

    public void UpdateSpeciesPopulationGraph(List<Species> species, int generation)
    {

        foreach (Species sp in species)
        {
            bool seriesFound = false;
            SeriesList<Vector2> seriesList = new SeriesList<Vector2>();
            foreach (SeriesList<Vector2> list in speciesPopulationList)
            {
                if (currentGeneration - list[0].x > generationLimit)
                {
                    list.RemoveAt(0);
                }

                if(list.Count == 0)
                {
                    speciesPopulationList.Remove(list);
                }
                else if (list.seriesName == sp.speciesName)
                {
                    seriesFound = true;
                    list.Add(new Vector2(generation, sp.population.Count));
                    seriesList = list;
                    break;
                }
            }
            if (!seriesFound)
            {
                speciesPopulationList.Add(new SeriesList<Vector2>());
                speciesPopulationList[speciesPopulationList.Count - 1].pointColor = sp.color;
                speciesPopulationList[speciesPopulationList.Count - 1].Add( new Vector2(generation, sp.population.Count));
                speciesPopulationList[speciesPopulationList.Count - 1].seriesName = sp.speciesName;
                seriesList = speciesPopulationList[speciesPopulationList.Count - 1];
            }

            if (graphDisplay == GraphDisplay.SpeciesPopulation)
            {
                foreach (WMG_Series series in speciesSeries)
                {
                    if (series.seriesName == sp.speciesName)
                    {
                        seriesFound = true;
                        series.pointValues = seriesList;
                        speciesSeries[speciesSeries.Count - 1].pointColor = sp.color;
                        series.pointValuesChanged();
                        break;
                    }
                }
                if (!seriesFound)
                {
                    speciesSeries.Add(graph.addSeries());
                    speciesSeries[speciesSeries.Count - 1].pointColor = sp.color;
                    //seriesList[seriesList.Count - 1].areaShadingType = WMG_Series.areaShadingTypes.Solid;
                    speciesSeries[speciesSeries.Count - 1].areaShadingColor = sp.color;
                    speciesSeries[speciesSeries.Count - 1].pointValues = seriesList;
                    speciesSeries[speciesSeries.Count - 1].seriesName = sp.speciesName;
                    speciesSeries[speciesSeries.Count - 1].pointValuesChanged();
                    
                }
            }
            
        }


        //bestFitnessSeries.RealTimeUpdate();
        //averageFitnessSeries.RealTimeUpdate();
        //graph.Refresh();
    }

    public void SetGraphToSpeciesPopulation()
    {
        SetSpeciesPopulationGraph();

        graphDisplay = GraphDisplay.SpeciesPopulation;

        speciesSeries = new List<WMG_Series>();

        foreach (SeriesList<Vector2> list in speciesPopulationList)
        {
            speciesSeries.Add(graph.addSeries());
            speciesSeries[speciesSeries.Count - 1].pointColor = list.pointColor;
            //seriesList[seriesList.Count - 1].areaShadingType = WMG_Series.areaShadingTypes.Solid;
            speciesSeries[speciesSeries.Count - 1].areaShadingColor = list.pointColor;
            speciesSeries[speciesSeries.Count - 1].pointValues = list;
            speciesSeries[speciesSeries.Count - 1].seriesName = list.seriesName;
            speciesSeries[speciesSeries.Count - 1].pointValuesChanged();
        }


    }

    private class SeriesList<Vector2> : WMG_List<Vector2>
    {
        public string seriesName;
        public Color pointColor;
    }
}

public enum GraphDisplay
{
    Fitness, SpeciesPopulation
}