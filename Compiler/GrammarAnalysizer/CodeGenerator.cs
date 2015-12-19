using Irony.Parsing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TriAxis.RunSharp;
using TryAxis.RunSharp;

namespace Zodiac {

    internal class CodeGenerator {
        private string name;
        private Hashtable varTable;
        private AssemblyGen ag;
        private StaticFactory st;
        private ExpressionFactory exp;
        private TypeGen defaultClass;
        private CodeGen mainMethod;
        private TypeGen IOClass;

        public CodeGenerator() {
            varTable = new Hashtable();
            name = "ZodiacConsole";
            string exeDir = string.Empty;
            string exeFilePath = string.Empty;
            exeDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            exeFilePath = Path.Combine(exeDir, name + ".exe");
            Directory.CreateDirectory(exeDir);
            //Console.WriteLine(exeFilePath);
            ag = new AssemblyGen(name, new CompilerOptions() { OutputPath = exeFilePath });
            st = ag.StaticFactory;
            exp = ag.ExpressionFactory;
        }

        public void initIO() {
            IOClass = ag.Public.Class("IO");
            CodeGen writeStrMethod = IOClass.Public.Method(typeof(void), "write")
                .Parameter(typeof(string), "arg");
            {
                var arg = writeStrMethod.Arg("arg");
                writeStrMethod.Local();
                writeStrMethod.WriteLine(arg);
            }

            CodeGen writeIntMethod = IOClass.Public.Method(typeof(void), "write")
             .Parameter(typeof(int), "arg");
            {
                var arg = writeIntMethod.Arg("arg");
                writeIntMethod.Local();
                writeIntMethod.WriteLine(arg);
            }
        }

        private void initTypeMethod() {
            // take long as a method
        }

        public void Generate(ParseTree parseTree) {
            if (parseTree == null) return;

            defaultClass = ag.Public.Class("Default");
            mainMethod = defaultClass.Public.Static.Method(typeof(void), "Main");
            initIO();
            // initTypeMethod();

            //mainMethod.Invoke(IOvar, "write", str);

            AddParseNodeRec(parseTree.Root);

            var IOvar = mainMethod.Local(exp.New(IOClass));
            mainMethod.Invoke(IOvar, "write", (ContextualOperand)varTable["i"]);
            mainMethod.Invoke(IOvar, "write", (ContextualOperand)varTable["j"]);

            //GenHello1(ag,parseTree);

            ag.Save();
            AppDomain.CurrentDomain.ExecuteAssembly(name + ".exe");
        }

        private void AddParseNodeRec(ParseTreeNode node) {
            if (node == null) return;
            BnfTerm term = node.Term;
            BNF bnf = GetBNF(node.Token == null ? node.Term.Name : node.Token.Terminal.ToString());
            switch (bnf) {
                case BNF.variable_definition:
                    VariableDefinition(node);
                    break;

                case BNF.member_access:
                    // memberAccess(node);
                    break;

                default:
                    break;
            }
            foreach (var child in node.ChildNodes)
                AddParseNodeRec(child);
        }

        private void VariableDefinition(ParseTreeNode node) {
            //ParseTreeNodeList childList = node.ChildNodes;
            //get variable name

            var nameList = new List<string>();

            // if (node.ChildNodes[0])

            var idtList = node.ChildNodes[1].ChildNodes;
            var expList = node.ChildNodes[3].ChildNodes;
            var idtIter = idtList.GetEnumerator();
            var expIter = expList.GetEnumerator();

            while (idtIter.MoveNext() && expIter.MoveNext()) {
                var ExpressionNode = expIter.Current as ParseTreeNode;
                string name = (idtIter.Current as ParseTreeNode).Token.Text;
                varTable.Add(name, Expression(ExpressionNode));
            }
            //expression_list
            // node identifierList =
            return;
            //
        }

        private ContextualOperand Expression(ParseTreeNode node) {
            if (node == null) return null;
            BNF bnf = GetBNF(node.Token == null ? node.Term.Name : node.Token.Terminal.ToString());
            switch (bnf) {
                case BNF.number:
                    return mainMethod.Local(typeof(int), int.Parse(node.Token.Text));

                case BNF.member_access:
                    return MemberAccess(node);

                default:
                    //@TODO
                    if (node.ChildNodes.Count == 3)
                        return Expression(node.ChildNodes[0]).Add(Expression(node.ChildNodes[2]));
                    //return mainMethod.AssignAdd(Expression(node.ChildNodes[0]), Expression(node.ChildNodes[2])) );
                    break;
            }
            return Expression(node.ChildNodes[0]);
        }

        private ContextualOperand MemberAccess(ParseTreeNode node) {
            return null;
            /*
            if (node == null) return null;
            BNF bnf = GetBNF(node.Token == null ? node.Term.Name : node.Token.Terminal.ToString());
            switch (bnf)
            {
                case BNF.identifier:
                    return (ContextualOperand) varTable[node.Token.Text];

                case BNF.member_access:
                    return MemberAccess(node);

                case BNF.Keyword: //type
                    return

                default:
                    //@TODO
            }

            //ParseTreeNodeList childlist = node.ChildNodes;
            //string Identifier = childlist[0].ChildNodes[0].Token.Text;
            var idtExtNode = node.ChildNodes[0].ChildNodes[0];

            // is
            var ExtNodeBNF =GetBNF(idtExtNode.Token.Terminal.ToString() );

            if (ExtNodeBNF == BNF.identifier)
            {
                return null;
            }
            else
            {
                var typeName = idtExtNode.ChildNodes[0].ChildNodes[0].Token.Text;
            }

            //for (int i=1; i< childlist.)

            return null;*/
        }

        public void GenHello1(AssemblyGen ag, ParseTree ParseTree) {
            TypeGen Hello1 = ag.Public.Class("Main");
            CodeGen main = Hello1.Public.Static.Method(typeof(void), "Main");
            Operand str = main.Local(typeof(string));

            ITypeMapper TypeMapper = ag.TypeMapper;
            var staticF = ag.StaticFactory;
            //main.Assign(str, staticF.Invoke(TypeMapper.MapType(typeof(Console)), "ReadLine"));
            //main.Invoke(TypeMapper.MapType(typeof(Console)), "WriteLine", str);
            if (Console.ReadLine() == "111") {
                main.Invoke(TypeMapper.MapType(typeof(Console)), "WriteLine", "111");
            }
            else {
                main.Invoke(TypeMapper.MapType(typeof(Console)), "WriteLine", "222");
            }
        }

        private BNF GetBNF(string BNFString) {
            BNF bnf;
            try {
                bnf = (BNF)Enum.Parse(typeof(BNF), BNFString);
            }
            catch (Exception e) {
                //for identifier like "1 <identifier>",then mannally set bnf = identifier
                bnf = BNF.program;
            }
            return bnf;
        }

        private enum BNF {
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