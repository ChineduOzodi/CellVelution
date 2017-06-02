using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using CodeControl;

public class GameManager : MonoBehaviour {

    public static GameManager instance; 

    public ModelRefs<TreeModel> trees;

    public GameObject seeds;

    private float distance = 20f;

	// Use this for initialization
	void Start () {
        trees = new ModelRefs<TreeModel>();
        seeds = new GameObject("Seeds");
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

}
