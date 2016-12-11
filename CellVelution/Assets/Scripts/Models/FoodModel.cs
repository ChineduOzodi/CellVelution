using UnityEngine;
using System.Collections;
using CodeControl;
public class FoodModel : Model {

    public Vector3 position;
    public FoodType type;
    public float food = .1f;
    public float growthRate;
    public Color color = new Color(0, .5f, .5f, .8f);
}
