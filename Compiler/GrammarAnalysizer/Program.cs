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
            //string code = System.IO.File.ReadAllText("../../../test/basic_type.zs");
            string code = System.IO.File.ReadAllText(args[0]);
            grammarAnalysizer.ParseSample(code);
            if (grammarAnalysizer.ParseTree.Root == null)
            {
                var s = grammarAnalysizer.ParseTree.ParserMessages;
                foreach ( var error in s)
                {
                    Console.WriteLine("("+(error.Location.Line+1)+","+error.Location.Column+")\t" + error.ToString());
                }
                return;
            }
            codeGenerator.Generate(grammarAnalysizer.ParseTree);
            
        }
    }
    
}
