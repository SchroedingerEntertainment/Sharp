// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SE.Json
{
    /// <summary>
    /// A DOM document in JSON format
    /// </summary>
    public partial class JsonDocument
    {
        protected List<JsonNode> nodes;
        /// <summary>
        /// The first node in this Document
        /// </summary>
        public JsonNode Root
        {
            get
            {
                if (nodes.Count > 0) return nodes[0];
                else return null;
            }
        }

        /// <summary>
        /// Determines the amount of nodes embedded in this document
        /// </summary>
        public int Count
        {
            get { return nodes.Count; }
        }

        protected Parser parser;

        /// <summary>
        /// A collection of error messages since last document parsing
        /// </summary>
        public IEnumerable<string> Errors
        {
            get
            {
                if (parser == null) return null;
                else return parser.Errors;
            }
        }

        /// <summary>
        /// Determines if the document has parser errors
        /// </summary>
        public bool HasErrors
        {
            get
            {
                if (parser != null) return (parser.Errors.Count > 0);
                else return false;
            }
        }

        /// <summary>
        /// Creates a new JSON DOM document
        /// </summary>
        public JsonDocument()
        {
            this.nodes = new List<JsonNode>();
        }

        /// <summary>
        /// Adds a new root node to this Document
        /// </summary>
        /// <param name="type">Defines the kind of node that should be added</param>
        /// <returns>The newly created node if successfull, null otherwise</returns>
        public JsonNode AddNode(JsonNodeType type)
        {
            if (nodes.Count > 0 || type > JsonNodeType.Array)
                return null;

            JsonNode node = new JsonNode();
            node.Type = type;

            nodes.Add(node);
            return node;
        }
        /// <summary>
        /// Adds a new child node to an element in this Document
        /// </summary>
        /// <param name="root">The newly created childs parent</param>
        /// <param name="type">Defines the kind of node that should be added</param>
        /// <returns>The newly created node if successfull, null otherwise</returns>
        public JsonNode AddNode(JsonNode root, JsonNodeType type)
        {
            if (root == null)
                return null;

            JsonNode node = new JsonNode();
            node.Type = type;

            AddChild(root, node);

            nodes.Add(node);
            return node;
        }
        /// <summary>
        /// Appends an existing node as child node to an element in this Document
        /// </summary>
        /// <param name="root">The appended childs parent</param>
        /// <param name="node">An existing node to append</param>
        public void AddAppend(JsonNode root, JsonNode node)
        {
            if (root == null)
            {
                if(Root == null) nodes.Add(node);
                return;
            }

            AddChild(root, node);
            nodes.Add(node);
        }

        void AddChild(JsonNode root, JsonNode child)
        {
            if (root.Child == null) root.Child = child;
            else
            {
                JsonNode prevChild = root.Child;
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

        /// <summary>
        /// Removes existing nodes from the root level and sets the provided node as
        /// root
        /// </summary>
        /// <param name="node">A node from the root level of this DOM</param>
        /// <returns>True if the provided root could successfully set, false otherwise</returns>
        public bool Rebase(JsonNode node)
        {
            JsonNode root = Root;
            if (root == null || node == null || root == node)
                return false;

            JsonNode child = root.Child;
            while (child != null)
            {
                if (child == node)
                {
                    break;
                }
                else child = child.Next;
            }
            if (child == null)
            {
                return false;
            }
            else
            {
                root = root.Child;
                while (root != null)
                {
                    JsonNode next = root.Next;
                    if (root != node)
                    {
                        RemoveNode(root);
                    }
                    root = next;
                }
                node.Next = nodes[0].Next;
                nodes[0].Child = null;
                nodes[0].Next = null;

                nodes.Remove(node);
                nodes[0] = node;
                return true;
            }
        }

        /// <summary>
        /// Removes an existing node and all child nodes attached to it from
        /// this Document
        /// </summary>
        /// <param name="node">An existing node to be removed</param>
        /// <returns>True if the node was removed successfully, false otherwise</returns>
        public bool RemoveNode(JsonNode node)
        {
            if (node == null) return false;

            /**
             Unlink node from DOM layer
            */
            for (int i = 0; i < nodes.Count; i++)
                if (nodes[i].Next == node)
                {
                    nodes[i].Next = node.Next;
                    break;
                }

            if (node.Type <= JsonNodeType.Array)
            {
                while (node.Child != null)
                {
                    RemoveNode(node.Child);
                    node.Child = node.Child.Next;
                }
            }

            nodes.Remove(node);
            return true;
        }
        /// <summary>
        /// Removes all nodes from this Document at once
        /// </summary>
        public void Clear()
        {
            nodes.Clear();
        }

        /// <summary>
        /// Tries to load this Document's content from a given stream
        /// </summary>
        /// <param name="stream">The stream to load content from</param>
        /// <returns>True if content was successfully parsed, false otherwise</returns>
        public virtual bool Load(Stream stream, Encoding encoding)
        {
            Clear();

            if(parser == null) parser = new Parser(this);
            return parser.Parse(stream, encoding, true);
        }
        /// <summary>
        /// Tries to load this Document's content from a given stream
        /// </summary>
        /// <param name="stream">The stream to load content from</param>
        /// <returns>True if content was successfully parsed, false otherwise</returns>
        public virtual bool Load(Stream stream)
        {
            return Load(stream, null);
        }

        /// <summary>
        /// Tries to save this Document's content to a given stream
        /// </summary>
        /// <param name="stream">The stream to save content to</param>
        /// <returns>True if content was successfully saved, false otherwise</returns>
        public virtual bool Save(Stream stream, Encoding encoding, bool formatted = false)
        {
            JsonNode root = Root;
            if (root == null)
                return false;

            root.Name = null;
            using (StreamWriter sw = new StreamWriter(stream, encoding))
            {
                Serialize(sw, formatted, root);
            }
            return true;
        }
        /// <summary>
        /// Tries to save this Document's content to a given stream
        /// </summary>
        /// <param name="stream">The stream to save content to</param>
        /// <returns>True if content was successfully saved, false otherwise</returns>
        public virtual bool Save(Stream stream, bool formatted = false)
        {
            return Save(stream, Encoding.UTF8, formatted);
        }

        void Serialize(StreamWriter sw, bool formatted, JsonNode node, int indentation = 0)
        {
            if (formatted)
            {
                sw.Write(string.Empty.PadLeft(indentation));
            }
            if (node.Name != null)
            {
                sw.Write("\"{0}\"", node.Name.Replace("\"", "\\\""));
                if (formatted)
                {
                    sw.Write(" : ");
                }
                else sw.Write(':');
            }
            switch (node.Type)
            {
                case JsonNodeType.Empty:
                    {
                        sw.Write("null");
                    }
                    break;
                case JsonNodeType.Object:
                    {
                        sw.Write('{');
                        if (node.Child != null)
                        {
                            if (formatted)
                            {
                                sw.WriteLine();
                            }
                            Serialize(sw, formatted, node.Child, indentation + 4);
                            if (formatted)
                            {
                                sw.WriteLine();
                                sw.Write(string.Empty.PadLeft(indentation));
                            }
                        }
                        sw.Write('}');
                    }
                    break;
                case JsonNodeType.Array:
                    {
                        sw.Write('[');
                        if (node.Child != null)
                        {
                            if (formatted)
                            {
                                sw.WriteLine();
                            }
                            Serialize(sw, formatted, node.Child, indentation + 4);
                            if (formatted)
                            {
                                sw.WriteLine();
                                sw.Write(string.Empty.PadLeft(indentation));
                            }
                        }
                        sw.Write(']');
                    }
                    break;
                case JsonNodeType.Bool:
                    {
                        sw.Write(node.ToBoolean().ToString().ToLowerInvariant());
                    }
                    break;
                case JsonNodeType.Integer:
                    {
                        sw.Write(node.ToInteger().ToString(System.Globalization.CultureInfo.InvariantCulture));
                    }
                    break;
                case JsonNodeType.Decimal:
                    {
                        sw.Write(node.ToDecimal().ToString(System.Globalization.CultureInfo.InvariantCulture));
                    }
                    break;
                case JsonNodeType.String:
                    {
                        sw.Write("\"{0}\"", node.ToString().Replace("\"", "\\\""));
                    }
                    break;
            }
            if (node.Next != null)
            {
                if (formatted)
                {
                    sw.WriteLine(',');
                }
                else sw.Write(',');
                Serialize(sw, formatted, node.Next, indentation);
            }
        }
    }
}
