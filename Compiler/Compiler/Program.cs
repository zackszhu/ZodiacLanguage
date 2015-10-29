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
            StringLiteral CharLiteral = ZodiacTerminalFactory.CreateZodiacChar("CharLiteral");
            StringLiteral StringLiteral = ZodiacTerminalFactory.CreateZodiacString("StringLiteral");
            NumberLiteral Number = ZodiacTerminalFactory.CreateZodiacNumber("Number");
            IdentifierTerminal identifier = ZodiacTerminalFactory.CreateZodiacIdentifier("Identifier");

            CommentTerminal SingleLineComment = new CommentTerminal("SingleLineComment", "@@", "\r", "\n");
            CommentTerminal MutiLineComment = new CommentTerminal("MutiLineComment", "@{", "}@");
            NonGrammarTerminals.Add(SingleLineComment);
            NonGrammarTerminals.Add(MutiLineComment);

            //Symbols
            KeyTerm colon = ToTerm(":", "colon");
            KeyTerm semi = ToTerm(";", "semi");
            KeyTerm dot = ToTerm(".", "dot");
            KeyTerm comma = ToTerm(",", "comma");
            KeyTerm Lbr = ToTerm("{");
            KeyTerm Rbr = ToTerm("}");
            KeyTerm Lpar = ToTerm("(");
            KeyTerm Rpar = ToTerm(")");

            #endregion

            #region NonTerminals

            /* 4 Variables definitions */
            var variable_definition = new NonTerminal("variable_definition");
            var variable_default_definition = new NonTerminal("variable_default_definition");
            var variable_access = new NonTerminal("variable_access");
            var entire_variable = new NonTerminal("entire_variable");
            var component_variable = new NonTerminal("component_variable");
            var indexed_variable = new NonTerminal("indexed_variable");
            var constructor_variable = new NonTerminal("constructor_variable");
            var converter_variable = new NonTerminal("converter_variable");

            /* 6 operator */
            var operators = new NonTerminal("operator"); //operator
            var self_operator = new NonTerminal("self_operator");
            var pow_operator = new NonTerminal("pow_operator");
            var multiply_operator = new NonTerminal("multiply_operator");
            var add_operator = new NonTerminal("add_operator");
            var shift_operator = new NonTerminal("shift_operator");
            var compare_operator = new NonTerminal("compare_operator");
            var equal_operator = new NonTerminal("equal_operator");
            var bit_and_operator = new NonTerminal("bit_and_operator ");
            var bit_xor_operator = new NonTerminal("bit_xor_operator ");
            var bit_or_operator = new NonTerminal("bit_or_operator");
            var and_operator = new NonTerminal("and_operator");
            var or_operator = new NonTerminal("or_operator");
            
            /* 7 Expressions */
            var expression = new NonTerminal("expression");


            /* 8 Statements */
            var statement = new NonTerminal("statement");
            var simple_statement = new NonTerminal("simple_statement");
            var structed_statement = new NonTerminal("structed_statement");
            var loop_statement = new NonTerminal("loop_statement");
            var break_statement = new NonTerminal("break_statement");
            var continue_statement = new NonTerminal("continue_statement");
            var ret_statement = new NonTerminal("ret_statement");
            #endregion

            /* Rule */

            /* 4 Variables definitions */
            variable_access.Rule = 
                entire_variable | 
                component_variable | 
                constructor_variable | 
                converter_variable;
            /* 4.2 Entire-Variables */
            entire_variable.Rule = identifier;


            /* 7 Expressions */


            /* 8 Statements */
            statement.Rule = 
                simple_statement |
                structed_statement |
                loop_statement |
                ret_statement;
            /* 8.2 Simple-statements */
            /* 8.3 Structed-statements */
            /* 8.4 Loop-statements */
            break_statement.Rule = "break" + semi;
            continue_statement.Rule = "continue" + semi;
            /* 8.5 Ret-statements */
            
        }

    }
}
