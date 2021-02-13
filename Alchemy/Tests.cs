using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using SE.Alchemy;

namespace Alchemy.Test
{
    public class Tests
    {
        private static double total;
        private static int count;

        public static void Run()
        {
            total = 0;
            count = 0;

            //Macros

            Run("token", "token");
            Run("#", "");
            Run("#define X Y\nX", "Y");
            Run("#define X (2 * Y)\n#define Y (4 + X)\nX", "(2 * (4 + X))");
            Run("#define X " + @"to\" + "\n" + "ken" + "\nX", "token");

            Run("#disable\n#define X", "#define X");
            Run("#define X Y\n#disable\nX\n#enable\nX", "X\nY");

            Run("#define X(x) token x other\nX(and one).", "token and one other.");
            Run("#define X(x,y) token x and y\nX(1, 2).", "token 1 and 2.");
            Run("#define INC(x) (x + 2)\nINC", "INC");
            Run("#define TEST(x) x\\\n?\nTEST(A)", "A?");

            Run("#define A B C D\nA", "B C D");
            Run("#define A B ## D\nA", "BD");
            Run("#define X # ## #\nX", "##");
            Run("#define F(A) A\n#define B 22\nF(B)", "22");
            Run("#define F(A) A ## 1\n#define B 22\n#define B1 C\nF(B)", "C");
            Run("#define F(A) #A\n#define B 22\nF(B)", "\"B\"");
            Run("#define F(A,B) in_between(A hash_hash B)\n#define hash_hash # ## #\n#define mkstr(a) # a\n#define in_between(a) mkstr(a)\nF(x, y)", "\"x ## y\"");
            Run("#define F(A) <A>\nF(x)", "<x>");
            Run("#define F(A,B) <A,B>\nF(x,y) + 1", "<x,y> + 1");
            Run("#define F(A,B) A ## B\n#define C D\nF(c,C)", "cC");
            Run("#define F(A,B,C) <A,B,C>\nF(x,y,z)", "<x,y,z>");

            Run("#define F(...) <VA_ARGS__>\nF(x)", "<x>");
            Run("#define F(...) <#VA_ARGS__>\nF(x)", "<\"x\">");
            Run("#define F(...) <VA_ARGS__>\nF(x,y)", "<x,y>");
            Run("#define F(...) <#VA_ARGS__>\nF(x,y)", "<\"x,y\">");
            Run("#define F(A,...) <A,VA_ARGS__>\nF(x,y)", "<x,y>");
            Run("#define F(A,...) <A, VA_ARGS__>\nF(x,y)", "<x, y>");
            Run("#define F(A,...) <A, VA_ARGS__>\nF(x,y, z)", "<x, y, z>");
            Run("#define F(A,...) <A, #VA_ARGS__>\nF(x,y, z)", "<x, \"y, z\">");

            Run("#define X list of tokens\nX", "list of tokens");
            Run("#define LST list\n#define TOKS tokens\n#define LOTS LST of TOKS\nLOTS LOTS", "list of tokens list of tokens");
            Run("#define LOTS LST of TOKS\n#define LST list\n#define TOKS tokens\nLOTS LOTS", "list of tokens list of tokens");
            Run("#define FUNC(x) arg=x.\nFUNC(var) FUNC(2)", "arg=var. arg=2.");
            Run("#define FUNC(x,y,z) int n = z+y*x;\nFUNC(1,2,3)", "int n = 3+2*1;");
            Run("#define X 20\n#define FUNC(x,y) x+y\nx=FUNC(X,Y);", "x=20+Y;");
            Run("#define FA(x,y) FB(x,y)\n#define FB(x,y) x + y\nFB(1,2);", "1 + 2;");
            Run("#define PRINTF(...) printf(VA_ARGS__)\nPRINTF()", "printf()");
            Run("#define PRINTF(...) printf(VA_ARGS__)\nPRINTF(\"hello\")", "printf(\"hello\")");
            Run("#define PRINTF(...) printf(VA_ARGS__)\nPRINTF(\"%d\", 1)", "printf(\"%d\", 1)");
            Run("#define PRINTF(FORMAT, ...) printf(FORMAT, VA_ARGS__)\nPRINTF(\"test\")", "printf(\"test\", )");
            Run("#define PRINTF(FORMAT, ...) printf(FORMAT, VA_ARGS__)\nPRINTF(\"test %s\", \"hello\")", "printf(\"test %s\", \"hello\")");
            Run("#define PRINTF(FORMAT, ...) printf(FORMAT, VA_ARGS__)\nPRINTF(\"test %s %d\", \"hello\", 1)", "printf(\"test %s %d\", \"hello\", 1)");

            //Compiler Extension (, ## VA_ARGS__)
            Run("#define PRINTF(FORMAT, ...) printf(FORMAT, ## VA_ARGS__)\nPRINTF(\"test\")", "printf(\"test\")");

            Run("#define INC(x) (x + 2)\nINC(\n1\n)", "(1 + 2)");

            Run("#define STRINGIZE(ARG) #ARG\nSTRINGIZE(+=)", "\"+=\"");
            Run("#define STRINGIZE(ARG) #ARG\nSTRINGIZE(:>)", "\":>\"");
            Run("#define STRINGIZE(ARG) #ARG\nSTRINGIZE(3.1415)", "\"3.1415\"");

            Run("#define CONCAT(X, Y) X ## Y\nCONCAT(3, .14159)", "3.14159");
            Run("#define CONCAT(X, Y) X ## #Y\nCONCAT(u, hello)", "u\"hello\"");
            Run("#define CONCAT(X, ...) X ## VA_ARGS__\nCONCAT(hello) there", "hello there");
            Run("#define CONCAT(X, ...) X ## VA_ARGS__\nCONCAT(hello, there)", "hellothere");
            Run("#define A _a_\n#define B _b_\n#define AB _ab_\n#define C(a,b) a##b\n#define D(a,b) C(a,b)\nC(A,B)D(A,B)", "_ab__a__b_");

            Run("#define A MACRO\nA", "MACRO");
            Run("#define A MACRO\n#undef A\nA", "A");
            Run("#define A(X) MACRO X\nA(x)", "MACRO x");
            Run("#define A(X) MACRO X\n#undef A\nA(x)", "A(x)");

            Run("#define REGISTER_NAME(num,name) NAME_##name = num,\nREGISTER_NAME(201, TRUE)", "NAME_TRUE = 201,");
            Run("#define FUNC_2(X) VALUE=X\n#define FUNC_N(X) FUNC_##X\n#define OBJECT FUNC_N(2)\nOBJECT(1234)", "VALUE=1234");

            //Compiler Extension (, ## VA_ARGS__)
            Run("#define FUNC(fmt,...) (fmt, ## VA_ARGS__)\nFUNC(a)", "(a)");
            Run("#define FUNC(fmt,...) (fmt, ## VA_ARGS__)\nFUNC(a,b)", "(a,b)");
            Run("#define FUNC(fmt,...) (fmt, ## VA_ARGS__)\nFUNC(a,b )", "(a,b)");
            Run("#define FUNC(fmt, ...) (fmt, ## VA_ARGS__)\nFUNC(a)", "(a)");
            Run("#define FUNC(fmt, ...) (fmt, ## VA_ARGS__)\nFUNC(a,b)", "(a,b)");
            Run("#define FUNC(fmt, ...) (fmt, ## VA_ARGS__)\nFUNC(a,b )", "(a,b)");

            Run("#define EMPTY_TOKEN\n#define FUNC(FuncName, HType1, HArg1) FuncName(HType1 HArg1);\nFUNC(hello, EMPTY_TOKEN, int)", "hello( int);");
            Run("#define FUNC(x) Value = x\n#define PP_JOIN(x, y) x ## y\n#define RESULT(x) PP_JOIN(FU, NC)(x)\nRESULT(1234)", "Value = 1234");
            Run("#define VALUE 1234\n#define PP_JOIN(x, y) x ## y\n#define RESULT PP_JOIN(V, ALUE)\nRESULT", "1234");

            Run("#define EMPTY_TOKEN\n#define FUNC(x) (x)\nFUNC(EMPTY_TOKEN A)", "( A)");
            Run("#define EMPTY_TOKEN\n#define FUNC(x,y) (x y)\nFUNC(EMPTY_TOKEN,A)", "( A)");

            Run("#define EMPTY\n#define FUNC(x) (x)\nFUNC(x    y)", "(x    y)");
            Run("#define EMPTY\n#define FUNC(x) (x)\nFUNC(x    EMPTY    y)", "(x    y)");
            Run("#define EMPTY\n#define FUNC(x) (x)\nFUNC(x    EMPTY    y    EMPTY)", "(x    y    )");
            Run("#define EMPTY\n#define FUNC(x) (x)\nFUNC(EMPTY    x    EMPTY    y)", "(    x    y)");
            Run("#define EMPTY\n#define FUNC(x) (x)\nFUNC(    EMPTY    x    EMPTY    y)", "(    x    y)");
            Run("#define EMPTY\n#define FUNC(x) (x)\nFUNC(EMPTY    x    EMPTY    y    )", "(    x    y)");

            Run("#define EMPTY\n#define FUNC(x) (x)\n#define FUNC_2(x) FUNC(x)\nFUNC_2(x    y)", "(x    y)");
            Run("#define EMPTY\n#define FUNC(x) (x)\n#define FUNC_2(x) FUNC(x)\nFUNC_2(x    EMPTY    y)", "(x    y)");
            Run("#define EMPTY\n#define FUNC(x) (x)\n#define FUNC_2(x) FUNC(x)\nFUNC_2(x    EMPTY    y    EMPTY)", "(x    y)");
            Run("#define EMPTY\n#define FUNC(x) (x)\n#define FUNC_2(x) FUNC(x)\nFUNC_2(EMPTY    x    EMPTY    y)", "(x    y)");
            Run("#define EMPTY\n#define FUNC(x) (x)\n#define FUNC_2(x) FUNC(x)\nFUNC_2(    EMPTY    x    EMPTY    y)", "(x    y)");
            Run("#define EMPTY\n#define FUNC(x) (x)\n#define FUNC_2(x) FUNC(x)\nFUNC_2(EMPTY    x    EMPTY    y    )", "(x    y)");

            Run("#define EMPTY\n#define FUNC(x) ( x )\nFUNC(EMPTY EMPTY)", "( )");
            Run("#define EMPTY\n#define FUNC(x) ( x)\nFUNC(EMPTY EMPTY x)", "( x)");
            Run("#define EMPTY\n#define FUNC(x,y) ( x y)\n#define FUNC2(x,y) FUNC(x,y)\nFUNC2(EMPTY EMPTY EMPTY, x)", "( x)");
            Run("#define EMPTY\n#define DOUBLE_EMPTY EMPTY EMPTY\n#define FUNC(x) ( x)\nFUNC(DOUBLE_EMPTY x)", "(   x)");
            Run("#define EMPTY\n#define DOUBLE_EMPTY EMPTY EMPTY\n#define FUNC(x,y) ( x y )\n#define FUNC2(x,y) FUNC(x,y)\nFUNC2(DOUBLE_EMPTY EMPTY, x)", "( x )");
            Run("#define EMPTY\n#define FUNC(x,y) ( x y )\nFUNC(EMPTY EMPTY EMPTY EMPTY EMPTY EMPTY, x)", "( x )");

            Run("#define NON_MEMBER_CALL(FUNC, CV_REF_OPT) FUNC(X,CV_REF_OPT)\n#define NON_MEMBER_CALL_CV(FUNC, REF_OPT) NON_MEMBER_CALL(FUNC, REF_OPT) NON_MEMBER_CALL(FUNC, const REF_OPT)\n#define NON_MEMBER_CALL_CV_REF(FUNC) NON_MEMBER_CALL_CV(FUNC, ) NON_MEMBER_CALL_CV(FUNC, &)\n#define IS_FUNCTION(X,CV_REF_OPT) (CV_REF_OPT)\nNON_MEMBER_CALL_CV_REF(IS_FUNCTION)", "() (const) (&) (const &)");

            //Compiler Extension (, ## VA_ARGS__)
            Run("#define TEXT(x) L ## x\n#define checkf(expr, format, ...) AssertFailed(#expr, format, ##VA_ARGS__)\ncheckf( true, TEXT( \"hello world\" ) );", "AssertFailed(\"true\", L\"hello world\");");

            //Logic

            Run("#if 3 > 3\nYes\n#else\nNo\n#endif", "No");
            Run("#if 3 >= 3\nYes\n#endif", "Yes");
            Run("#define ONE 1\n#define TWO 2\n#define PLUS(x, y) x ## y\n#if PLUS(ONE, TWO) > 12\nYes\n#endif\nNo", "No");
            Run("#define UNPACK(x, y) x ## y\n#define ONE 1\n#define TWO 2\n#define PLUS(x, y) UNPACK(x, y)\n#if PLUS(ONE, TWO) >= 12\nYes\n#elsePLUS(ONE, TWO)\n#endif", "Yes");
            Run("#define ONE 1\n#if ONE\nOne\n#elif TWO\nTwo\n#else\nThree\n#endif", "One");
            Run("#define TWO 1\n#if ONE\nOne\n#elif TWO\nTwo\n#else\nThree\n#endif", "Two");

            Console.WriteLine("{0} tests completed in {1}ms (~{2}ms per test)", count, total, total / count);
            Console.Read();
        }
        private static void Run(string input, string expected)
        {
            string result;

            using (MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(input)))
            using (ProcessorStream stream = new ProcessorStream(ms, Encoding.ASCII))
            using (StreamReader sr = new StreamReader(stream))
            {
                ms.Position = 0;

                Stopwatch sw = new Stopwatch();
                sw.Start();
                {
                    result = sr.ReadToEnd().Trim();
                }
                sw.Stop();
                total += sw.Elapsed.TotalMilliseconds;
                count++;

                Debug.Assert(stream.Errors.Count == 0);
            }
            Debug.Assert(result.Equals(expected));
        }
    }
}