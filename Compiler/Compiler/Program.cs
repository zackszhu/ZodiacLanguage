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

            StringLiteral charLiteral = ZodiacTerminalFactory.CreateZodiacChar("CharLiteral");
            StringLiteral stringLiteral = ZodiacTerminalFactory.CreateZodiacString("StringLiteral");
            NumberLiteral number = ZodiacTerminalFactory.CreateZodiacNumber("Number");
            IdentifierTerminal identifier = ZodiacTerminalFactory.CreateZodiacIdentifier("Identifier");

            CommentTerminal singleLineComment = new CommentTerminal("SingleLineComment", "@@", "\r", "\n");
            CommentTerminal mutiLineComment = new CommentTerminal("MutiLineComment", "@{", "}@");
            NonGrammarTerminals.Add(singleLineComment);
            NonGrammarTerminals.Add(mutiLineComment);

            //Symbols
            KeyTerm colon = ToTerm(":", "colon");
            KeyTerm semi = ToTerm(";", "semi");
            KeyTerm dot = ToTerm(".", "dot");
            KeyTerm comma = ToTerm(",", "comma");
            KeyTerm Lbr = ToTerm("{");
            KeyTerm Rbr = ToTerm("}");
            KeyTerm Lbra = ToTerm("[");
            KeyTerm Rbra = ToTerm("]");
            KeyTerm Lpar = ToTerm("(");
            KeyTerm Rpar = ToTerm(")");

            #endregion Lexical

            #region 
            /* 2 Type declarations and definitions */
            var required_type = new NonTerminal("required_type");
            var simple_type = new NonTerminal("simple_type");
            var list_type = new NonTerminal("list_type");
            var func_type = new NonTerminal("func_type");
            var type_declaration = new NonTerminal("type_declaration");
            var type_identifier = new NonTerminal("type_identifier");
            var type_definition = new NonTerminal("type_definition");
            var structed_type = new NonTerminal("structed_type");
            var type_member = new NonTerminal("type_member");
            var member_variable = new NonTerminal("member_variable");
            var member_function = new NonTerminal("member_function");

            /* 3 Constructors and Convertor */
            var constructor = new NonTerminal("constructor");
            var default_constructor = new NonTerminal("default_constructor");
            var required_type_default_constructor = new NonTerminal("required_type_default_constructor");
            var structed_type_default_constructor = new NonTerminal("structed_type_default_constructor");
            var structed_type_identifier = new NonTerminal("structed_type_identifier");
            var defined_constructor = new NonTerminal("defined_constructor");
            var required_type_defined_constructor = new NonTerminal("required_type_defined_constructor");
            var simple_type_defined_constructor = new NonTerminal("simple_type_defined_constructor");
            var list_type_defined_constructor = new NonTerminal("list_type_defined_constructor");
            var range_parameter = new NonTerminal("range_parameter");
            var indexed_parameter = new NonTerminal("indexed_parameter");
            var fill_parameter = new NonTerminal("fill_parameter");
            var index = new NonTerminal("index");
            var converter = new NonTerminal("converter");
            var inherit_converter = new NonTerminal("inherit_converter");
            var ansc_converter = new NonTerminal("ansc_converter");
            var desc_converter = new NonTerminal("desc_converter");
          
            /* 4 Variables definitions */
            var variable_definition = new NonTerminal("variable_definition");
            var variable_default_definition = new NonTerminal("variable_default_definition");
            var variable_access = new NonTerminal("variable_access");
            var variable_identifier = new NonTerminal("variable_identifier");
            var entire_variable = new NonTerminal("entire_variable");
            var component_variable = new NonTerminal("component_variable");
            var indexed_variable = new NonTerminal("indexed_variable");
            var array_variable = new NonTerminal("array_variable");
            var index_expression = new NonTerminal("index_expression");
            var field_designator = new NonTerminal("field_designator");
            var record_variable = new NonTerminal("record_variable");
            var field_identifier = new NonTerminal("field_identifier");
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
            var parenthesized_expression = new NonTerminal("parenthesized_expression");
            var bin_op_expression = new NonTerminal("bin_op_expression");
            var self_expression = new NonTerminal("self_expression");
            var term = new NonTerminal("term");
            var function_designator = new NonTerminal("function_designator");

            /* 8 Statements */
            var statement = new NonTerminal("statement");
            var simple_statement = new NonTerminal("simple_statement");
            var structed_statement = new NonTerminal("structed_statement");
            var loop_statement = new NonTerminal("loop_statement");
            var break_statement = new NonTerminal("break_statement");
            var continue_statement = new NonTerminal("continue_statement");
            var ret_statement = new NonTerminal("ret_statement");

            /* 9 Scope */
            var scope = new NonTerminal("scope");
            var scope_body = new NonTerminal("scope_body");
            var declaration = new NonTerminal("declaration");
            var definition = new NonTerminal("definition");

            #endregion NonTerminals

            #region Operator
            RegisterOperators(-1, "=");
            RegisterOperators(1, "||");
            RegisterOperators(2, "&&");
            RegisterOperators(3, "|");
            RegisterOperators(4, "^");
            RegisterOperators(5, "&");
            RegisterOperators(6, "==", "!=");
            RegisterOperators(7, "<", ">", "<=", ">=");
            RegisterOperators(8, "<<", ">>");
            RegisterOperators(9, "+", "-");
            RegisterOperators(10, "*", "/", "%");
            RegisterOperators(11, "^^");

            #endregion Operator





            /* Rule */

            /* 4 Variables definitions */
            variable_access.Rule = 
                entire_variable | 
                component_variable | 
                constructor_variable | 
                converter_variable;
            /* 4.2 Entire-Variables */
            variable_identifier.Rule = identifier;
            entire_variable.Rule = variable_identifier;
            /* 4.3 Component-Variables */
            component_variable.Rule = indexed_variable | field_designator;
            indexed_variable.Rule = array_variable + Lbra + index_expression + Rbra;
            index_expression.Rule = expression;
            field_designator.Rule = record_variable + dot + field_identifier;
            record_variable.Rule = variable_access;
            field_identifier.Rule = identifier;
            /* 4.4 Field-designators */
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
            expression.Rule = parenthesized_expression| term | bin_op_expression;
            parenthesized_expression.Rule = Lpar + expression + Rpar;
            term.Rule = variable_access | parenthesized_expression | function_designator;

            /* 7.3 Function-designators */
            function_designator.Rule = function_identifier;

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
