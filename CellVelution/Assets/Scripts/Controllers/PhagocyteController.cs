using UnityEngine;
using System.Collections;
using CodeControl;

public class PhagocyteController : Controller<CellModel> {

    //model variables
    public float food;
    public float foodToGrow;
    public float foodToDie = .1f;

    public float duplicationAngle;

    public Color color;

    //Gameobject Variables

    internal float forceToAdd = 100;
    internal float consumptionValue = .01f;
    internal float foodMultiplier = 10;

    Rigidbody2D rgb2D;

    SpriteRenderer rend;

    protected override void OnInitialize()
    {
        //Setup of variables
        food = model.food;
        foodToGrow = model.foodToGrow;
        foodToDie = model.foodToDie;

        duplicationAngle = model.duplicationAngle;

        color = model.color;

        //Setup of GameObject

        rgb2D = GetComponent<Rigidbody2D>();
        rend = GetComponent<SpriteRenderer>();

        //Update GameObject
        transform.name = model.name + " " + model.generation.ToString();
        transform.localScale = Vector2.one * Mathf.Pow(food, .2f);
        rend.color = color;
    }

    // Update is called once per frame
    void Update()
    {

        //Update food
        food += Time.deltaTime * (-consumptionValue);
        if (food < 0)
            food = 0;

        //Update Model
        model.food = food;
        model.foodToGrow = foodToGrow;
        model.foodToDie = foodToDie;
        model.duplicationAngle = duplicationAngle;

        model.color = color;

        //Update GameObject

        transform.localScale = Vector2.one * Mathf.Pow(food, .5f) * 2;
        rend.color = color;

        //If Splitting or Dying
        if (food > foodToGrow)
        {
            Split();
        }
        else if (food < foodToDie)
        {
            model.Delete();
        }

    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        if(coll.gameObject.tag == "food")
        {
            FoodController collController = coll.gameObject.GetComponent<FoodController>();
            food += collController.GetFood() * foodMultiplier ;

            collController.DestroyModel();

        }
    }
    /// <summary>
    /// Splits the cell
    /// </summary>
    private void Split()
    {
        //Update Self
        food *= .5f;

        Polar2 forcePolar = new Polar2(forceToAdd, (duplicationAngle + transform.eulerAngles.z) * Mathf.Deg2Rad);
        rgb2D.AddForce(forcePolar.cartesian);

        //Create split model
        CellModel splitModel = new CellModel();
        splitModel.generation = model.generation + 1;

        splitModel.food = food;
        splitModel.foodToGrow = foodToGrow;
        splitModel.foodToDie = foodToDie;

        splitModel.duplicationAngle = duplicationAngle;

        splitModel.color = color;

        //Update Split Controller
        float evolveNum = UnityEngine.Random.Range(0f, 1f);
        if (evolveNum < model.mutationChance)
        {
            int randomNum = UnityEngine.Random.Range(0, 2);
            model.color = new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), .8f);

            if (randomNum == 0)
            {
                var splitController = Controller.Instantiate<PhagocyteController>("phagocyte", splitModel);

                splitController.transform.position = transform.position;
                splitController.transform.eulerAngles = new Vector3(0, 0, duplicationAngle + transform.eulerAngles.z);

                splitController.GetComponent<Rigidbody2D>().AddForce(-forcePolar.cartesian);
            }
            else
            {
                var splitController = Controller.Instantiate<PhotocyteController>("photocyte", splitModel);

                splitController.transform.position = transform.position;
                splitController.transform.eulerAngles = new Vector3(0, 0, duplicationAngle + transform.eulerAngles.z);

                splitController.GetComponent<Rigidbody2D>().AddForce(-forcePolar.cartesian);
            }
        }
        else
        {
            var splitController = Controller.Instantiate<PhagocyteController>("phagocyte", splitModel);

            splitController.transform.position = transform.position;
            splitController.transform.eulerAngles = new Vector3(0, 0, duplicationAngle + transform.eulerAngles.z);

            splitController.GetComponent<Rigidbody2D>().AddForce(-forcePolar.cartesian);
        }

    }
}
