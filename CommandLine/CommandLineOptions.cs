// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SE.CommandLine
{
    /// <summary>
    /// Manages a set of command line arguments
    /// </summary>
    public partial class CommandLineOptions
    {
        /// <summary>
        /// The global instance of the command line arguments loaded
        /// </summary>
        public readonly static CommandLineOptions Default;

        readonly Dictionary<string, HashSet<CommandLineToken>> properties;

        protected List<CommandLineToken> tokens;
        /// <summary>
        /// The first option in this collection
        /// </summary>
        public CommandLineToken First
        {
            get
            {
                if (tokens.Count > 0) return tokens[0];
                else return null;
            }
        }

        CommandLineFlags flags;
        /// <summary>
        /// A set of configuration flags passed to the parser
        /// </summary>
        public CommandLineFlags Flags
        {
            get { return flags; }
            set { flags = value; }
        }

        static CommandLineOptions()
        {
            Default = new CommandLineOptions(CommandLineFlags.AllowVerbValues);
        }
        /// <summary>
        /// Creates a new arguments cache from the provided parser flags
        /// </summary>
        /// <param name="flags">Configuration flags passed to the parser</param>
        public CommandLineOptions(CommandLineFlags flags)
        {
            this.flags = flags;
            this.tokens = new List<CommandLineToken>();
            this.properties = new Dictionary<string, HashSet<CommandLineToken>>();
        }

        /// <summary>
        /// Adds an auto generated property to the head of this collection
        /// </summary>
        public void GenerateProperty()
        {
            CommandLineToken root = First;
            if (root == null || root.Type != CommandLineTokenType.ParserGenerated)
            {
                CommandLineToken token = new CommandLineToken(this, CommandLineTokenType.ParserGenerated);
                token.Next = root;

                tokens.Insert(0, token);
            }
        }

        /// <summary>
        /// Adds a new token to the end of this collection
        /// </summary>
        /// <param name="type">Defines the kind of token that should be added</param>
        /// <returns>The newly created token if successfull, null otherwise</returns>
        public CommandLineToken AddToken(CommandLineTokenType type)
        {
            if (tokens.Count == 0)
            {
                CommandLineToken token = new CommandLineToken(this, type);
                tokens.Add(token);
                return token;
            }
            else
            {
                CommandLineToken root = First;
                while (root.Next != null)
                    root = root.Next;

                return AddToken(root, type);
            }
        }
        /// <summary>
        /// Adds a new child token to an existing token in this collection
        /// </summary>
        /// <param name="root">The newly created tokens predecessor</param>
        /// <param name="type">Defines the kind of token that should be added</param>
        /// <returns>The newly created token if successfull, null otherwise</returns>
        public CommandLineToken AddToken(CommandLineToken root, CommandLineTokenType type)
        {
            if (root == null)
                return null;

            CommandLineToken token = new CommandLineToken(this, type);

            AddSibling(root, token);

            tokens.Add(token);
            return token;
        }

        void AddSibling(CommandLineToken root, CommandLineToken successor)
        {
            if (root.Next == null) root.Next = successor;
            else
            {
                CommandLineToken prevNext = root.Next;
                successor.Next = prevNext;
                root.Next = successor;
            }
        }

        /// <summary>
        /// Adds an enumeration into the command line options collection
        /// </summary>
        /// <param name="args">The input enumerator to parse</param>
        public void Load(IEnumerable<string> args)
        {
            CommandLineParser parser = new CommandLineParser(this);
            StringBuilder plainTextTokenBuffer = null;
            using (MemoryStream data = new MemoryStream())
            using (CommandLineParser.ParserContext context = parser.BeginParse(data))
            {
                IEnumerator<string> blocks = args.GetEnumerator();
                while (blocks.MoveNext())
                {
                    switch (parser.BuilderState.Current)
                    {
                        #region Parse
                        default:
                            {
                                parser.BuilderState.Reset();

                                data.SetLength(0);
                                data.Write(Encoding.UTF8.GetBytes(blocks.Current));
                                data.Position = 0;

                                while (!parser.EndOfStream && parser.BuilderState.Current < CommandLineParserState.PlainText)
                                {
                                    context.ParseNext();
                                }
                                if (parser.BuilderState.Current == CommandLineParserState.PlainText)
                                {
                                    plainTextTokenBuffer = new StringBuilder();
                                    plainTextTokenBuffer.Append(new StreamReader(data, Encoding.UTF8).ReadToEnd());
                                }
                            }
                            break;
                        #endregion

                        #region After PlainText delimiter (--)
                        case CommandLineParserState.PlainText:
                            {
                                plainTextTokenBuffer.Append(" ");
                                plainTextTokenBuffer.Append(blocks.Current);
                            }
                            break;
                        #endregion
                    }
                }
            }
            if (plainTextTokenBuffer != null)
            {
                AddToken(CommandLineTokenType.PlainText).Value = plainTextTokenBuffer.ToString();
            }
        }

        internal void Invalidate(CommandLineToken token, object cachedData)
        {
            if (token.Type == CommandLineTokenType.Property)
            {
                string id;
                if (cachedData != null)
                {
                    id = cachedData.ToString();
                    HashSet<CommandLineToken> tokens; if (properties.TryGetValue(id, out tokens))
                    {
                        tokens.Remove(token);
                        if (tokens.Count == 0)
                        {
                            properties.Remove(id);
                        }
                    }
                }
                id = token.Value as string;
                if (id != null)
                {
                    HashSet<CommandLineToken> tokens; if (!properties.TryGetValue(id, out tokens))
                    {
                        tokens = new HashSet<CommandLineToken>();
                        properties.Add(id, tokens);
                    }
                    tokens.Add(token);
                }
            }
        }

        /// <summary>
        /// Removes an existing token from the collection
        /// </summary>
        /// <param name="token">An existing token to be removed</param>
        /// <returns>True if the token was removed successfully, false otherwise</returns>
        public bool RemoveNode(CommandLineToken token)
        {
            if (token == null) return false;

            /**
             Unlink node from DOM layer
            */
            for (int i = 0; i < tokens.Count; i++)
                if (tokens[i].Next == token)
                {
                    tokens[i].Next = token.Next;
                    break;
                }

            tokens.Remove(token);
            return true;
        }
        /// <summary>
        /// Removes all tokens from this collection at once
        /// </summary>
        public void Clear()
        {
            tokens.Clear();
        }
    }
}
