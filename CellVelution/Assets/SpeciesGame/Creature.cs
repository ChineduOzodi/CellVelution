using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text.RegularExpressions;
using CodeControl;
using UnityEngine.UI;
using NeuralNetwork;


public class Creature : Controller<CreatureModel> {

    public PolygonCollider2D visualTriangle;
    public CircleCollider2D visualCircle;

    public float nutrientMateLevel = 50;

    Rigidbody2D rgb;
    SpeciesGame gameController;
    public CreatureModel Model;
  

    Text text;

    // Use this for initialization
    protected override void OnInitialize()
    {
        rgb = transform.GetComponent<Rigidbody2D>();
        
        gameController = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<SpeciesGame>();

        Model = model;

        model.name = model.sex + " " + model.foodPref + " - " + model.generation;

        text =  GetComponentInChildren< Text >();

        text.text = model.name;

        transform.eulerAngles = new Vector3(0, 0, UnityEngine.Random.Range(0f, 360f));
        //Apply Creature effects

        if (model.foodPref ==  FoodType.Omni)
        {
            GetComponent<SpriteRenderer>().color = new Color(0, 0, model.omni / 10 * .5f + .5f);
        }
        else if (model.foodPref == FoodType.Carni)
        {
            GetComponent<SpriteRenderer>().color = new Color(model.carni / 10 * .5f + .5f, 0, 0);
        }
        if (model.foodPref == FoodType.Herb)
        {
            GetComponent<SpriteRenderer>().color = new Color(0, model.herb / 10 * .5f + .5f, 0);
        }
        transform.localScale = Vector3.one * model.size * .1f;
        rgb.mass = model.mass;
        visualCircle.transform.localScale = Vector3.one * model.sightDistance * .9f;
        visualTriangle.transform.localScale = Vector3.one * model.sightDistance * .9f;
        //Vector3.one * Mathf.Sqrt(model.mass/Mathf.PI)
    }

    // Update is called once per frame
    void Update () {
        text.transform.eulerAngles = new Vector3(0, 0, 0);
        SetInputs();
        model.ouputs = model.nNetwork.Update(model.inputs);
        ApplyOutputs();

        //Apply effects
        transform.localScale = Vector3.one * Mathf.Sqrt(model.mass / Mathf.PI);
        model.nutrition -= model.metabolism * Time.deltaTime * .001f;
        model.age += Time.deltaTime;

        if (transform.position.y < 0)
            transform.position = new Vector3(transform.position.x, gameController.land.y);
        else if (transform.position.y > gameController.land.y)
            transform.position = new Vector3(transform.position.x, 0);

        if (transform.position.x < 0)
            transform.position = new Vector3(gameController.land.x, transform.position.y);
        else if (transform.position.x > gameController.land.x)
            transform.position = new Vector3(0, transform.position.y);

        if (model.age > 100)
        {
            bool[] babyGenes = model.genes.Duplicate();

            CreatureModel babyModel = new CreatureModel();

            babyModel.numInputs = model.numInputs;
            babyModel.numOutputs = model.numOutputs;
            babyModel.numHiddenLayers = model.numHiddenLayers;
            babyModel.numNodeHiddenLayers = model.numNodeHiddenLayers;
            babyModel.generation = model.generation + 1;
            babyModel = CreateCreature(babyModel, babyGenes);

            Creature creature = Controller.Instantiate<Creature>("creature", babyModel, transform.parent);

            babyModel.nNetwork.PutWeights(ShareKnowledge(this));

            Vector3 location = transform.position;

            creature.transform.position = new Vector3(UnityEngine.Random.Range(location.x - 10, location.x + 10), UnityEngine.Random.Range(location.y - 10, location.y + 10));

            Message.Send("Creature Birth");

            babyModel = new CreatureModel();

            babyModel.numInputs = model.numInputs;
            babyModel.numOutputs = model.numOutputs;
            babyModel.numHiddenLayers = model.numHiddenLayers;
            babyModel.numNodeHiddenLayers = model.numNodeHiddenLayers;
            babyModel.generation = model.generation + 1;
            babyModel = CreateCreature(babyModel, babyGenes);
            babyModel.nNetwork.PutWeights(ShareKnowledge(this));

            creature = Controller.Instantiate<Creature>("creature", babyModel, transform.parent);

            location = transform.position;

            creature.transform.position = new Vector3(UnityEngine.Random.Range(location.x - 10, location.x + 10), UnityEngine.Random.Range(location.y - 10, location.y + 10));

            Message.Send("Creature Birth");

            print("Duplication");

            Message.Send("Creature Death");

            model.Delete();

        }
        else if (model.nutrition < 0)
        {
            //FoodModel foodModel = new FoodModel();
            //foodModel.food = model.mass;
            //foodModel.type = FoodType.Carni;
            //foodModel.color = new Color(.7f, .1f, .1f);
            //foodModel.growthRate = -1;

            //Vector2 spawnPosition = transform.position;

            //var modCont = Controller.Instantiate<FoodController>("food", foodModel, transform.parent);

            //modCont.transform.position = spawnPosition;

            Message.Send("Creature Death");
            model.Delete();

        }



    }

