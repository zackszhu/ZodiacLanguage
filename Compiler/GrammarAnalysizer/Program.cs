using System;

namespace Zodiac {
    class Program {
        [STAThread]
        static void Main(string[] args) {
            var grammarAnalysizer = new GrammarAnalysizer();
            grammarAnalysizer.load();
            //string code = Console.ReadLine();
           //grammarAnalysizer.ParseSample(code);
           //grammarAnalysizer.ShowParseTree();
        }
    }
}
