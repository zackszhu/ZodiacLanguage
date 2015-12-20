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
            string code = " var i = long(1); ";
            //string code = "static func getAB : long { param a = long; param b = long; return a,b;} var i = 2; var j = 3; var a, b = getAB();";
            grammarAnalysizer.ParseSample(code);
            //grammarAnalysizer.ShowParseTree();
            codeGenerator.Generate(grammarAnalysizer.ParseTree);
            code = Console.ReadLine();
            
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
