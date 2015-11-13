using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Parsing;

namespace Compiler
{
    public static class ZodiacTerminalFactory {
        public static NumberLiteral CreateZodiacNumber(string name) {
            NumberLiteral term = new NumberLiteral(name);
            term.DefaultIntTypes = new TypeCode[] { TypeCode.Int32, TypeCode.UInt32 };
            //term.DefaultFloatType = TypeCode.Double; //default
            return term;
        }

        public static StringLiteral CreateZodiacChar(string name) {
            StringLiteral term = new StringLiteral(name, "'", StringOptions.IsChar);
            return term;
        }

        public static StringLiteral CreateZodiacString(string name) {
            StringLiteral term = new StringLiteral(name);
            //term.AddStartEnd("'", StringOptions.AllowsAllEscapes);// AllowLineBreak??
            term.AddStartEnd("\"", StringOptions.AllowsAllEscapes);// AllowLineBreak??
            return term;
        }

        public static IdentifierTerminal CreateZodiacIdentifier(string name) {
            IdentifierTerminal id = new IdentifierTerminal(name, IdOptions.None); 
            return id;
        }
    }
}
