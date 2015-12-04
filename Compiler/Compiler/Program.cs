using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler {

    [Language("Zodiac", "1.0", "Course Language")]
    public class ZodiacGrammar : Grammar {

        public ZodiacGrammar() {
            #region Lexical

            StringLiteral charLiteral = ZodiacTerminalFactory.CreateZodiacChar("charLiteral");
            StringLiteral stringLiteral = ZodiacTerminalFactory.CreateZodiacString("stringLiteral");
            NumberLiteral number = ZodiacTerminalFactory.CreateZodiacNumber("number");
            IdentifierTerminal identifier = ZodiacTerminalFactory.CreateZodiacIdentifier("identifier");

            CommentTerminal singleLineComment = new CommentTerminal("singleLineComment", "@@", "\r", "\n");
            CommentTerminal mutiLineComment = new CommentTerminal("mutiLineComment", "@{", "}@");
            NonGrammarTerminals.Add(singleLineComment);
            NonGrammarTerminals.Add(mutiLineComment);
            var comment = new NonTerminal("comment");
            comment.Rule = singleLineComment | mutiLineComment;

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
          //  var func_type = new NonTerminal("func_type");
            //var type_declaration = new NonTerminal("type_declaration");
            var type_definition = new NonTerminal("type_definition");
            var structed_type = new NonTerminal("structed_type");
            var basic_type = new NonTerminal("basic_type");
            var type_identifier_list = new NonTerminal("type_identifier_list");
            var type_member = new NonTerminal("type_member");
            var member_variable = new NonTerminal("member_variable");
            var member_function = new NonTerminal("member_function");
            var member_family_opt = new NonTerminal("family option");

            /* 3 Constructors and Convertor */
            var constructor = new NonTerminal("constructor");
            var default_constructor = new NonTerminal("default_constructor");
            var required_type_default_constructor = new NonTerminal("required_type_default_constructor");
            var structed_type_default_constructor = new NonTerminal("structed_type_default_constructor");
            var structed_type_identifier = new NonTerminal("structed_type_identifier");
            var defined_constructor = new NonTerminal("defined_constructor");
            var required_type_defined_constructor = new NonTerminal("required_type_defined_constructor");
            var structed_type_defined_constructor = new NonTerminal("structed_type_defined_constructor");
            var simple_type_defined_constructor = new NonTerminal("simple_type_defined_constructor");
            var list_type_defined_constructor = new NonTerminal("list_type_defined_constructor");
            var range_parameter = new NonTerminal("range_parameter");
            var range = new NonTerminal("range");
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
           // var function_declaration = new NonTerminal("function_declaration");
           //  var function_normal_declaration = new NonTerminal("normal_function_declaration");
           //  var function_operator_declaration = new NonTerminal("operator_function_declaration");
            var function_option = new NonTerminal("function_option");

            var function_definition = new NonTerminal("function_definition");
            var function_normal_definition = new NonTerminal("normal_function_definition");
            var function_operator_definition = new NonTerminal("operator_function_definition");


            var function_body = new NonTerminal("function_body");
            var function_parameter_block = new NonTerminal("function_parameter_block");
            var function_parameter_list_opt = new NonTerminal("function_parameter_list_opt");
            var function_parameter_list = new NonTerminal("function_parameter_list");
            var function_parameter = new NonTerminal("function_parameter");
            var function_parameter_default_list_opt = new NonTerminal("function_parameter_default_list_opt");
            var function_parameter_default_list = new NonTerminal("function_parameter_default_list");
            var function_parameter_default = new NonTerminal("function_parameter_default");
            var function_instruction_block = new NonTerminal("function_instruction_block");
            var function_scope_body = new NonTerminal("function_scope_body");

            /* 6 operator */
            var operators = new NonTerminal("operator"); //operator
            var unary_operator = new NonTerminal("unary_operator");
            var pow_operator = new Terminal("pow_operator");
            var multiply_operator = new Terminal("multiply_operator");
            var add_operator = new Terminal("add_operator");
            var shift_operator = new Terminal("shift_operator");
            var compare_operator = new Terminal("compare_operator");
            var equal_operator = new Terminal("equal_operator");
            var bit_and_operator = new Terminal("bit_and_operator ");
            var bit_xor_operator = new Terminal("bit_xor_operator ");
            var bit_or_operator = new Terminal("bit_or_operator");
            var and_operator = new Terminal("and_operator");
            var or_operator = new Terminal("or_operator");
            var bin_operator = new NonTerminal("bin_operator");
            var assignment_operator = new NonTerminal("assignment_operator");
            var assignment_value_operator = new NonTerminal("assignment_value_operator");
            var assignment_reference_operator = new NonTerminal("assignment_reference_operator");

            /* 7 Expressions */
            var expression = new NonTerminal("expression");
            var parenthesized_expression = new NonTerminal("parenthesized_expression");
            var bin_op_expression = new NonTerminal("bin_op_expression");
            var unary_expression = new NonTerminal("unary_expression");
            var primary_expression = new NonTerminal("primary_expression");
            var list_expression = new NonTerminal("list_expression");
            var list_normal_expression = new NonTerminal("list_normal_expression");
            var list_select_expression = new NonTerminal("list_select_expression");
            var list_string_expression = new NonTerminal("list_string_expression");
            
            var term = new NonTerminal("term");//TODO
            var literal = new NonTerminal("literal");
            var expression_list = new NonTerminal("expression_list");

            /* 8 Statements */
            var statement = new NonTerminal("statement");
            var simple_statement = new NonTerminal("simple_statement");
            var variable_definition_statement = new NonTerminal("variable_definition_statement");
            var assignment_statement = new NonTerminal("assignment_statement");
            var access_statement = new NonTerminal("access_statement");
            var structed_statement = new NonTerminal("structed_statement");
            var if_statement = new NonTerminal("if_statement");
            var else_part_opt = new NonTerminal("else_part_opt");
            var else_part = new NonTerminal("else_part");
            var while_statement = new NonTerminal("while_statement");
            var for_statement = new NonTerminal("for_statement");
            var loop_statement = new NonTerminal("loop_statement");
            var break_statement = new NonTerminal("break_statement");
            var continue_statement = new NonTerminal("continue_statement");
            var ret_statement = new NonTerminal("ret_statement");
            var return_statement = new NonTerminal("return_statement");
            var escape_statement = new NonTerminal("escape_statement");

            /* 9 Scope */
            var scope = new NonTerminal("scope");
            var scope_body = new NonTerminal("scope_body");
            var scope_body_opt = new NonTerminal("scope_body_opt");
            var statement_list = new NonTerminal("statement_list");
            //var declaration = new NonTerminal("declaration");
            var definition = new NonTerminal("definition");

            /* 11 Programs */
            var program = new NonTerminal("program");
            var program_heading = new NonTerminal("program_heading");
            var have_sequence_list = new NonTerminal("program_heading");
            var have_sequence = new NonTerminal("program_heading");

            #endregion NonTerminals

            RegisterOperators(-1, "=", ":=");
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
            RegisterOperators(11, Associativity.Right, "^^");

            this.MarkPunctuation(";", ",", "(", ")", "{", "}", "[", "]", ":");
            // this.MarkTransient(type_definition, statement, expression, bin_operator, expression);
            this.MarkTransient(statement, expression, bin_operator, expression);

            var identifier_ext = new NonTerminal("identifier_ext");
            var identifier_list = new NonTerminal("identifier_list");
            var member_access_list = new NonTerminal("member_access_list");
            var member_access = new NonTerminal("member_access");
            var member_access_segments_opt = new NonTerminal("member_access_segments_opt");
            var member_access_segment = new NonTerminal("member_access");
            var array_indexer = new NonTerminal("array_indexer");
            var argument = new NonTerminal("argument");
            var argument_list_par = new NonTerminal("argument_list_par");
            var argument_list_par_opt = new NonTerminal("argument_list_par_opt");
            var argument_list = new NonTerminal("argument_list");
            var argument_list_opt = new NonTerminal("argument_list_opt");

            /* Basic */
            /* identifier */
            identifier_ext.Rule = identifier | required_type;  //??
            identifier_list.Rule = MakePlusRule(identifier_list, comma, identifier);
            /* member_access*/
            member_access_list.Rule = MakePlusRule(member_access, comma, member_access);

            member_access.Rule = identifier_ext + member_access_segments_opt;

            member_access_segments_opt.Rule = MakeStarRule(member_access_segments_opt, null, member_access_segment);
            member_access_segment.Rule = ( dot + identifier )
                                       | array_indexer
                                       | argument_list_par;
           


            array_indexer.Rule = "[" + expression + "]";

            /* arguments */
            argument_list.Rule = expression;
            argument_list_par.Rule = Lpar + argument_list_opt + Rpar;
            argument_list_par_opt.Rule = Empty | argument_list_par;
            argument_list.Rule = MakePlusRule(argument_list, comma, expression);
            argument_list_opt.Rule = Empty | argument_list;

            /* Rule */
            /* 2 Type declarations and definitions */
            /* 2.1 Required-types */
            required_type.Rule = simple_type | list_type /*| func_type*/;
            simple_type.Rule = ToTerm("long") | "real" | "bool" | "char" ;
            list_type.Rule = ToTerm("list");
           // func_type.Rule = ToTerm("func");

            /* 2.1 Type-declarations */
            //type_declaration.Rule = ToTerm("type") + identifier + semi;

            /* 2.2 Type-definitions */
            basic_type.Rule = (ToTerm("<-") + identifier) | Empty;
            type_definition.Rule = ToTerm("type") + identifier + basic_type + Lbr + structed_type + Rbr; //TODO
                                                                                                         //type_basic.Rule
                                                                                                         // type_identifier_list.Rule = MakePlusRule(type_identifier_list, comma, identifier);
            structed_type.Rule = MakeStarRule(structed_type, null, type_member);
            type_member.Rule = member_variable | member_function;
            member_variable.Rule = ToTerm("var") + identifier + assignment_reference_operator + required_type + semi;
            member_family_opt.Rule = ToTerm("family") | Empty;

            member_function.Rule = member_family_opt + function_definition;

            /* 3 Constructors and Converters */
            constructor.Rule = defined_constructor | default_constructor;
            /* 3.2 Default-Constructors */
            default_constructor.Rule = required_type_default_constructor | structed_type_default_constructor;
            required_type_default_constructor.Rule = required_type;
            structed_type_default_constructor.Rule = identifier;
            /* 3.3 Defined-Constructors */
            defined_constructor.Rule = required_type_defined_constructor | structed_type_default_constructor;
            required_type_defined_constructor.Rule = simple_type_defined_constructor | list_type_defined_constructor;
            simple_type_defined_constructor.Rule = simple_type + Lpar + expression + Rbr;
            list_type_defined_constructor.Rule = list_type + (range_parameter | indexed_parameter);
            range_parameter.Rule = Lpar + range + comma + range_parameter + Rpar;
            range.Rule = expression;
            indexed_parameter.Rule = Lbra + index + comma + index + Rbra + (range_parameter | fill_parameter);
            fill_parameter.Rule = Lpar + expression + Rpar;
            index.Rule = expression;
            structed_type_defined_constructor.Rule = structed_type_identifier + Lpar + Rpar;//------
            /* 3.4 Converter */
            converter.Rule = constructor | inherit_converter;
            inherit_converter.Rule = ansc_converter | desc_converter;
            ansc_converter.Rule = ToTerm("ansc") + Lpar + identifier + Rpar;
            desc_converter.Rule = ToTerm("desc") + Lpar + identifier + Rpar;

            /* 4 Variables definitions */
            variable_definition.Rule = ToTerm("var") + identifier_list + assignment_operator + expression_list;

            variable_access.Rule = member_access;
            //    converter_variable;
            /* 4.2 Entire-Variables */
            // entire_variable.Rule = identifier;
            /* 4.3 Component-Variables */
            //component_variable.Rule = indexed_variable | field_designator;
            //indexed_variable.Rule = array_variable + Lbra + index_expression + Rbra;
            //array_variable.Rule = variable_access;
            //index_expression.Rule = expression;
            //field_designator.Rule = record_variable + dot + field_identifier;
            //record_variable.Rule = variable_access;
            //field_identifier.Rule = identifier;
            /* 4.4 Field-designators */
            //constructor_variable.Rule = constructor;
            //converter_variable.Rule = converter;
            /* 5 Function declarations and definition */
            

            function_option.Rule = "static" | Empty;

            function_definition.Rule = function_option + function_normal_definition | function_operator_definition;

            function_normal_definition.Rule =  "func" + identifier + Lbr + function_body + Rbr;
            function_operator_definition.Rule = "oper" + bin_operator + Lbr + function_body + Rbr;  

            function_body.Rule = function_parameter_block + function_instruction_block;
            function_parameter_block.Rule = function_parameter_list | function_parameter_default_list | (function_parameter_list + function_parameter_default_list) | Empty;
            //function_parameter_list_opt.Rule = function_parameter_list | Empty;
            function_parameter_list.Rule = MakePlusRule(function_parameter_list, null, function_parameter);
            function_parameter.Rule = "param" + identifier_list + assignment_operator + required_type + semi;
            //function_parameter_default_list_opt.Rule = function_parameter_default_list | Empty;
            function_parameter_default_list.Rule = MakePlusRule(function_parameter_default_list, null, function_parameter_default);
            function_parameter_default.Rule = "param" + identifier_list + assignment_operator + required_type + argument_list_par + semi;
            function_instruction_block.Rule = function_scope_body | Empty;
            function_scope_body.Rule = scope_body;

            /* 6 operator */
            operators.Rule =
                unary_operator |
                pow_operator |
                bin_operator;
            unary_operator.Rule = ToTerm("!") | "~" | "+" | "-" ;

            bin_operator.Rule = ToTerm("^^") | "*" | "/" | "%" | "+" | "-" | "<<" | ">>" | "<" | ">" | "==" | "&"
                | "^" | "|" | "&&" | "||";

            assignment_value_operator.Rule = ToTerm(":=");
            assignment_reference_operator.Rule = ToTerm("=");
            assignment_operator.Rule = assignment_value_operator | assignment_reference_operator;

            /* 7 Expressions */

            expression.Rule = primary_expression | bin_op_expression;
            parenthesized_expression.Rule = Lpar + expression + Rpar;
            bin_op_expression.Rule = expression + bin_operator + expression;
            unary_expression.Rule = unary_operator + primary_expression;
            list_expression.Rule = list_normal_expression | list_select_expression | list_string_expression;
            list_normal_expression.Rule = ToTerm("[") + expression_list + "]";
           // list_select_expression.Rule = ToTerm("asdsadasdsa");
            list_select_expression.Rule = ToTerm("from") + identifier + "in" + expression + "where" + expression + ToTerm("select") + identifier;
            list_string_expression.Rule = ToTerm("\"") + stringLiteral + "\"";


            primary_expression.Rule = literal
                | unary_expression
                | parenthesized_expression
                | member_access
                | list_expression ;

            literal.Rule = number | stringLiteral | charLiteral | "True" | "False" | "Null";

            expression_list.Rule = MakePlusRule(expression_list, comma, expression);

            /* 8 Statements */
            statement.Rule =
                simple_statement |
                structed_statement |
                loop_statement |
                ret_statement;
            /* 8.2 Simple-statements */
            simple_statement.Rule = assignment_statement | variable_definition_statement | function_definition /*| function_declaration */| type_definition | access_statement ;

            variable_definition_statement.Rule = variable_definition + semi;
            assignment_statement.Rule = member_access_list + assignment_operator + expression_list + semi;

            access_statement.Rule = member_access + semi;
            
            /* 8.3 Structed-statements */
            structed_statement.Rule = if_statement | while_statement | for_statement;
            if_statement.Rule = ToTerm("if") + Lpar + expression + Rpar + scope + else_part_opt;
            else_part_opt.Rule = else_part | Empty;
            else_part.Rule = ToTerm("else") + scope;
            while_statement.Rule = ToTerm("while") + Lpar + expression + Rpar + scope;
            for_statement.Rule = ToTerm("for") + identifier + "in" + member_access + scope;
            /* 8.4 Loop-statements */
            loop_statement.Rule = break_statement | continue_statement;
            break_statement.Rule = "break" + semi;
            continue_statement.Rule = "continue" + semi;
            /* 8.5 Ret-statements */
            ret_statement.Rule = return_statement | escape_statement;
            return_statement.Rule = ToTerm("return") + argument_list_opt + semi;
            escape_statement.Rule = ToTerm("escape") + identifier_list + semi;

            /* 9 Scope */
            scope.Rule = Lbr + scope_body_opt + Rbr;
            scope_body_opt.Rule = scope_body | Empty;
            /* 9.2 Scope-bodys */
            //scope_body.Rule = declaration | definition | statement | comment;
            scope_body.Rule = statement_list;
            statement_list.Rule = MakePlusRule(statement_list, null, statement);
            //declaration.Rule = function_declaration;
            definition.Rule = function_definition | type_definition;

            /* 11 Program */
            program.Rule = program_heading + scope_body;
            program_heading.Rule = have_sequence_list | Empty;
            have_sequence_list.Rule = MakePlusRule(have_sequence_list, comma, have_sequence);
            have_sequence.Rule = ToTerm("have") + identifier + ".d" ;

            this.Root = program;

            #region Define Keywords

            this.MarkReservedWords("long", "real", "char", "bool", "list", "func", "oper", "var", "param", "return", "escape", "type", "family", "static", "True"
                , "False", "if", "else", "for in", "while", "break", "continue", "from", "where", "select", "ance", "desc", "IO", "have", "Null");

            #endregion
        }
    }
}