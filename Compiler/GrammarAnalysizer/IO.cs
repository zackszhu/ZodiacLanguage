using System;

namespace GrammarAnalysizer {
    public class IO {
        public static void writeln(object arg) {
            Console.WriteLine(arg.ToString());
        }

        public static int readln() {
            var s = Console.ReadLine();
            return int.Parse(s);
        }
    }
}
