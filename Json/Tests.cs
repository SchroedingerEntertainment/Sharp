// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.IO;
using SE.Json;

namespace Json.Test
{
    public class Tests
    {
        private static double total;
        private static int count;

        public static void Run()
        {
            total = 0;
            count = 0;

            Run("{}", 1);
            Run("[]", 1);
            Run("{  }{}", 1);
            Run("[  ][]", 1);

            Run("[\"Hello World\"]", 2);
            Run("[\"Hello World\", 0, -7e+2, null]", 5);

            Run("{\"Message\":\"Hello World\"}", 2);
            Run("{\"Message\":\"Hello World\",\"X\":0,\"Y\":-7e-1,\"item1\":true,\"item2\":false}", 6);

            Console.WriteLine("{0} tests completed in {1}ms (~{2}ms per test)", count, total, total / count);
            Console.Read();
        }
        private static void Run(string input, int finalNodeCount)
        {
            JsonDocument jdoc = new JsonDocument();
            using (MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(input)))
            {
                ms.Position = 0;

                Stopwatch sw = new Stopwatch();
                sw.Start();
                {
                    Debug.Assert(jdoc.Load(ms));
                }
                sw.Stop();
                total += sw.Elapsed.TotalMilliseconds;
                count++;

                Debug.Assert(jdoc.Count == finalNodeCount);
                Debug.Assert(!jdoc.HasErrors);
            }
        }
    }
}