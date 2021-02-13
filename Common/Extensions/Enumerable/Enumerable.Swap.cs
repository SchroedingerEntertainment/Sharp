// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace System.Collections.Generic
{
    public static partial class EnumerableExtension
    {
        /// <summary>
        /// Swaps two items at the given position with each other
        /// </summary>
        /// <returns>The modified collection</returns>
        public static IEnumerable<T> Swap<T>(this IEnumerable<T> items, int lhs, int rhs)
        {
            if(lhs < 0) throw new ArgumentOutOfRangeException();
            T onHold = default(T);

            IEnumerator<T> enumerator = items.GetEnumerator();
            for (int i = 0; lhs != rhs && enumerator.MoveNext(); i++)
            {
                if (lhs == i)
                {
                    onHold = enumerator.Current;
                    i++;

                    for (; enumerator.MoveNext(); i++)
                        if (i == rhs)
                        {
                            yield return enumerator.Current;
                            break;
                        }

                    if (i != rhs)
                        throw new ArgumentOutOfRangeException();

                    enumerator.Reset();
                    i = 0;

                    for (; enumerator.MoveNext(); i++)
                        if (i == lhs)
                            break;
                }
                else if (rhs == i)
                {
                    yield return onHold;
                    break;
                }
                else yield return enumerator.Current;
            }
            while (enumerator.MoveNext())
                yield return enumerator.Current;
        }
    }
}
