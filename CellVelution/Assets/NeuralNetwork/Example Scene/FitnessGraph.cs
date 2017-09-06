using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FitnessGraph : MonoBehaviour {

    public GameObject graphPrefab;

    GameObject graphObj;

    WMG_Axis_Graph graph;

    WMG_Series bestFitnessSeries;
    WMG_Series averageFitnessSeries;

    int currentGeneration;

    // Use this for initialization
    void Start () {

        graphObj = Instantiate(graphPrefab, transform);
        

        graph = graphObj.GetComponent<WMG_Axis_Graph>();

        bestFitnessSeries = graph.addSeries();
        averageFitnessSeries = graph.addSeries();

        graph.yAxis.AxisTitleString = "Fitness";
        graph.xAxis.AxisTitleString = "Generations";

        graph.yAxis.MaxAutoGrow = true;
        graph.yAxis.MaxAutoShrink = true;
        graph.xAxis.MaxAutoGrow = true;
        graph.xAxis.MaxAutoShrink = true;

        graph.xAxis.SetLabelsUsingMaxMin = true;
        graph.xAxis.LabelType = WMG_Axis.labelTypes.ticks;

        bestFitnessSeries.seriesName = "Best Fitness";
        averageFitnessSeries.seriesName = "Average Fitness";

        bestFitnessSeries.pointColor = Color.green;
        averageFitnessSeries.pointColor = Color.yellow;

        bestFitnessSeries.pointValues = new WMG_List<Vector2>() { Vector2.right };
        averageFitnessSeries.pointValues = new WMG_List<Vector2>() { Vector2.right };

		
	}
	
	// Update is called once per frame
	public void UpdateGraph (int generation, double bestFitness, double averageFitness) {

        if(generation == currentGeneration)
        {
            bestFitnessSeries.pointValues[generation] = new Vector2(generation, (float)bestFitness);
            averageFitnessSeries.pointValues[generation] = new Vector2(generation, (float)averageFitness);

            bestFitnessSeries.pointValuesChanged();
            averageFitnessSeries.pointValuesChanged();
        }
        else
        {
            bestFitnessSeries.pointValues.Add( new Vector2(generation,(float) bestFitness));
            averageFitnessSeries.pointValues.Add(new Vector2(generation,(float) averageFitness));

            currentGeneration = generation;

            bestFitnessSeries.pointValuesCountChanged();
            averageFitnessSeries.pointValuesCountChanged();
        }

        
        //bestFitnessSeries.RealTimeUpdate();
        //averageFitnessSeries.RealTimeUpdate();
        //graph.Refresh();
	}
}
