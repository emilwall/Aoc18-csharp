using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day8
{
    class Program
    {
        static readonly string[] Input = File.ReadAllLines("input.txt");

        static void Main(string[] args)
        {
            var tree = CreateTree();
            var metadataSum = SumMetadata(tree);
            Console.WriteLine("Part 1: " + metadataSum);

            int value = CalculateNodeValue(tree);
            Console.WriteLine("Part 2: " + value);
        }

        private class Node
        {
            public Node[] Children { get; set; }
            public int[] Metadata { get; set; }
        }

        private static Node CreateTree()
        {
            var tokens = Input.First().Split(' ').Select(int.Parse).ToList();
            var numberOfChildren = tokens[0];
            var numberOfMetadata = tokens[1];
            tokens.RemoveRange(0, 2);
            return CreateSubtree(tokens, numberOfChildren, numberOfMetadata);
        }

        private static Node CreateSubtree(List<int> tokens, int numberOfChildren, int numberOfMetadata)
        {
            var children = new Node[numberOfChildren];
            for (var i = 0; i < children.Length; i++)
            {
                var grandChildCount = tokens[0];
                var childMetadataCount = tokens[1];
                tokens.RemoveRange(0, 2);
                children[i] = CreateSubtree(tokens, grandChildCount, childMetadataCount);
            }

            var metadata = Enumerable.Range(0, numberOfMetadata).Select(i => tokens[i]).ToArray();
            tokens.RemoveRange(0, numberOfMetadata);

            return new Node
            {
                Children = children,
                Metadata = metadata
            };
        }

        private static int SumMetadata(Node tree)
        {
            return tree.Children.Sum(SumMetadata) + tree.Metadata.Sum();
        }

        private static int CalculateNodeValue(Node tree)
        {
            if (!tree.Children.Any())
            {
                return tree.Metadata.Sum();
            }

            var childIndices = tree.Metadata.Where(i => i > 0 && i <= tree.Children.Length).ToArray();
            var children = childIndices.Select(i => tree.Children[i - 1]);
            var sum = children.Sum(CalculateNodeValue);
            return sum;
        }
    }
}
