using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace Huffman
{
    class TreeBuilder
    {
        private Dictionary<char, int> _dictionary;
        private List<KeyValuePair<char, int>> _dictInListForm;

        public TreeBuilder()
        {
            _dictionary = new Dictionary<char, int>();
        }
        public void AddToDictionary(char readByte)
        {
            if (_dictionary.ContainsKey(readByte))
                _dictionary[readByte]++;
            else
                _dictionary.Add(readByte, 1);
        }
        public void ConvertToList()
        {
            _dictInListForm = _dictionary.OrderBy(d => d.Value).ToList();
        }
        public KeyValuePair<KeyValuePair<char, int>, KeyValuePair<char, int>> GetNodes()
        {
            if (_dictInListForm.Count > 1)
            {
                KeyValuePair<char, int> firstPair = _dictInListForm[0];
                _dictInListForm.RemoveAt(0);

                KeyValuePair<char, int> secondPair = _dictInListForm[0];
                _dictInListForm.RemoveAt(0);

                return new KeyValuePair<KeyValuePair<char, int>, KeyValuePair<char, int>>(firstPair, secondPair);
            }
            else
            {
                return new KeyValuePair<KeyValuePair<char, int>, KeyValuePair<char, int>>();
            }

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

            while (inputProcessor.ReadByte() > -1)
            {
                treeBuilder.
            }
                    //Console.WriteLine(counter);
                    WordProcessing.PrintDictionary();
                


            return (0);
        }
    }
}