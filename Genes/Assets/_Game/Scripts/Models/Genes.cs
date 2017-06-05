using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Genes {

    public string genotype { get; set; }
    public string phenotype { get; set; }

    internal Dictionary<string,string[]> phenotypes;

    public Genes Child(Genes gene1, Genes gene2)
    {
        if (gene1.phenotypes != gene2.phenotypes)
        {
            throw new System.Exception("phenotype dictionarys do not match");
        }
        Genes child = new Genes(gene1.phenotypes);
        for (int i = 0; i < gene1.genotype.Length; i++)
        {
            int num = Random.Range(0, 2);
            if ( num == 0)
            {
                child.genotype += gene1.genotype[i];
            }
            else
            {
                child.genotype += gene2.genotype[i];
            }
        }
        SetPhenotype();
        return child;

    }
    public Genes(Dictionary<string,string[]> _phenotypes)
    {
        phenotypes = _phenotypes;
    }
    public Genes(int geneLength, string geneTypes, Dictionary<string,string[]> _phenotypes)
    {
        phenotypes = _phenotypes;

        for (int i = 0; i < geneLength; i++)
        {
            int num = Random.Range(0, geneTypes.Length);
            genotype += geneTypes[num];
        }

        SetPhenotype();
    }

    private void SetPhenotype()
    {
        foreach (string ptype in phenotypes.Keys)
        {
            for (int i = 0; i < phenotypes[ptype].Length; i++)
            {
                if (genotype == phenotypes[ptype][i])
                {
                    phenotype = ptype;
                    break;
                }
            }
        }

        throw new System.Exception("Could not find a matching phenotype for this genotype");
    }
}
