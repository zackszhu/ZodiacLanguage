using System;

namespace Zodiac
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            GrammarAnalysizer grammarAnalysizer = new GrammarAnalysizer();
            CodeGenerator codeGenerator = new CodeGenerator();

            //string code = "var i,console = long(1*1+1), IO; console.write(i+1);";
            //string code = " var i,j = 1+1+1+1, 1+2+3; ";
            // string code = " var i = long(1); ";
            //string code = System.IO.File.ReadAllText("../../test/cast.zs");
            string code = System.IO.File.ReadAllText("../../test/qsort_primitive.zs");

            //string code = "static func getAB : long { param a = long; param b = long; param c = long; return b;} var i = getAB(1,2,3);";
           // string code = "var i = 111; var j = i.ToString()[2];";
            grammarAnalysizer.ParseSample(code);

            if (grammarAnalysizer.ParseTree.Root == null)
            {
                Console.WriteLine("bad code");
                return;
            }
            //grammarAnalysizer.ShowParseTree();
            codeGenerator.Generate(grammarAnalysizer.ParseTree);
            //code = Console.ReadLine();
            
            /*
            var i = 10;
            var j = 10;
            Console.WriteLine("i:"+(int)&i+"j:"+(int)&j);
            i = 100000000;
            j = i;
            Console.WriteLine("i:" + (int)&i + "j:" + (int)&j);
            */
        }
    }
    
}
