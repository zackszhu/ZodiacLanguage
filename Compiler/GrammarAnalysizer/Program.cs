using System;

namespace Zodiac {
    class Program {
        [STAThread]
        static void Main(string[] args) {
            var grammarAnalysizer = new GrammarAnalysizer();
            string code = "var a = 1 + 1;var io = IO; io.write(a);";
            grammarAnalysizer.ParseSample(code);
            grammarAnalysizer.ShowParseTree();
            code = Console.ReadLine();
        }
    }
}
