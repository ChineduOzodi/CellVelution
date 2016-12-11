using UnityEngine;
using System.Collections;
using CodeControl;
using System;

public class FoodController : Controller<FoodModel> {

    public FoodModel Model;

    bool destroy = false;
    protected override void OnInitialize()
    {
        //Setup Object
        Model = model;
        transform.name = "food";
        transform.localScale = Vector2.one * Mathf.Pow(model.food, .2f);
        GetComponent<SpriteRenderer>().color = model.color;

    }

    void Update()
    {

        //Update food
        model.food += Time.deltaTime * (-model.growthRate);
        if (model.food < 0)
        {
            model.food = 0;

            destroy = true;
        }

        //Update Object
        transform.localScale = Vector2.one * Mathf.Pow(model.food, .2f);

        if (destroy)
            model.Delete();
    }
    public float GetFood()
    {
        return model.food;
    }

    internal void DestroyModel()
    {
        destroy = true;
    }
}
