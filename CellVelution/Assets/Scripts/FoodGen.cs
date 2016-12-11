using UnityEngine;
using System.Collections;
using CodeControl;

public class FoodGen : MonoBehaviour {

    //Corners of Map and other Variables
    public Transform bottomLeft;
    public Transform topRight;

    internal GameObject foodEmpty;

    internal float time = 0;

    public int foodAmount = 500;
    public float foodSize = 1f;
    public float foodFreq = .1f;
    public int foodFreqAmount = 1;

    internal ModelRefs<FoodModel> foodRefs = new ModelRefs<FoodModel>();

	// Use this for initialization
	void Start () {

        foodEmpty = new GameObject("FoodEmpty");

        //Initial Food Creation
        for (int i = 0; i < foodAmount; i++)
        {
            CreateFood();
        }
        
	
	}
    /// <summary>
    /// Creates the Food Particle in a random range between the two determined empties
    /// </summary>
    private void CreateFood()
    {
        FoodModel model = new FoodModel();
        model.food = foodSize;
        model.type =(FoodType) Random.Range(0, 2);
        model.color = new Color(0, 1 - (int)model.type, (int)model.type);

        Vector2 spawnPosition = new Vector2(UnityEngine.Random.Range(bottomLeft.position.x, topRight.position.x), UnityEngine.Random.Range(bottomLeft.position.y, topRight.position.y));
        var modCont = Controller.Instantiate<FoodController>("food", model, foodEmpty.transform);

        modCont.transform.position = spawnPosition;

        foodRefs.Add(model);

    }

    // Update is called once per frame
    void Update () {

        if (Time.time > time)
        {
            time = Time.time + foodFreq; //Update Food in Frequency

            if (foodRefs.Count < foodAmount)
            {
                for (int i = 0; i < foodFreqAmount; i++)
                {
                    CreateFood();
                }

                //if (foodRefs.Count < foodAmount * .5)
                //{
                //    for (int i = 0; i < Mathf.Pow(foodRefs.Count + 2, spawnAmountModifier) * .5; i++)
                //    {
                //        CreateFood();
                //    }
                //}
                //else
                //{
                //    for (int i = 0; i < Mathf.Pow(foodAmount - foodRefs.Count, spawnAmountModifier) * .5; i++)
                //    {
                //        CreateFood();
                //    }
                //}

            }
        }
        
	
	}
}
