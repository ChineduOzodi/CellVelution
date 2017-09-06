/// <copyright file="Genome.cs">Copyright (c) 2016 All Rights Reserved</copyright>
/// <author>Chinedu Ozodi</author>
/// <date>12/12/2016</date>

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace NeuralNetwork
{
    [System.Serializable]
    /// <summary>
    /// Houses the weight information for the neural network
    /// </summary>
    public class Genome
    {

        private List<NodeGenes> nodeGenes;
        private List<ConnectGenes> connectGenes;

        private int innov;
        public int id;
        int numSensors;
        int numOutput;

        /// <summary>
        /// fitness level of the current weight information
        /// </summary>
        public double fitness;

        static double mutationChance = .8;
        /// <summary>
        /// Mutation rate
        /// </summary>
        public static double mutationRate = .9;
        static double newValueChance = .1;
        /// <summary>
        /// Crossover rate
        /// </summary>
        public static double crossOverRate = .75;
        public static double geneExpressionChance = .00001;
        
        double interspeciesMating = .001;
        double newNodeprobability = .1; //defualt .o3
        double newLinkMutation = .15; //DEFAULT : .05

        public int NumberNodes { get { return nodeGenes.Count; } }
        public int NumberLinks { get {

                int num = 0;

                foreach (ConnectGenes gene in connectGenes)
                {
                    if (gene.enabled)
                        num++;
                }
                return num; } }   

        /// <summary>
        /// Weght struct constructor, with default fitness of zero
        /// </summary>
        /// <param name="_weights"> double array of the weights to be used</param>
        /// <param name="_fitness"> fitness of the set of weights</param>
        public Genome(int _numSensors, int _numOutput)
        {
            fitness = 0;
            innov = 0;
            numSensors = _numSensors;
            numOutput = _numOutput;
            nodeGenes = new List<NodeGenes>();
            connectGenes = new List<ConnectGenes>();
            id = Random.Range(0, 10000);

            for(int i = 0; i < numSensors; i++)
            {
                nodeGenes.Add(new NodeGenes(nodeGenes.Count, NodeType.Sensor));
            }
            for (int i = 0; i < numOutput; i++)
            {
                nodeGenes.Add(new NodeGenes(nodeGenes.Count, NodeType.Output));
            }

            var inputGenes = GetGenes(NodeType.Sensor);
            var outputGenes = GetGenes(NodeType.Output);

            for (int i = 0; i < numSensors; i++)
            {
                for (int b = 0; b < numOutput; b++)
                {
                    connectGenes.Add(new ConnectGenes(inputGenes[i].node, outputGenes[b].node, Random.Range(-1f, 1f), true, innov++));
                }
            }



        }

        internal List<int> NodeColumns()
        {
            List<int> list = new List<int>();

            int input = 1;
            int output = 1;

            for (int i = 0; i < nodeGenes.Count; i++)
            {
                if (nodeGenes[i].nodeType == NodeType.Sensor)
                    list.Add(input++);
                else if (nodeGenes[i].nodeType == NodeType.Hidden)
                    list.Add(Random.Range(2, 9));
                else
                    list.Add(output++);
            }

            return list;
        }

        internal List<double> GetNodeValues()
        {
            List<double> list = new List<double>();

            for (int i = 0; i < nodeGenes.Count; i++)
            {
                list.Add(nodeGenes[i].value);
            }

            return list;
        }

        internal List<double> GetLinkValues()
        {
            List<double> list = new List<double>();

            for (int i = 0; i < connectGenes.Count; i++)
            {
                if (connectGenes[i].enabled)
                {
                    list.Add(connectGenes[i].weight * nodeGenes.Find(x => x.node == connectGenes[i].inn).value);
                }
            }

            return list;
        }

        internal List<int> LinkNodeFromIDs()
        {
            List<int> list = new List<int>();

            for (int i = 0; i < connectGenes.Count; i++)
            {
                if (connectGenes[i].enabled)
                    list.Add(nodeGenes.IndexOf(nodeGenes.Find(x => x.node == connectGenes[i].inn)) + 1);
            }

            return list;
        }

        internal List<int> LinkNodeToIDs()
        {
            List<int> list = new List<int>();

            for (int i = 0; i < connectGenes.Count; i++)
            {
                if (connectGenes[i].enabled)
                    list.Add(nodeGenes.IndexOf(nodeGenes.Find(x => x.node == connectGenes[i].outt)) + 1);
            }

            return list;
        }

        internal List<int> NodeRows()
        {
            List<int> list = new List<int>();

            int input = 1;
            int output = 10;

            for (int i = 0; i < nodeGenes.Count; i++)
            {
                if (nodeGenes[i].nodeType == NodeType.Sensor)
                    list.Add(input);
                else if (nodeGenes[i].nodeType == NodeType.Hidden)
                    list.Add(Random.Range(2, 9));
                else
                    list.Add(output);
            }

            return list;
        }


        private List<NodeGenes> GetGenes(NodeType sensor)
        {
            List<NodeGenes> selectedGenes = new List<NodeGenes>();
            
            foreach (NodeGenes gene in nodeGenes)
            {
                if (sensor == gene.nodeType)
                {
                    selectedGenes.Add(gene);
                }
            }

            return selectedGenes;
        }


        public static bool operator <(Genome g1, Genome g2)
        {
            return (g1.fitness < g2.fitness);
        }
        public static bool operator >(Genome g1, Genome g2)
        {
            return (g1.fitness > g2.fitness);
        }

        private enum NodeType
        {
            Sensor, Output, Hidden
        }
        [System.Serializable]
        private class NodeGenes
        {
            public int node;
            public NodeType nodeType;

            public bool valueSet;
            public double value;

            public NodeGenes(int _node, NodeType _nodeType)
            {
                node = _node;
                nodeType = _nodeType;

                valueSet = false;
                value = 0;
            }

            public void ResetValue()
            {
                valueSet = false;
                value = 0;
            }

            public void SetValue(double num)
            {
                valueSet = true;
                value = num;
            }
        }
        [System.Serializable]
        private class ConnectGenes
        {
            public int inn;
            public int outt;
            public double weight;
            public bool enabled;
            public int innov;

            public ConnectGenes(int inn, int outt, double weight, bool enabled, int innov)
            {
                this.inn = inn;
                this.outt = outt;
                this.weight = weight;
                this.enabled = enabled;
                this.innov = innov;
            }

            public void setEnabled(bool value)
            {
                enabled = value;
            }

            public void setWeight(double newWeight)
            {
                weight = newWeight;
            }
        }

        public void MutateAddConnection()
        {
            List<NodeGenes> connectStart = GetGenes(NodeType.Sensor);
            var connectEnd = GetGenes(NodeType.Output);
            var hidden = GetGenes(NodeType.Hidden);
            connectStart.AddRange(hidden);
            connectEnd.AddRange(hidden);

            int startNum = Random.Range(0, connectStart.Count);
            int endNum = Random.Range(0, connectEnd.Count);

            if (connectStart[startNum].node == connectEnd[endNum].node)
                return;

            bool add = true;

            foreach (ConnectGenes connection in connectGenes)
            {
                if (connection.inn == connectStart[startNum].node && connection.outt == connectEnd[endNum].node)
                {
                    if (!connection.enabled)
                        connection.setEnabled(true);
                    add = false;
                    break;
                }
            }

            if (add)
            {
                connectGenes.Add(new ConnectGenes(connectStart[startNum].node, connectEnd[endNum].node, Random.Range(-1f, 1f), true, innov++));
            }
        }

        public void MutateAddNode()
        {
            nodeGenes.Add(new NodeGenes(nodeGenes.Count, NodeType.Hidden));

            var newNodeGene = nodeGenes[nodeGenes.Count-1];

            var connection = connectGenes[Random.Range(0, connectGenes.Count)];

            connection.enabled = false;

            connectGenes.Add(new ConnectGenes(connection.inn, newNodeGene.node, 1, true, innov++));
            connectGenes.Add(new ConnectGenes(newNodeGene.node,connection.outt, connection.weight, true, innov++));
        }

        public static Genome Mate(Genome p1, Genome p2)
        {
            Genome newGenome = new Genome(p1.numSensors,p1.numOutput);
            newGenome.connectGenes = new List<ConnectGenes>();
            newGenome.nodeGenes = new List<NodeGenes>();

            if (Random.value < crossOverRate)
            {
                int p1L = 0;
                int p2L = 0;

                bool p1End = false;
                bool p2End = false;

                while (true)
                {
                    if (p1L >= p1.connectGenes.Count)
                    {
                        p1End = true;
                    }

                    if (p2L >= p2.connectGenes.Count)
                    {
                        p2End = true;
                    }

                    if (p1End && p2End)
                    {
                        break;
                    }

                    ConnectGenes p2Connect = null;
                    ConnectGenes p1Connect = null;

                    if (!p2End)
                        p2Connect = new ConnectGenes(p2.connectGenes[p2L].inn, p2.connectGenes[p2L].outt, p2.connectGenes[p2L].weight, p2.connectGenes[p2L].enabled, p2.connectGenes[p2L].innov);
                    if (!p1End)
                        p1Connect = new ConnectGenes(p1.connectGenes[p1L].inn, p1.connectGenes[p1L].outt, p1.connectGenes[p1L].weight, p1.connectGenes[p1L].enabled, p1.connectGenes[p1L].innov);

                    if ( p1End)
                    {
                        

                        if (p1.fitness < p2.fitness)
                        {
                            newGenome.connectGenes.Add(p2Connect );
                            newGenome.AddNodes(p2.connectGenes[p2L], p2);
                        }
                        else if (p1.fitness > p2.fitness)
                        {
                        }
                        else
                        {
                            if (Random.value > .5f)
                            {
                                newGenome.connectGenes.Add(p2Connect);
                                newGenome.AddNodes(p2.connectGenes[p2L], p2);
                            }
                        }

                        p2L++;
                    }
                    else if ( p2End)
                    {
                        if (p1.fitness > p2.fitness)
                        {
                            newGenome.connectGenes.Add(p1Connect);
                            newGenome.AddNodes(p1.connectGenes[p1L], p1);
                        }
                        else if (p1.fitness < p2.fitness)
                        {
                        }
                        else
                        {
                            if (Random.value > .5f)
                            {
                                newGenome.connectGenes.Add(p1Connect);
                                newGenome.AddNodes(p1.connectGenes[p1L], p1);
                            }
                        }

                        p1L++;
                    }
                    else if ((p1.connectGenes[p1L].innov > p2.connectGenes[p2L].innov))
                    {
                        if (p1.fitness < p2.fitness)
                        {
                            newGenome.connectGenes.Add(p2Connect);
                            newGenome.AddNodes(p2.connectGenes[p2L], p2);
                        }
                        else if (p1.fitness > p2.fitness)
                        {
                        }
                        else
                        {
                            if (Random.value > .5f)
                            {
                                newGenome.connectGenes.Add(p2Connect);
                                newGenome.AddNodes(p2.connectGenes[p2L], p2);
                            }
                        }

                        p2L++;
                    }
                    else if ((p1.connectGenes[p1L].innov < p2.connectGenes[p2L].innov))
                    {
                        if (p1.fitness > p2.fitness)
                        {
                            newGenome.connectGenes.Add(p1Connect);
                            newGenome.AddNodes(p1.connectGenes[p1L], p1);
                        }
                        else if (p1.fitness < p2.fitness)
                        {
                        }
                        else
                        {
                            if (Random.value > .5f)
                            {
                                newGenome.connectGenes.Add(p1Connect);
                                newGenome.AddNodes(p1.connectGenes[p1L], p1);
                            }
                        }

                        p1L++;
                    }
                    else if (p1.connectGenes[p1L].innov == p2.connectGenes[p2L].innov)
                    {
                        if (Random.value > .5f)
                        {
                            newGenome.connectGenes.Add(p1Connect);
                            newGenome.AddNodes(p1.connectGenes[p1L], p1);
                        }
                        else
                        {
                            newGenome.connectGenes.Add(p2Connect);
                            newGenome.AddNodes(p2.connectGenes[p2L], p2);
                        }

                        if ((!p1.connectGenes[p1L].enabled || !p2.connectGenes[p2L].enabled) && Random.value > .25f)
                        {
                            newGenome.connectGenes[newGenome.connectGenes.Count-1].setEnabled(false);
                        }

                        p1L++;
                        p2L++;
                    }

                    
                }

                newGenome.nodeGenes.Sort((x, y) => x.node.CompareTo(y.node));
                newGenome.connectGenes.Sort((x, y) => x.innov.CompareTo(y.innov));
                newGenome.innov = newGenome.connectGenes[newGenome.connectGenes.Count - 1].innov;
            }
            else if (Random.value < .5)
            {
                newGenome.connectGenes = new List<ConnectGenes>(p1.connectGenes);
                newGenome.nodeGenes = new List<NodeGenes>(p1.nodeGenes);
                newGenome.innov = p1.innov;
            }
            else
            {
                newGenome.connectGenes = new List<ConnectGenes>(p2.connectGenes);
                newGenome.nodeGenes = new List<NodeGenes>(p2.nodeGenes);
                newGenome.innov = p2.innov;
            }            

            if (Random.value < mutationChance)
            {
                newGenome.Mutate();
            }
            return newGenome;
            
        }

        private void Mutate()
        {
            if (Random.value < newNodeprobability)
            {
                MutateAddNode();
            }
            if (Random.value < newLinkMutation)
            {
                MutateAddConnection();
            }
            foreach (ConnectGenes connect in connectGenes)
            {
                if (Random.value < newValueChance)
                {
                    connect.setWeight(Random.Range(-1f, 1f));
                }
                else if (Random.value < mutationRate)
                {
                    connect.setWeight(connect.weight + (double)Random.Range(-.01f, .01f));

                    if (connect.weight > 1)
                        connect.setWeight(1);
                    else if (connect.weight < -1)
                        connect.setWeight(-1);

                }

                //if (Random.value < geneExpressionChance)
                //{
                //    connect.enabled = !connect.enabled;
                //}
            }

            
        }

        private void AddNodes(ConnectGenes checkGenes, Genome p2)
        {
            NodeGenes inNode = p2.nodeGenes.Find(x => x.node == checkGenes.inn);
            NodeGenes outNode = p2.nodeGenes.Find(x => x.node == checkGenes.outt);

            if (nodeGenes.FindAll(x => x.node == inNode.node).Count == 0)
            {
                nodeGenes.Add(new NodeGenes(inNode.node, inNode.nodeType));
            }

            if (nodeGenes.FindAll(x => x.node == outNode.node).Count == 0)
            {
                nodeGenes.Add(new NodeGenes(outNode.node, outNode.nodeType));
            }
        }

        public static double CompatibilityDistance(Genome p1, Genome p2)
        {
            double c1 = 1;
            double c2 = 1;
            double c3 = .4;

            double n = p1.connectGenes.Count;
            if (p2.connectGenes.Count > n)
                n = p2.connectGenes.Count;

            int E = 0;
            int D = 0;
            double W = 0;
            int matching = 0;

            int p1L = 0;
            int p2L = 0;

            bool p1End = false;
            bool p2End = false;

            while (true)
            {
                if (p1L >= p1.connectGenes.Count)
                {
                    p1End = true;
                }

                if (p2L >= p2.connectGenes.Count)
                {
                    p2End = true;
                }

                if (p1End && p2End)
                {
                    break;
                }
                else if (p1End)
                {
                    if (p1End)
                    {
                        E++;
                    }
                    else
                    {
                        D++;
                    }

                    p2L++;
                }
                else if (p2End)
                {
                    if (p2End)
                    {
                        E++;
                    }
                    else
                    {
                        D++;
                    }
                    p1L++;
                }
                else if (p1.connectGenes[p1L].innov > p2.connectGenes[p2L].innov)
                {
                    if (p1End)
                    {
                        E++;
                    }
                    else
                    {
                        D++;
                    }

                    p2L++;
                }
                else if (p1.connectGenes[p1L].innov < p2.connectGenes[p2L].innov)
                {
                    if (p2End)
                    {
                        E++;
                    }
                    else
                    {
                        D++;
                    }
                    p1L++;
                }
                else if (p1.connectGenes[p1L].innov == p2.connectGenes[p2L].innov)
                {
                    W += Mathf.Abs((float)(p1.connectGenes[p1L].weight - p2.connectGenes[p2L].weight));
                    matching++;

                    p1L++;
                    p2L++;
                }
            }

            double diff = (c1 * E) / n + (c2 * D) / n + c3 * (W / matching);
            return diff;
        }

        /// <summary>
        /// Runs through the network to generate an output from your input
        /// </summary>
        /// <param name="inputs">List of inputs for the network, should match the number of inputs created for the network</param>
        /// <returns>List of ouputs matching the number of ouputs created for the network</returns>
        public List<double> Run(List<double> inputs)
        {

            if (inputs.Count != numSensors)
            {
                throw new System.Exception("Number of inputs does not match specifications");
            }

            List<double> outputs = new List<double>();

            for( int i = 0; i < nodeGenes.Count; i++)
            {
                nodeGenes[i].ResetValue();
            }

            var inputNodes = GetGenes(NodeType.Sensor);
            var outputNodes = GetGenes(NodeType.Output);

            for (int i = 0; i < inputNodes.Count; i++)
            {
                inputNodes[i].SetValue(inputs[i]);
            }

            for (int i = 0; i < outputNodes.Count; i++)
            {
                var num = CalculateValue(outputNodes[i]);
                outputs.Add(num);
            }
            return outputs;
        }

        private double CalculateValue(NodeGenes nodeGene)
        {
            if (nodeGene.valueSet || nodeGene.nodeType == NodeType.Sensor)
                return nodeGene.value;

            List<ConnectGenes> connections = connectGenes.FindAll(x => x.outt == nodeGene.node);

            double value = 0;

            for (int j = 0; j < connections.Count; j++)
            {
                if (connections[j].enabled)
                    value += CalculateValue(nodeGenes.Find(x => x.node == connections[j].inn)) * connections[j].weight;
            }

            nodeGene.SetValue(Sigmoid(value, 4.9));
            return nodeGene.value;
        }

        /// <summary>
        /// Sigmoid function to be used in network calculations
        /// </summary>
        /// <param name="activation">The activation number from weight and nodes</param>
        /// <param name="response">How responsive the sigmoid should be to the numbers, 1 is usually okay</param>
        /// <returns>Output signal</returns>
        internal static double Sigmoid(double activation, double response)
        {
            return 1.0 / (1 + Mathf.Pow(Mathf.Epsilon, (float)(-activation * response)));
        }

        public static T DeepCopy<T>(T other)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(ms, other);
                ms.Position = 0;
                return (T)formatter.Deserialize(ms);
            }
        }
    }
}

