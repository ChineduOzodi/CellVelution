using System;
using System.Collections;
using System.Collections.Generic;
using NeuralNetwork;
using UnityEngine;
using UnityEngine.UI;

public class NeuralMap : MonoBehaviour {

    public GameObject treePrefab;

    GameObject treeObj;

    Image[] nodes;

    List<Image[]> linkSprites;

    WMG_Hierarchical_Tree tree;

    int numNodes;
    int numLinks;

    bool init = true;

	// Use this for initialization
	void Awake () {

        treeObj = Instantiate(treePrefab, transform);
        tree = treeObj.GetComponent<WMG_Hierarchical_Tree>();

        tree.numNodes = 2;
        tree.numLinks = 1;
        tree.nodeColumns = new List<int> { 1, 1 };
        tree.nodeRows = new List<int> { 1, 2 };
        tree.linkNodeToIDs = new List<int> { 2 };
        tree.linkNodeFromIDs = new List<int> { 1 };

        tree.Init();

        var rect = treeObj.GetComponent<RectTransform>();
        //rect.anchoredPosition = new Vector3(Screen.width * -.25f, 0, 0);
	}
	
	// Update is called once per frame
	void Update () {


		
	}

    internal void CreateGraph(Genome genome)
    {

        Destroy(treeObj);

        treeObj = Instantiate(treePrefab, transform);
        tree = treeObj.GetComponent<WMG_Hierarchical_Tree>();

        numNodes = genome.NumberNodes;
        numLinks = genome.NumberLinks;

        tree.numNodes = numNodes;
        tree.numLinks = numLinks;

        tree.nodeColumns = genome.NodeColumns();
        tree.nodeRows = genome.NodeRows();
        tree.linkNodeToIDs = genome.LinkNodeToIDs();
        tree.linkNodeFromIDs = genome.LinkNodeFromIDs();

        tree.Init();

        var rect = treeObj.GetComponent<RectTransform>();
        //rect.anchoredPosition = new Vector3(Screen.width * -.25f, 0, 0);

        init = true;


    }

    internal void NodeUpdate(Genome genome)
    {
        if (init)
        {
            nodes = tree.nodeParent.GetComponentsInChildren<Image>();
            linkSprites = new List<Image[]>();
            var links = tree.LinksParent;

            for (int i = 0; i < numLinks; i++)
            {
                var sprites = links[i].GetComponentsInChildren<Image>();
                linkSprites.Add(sprites);
            }

            init = false;
        }
        List<double> nodeValues = genome.GetNodeValues();

        List<double> linkValues = genome.GetLinkValues();

        for(int i = 0; i < numNodes; i++)
        {
            if (nodeValues[i] > 0)
            {
                nodes[i].color = new Color(0,(float)(nodeValues[i]),0,1);
                //nodes[i].transform.localScale = Vector3.one * ((float)nodeValues[i]);
            }
            else
            {
                nodes[i].color = new Color((float)(nodeValues[i] * -1), 0, 0);
                //nodes[i].transform.localScale = Vector3.one * ((float)nodeValues[i] * -1);
            }
        }

        for (int i = 0; i < numLinks; i++)
        {
            if (linkValues[i] > 0)
            {
                var sprites = linkSprites[i];
                foreach (Image image in sprites)
                {
                    image.color = new Color(0, (float)(linkValues[i]), 0, 1);
                    //image.transform.localScale = Vector3.one * ((float)linkValues[i]);
                }
                
            }
            else
            {
                var sprites = linkSprites[i];
                foreach (Image image in sprites)
                {
                    image.color = new Color((float)(linkValues[i] * -1), 0, 0);
                    //image.transform.localScale = Vector3.one * ((float)linkValues[i] * -1);
                }
                
            }
        }
    }
}
