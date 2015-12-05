using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Irony.Parsing;
using TriAxis.RunSharp;


namespace Zodiac
{
    class ILGenerator
    {
        public void Generate(ParseTree parseTree)
        {
            string name = "ZodiacConsole";
            string exeDir = string.Empty;
            string exeFilePath = string.Empty;
            exeDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            exeFilePath = Path.Combine(exeDir, name + ".exe");
            Directory.CreateDirectory(exeDir);
            //Console.WriteLine(exeFilePath);
            AssemblyGen ag = new AssemblyGen(name, new CompilerOptions() { OutputPath = exeFilePath });
            Perpare(parseTree);
            GenHello1(ag, parseTree);
            ag.Save();
            AppDomain.CurrentDomain.ExecuteAssembly("ZodiacConsole.exe");

        }
        public void Perpare(ParseTree parseTree)
        {
            if (parseTree == null) return;
            AddParseNodeRec(parseTree.Root);
        }
        private void AddParseNodeRec(ParseTreeNode node)
        {
            if (node == null) return;
            BnfTerm term = node.Term;
            BNF bnf = GetBNF(node.ToString());
            switch (bnf)
            {
                case BNF.program:
                    break;
                default:
                    break;
            }
            foreach (var child in node.ChildNodes)
                AddParseNodeRec(child);
        }

        public void GenHello1(AssemblyGen ag, ParseTree ParseTree)
        {
            
            TypeGen Hello1 = ag.Public.Class("Main");
            CodeGen main = Hello1.Public.Static.Method(typeof(void), "Main");
            Operand str = main.Local(typeof(string));

            ITypeMapper TypeMapper = ag.TypeMapper;
            var staticF = ag.StaticFactory;
            //main.Assign(str, staticF.Invoke(TypeMapper.MapType(typeof(Console)), "ReadLine"));
            //main.Invoke(TypeMapper.MapType(typeof(Console)), "WriteLine", str);
            if (Console.ReadLine() == "111")
            {
                main.Invoke(TypeMapper.MapType(typeof(Console)), "WriteLine", "111");
            }
            else
            {
                main.Invoke(TypeMapper.MapType(typeof(Console)), "WriteLine", "222");
            }
        }
        private BNF GetBNF(string BNFString)
        {
            BNF bnf;
            try
            {
                bnf = (BNF)Enum.Parse(typeof(BNF), BNFString);
            }
            catch (Exception e)
            {
                //for identifier like "1 <identifier>",then mannally set bnf = identifier
                bnf = BNF.program;
            }
            return bnf;
        }

        private enum BNF
        {
            program = 0,
            program_heading,
            scope_body,
            statement_list,
            simple_statement,
            variable_definition_statement,
            variable_definition,
            Keyword,
            identifier_list,
            identifier,
            assignment_operator,
            assignment_reference_operator,
            Keysymbol,
            expression_list,
            bin_op_expression,
            primary_expression,
            literal,
            number,
            member_access,
            identifier_ext,
            required_type,
            simple_type,
            member_access_segments_opt,
            access_statement,
            argument_list_par,
            argument_list_opt,
            argument_list
        }
    }
}
