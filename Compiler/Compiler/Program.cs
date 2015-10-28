using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Parsing;

namespace Compiler {
    public class ZodiacGrammar : Grammar {
        public ZodiacGrammar() {

            #region Lexical
            StringLiteral stringLiteral = TerminalFactory.CreatePythonString("stringLiteral");
            #endregion

        }

    }
}
