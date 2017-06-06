using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using CodeControl;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public static GameManager instance;
    public ModelRefs<TreeModel> trees;

    internal Date date;
    internal float curTemp;
    internal float aveTemp = 20;
    private string info;

    internal GameObject seeds;

    private float distance = 50f;
    private int i = 0;
    public float windScale = 1;
    public float windStrength = 5;
    public float windStrengthScale = .01f;
    public float dragCoefficient = 1;
    public float airDensity = 1;

	// Use this for initialization
	void Start () {
        trees = new ModelRefs<TreeModel>();
        seeds = new GameObject("Seeds");
        date = new Date();
        instance = this;
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            if(EventSystem.current.currentSelectedGameObject == null)
            {
                CreateTree();
                
            }
        }

        if (trees.Count > 0)
        {
            if (i >= trees.Count) i = 0;
            //print("Updating " + tree.name);
            trees[i].Grow(date.time - trees[i].lastUpdated);

            trees[i].NotifyChange();

            i++;
        }

        date.AddTime(Time.deltaTime);
        UpdateTemperature();

    }


    void CreateTree()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, distance))
        {
            Vector3 point = ray.origin + (ray.direction * 4);

            if (hit.collider.tag == "tree")
            {
                info = hit.collider.GetComponentInParent<TreeController>().GetInfo();
            }
            else if (hit.collider.tag == "Terrain")
            {
                TreeModel tree = new TreeModel("Tree" + trees.Count + 1, hit.point, date);
                GameManager.instance.trees.Add(tree);
                Controller.Instantiate<TreeController>("tree", tree);
            }
            

        }
    }

   void OnGUI()
    {

        GUI.TextArea(new Rect(10, 10, 200, 150), date.GetDateTime() + "\nTemp: " + curTemp.ToString("0.0") + "\n" + info);
    }
    internal void UpdateTemperature()
    {
        int yearTempRange = 7;
        int dayTempRange = 3;

        curTemp = aveTemp - yearTempRange * Mathf.Cos(5 / (Mathf.PI * 2)) - yearTempRange * Mathf.Cos((date.Day + 5) / (2 * Mathf.PI)) - dayTempRange * Mathf.Cos((date.Hour) * Mathf.PI / 12);

    }

    /// <summary>
    /// Gets wind force for a given position
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public Vector3 WindForce(Vector3 position)
    {
        float strength = windStrength * Mathf.PerlinNoise(date.time * windStrengthScale,date.time * windStrengthScale);

        return new Vector3((Mathf.PerlinNoise(position.x * windScale, date.time * windScale) - .5f) * strength,
            (Mathf.PerlinNoise(position.y * windScale, date.time * windScale) - .5f) * strength,
        (Mathf.PerlinNoise(position.z * windScale, date.time * windScale) - .5f) * strength * .25f);
    }

    public Vector3 SphereDrag(GameObject sphere)
    {
        float velSquared = Mathf.Pow(sphere.GetComponent<Rigidbody>().velocity.magnitude,2);
        Vector3 direciton = sphere.GetComponent<Rigidbody>().velocity.normalized;
        return dragCoefficient * .5f * airDensity * AreaOfCircle(sphere.transform.localScale.y * .5f) * direciton * -velSquared;
    }

    private float AreaOfCircle(float radius)
    {
        return Mathf.PI * radius * radius;
    }

    private void CreatePollen()
    {
        PollenModel pollen = new PollenModel();
        pollen.position = Vector3.up * 10;
        pollen.rotation = Quaternion.identity;
        Controller.Instantiate<PollenController>("pollen", pollen);
    }
}
