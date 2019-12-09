using System;
using System.Collections.Generic;

namespace Huffman2
{
    internal class Node
    {
        public int CreationOrder;
        public Node Left;
        public Node Right;
        public long Weight;
        public int ByteValue;

        public Node(int byteValue, long weight)
        {
            Weight = weight;
            ByteValue = byteValue;
            Left = null;
            Right = null;
        }

        public Node(Tuple<Node, Node> nodePair, int creationOrder)
        {
            CreationOrder = creationOrder;
            Weight = nodePair.Item1.Weight + nodePair.Item2.Weight;
            ByteValue = 256;
            Left = nodePair.Item1;
            Right = nodePair.Item2;
        }
    }

    internal class Tree
    {
        public static ulong[,] CodingCache = new ulong[256, 3];

        private static ulong Reverse(ulong input)
        {
            var x = input;
            ulong y = 0;
            while (x != 0)
            {
                y <<= 1;
                y |= x & 1;
                x >>= 1;
            }

            return y;
        }

        public static void ReverseCache()
        {
            for (int i = 0; i < 256; i++)
            {
                CodingCache[i, 0] = Reverse(CodingCache[i, 0]) << (int)CodingCache[i, 2];
            }
        }
        public static void GetLeadingZeroes()
        {
            for (int i = 0; i < 256; i++)
            {
                var bitSequence = CodingCache[i, 0];
                var sequenceLength = CodingCache[i, 1];
                var temp = bitSequence;
                ulong actualLength = 0;

                while (temp != 0)
                {
                    actualLength++;
                    temp >>= 1;
                }

                CodingCache[i, 2] = sequenceLength - actualLength;
            }
        }
        
        public static ulong GenerateByteSequence(ulong value, byte nodeType, ulong nodeValue)
        {
            ulong nodePrefix = nodeValue << 56;
            ulong temp = value << 9;
            temp = temp >> 8;
            return (nodePrefix | temp | nodeType);
        }
        public static void TraversePreorder(Node node, List<ulong> preorderList, ulong bitSequence, ulong sequenceLength)
        {
            if (node == null)
                return;

            if (node.Left == null && node.Right == null)
            {
                ulong byteSequence = GenerateByteSequence((ulong)node.Weight, 0x01, (ulong)node.ByteValue);
                preorderList.Add(byteSequence);
                CodingCache[node.ByteValue, 0] = bitSequence;
                CodingCache[node.ByteValue, 1] = sequenceLength;
            }
            else
            {
                ulong byteSequence = GenerateByteSequence((ulong)node.Weight, 0x00, 0x00);
                preorderList.Add(byteSequence);
            }

            TraversePreorder(node.Left, preorderList, bitSequence << 1, sequenceLength + 1);
            TraversePreorder(node.Right, preorderList, (bitSequence << 1) + 1, sequenceLength + 1);
        }
    }
    internal class TreeBuilder
    {
        public HashTable HashTable;
        public Node GetRoot(PriorityQueue priorityQueue)
        {
            return priorityQueue.NodeList[0];
        }
        public TreeBuilder()
        {
            HashTable = new HashTable();
        }

        public Node JoinTwoNodes(Tuple<Node, Node> nodePair, int creationOrder)
        {
            return new Node(nodePair, creationOrder);
        }

    }
}
