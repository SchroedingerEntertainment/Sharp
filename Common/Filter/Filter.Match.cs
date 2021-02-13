// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace System.Text
{
    public partial class Filter
    {
        /// <summary>
        /// Determines if a sequence of strings matches this Filter. The sequence will
        /// be reversed while processing
        /// </summary>
        /// <param name="components">A set of string components to check</param>
        /// <returns>True if the whole sequence matches this Filter, false otherwise</returns>
        public bool IsMatch(params string[] components)
        {
            Array.Reverse(components);

            int i = 0;
            FilterToken token = GetMatch(Root, components, ref i);
            return (token != null);
        }

        /// <summary>
        /// Processes a match up to the left-most token that suceeded last
        /// </summary>
        /// <param name="currentToken">The token to start matching the sequence</param>
        /// <param name="components">A set of string components to check</param>
        /// <param name="componentIndex">An index to the component matched successfully</param>
        /// <returns>The token that last matched the sequence or null</returns>
        public FilterToken GetMatch(FilterToken currentToken, string[] components, ref int componentIndex)
        {
            FilterToken match = null;
            FilterToken current = currentToken;
            while (current != null)
            {
                bool isMatch = false;
                int index = componentIndex;
                if (current.Child != null)
                {
                    FilterToken token = GetMatch(current.Child, components, ref index);
                    if (token != null)
                    {
                        index++;
                        match = token;
                        isMatch = true;
                    }
                    else
                    {
                        while (current.Next != null && current.Type == FilterType.And)
                            current = current.Next;

                        if (current != null)
                            current = current.Next;

                        match = null;
                        continue;
                    }
                }

                if (index >= components.Length)
                {
                    if (isMatch)
                        isMatch = (current.Pattern == SequenceStart);
                }
                else
                {
                    bool tmp = current.IsMatch(components[index]);
                    isMatch = (tmp && !current.Exclude || !tmp && current.Exclude);
                }

                if (isMatch)
                {
                    if (match == null)
                        match = current;

                    componentIndex = index;

                    if (current.Type != FilterType.And)
                        break;
                }
                else
                {
                    while (current.Next != null && current.Type == FilterType.And)
                        current = current.Next;

                    match = null;
                }
                if (current != null)
                    current = current.Next;
            }
            return match;
        }
    }
}
