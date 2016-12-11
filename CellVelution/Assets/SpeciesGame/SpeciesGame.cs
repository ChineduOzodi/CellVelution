using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using CodeControl;

public class SpeciesGame : MonoBehaviour {

    internal NeuralVisualizer nVisual;

    public Text infoText;
    public int minPop = 100;
    public int startingPop = 250;

    public float speedAdjust = 1000;
    public float rotationAdjust = 100;
    public float nutritionAdjustment = 100;

    internal int population;
    
    //Neural Stuff
    internal int numInputs = 9 * 4;
    internal int numOutputs = 3;
    public int numHiddenLayers = 1;
    public int numNodeHiddenLayers = 5;

    public Vector2 land = new Vector2(100, 100);

    internal int updateTime;
    GameObject parent;

    // Use this for initialization
    void Start () {
        nVisual = GetComponent<NeuralVisualizer>();
        population = startingPop;
        parent = new GameObject("Population");
        for (int i = 0; i < startingPop; i++)
        {

            CreatureModel model = new CreatureModel();

            model.numInputs = numInputs;
            model.numOutputs = numOutputs;
            model.numHiddenLayers = numHiddenLayers;
            model.numNodeHiddenLayers = numNodeHiddenLayers;

            model = Creature.CreateCreature(model);

            Creature creature = Controller.Instantiate<Creature>("creature",model,parent.transform);

            creature.transform.position = new Vector3(Random.Range(0, land.x), Random.Range(0, land.y));

        }

        Message.AddListener("Creature Death", OnDeath);
        Message.AddListener("Creature Birth", OnBirth);

    }


    // Update is called once per frame
    void Update() {

        if (Input.GetMouseButtonDown(0))
        {

            Vector3 rayPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.CircleCast(rayPos, .1f, Vector2.up, .1f);

            if (hit.collider != null)
            {
                Debug.Log(hit.transform.gameObject.name);
                nVisual.selectedCreature = hit.transform.GetComponent<Creature>();

            }
            else
            {
                nVisual.selectedCreature = null;
            }

        }

        if (Input.GetKeyDown(KeyCode.Comma)){
            Time.timeScale *= .5f;
        }
        if (Input.GetKeyDown(KeyCode.Period)){
            Time.timeScale *= 2;
        }

        infoText.text = "Population: " + population
            + "\nTime: " + Time.time.ToString("0")
            + "\nTime Scale: " + Time.timeScale;


        //Check pop
        if (population < minPop)
        {
            CreatureModel model = new CreatureModel();

            model.numInputs = numInputs;
            model.numOutputs = numOutputs;
            model.numHiddenLayers = numHiddenLayers;
            model.numNodeHiddenLayers = numNodeHiddenLayers;

            model = Creature.CreateCreature(model);

            Creature creature = Controller.Instantiate<Creature>("creature", model, parent.transform);

            creature.transform.position = new Vector3(Random.Range(0, land.x), Random.Range(0, land.y));

            population++;
        }
    }



    private void OnBirth()
    {
        population++;
    }

    private void OnDeath()
    {
        population--;
    }

    
}
