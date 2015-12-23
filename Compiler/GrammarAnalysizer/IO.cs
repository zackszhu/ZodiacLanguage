using System;

namespace GrammarAnalysizer {
    public class IO {
        public void writeln(object arg) {
            Console.WriteLine(arg.ToString());
        }

        public int readln() {
            return int.Parse(s: Console.ReadLine());
        }
    }
}
