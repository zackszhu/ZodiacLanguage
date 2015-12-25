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
            //string code = System.IO.File.ReadAllText("../../../test/while_statement.zs");
            string code = System.IO.File.ReadAllText(args[0]);
            //string code = "static func getAB : long { param a = long; param b = long; param c = long; return b;} var i = getAB(1,2,3);";
            // string code = "var i = 111; var j = i.ToString()[2];";
            grammarAnalysizer.ParseSample(code);

            if (grammarAnalysizer.ParseTree.Root == null)
            {
                var s = grammarAnalysizer.ParseTree.ParserMessages;
                foreach ( var error in s)
                {
                    Console.WriteLine("("+error.Location.Line+","+error.Location.Column+")\t" + error.ToString());
                }
                return;
            }
            //grammarAnalysizer.ShowParseTree();
            codeGenerator.Generate(grammarAnalysizer.ParseTree);
            
        }
    }
    
}
