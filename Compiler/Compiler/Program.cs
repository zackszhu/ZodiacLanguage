using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            #endregion Lexical

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

            /* 5 Function declarations and definitions */
            var function_declaration = new NonTerminal("function_declaration");
            var function_identifier = new NonTerminal("function_identifier");
            var function_option = new NonTerminal("function_option");
            var function_definiition = new NonTerminal("function_definition");
            var function_body = new NonTerminal("function_body");
            var function_parameter_block = new NonTerminal("function_parameter_block");
            var function_parameters = new NonTerminal("function_parameters");
            var function_parameter = new NonTerminal("function_parameter");
            var function_parameters_default = new NonTerminal("function_parameters_default");
            var function_parameter_default = new NonTerminal("function_parameter_default");
            var function_instruction_block = new NonTerminal("function_instruction_block");
            var function_scope_body = new NonTerminal("function_scope_body");

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

            #endregion NonTerminals

            /* Rule */

            /* 4 Variables definitions */
            variable_access.Rule = 
                entire_variable | 
                component_variable | 
                constructor_variable | 
                converter_variable;
            /* 4.2 Entire-Variables */
            entire_variable.Rule = identifier;

            /* 5 Function declarations and definition */
            function_declaration.Rule = function_option + "func" + function_identifier + semi;
            function_identifier.Rule = identifier;
            function_option.Rule = "static";
            function_definiition.Rule = function_option + "func" + function_identifier + Lbr + function_body + Rbr;
            function_body.Rule = function_parameter_block + function_instruction_block;
            function_parameter_block.Rule = function_parameters + function_parameters_default;
            function_parameters.Rule = MakeStarRule(function_parameters, function_parameter);
            function_parameter.Rule = "param" + variable_default_definition + semi;
            function_parameters_default.Rule = MakeStarRule(function_parameters_default, function_parameter_default);
            function_parameter_default.Rule = "param" + variable_definition + semi;
            function_instruction_block.Rule = MakePlusRule(function_instruction_block, function_scope_body);
            function_scope_body.Rule = scope_body;
            /* 6 operator */
            operators.Rule = 
                self_operator |
                pow_operator |
                multiply_operator |
                add_operator |
                shift_operator |
                compare_operator |
                equal_operator |
                bit_and_operator |
                bit_xor_operator |
                bit_or_operator |
                and_operator |
                or_operator;
            self_operator.Rule = ToTerm("!") | "~" | "+" | "-";
            pow_operator.Rule = ToTerm("^^");
            multiply_operator.Rule = ToTerm("*");
            add_operator.Rule = ToTerm("+") | "-";
            shift_operator.Rule = ToTerm("<<") | ">>";
            compare_operator.Rule = ToTerm("<") | ">";
            equal_operator.Rule = ToTerm("==");
            bit_and_operator.Rule = ToTerm("&");
            bit_xor_operator.Rule = ToTerm("^");
            bit_or_operator.Rule = ToTerm("|");
            and_operator.Rule = ToTerm("&&");
            or_operator.Rule = ToTerm("||");

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
