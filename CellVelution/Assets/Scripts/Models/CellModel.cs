using UnityEngine;
using System.Collections;
using CodeControl;

public class CellModel : Model
{
    public string name = "Cell";
    public int generation = 1;

    //Mutation
    public float mutationChance = .01f;

    public float food;
    public float foodToGrow;
    public float foodToDie = .1f;

    public float duplicationAngle;

    public Color color;
}
