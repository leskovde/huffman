using System;
using System.Collections.Generic;
using System.IO;

namespace Huffman2
{
    internal class InputProcessor
    {
        private readonly FileStream _reader;

        public InputProcessor(string fileName)
        {
            try
            {
                _reader = new FileStream(fileName, FileMode.Open);
            }
            catch
            {
                Console.WriteLine("File Error");
                Environment.Exit(0);
            }
        }

        public int ReadByte()
        {
            try
            {
                int buffer;
                if ((buffer = _reader.ReadByte()) >= 0)
                    return buffer;
            }
            catch
            {
                Console.WriteLine("File Error");
                Environment.Exit(0);
            }

            return -1;
        }

        public void SeekSet()
        {
            try
            {
                _reader.Seek(0, SeekOrigin.Begin);
            }
            catch
            {
                Console.WriteLine("File Error");
                Environment.Exit(0);
            }
        }
    }

    internal class OutputProcessor
    {
        private readonly FileStream _writer;

        public OutputProcessor(string fileName)
        {
            try
            {
                _writer = new FileStream(fileName + ".huff", FileMode.CreateNew, FileAccess.Write);
            }
            catch
            {
                Console.WriteLine("File Error");
                Environment.Exit(0);
            }
        }

        public void WriteBuffer(byte[] buffer)
        {
            try
            {
                _writer.Write(buffer, 0, buffer.Length);
            }
            catch
            {
                Console.WriteLine("File Error");
                Environment.Exit(0);
            }
        }

        public void CloseFile()
        {
            _writer.Close();
        }
    }

    internal class Coding
    {
        public readonly byte[] Header =
        {
            0x7B, 0x68, 0x75, 0x7C,
            0x6D, 0x7D, 0x66, 0x66
        };

        public readonly byte[] Separator =
        {
            0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00
        };

        public byte[] TreeNotation;

        public Coding(Node root)
        {
            GenerateTreeNotation(root);
        }

        private byte[] SplitIntoBytes(ulong sequence)
        {
            var byteArray = new byte[8];
            for (var i = 0; i < 8; i++)
            {
                var temp = sequence << (56 - i * 8);
                byteArray[i] = (byte) (temp >> 56);
            }

            return byteArray;
        }

        public void GenerateTreeNotation(Node root)
        {
            var preorderList = new List<ulong>();
            Tree.TraversePreorder(root, preorderList, 0, 0);
            TreeNotation = new byte[preorderList.Count * 8];
            for (var i = 0; i < preorderList.Count; i++)
            {
                var bytes = SplitIntoBytes(preorderList[i]);
                for (var j = 0; j < 8; j++) TreeNotation[i * 8 + j] = bytes[j];
            }
        }

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

        public byte? Encode(byte? readByte, ref ulong remainder, ref ulong remainderLength)
        {
            if (readByte != null)
            {
                var inputByteBitSequence = Tree.CodingCache[(byte) readByte, 0];
                var inputByteLength = Tree.CodingCache[(byte) readByte, 1];
                var inputByteLeadingZeroes = Tree.CodingCache[(byte) readByte, 2];

                //var temp = Reverse(inputByteBitSequence);
                inputByteBitSequence <<= (int)(remainderLength);

                remainder |= inputByteBitSequence;
                remainderLength += inputByteLength;
            }

            if (remainderLength < 8) return null;
            {
                var temp = remainder;
                remainder >>= 8;
                remainderLength -= 8;
                return (byte?) temp;
            }

        }

        public byte[] FinishCoding(byte[] buffer, int index, ulong remainder, ulong remainderLength)
        {
            byte[] returnVal;

            if (remainderLength != 0)
            {
                returnVal = new byte[index + 1];
                returnVal[index] = (byte) remainder;
            }
            else
            {
                returnVal = new byte[index];
            }

            for (var i = 0; i < index; i++) returnVal[i] = buffer[i];

            return returnVal;
        }
    }

    internal class Program
    {
        private static int Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Argument Error");
                return 0;
            }

            var inputProcessor = new InputProcessor(args[0]);
            var outputProcessor = new OutputProcessor(args[0]);
            var treeBuilder = new TreeBuilder();
            var priorityQueue = new PriorityQueue();

            int inputByte;
            while ((inputByte = inputProcessor.ReadByte()) > -1) treeBuilder.HashTable.Insert(inputByte);

            priorityQueue.InitQueue(treeBuilder.HashTable);

            var i = 0;
            while (priorityQueue.NodeList.Count > 1)
            {
                var innerNode = treeBuilder.JoinTwoNodes(priorityQueue.ExtractMin(), i);
                priorityQueue.QueueInsert(innerNode);
                i++;
            }

            var coding = new Coding(treeBuilder.GetRoot(priorityQueue));

            outputProcessor.WriteBuffer(coding.Header);
            outputProcessor.WriteBuffer(coding.TreeNotation);
            outputProcessor.WriteBuffer(coding.Separator);
            Tree.GetLeadingZeroes();
            Tree.ReverseCache();
            inputProcessor.SeekSet();

            i = 0;
            ulong remainder = 0, remainderLength = 0;
            var buffer = new byte[8];

            while ((inputByte = inputProcessor.ReadByte()) > -1)
            {
                var codedByte = coding.Encode((byte?) inputByte, ref remainder, ref remainderLength);
                if (codedByte == null) continue;
                buffer[i] = (byte) codedByte;
                i++;
                if (i == 8)
                {
                    outputProcessor.WriteBuffer(buffer);
                    Array.Clear(buffer, 0, 8);
                    i = 0;
                }

                while (remainderLength > 12)
                {
                    try
                    {
                        buffer[i] = (byte) coding.Encode(null, ref remainder, ref remainderLength);
                    }
                    catch
                    {
                        Console.WriteLine("This shouldn't have happened.");
                        throw;
                    }
                    i++;

                    if (i != 8) continue;
                    outputProcessor.WriteBuffer(buffer);
                    Array.Clear(buffer, 0, 8);
                    i = 0;
                }
            }

            buffer = coding.FinishCoding(buffer, i, remainder, remainderLength);
            outputProcessor.WriteBuffer(buffer);

            outputProcessor.CloseFile();

            return 0;
        }
    }
}