    private void SetInputs()
    {
        model.inputs = new List<double>();
        Collider2D col;
        col = visualTriangle;

        RaycastHit2D[] results = new RaycastHit2D[1];

        col.Cast(Vector2.up, results, 0);

        if (!results[0])
        {
            model.inputs.Add(0);
            model.inputs.Add(0);
            model.inputs.Add(0);
            model.inputs.Add(0);
            model.inputs.Add(0);
            model.inputs.Add(0);
            model.inputs.Add(0);
            Model.inputs.Add(0);
            Model.inputs.Add(0);

            col.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, .1f);
        }
        else
        {
            if (results[0].transform.GetComponent<Creature>())
            {
                Creature cre = results[0].transform.GetComponent<Creature>();

                Vector2 relPos = cre.transform.position - transform.position;

                relPos.Normalize();

                model.inputs.Add(cre.model.herb/10);
                model.inputs.Add(cre.model.omni / 10);
                model.inputs.Add(cre.model.carni / 10);
                model.inputs.Add(0);
                model.inputs.Add(0);
                model.inputs.Add(0);
                model.inputs.Add(1);
                Model.inputs.Add(relPos.x);
                Model.inputs.Add(relPos.y);

                if (Vector3.Distance(transform.position, cre.transform.position) < 5)
                    OnContact(cre);

                col.GetComponent<SpriteRenderer>().color = new Color(.5f, 1, 1, .1f);

            }
            else if (results[0].transform.GetComponent<FoodController>())
            {
                FoodController food = results[0].transform.GetComponent<FoodController>();

                Vector2 relPos = food.transform.position - transform.position;

                relPos.Normalize();

                model.inputs.Add(0);
                model.inputs.Add(0);
                model.inputs.Add(0);
                model.inputs.Add((food.Model.type == 0) ? 1 : 0);
                model.inputs.Add(((int)food.Model.type == 1) ? 1 : 0);
                model.inputs.Add(((int)food.Model.type == 2) ? 1 : 0);
                model.inputs.Add(-1);
                Model.inputs.Add(relPos.x);
                Model.inputs.Add(relPos.y);

                if (Vector3.Distance(transform.position, food.transform.position) < 5)
                    OnContact(food);

                col.GetComponent<SpriteRenderer>().color = new Color(1f, .5f, 1, .1f);

                //print(food.Model.type.ToString() + food.Model.type);
            }
            else
            {
                model.inputs.Add(0);
                model.inputs.Add(0);
                model.inputs.Add(0);
                model.inputs.Add(0);
                model.inputs.Add(0);
                model.inputs.Add(0);
                model.inputs.Add(0);
                Model.inputs.Add(0);
                Model.inputs.Add(0);

                col.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, .1f);
            }
        }

        
        

        //Visual Circle
        col = visualCircle;
        results = new RaycastHit2D[3];

        col.Cast(Vector2.up, results, 0);


        foreach ( RaycastHit2D result in results)
        {
            if (!result)
            {
                model.inputs.Add(0);
                model.inputs.Add(0);
                model.inputs.Add(0);
                model.inputs.Add(0);
                model.inputs.Add(0);
                model.inputs.Add(0);
                model.inputs.Add(0);
                Model.inputs.Add(0);
                Model.inputs.Add(0);

                col.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, .1f);

                continue;

                
            }

            if (result.transform.GetComponent<Creature>())
            {
                Creature cre = result.transform.GetComponent<Creature>();

                Vector2 relPos = cre.transform.position - transform.position;

                relPos.Normalize();

                model.inputs.Add(cre.model.herb / 10);
                model.inputs.Add(cre.model.omni / 10);
                model.inputs.Add(cre.model.carni / 10);
                model.inputs.Add(0);
                model.inputs.Add(0);
                model.inputs.Add(0);
                model.inputs.Add(1);
                Model.inputs.Add(relPos.x);
                Model.inputs.Add(relPos.y);

                if (Vector3.Distance(transform.position,cre.transform.position) < 5)
                    OnContact(cre);

                col.GetComponent<SpriteRenderer>().color = new Color(.5f, 1, 1, .1f);

            }
            else if (results[0].transform.GetComponent<FoodController>())
            {
                FoodController food = result.transform.GetComponent<FoodController>();
                Vector2 relPos = food.transform.position - transform.position;

                relPos.Normalize();

                model.inputs.Add(0);
                model.inputs.Add(0);
                model.inputs.Add(0);
                model.inputs.Add((food.Model.type == 0) ? 1 : 0);
                model.inputs.Add(((int)food.Model.type == 1) ? 1 : 0);
                model.inputs.Add(((int)food.Model.type == 2) ? 1 : 0);
                model.inputs.Add(-1);
                Model.inputs.Add(relPos.x);
                Model.inputs.Add(relPos.y);

                if (Vector3.Distance(transform.position, food.transform.position) < 5)
                    OnContact(food);

                col.GetComponent<SpriteRenderer>().color = new Color(1f, .5f, 1, .1f);

                //print(food.Model.type.ToString() + food.Model.type);
            }
            else
            {
                model.inputs.Add(0);
                model.inputs.Add(0);
                model.inputs.Add(0);
                model.inputs.Add(0);
                model.inputs.Add(0);
                model.inputs.Add(0);
                model.inputs.Add(0);
                Model.inputs.Add(0);
                Model.inputs.Add(0);

                col.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, .1f);
            }

        }
    }

    private void OnContact(FoodController food)
    {
        try
        {
            model.nutrition -= (-model.herb / 10 * .5f + .5f) * Time.deltaTime * gameController.nutritionAdjustment + (float)model.ouputs[2] * .5f;
            model.nutrition += (-model.herb / 10 * .5f + .5f) * Time.deltaTime * gameController.nutritionAdjustment + (float)model.ouputs[2] * .5f;
        }
        catch (ArgumentOutOfRangeException)
        {
            Vector3 location = transform.position;
            transform.position = new Vector3(UnityEngine.Random.Range(location.x - 5, location.x + 5), UnityEngine.Random.Range(location.y - 5, location.y + 5));
            return;
        }
        if (model.foodPref == FoodType.Herb)
        {
            if (food.Model.type == FoodType.Herb)
            {
                model.nutrition += (model.herb / 10 * .5f + .5f) * Time.deltaTime * 2 * gameController.nutritionAdjustment - (float) model.ouputs[2] * .5f;
            }
            else if (food.Model.type == FoodType.Omni)
            {
                model.nutrition += (model.omni / 10 * .5f + .5f) * Time.deltaTime * gameController.nutritionAdjustment - (float)model.ouputs[2] * .3f;
            }
            else
            {
                model.nutrition -= (-model.carni / 10 * .5f + .5f) * Time.deltaTime * 2 * gameController.nutritionAdjustment + (float)model.ouputs[2] * .5f;
            }
        }
        else if (model.foodPref == FoodType.Omni)
        {
            if (food.Model.type == FoodType.Herb)
            {
                model.nutrition -= (-model.herb / 10 * .5f + .5f) * Time.deltaTime * gameController.nutritionAdjustment + (float)model.ouputs[2] * .5f;
            }
            else if (food.Model.type == FoodType.Omni)
            {
                model.nutrition += (model.omni / 10 * .5f + .5f) * Time.deltaTime * 2 * gameController.nutritionAdjustment - (float)model.ouputs[2] * .5f;
            }
            else
            {
                model.nutrition += (model.carni / 10 * .5f + .5f) * Time.deltaTime * .5f * gameController.nutritionAdjustment - (float)model.ouputs[2] * .5f;
            }
        }

        else if (model.foodPref == FoodType.Carni)
        {
            if (food.Model.type == FoodType.Herb)
            {
                model.nutrition -= (-model.herb / 10 * .5f + .5f) * Time.deltaTime * 2 * gameController.nutritionAdjustment + (float)model.ouputs[2] * .5f;
            }
            else if (food.Model.type == FoodType.Omni)
            {
                model.nutrition -= (-model.omni / 10 * .5f + .5f) * Time.deltaTime * gameController.nutritionAdjustment - (float)model.ouputs[2] * .5f;
            }
            else
            {
                model.nutrition += (model.carni / 10 * .5f + .5f) * Time.deltaTime * gameController.nutritionAdjustment - (float)model.ouputs[2] * .5f;
            }
        }

        food.DestroyModel();
    }

    private void OnContact(Creature cre)
    {
        if (model.foodPref == FoodType.Herb)
        {
            if (cre.Model.foodPref == FoodType.Herb)
            {
                if (cre.model.nutrition > nutrientMateLevel && model.nutrition > nutrientMateLevel)
                {
                    if (cre.model.sex == "female" && model.sex == "male")
                    {
                        Mate(cre);
                        model.nutrition *= .75f;
                        cre.model.nutrition *= .75f;

                        model.nNetwork.PutWeights(ShareKnowledge(cre));
                    }
                    else
                    {
                        model.nNetwork.PutWeights(ShareKnowledge(cre));
                    }
                }
            }
        }
        else if (model.foodPref == FoodType.Omni)
        {
            if (cre.Model.foodPref == FoodType.Herb)
            {
                Kill(cre);
            }
            else if (cre.Model.foodPref == FoodType.Omni)
            {
                if (cre.model.nutrition > nutrientMateLevel && model.nutrition > nutrientMateLevel)
                {
                    if (cre.model.sex == "female" && model.sex == "male")
                    {
                        Mate(cre);
                        model.nutrition *= .75f;
                        cre.model.nutrition *= .75f;

                        model.nNetwork.PutWeights(ShareKnowledge(cre));
                    }
                    else
                    {
                        model.nNetwork.PutWeights(ShareKnowledge(cre));
                    }
                }
            }
        }

        else if (model.foodPref == FoodType.Carni)
        {
            if (cre.Model.foodPref == FoodType.Herb)
            {
                Kill(cre);
            }
            else if (cre.Model.foodPref == FoodType.Omni)
            {
                Kill(cre);
            }
            else
            {
                if (cre.model.nutrition > nutrientMateLevel && model.nutrition > nutrientMateLevel)
                {
                    if (cre.model.sex == "female" && model.sex == "male")
                    {
                        Mate(cre);
                        model.nutrition *= .75f;
                        cre.model.nutrition *= .75f;

                        model.nNetwork.PutWeights(ShareKnowledge(cre));
                    }
                    else
                    {
                        model.nNetwork.PutWeights(ShareKnowledge(cre));
                    }
                }
            }
        }
    }

    private void Kill(Creature cre)
    {
        FoodModel model = new FoodModel();
        model.food = cre.Model.mass;
        model.type = FoodType.Carni;
        model.color = new Color(.7f, .1f, .1f);
        model.growthRate = -1;
        Vector2 spawnPosition = cre.transform.position;

        var modCont = Controller.Instantiate<FoodController>("food", model, transform.parent);

        modCont.transform.position = spawnPosition;

        cre.Model.Delete();

        Message.Send("Creature Death");
    }

    private double[] ShareKnowledge(Creature cre)
    {
        double[] p1 = model.nNetwork.GetWeights();
        double[] p2 = cre.model.nNetwork.GetWeights();

        double[] baby1 = new double[p1.Length];
        double[] baby2 = new double[p1.Length];

        GeneticAlgorithm.Crossover(p1, p2, ref baby1,ref baby2);

        int selection = UnityEngine.Random.Range(0, 2);

        if (selection == 0)
        {
            GeneticAlgorithm.Mutate(ref baby1);

            return baby1;
        }
        else
        {
            GeneticAlgorithm.Mutate(ref baby2);

            return baby2;
        }

        print("Shared knowledge");
    }

    private void Mate(Creature cre)
    {
        bool[] babyGenes = cre.Model.genes.Mate(model.genes.genes);

        CreatureModel babyModel = new CreatureModel();

        babyModel.numInputs = model.numInputs;
        babyModel.numOutputs = model.numOutputs;
        babyModel.numHiddenLayers = model.numHiddenLayers;
        babyModel.numNodeHiddenLayers = model.numNodeHiddenLayers;
        babyModel.generation = model.generation + 1;

        babyModel = CreateCreature(babyModel, babyGenes);

        babyModel.nNetwork.PutWeights(ShareKnowledge(cre));

        Creature creature = Controller.Instantiate<Creature>("creature", babyModel, transform.parent);

        Vector3 location = transform.position;

        creature.transform.position = new Vector3(UnityEngine.Random.Range(location.x - 10, location.x + 10), UnityEngine.Random.Range(location.y - 10, location.y + 10));

        Message.Send("Creature Birth");

        print("Mated");

    }

    private void ApplyOutputs()
    {
        transform.Rotate(new Vector3(0,0,(float)( Time.deltaTime * gameController.rotationAdjust * model.reactionTime * (model.ouputs[1] - model.ouputs[0]))));
        transform.Translate(transform.up * model.moveSpeed * Time.deltaTime * gameController.speedAdjust * (float) model.ouputs[2]);
    }

    private static void ApplyGene(CreatureModel model, List<string> geneInterpret)
    {
        for (int i = 0; i < geneInterpret.Count; i++)
        {
            if (i == 0)
            {
                model.sex = geneInterpret[i];
            }
            else if (geneInterpret[i] == "leg")
            {
                string resultString = Regex.Match(geneInterpret[i + 2], @"-?\d+").Value;
                float amount = int.Parse(resultString);


                model.mass += model.mass * amount * .02f;
                model.numLegs++;
                model.metabolism += amount * .02f;
                model.moveSpeed += amount;
                model.moveSpeed *= .75f;

                i += 2; 
            }
            else if (geneInterpret[i] == "eyes")
            {
                string resultString = Regex.Match(geneInterpret[i + 2], @"-?\d+").Value;
                float amount = int.Parse(resultString);

                model.mass += model.mass * amount * .01f;
                model.numEyes++;
                model.metabolism += amount * .04f;
                model.sightDistance += amount;
                model.sightDistance *= .5f;

                i += 2;
            }
            else if (geneInterpret[i] == "size")
            {
                string resultString = Regex.Match(geneInterpret[i + 1] + geneInterpret[i + 2], @"-?\d+").Value;
                float amount = int.Parse(resultString);


                model.mass += model.mass * (amount * .1f);
                model.size += amount;

                i += 2;
            }
            else if (geneInterpret[i] == "herb")
            {
                string resultString = Regex.Match(geneInterpret[i + 1] + geneInterpret[i + 2], @"-?\d+").Value;
                int amount = int.Parse(resultString);

                model.herb += amount;

                i += 2;
            }
            else if (geneInterpret[i] == "omni")
            {
                string resultString = Regex.Match(geneInterpret[i + 1] + geneInterpret[i + 2], @"-?\d+").Value;
                int amount = int.Parse(resultString);

                model.omni += amount;

                i += 2;
            }
            else if (geneInterpret[i] == "carni")
            {
                string resultString = Regex.Match(geneInterpret[i + 1] + geneInterpret[i + 2], @"-?\d+").Value;
                int amount = int.Parse(resultString);

                model.carni += amount;

                i += 2;
            }
            else if (geneInterpret[i] == "reaction")
            {
                string resultString = Regex.Match( geneInterpret[i + 2], @"-?\d+").Value;
                int amount = int.Parse(resultString);

                model.reactionTime += amount;

                i += 2;
            }
            else if (geneInterpret[i] == "metabolism")
            {
                string resultString = Regex.Match(geneInterpret[i + 2], @"-?\d+").Value;
                int amount = int.Parse(resultString);

                model.metabolism += model.metabolism * (amount * .05f);

                i += 2;
            }
        }

        model.metabolism += model.mass * .5f;

        model.foodPref = FoodType.Herb;
        if (model.omni > model.herb && model.omni > model.carni)
            model.foodPref = FoodType.Omni;
        else if (model.carni > model.omni || model.carni > model.herb)
            model.foodPref = FoodType.Carni;

        
    }

    public static CreatureModel CreateCreature(CreatureModel model, bool[] genes = null)
    {

        model.inputs = new List<double>();
        model.ouputs = new List<double>();

        model.nNetwork = new NeuralNet(model.numInputs, model.numOutputs, model.numHiddenLayers, model.numNodeHiddenLayers);


        string[][] startBlock =
        {

            new string[] {"female", "male" }, //sex

        };

        string[][] geneBlock =
        {
            new string[] {"leg", "eyes", "size", "herb", "omni","carni", "reaction",  "metabolism"}, //attributes

             new string[] {"-", "" }, //signs

             new string[] { "0", "1", "2","3","4" , "5", "6", "7"}, //number;
        };

        string[][] endBlock =
        {
             new string[] {"tail", "notail" }, //body feature
        };


        if (genes == null)
            model.genes = new Genes(geneBlock, 18, startBlock);
        else
        {
            model.genes = new Genes(geneBlock, 18, startBlock);
            model.genes.genes = genes;
        }

        model.geneInterpret = model.genes.ReadGenes();

        ApplyGene(model, model.geneInterpret);



        return model;
    }


    public string GetInfo()
    {
        string info = "";
        info += "Name: " + model.name +
            "\nGender: " + model.sex +
            "\nAge: " + model.age.ToString("0") +
            "\nGeneration: " + model.generation +
            "\nSize: " + model.size +
            "\nMass: " + model.mass +
            "\nFood Preference: " + model.foodPref +
            "\nNutrition: " + model.nutrition +
            "\nReaction TIme: " + model.reactionTime +
            "\nMove Speed: " + model.moveSpeed +
            "\nNumber of Legs: " + model.numLegs +
            "\nNumber of Eyes: " + model.numEyes +
            "\nSightDistance: " + model.sightDistance +
            "\nMetabolism: " + model.metabolism +
            "\nHerb: " + model.herb +
            "\nOmni: " + model.omni +
            "\nCarni: " + model.carni;

        return info;
    }
    
}
