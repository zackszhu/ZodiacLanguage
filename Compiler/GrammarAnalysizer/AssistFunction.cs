using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrammarAnalysizer {
    class AssistFunction {
        public static Type GetNumberType(string text) {
            try {
                int.Parse(text);
                return typeof (int);
            }
            catch (Exception) {
                return typeof (double);
            }
        }
    }
}
