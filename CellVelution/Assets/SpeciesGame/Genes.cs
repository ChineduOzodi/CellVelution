using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Genes {

    [SerializeField]
    int length;
    [SerializeField]
    int numBlocks;
    [SerializeField]
    string[][] startBlock;
    [SerializeField]
    string[][] geneBlock;
    [SerializeField]
    float crossOverRate;
    [SerializeField]
    float mutationRate;
    [SerializeField]
    public bool[] genes;

	
    public Genes(string[][] _geneBlock, int _numBlocks, string[][] _startBlock = null, float _crossOverRate = .7f, float _mutationRate = .1f)
    {

        mutationRate = _mutationRate;
        crossOverRate = _crossOverRate;
        numBlocks = _numBlocks;
        
        startBlock = _startBlock;
        geneBlock = _geneBlock;

        //Find Gene Length
        length = 0;

        foreach (string[] block in startBlock)
        {
            int bitCount = 2;
            int bits = 1;
            while (bitCount < block.Length)
            {
                bitCount *= 2;
                bits++;
            }

            length += bits;
        }
        for (int i =0; i < numBlocks; i++)
        {
            foreach (string[] block in geneBlock)
            {
                int bitCount = 2;
                int bits = 1;
                while (bitCount < block.Length)
                {
                    bitCount *= 2;
                    bits++;
                }

                length += bits;
            }
        }

        //Create Random genes

        genes = new bool[length];

        for (int i = 0; i < length; i++)
        {
            int r = Random.Range(0, 2);
            if (r == 0)
                genes[i] = false;
            else
                genes[i] = true;
        }
    }

    private void Crossover(bool[] mum, bool[] dad, ref bool[] baby1, ref bool[] baby2)
    {
        if (Random.Range(0f, 1f) > crossOverRate || mum == dad)
        {
            baby1 = mum;
            baby2 = dad;

            return;
        }

        int cp = Random.Range(0, length - 1);
        //print("crossover at " + cp);
        //Create the offspring

        for (int i = 0; i < cp; i++)
        {
            baby1[i] = mum[i];
            baby2[i] = dad[i];
        }
        for (int i = cp; i < mum.Length; i++)
        {
            baby1[i] = dad[i];
            baby2[i] = mum[i];
        }

        return;
    }

    private void Mutate(ref bool[] chromo)
    {
        for (int i = 0; i < chromo.Length; i++)
        {
            float randFloat = Random.Range(0f, 1f);
            if (randFloat < mutationRate)
            {
                int r = Random.Range(0, 2);
                if (r == 0)
                    chromo[i] = false;
                else
                    chromo[i] = true;
            }
        }
    }

    public bool[] Mate(bool[] partner)
    {
        bool[] p1 = partner;
        bool[] p2 = genes;

        bool[] baby1 = new bool[length];
        bool[] baby2 = new bool[length];

        Crossover(p1, p2, ref baby1, ref baby2);

        int selection = Random.Range(0, 2);

        if (selection == 0)
        {
            Mutate(ref baby1);

            return baby1;
        }
        else
        {
            Mutate(ref baby2);

            return baby2;
        }
    }



    public bool[] Duplicate()
    {
        bool[] p1 = genes;
        bool[] p2 = genes;

        bool[] baby1 = new bool[length];
        bool[] baby2 = new bool[length];

        baby1 = p1;

        Mutate(ref baby1);

        return baby1;
    }

    public List<string> ReadGenes()
    {
        List<string> read = new List<string>();
        int geneIndex = 0;
        for (int i = 0; i < startBlock.Length; i++)
        {
            
        }

        foreach (string[] block in startBlock)
        {
            int bitCount = 2;
            int bits = 1;
            while (bitCount < block.Length)
            {
                bitCount *= 2;
                bits++;
            }

            BitArray bit = new BitArray(bitCount);

            for (int j = 0; j < bits; j++)
            {
                bit[j] = genes[geneIndex++];
            }

            int[] num = { 0 };

            bit.CopyTo(num, 0);

            if (num[0] >= block.Length)
            {
                
            }
            else
                read.Add(block[num[0]]);
        }
        for ( int i = 0; i < numBlocks; i++)
        {
            foreach (string[] block in geneBlock)
            {
                int bitCount = 2;
                int bits = 1;
                while (bitCount < block.Length)
                {
                    bitCount *= 2;
                    bits++;
                }

                BitArray bit = new BitArray(bitCount);

                for (int j = 0; j < bits; j++)
                {
                    bit[j] = genes[geneIndex++];
                }

                int[] num = { 0 };

                bit.CopyTo(num, 0);

                if (num[0] >= block.Length)
                {
                    
                }
                else
                    read.Add(block[num[0]]);
            }
        }

        return read;

    }
}
