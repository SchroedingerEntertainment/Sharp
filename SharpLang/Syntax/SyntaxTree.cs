// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE.SharpLang
{
    /// <summary>
    /// A tree like structure of syntax nodes
    /// </summary>
    public partial class SyntaxTree
    {
        protected List<SyntaxNode> nodes;
        /// <summary>
        /// The first node in this tree
        /// </summary>
        public SyntaxNode Root
        {
            get
            {
                if (nodes.Count > 0) return nodes[0];
                else return null;
            }
        }

        /// <summary>
        /// Determines the amount of nodes embedded in this tree
        /// </summary>
        public int Count
        {
            get { return nodes.Count; }
        }

        /// <summary>
        /// Creates a new tree like collection of syntax nodes
        /// </summary>
        public SyntaxTree()
        {
            this.nodes = new List<SyntaxNode>();
        }

        /// <summary>
        /// Adds a new root node to this tree
        /// </summary>
        /// <param name="type">Defines the kind of node that should be added</param>
        /// <returns>The newly created node</returns>
        public SyntaxNode AddNode(SyntaxNodeType type)
        {
            SyntaxNode node = new SyntaxNode();
            node.Type = type;

            if (Root != null)
                AddSibling(Root, node);

            nodes.Add(node);
            return node;
        }
        /// <summary>
        /// Adds a new child node to an element in this tree
        /// </summary>
        /// <param name="root">The newly created childs parent</param>
        /// <param name="type">Defines the kind of node that should be added</param>
        /// <returns>The newly created node</returns>
        public SyntaxNode AddNode(SyntaxNode root, SyntaxNodeType type)
        {
            if (root == null)
                return null;

            SyntaxNode node = new SyntaxNode();
            node.Type = type;

            AddChild(root, node);

            nodes.Add(node);
            return node;
        }
        /// <summary>
        /// Appends an existing node as child node to an element in this tree
        /// </summary>
        /// <param name="root">The appended childs parent</param>
        /// <param name="node">An existing node to append</param>
        public void AddAppend(SyntaxNode root, SyntaxNode node)
        {
            if (root == null)
            {
                if(Root == null) nodes.Add(node);
                return;
            }

            AddChild(root, node);
            nodes.Add(node);
        }

        void AddChild(SyntaxNode root, SyntaxNode child)
        {
            if (root.Child == null) root.Child = child;
            else
            {
                SyntaxNode prevChild = root.Child;
                for (; ; )
                {
                    if (prevChild.Next != null) prevChild = prevChild.Next;
                    else
                    {
                        prevChild.Next = child;
                        break;
                    }
                }
            }
        }
        void AddSibling(SyntaxNode root, SyntaxNode next)
        {
            if (root.Next == null) root.Next = next;
            else
            {
                SyntaxNode prevSibling = root.Next;
                for (; ; )
                {
                    if (prevSibling.Next != null) prevSibling = prevSibling.Next;
                    else
                    {
                        prevSibling.Next = next;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Removes an existing node and all child nodes attached to it from
        /// this tree
        /// </summary>
        /// <param name="node">An existing node to be removed</param>
        /// <returns>True if the node was removed successfully, false otherwise</returns>
        public bool RemoveNode(SyntaxNode node)
        {
            if (node == null)
                return false;

            /**
             Unlink node from tree layer
            */
            for (int i = 0; i < nodes.Count; i++)
                if (nodes[i].Next == node)
                {
                    nodes[i].Next = node.Next;
                    break;
                }

            while (node.Child != null)
            {
                RemoveNode(node.Child);
                node.Child = node.Child.Next;
            }

            nodes.Remove(node);
            return true;
        }
        /// <summary>
        /// Removes all nodes from this tree at once
        /// </summary>
        public void Clear()
        {
            nodes.Clear();
        }
    }
}
