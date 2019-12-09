using System;
using System.Collections.Generic;

namespace Huffman2
{
    internal class HashTable
    {
        private readonly long[] _hashTable;

        public HashTable()
        {
            _hashTable = new long[256];
        }

        public void Insert(int readByte)
        {
            _hashTable[readByte]++;
        }

        public List<Node> ConvertToList()
        {
            var nodeList = new List<Node>();

            for (var i = 0; i < _hashTable.Length; i++)
                if (_hashTable[i] > 0)
                    nodeList.Add(new Node(i, _hashTable[i]));

            return nodeList;
        }
    }

    internal class PriorityQueue
    {
        public List<Node> NodeList;

        public void InitQueue(HashTable hashTable)
        {
            NodeList = hashTable.ConvertToList();
            ReSort();
        }

        public void ReSort()
        {
            SortByWeight();
            SortByByte();
            SortByOriginCount();
        }

        public void QueueInsert(Node node)
        {
            NodeList.Insert(0, node);
            ReSort();
        }

        public Tuple<Node, Node> ExtractMin()
        {
            if (NodeList.Count <= 1) return null;
            var returnVal = new Tuple<Node, Node>(NodeList[0], NodeList[1]);
            NodeList.RemoveAt(0);
            NodeList.RemoveAt(0);
            return returnVal;
        }

        private void SortByWeight()
        {
            var unsorted = true;
            while (unsorted)
            {
                unsorted = false;
                for (var i = 0; i < NodeList.Count - 1; i++)
                    if (NodeList[i].Weight > NodeList[i + 1].Weight)
                    {
                        var node = NodeList[i];
                        NodeList[i] = NodeList[i + 1];
                        NodeList[i + 1] = node;
                        unsorted = true;
                    }
            }
        }

        private void SortByByte()
        {
            var unsorted = true;
            while (unsorted)
            {
                unsorted = false;
                for (var i = 0; i < NodeList.Count - 1; i++)
                    if (NodeList[i].Weight == NodeList[i + 1].Weight)
                        if (NodeList[i].ByteValue > NodeList[i + 1].ByteValue)
                        {
                            var node = NodeList[i];
                            NodeList[i] = NodeList[i + 1];
                            NodeList[i + 1] = node;
                            unsorted = true;
                        }
            }
        }

        private void SortByOriginCount()
        {
            var unsorted = true;
            while (unsorted)
            {
                unsorted = false;
                for (var i = 0; i < NodeList.Count - 1; i++)
                    if (NodeList[i].Weight == NodeList[i + 1].Weight)
                        if (NodeList[i].ByteValue == NodeList[i + 1].ByteValue)
                            if (NodeList[i].CreationOrder > NodeList[i + 1].CreationOrder)
                            {
                                var node = NodeList[i];
                                NodeList[i] = NodeList[i + 1];
                                NodeList[i + 1] = node;
                                unsorted = true;
                            }
            }
        }
    }
}