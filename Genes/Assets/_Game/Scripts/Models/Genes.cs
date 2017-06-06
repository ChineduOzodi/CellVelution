using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Genes <T> {

    public string genotype { get; set; }
    public T phenotype { get; set; }

    internal Dictionary<T,string[]> phenotypes;

    public Genes<T> Child(Genes<T> gene1, Genes<T> gene2)
    {
        if (gene1.phenotypes.Count != gene2.phenotypes.Count)
        {
            throw new System.Exception("phenotype dictionarys do not match");
        }
        Genes<T> child = new Genes<T>(gene1.phenotypes);
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
        child.SetPhenotype();
        return child;

    }
    public Genes(Dictionary<T,string[]> _phenotypes)
    {
        phenotypes = _phenotypes;
    }
    public Genes(int geneLength, string geneTypes, Dictionary<T,string[]> _phenotypes)
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
        foreach (T ptype in phenotypes.Keys)
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
        if (phenotype == null)
        throw new System.Exception("Could not find a matching phenotype for this genotype: " + genotype);
    }
}
