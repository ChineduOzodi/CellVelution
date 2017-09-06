using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NeuralNetwork;

public class SpeciesGraph : MonoBehaviour {

    public GameObject graphPrefab;

    GameObject graphObj;

    WMG_Axis_Graph graph;

    List<WMG_Series> seriesList = new List<WMG_Series>();

    int currentGeneration;

    // Use this for initialization
    void Awake () {

        graphObj = Instantiate(graphPrefab, transform);
        

        graph = graphObj.GetComponent<WMG_Axis_Graph>();

        graph.yAxis.AxisTitleString = "Population";
        graph.xAxis.AxisTitleString = "Generations";

        graph.yAxis.MaxAutoGrow = true;
        graph.yAxis.MaxAutoShrink = true;
        graph.xAxis.MaxAutoGrow = true;
        graph.xAxis.MaxAutoShrink = true;
        graph.graphType = WMG_Axis_Graph.graphTypes.line_stacked;

        graph.xAxis.SetLabelsUsingMaxMin = true;
        graph.xAxis.LabelType = WMG_Axis.labelTypes.ticks;

        var rect = graphObj.GetComponent<RectTransform>();
        rect.anchorMax = new Vector2(.5f, 0);
        rect.anchorMin = new Vector2(.5f, 0);
        rect.pivot = new Vector2(.5f, 0);

		
	}
	
	// Update is called once per frame
	public void UpdateGraph (List<Species> species, int generation) {

        foreach(Species sp in species)
        {
            bool seriesFound = false;
            foreach(WMG_Series series in seriesList)
            {
                if (series.seriesName == sp.speciesName)
                {
                    seriesFound = true;
                    series.pointValues.Add(new Vector2(generation, sp.population.Count));
                    seriesList[seriesList.Count - 1].pointColor = sp.color;
                    series.pointValuesChanged();
                    break;
                } 
            }
            if (!seriesFound)
            {
                seriesList.Add(graph.addSeries());
                seriesList[seriesList.Count - 1].pointColor = sp.color;
                //seriesList[seriesList.Count - 1].areaShadingType = WMG_Series.areaShadingTypes.Solid;
                seriesList[seriesList.Count - 1].areaShadingColor = sp.color;
                seriesList[seriesList.Count - 1].pointValues = new WMG_List<Vector2>() { new Vector2(generation, sp.population.Count) };
                seriesList[seriesList.Count - 1].pointValuesChanged();
                seriesList[seriesList.Count - 1].seriesName = sp.speciesName;
            }
        }

        
        //bestFitnessSeries.RealTimeUpdate();
        //averageFitnessSeries.RealTimeUpdate();
        graph.Refresh();
	}
}
