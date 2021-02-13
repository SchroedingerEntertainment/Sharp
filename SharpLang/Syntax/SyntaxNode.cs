// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE.SharpLang
{
    /// <summary>
    /// A syntax data node
    /// </summary>
    public class SyntaxNode
    {
        protected SyntaxNodeType type;
        /// <summary>
        /// The syntax token type of this Node
        /// </summary>
        public SyntaxNodeType Type
        {
            get 
            {
                return type;
            }
            internal set 
            {
                type = value;
            }
        }

        protected SyntaxNode child;
        /// <summary>
        /// A connection to the first child node
        /// </summary>
        public SyntaxNode Child
        {
            get 
            {
                return child;
            }
            internal set 
            {
                child = value;
            }
        }

        protected string buffer;
        /// <summary>
        /// An optional data buffer this Node contains
        /// </summary>
        public string Buffer
        {
            get 
            {
                return buffer;
            }
            set 
            {
                buffer = value;
            }
        }

        protected SyntaxNode next;
        /// <summary>
        /// A connection to the next sibling node
        /// </summary>
        public SyntaxNode Next
        {
            get 
            {
                return next;
            }
            internal set 
            {
                next = value;
            }
        }

        /// <summary>
        /// Creates a new empty syntax node
        /// </summary>
        internal SyntaxNode()
        { }

        /// <summary>
        /// Returns the contained raw data as .NET string value
        /// </summary>
        public override string ToString()
        {
            if (!string.IsNullOrWhiteSpace(buffer))
            {
                return string.Concat(type, "(", buffer, ")");
            }
            else return type.ToString();
        }
    }
}
