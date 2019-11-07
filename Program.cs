﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace Huffman
{
    class Node
    {
        public long weight;
        public int creationOrder;
        public KeyValuePair<char, int> pair;
        public Node left;
        public Node right;

        public Node(KeyValuePair<char, int> dictPair)
        {
            weight = dictPair.Value;
            pair = dictPair;
            left = null;
            right = null;
        }
        private bool CheckNodeOrder(Tuple<Node, Node> nodePair)
        {
            if (nodePair.Item1.left == null && nodePair.Item1.right == null)
            {
                // Node 1 is a leaf
                if (nodePair.Item2.left == null && nodePair.Item2.right == null)
                {
                    // Node 2 is also a leaf
                    if (nodePair.Item1.weight < nodePair.Item2.weight)
                    {
                        // Node 1 goes left
                        return true;
                    }
                    else
                    {
                        // Node 2 goes left
                        return false;
                    }
                }
                else
                {
                    // Node 2 is not a leaf
                    return true;
                }
            }
            else if (nodePair.Item2.left == null && nodePair.Item2.right == null)
            {
                // Node 2 is a leaf while Node 1 isn't
                return false;
            }
            else
            {
                // Neither Node 1 nor Node 2 are leaves
                if (nodePair.Item1.creationOrder < nodePair.Item2.creationOrder)
                {
                    // Node 1 goes left
                    return true;
                }
                else
                {
                    // Node 2 goes left
                    return false;
                }
            }
        }
        public Node(Tuple<Node, Node> nodePair, int creationOrder)
        {
            this.creationOrder = creationOrder;
            weight = nodePair.Item1.weight + nodePair.Item2.weight;

            if (CheckNodeOrder(nodePair))
            {
                left = nodePair.Item1;
                right = nodePair.Item2;
            }
            else
            {
                left = nodePair.Item2;
                right = nodePair.Item1;
            }
        }
    }

    class Tree
    {
        public void Traverse(Node root)
        {
            if (root == null)
            {
                return;
            }

            Traverse(root.left);
            Traverse(root.right);
        }
    }

    class TreeBuilder
    {
        private Dictionary<char, int> _dictionary;
        private List<KeyValuePair<char, int>> _dictInListForm;
        private List<Node> _nodeList;

        public TreeBuilder()
        {
            _dictionary = new Dictionary<char, int>();
        }

        public int GetNodeListCount()
        {
            return _nodeList.Count;
        }

        public Node GetRoot()
        {
            return _nodeList[0];
        }
        public void AddToDictionary(char readByte)
        {
            if (_dictionary.ContainsKey(readByte))
                _dictionary[readByte]++;
            else
                _dictionary.Add(readByte, 1);
        }
        private void ConvertToList()
        {
            _dictInListForm = _dictionary.OrderBy(d => d.Value).ToList();
        }
        private void ConvertToNodes()
        {
            foreach (KeyValuePair<char, int> pair in _dictInListForm)
            {
                _nodeList.Add(new Node(pair));
            }
            SortByByte();
        }

        public void InsertIntoNodeList(Node node)
        {
            _nodeList.Insert(0, node);
        }

        private void SortByByte()
        {
            bool unsorted = true;
            while (unsorted)
            {
                for (int i = 0; i < _nodeList.Count - 1; i++)
                {
                    unsorted = false;
                    if (_nodeList[i].weight == _nodeList[i + 1].weight)
                    {
                        if (_nodeList[i].pair.Key < _nodeList[i + 1].pair.Key)
                        {
                            var node = _nodeList[i];
                            _nodeList[i] = _nodeList[i + 1];
                            _nodeList[i + 1] = _nodeList[i];
                            unsorted = true;
                        }
                    }
                }
            }
        }
        public Tuple<Node, Node> GetNodes()
        {
            if (_nodeList.Count > 1)
            {
                return new Tuple<Node, Node>(_nodeList[0], _nodeList[1]);
                // !!!! ADD SORTING BY KEY !!!! !!!! ADD SORTING BY KEY !!!! !!!! ADD SORTING BY KEY !!!! !!!! ADD SORTING BY KEY !!!! !!!! ADD SORTING BY KEY !!!! !!!! ADD SORTING BY KEY !!!! !!!! ADD SORTING BY KEY !!!! 
            }
            else
            {
                return null;
            }
        }
        public Node JoinTwoNodes(Tuple<Node, Node> nodePair, int creationOrder)
        {
            return new Node(nodePair, creationOrder);
        }
        public void PrintDictionary()
        {
            foreach (KeyValuePair<char, int> entry in _dictionary)
            {
                Console.WriteLine("{0}: {1}", entry.Key, entry.Value);
            }
        }
    }

    class InputProcessor
    {
        private FileStream _reader;
        public InputProcessor(string fileName)
        {
            try
            {
                FileStream reader = new FileStream(fileName, FileMode.Open);
                _reader = reader;
            }
            catch
            {
                Console.WriteLine("File Error");
            }
        }
        public int ReadByte()
        {
            int buffer;
            if ((buffer = _reader.ReadByte()) >= 0)
                return buffer;
            else
                return -1;
        }
    }
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Argument Error");
                return (0);
            }

            InputProcessor inputProcessor = new InputProcessor(args[0]);
            TreeBuilder treeBuilder = new TreeBuilder();

            int inputByte;
            while ((inputByte = inputProcessor.ReadByte()) > -1)
            {
                treeBuilder.AddToDictionary((char) inputByte);
            }

            int i = 0;
            while (treeBuilder.GetNodeListCount() > 1)
            {
                Node innerNode = treeBuilder.JoinTwoNodes(treeBuilder.GetNodes(), i);
                treeBuilder.InsertIntoNodeList(innerNode);
                i++;
            }
            


            return (0);
        }
    }
}