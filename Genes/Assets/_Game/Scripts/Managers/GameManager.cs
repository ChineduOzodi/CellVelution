using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using CodeControl;

public class GameManager : MonoBehaviour {

    public static GameManager instance; 

    public ModelRefs<TreeModel> trees;

    internal Date date;
    internal float curTemp;
    internal float aveTemp = 20;

    internal GameObject seeds;

    private float distance = 20f;

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

        foreach (TreeModel tree in trees)
        {
            //print("Updating " + tree.name);
            tree.Grow(Time.deltaTime);
            
            tree.NotifyChange();
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
            Vector3 point = ray.origin + (ray.direction * distance);
            TreeModel tree = new TreeModel("Tree" + trees.Count + 1, hit.point);
            GameManager.instance.trees.Add(tree);
            Controller.Instantiate<TreeController>("tree", tree);

        }
    }

   void OnGUI()
    {

        GUI.TextArea(new Rect(10, 10, 150, 100), date.GetDateTime() + " Temp: " + curTemp);
    }
    internal void UpdateTemperature()
    {
        int yearTempRange = 7;
        int dayTempRange = 3;

        curTemp = aveTemp - yearTempRange * Mathf.Cos(5 / (Mathf.PI * 2)) - yearTempRange * Mathf.Cos((date.day + 5) / (2 * Mathf.PI)) - dayTempRange * Mathf.Cos((date.hour) * Mathf.PI / 12);

    }
}
