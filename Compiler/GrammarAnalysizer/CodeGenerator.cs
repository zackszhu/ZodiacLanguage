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
        private Dictionary<string, Operand> varTable;
        private Dictionary<string, Dictionary<string, string>> typeTable;
        private AssemblyGen ag;
        private StaticFactory st;
        private ExpressionFactory exp;
        private TypeGen defaultClass;
        private CodeGen mainMethod;
        private TypeGen IOClass;

        public CodeGenerator() {
            varTable = new Dictionary<string, Operand>();
            typeTable = new Dictionary<string, Dictionary<string, string>>();
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
            typeTable["Default"] = new Dictionary<string, string>();
            mainMethod = defaultClass.Public.Static.Method(typeof(void), "Main");
            initIO();
            // initTypeMethod();

            //mainMethod.Invoke(IOvar, "write", str);

            AddParseNodeRec(parseTree.Root);

            var IOvar = mainMethod.Local(exp.New(IOClass));
            mainMethod.Invoke(IOvar, "write", (ContextualOperand)varTable["a"]);
            mainMethod.Invoke(IOvar, "write", (ContextualOperand)varTable["b"]);

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
                    return;
                case BNF.function_definition:
                    FunctionDefinition(node);
                    return;
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

            if(idtList.Count == expList.Count)
            {
                while (idtIter.MoveNext() && expIter.MoveNext())
                {
                    var ExpressionNode = expIter.Current as ParseTreeNode;
                    string name = (idtIter.Current as ParseTreeNode).Token.Text;
                    varTable.Add(name, Expression(ExpressionNode));
                }
            }
            else
            {
                ContextualOperand ret = mainMethod.Local(typeof(ArrayList));
                mainMethod.Assign(ret, ag.StaticFactory.Invoke(defaultClass,"getAB",(Operand)varTable["i"], (Operand)varTable["j"]));
                Operand a = mainMethod.Local(typeof(int));
                Operand b = mainMethod.Local(typeof(int));
                
                mainMethod.Assign(a, ret[0].Cast(typeof(int)));
                mainMethod.Assign(b, ret[1].Cast(typeof(int)));
                varTable.Add("a", a);
                varTable.Add("b", b);

            }

            //expression_list
            // node identifierList =
            return;
            //
        }

        private void FunctionDefinition(ParseTreeNode node, TypeGen ownerType = null)
        {
            if (ownerType == null) ownerType = defaultClass;
            var isStatic = false;
            if (node.ChildNodes[0].ChildNodes.Count != 0) isStatic = true;//static
            var funcIdt = node.ChildNodes[1].ChildNodes[0].ChildNodes[1].Token.Text;//function_identifier


            var retNode = node.ChildNodes[2].ChildNodes[0].ChildNodes[0];
            string retType = getRetType(retNode);
            
                     
            //var retTypeList = new ArrayList();
            //int retSize = retNode.ChildNodes.Count;
            Type funcRetType = getType(retType);

            typeTable[ownerType.Name][funcIdt] = "1";

            MethodGen func;
            if (isStatic)
                func = ownerType.Public.Static.Method(funcRetType, funcIdt);
            else
                func = ownerType.Public.Method(funcRetType, funcIdt);

            var paraNode = node.ChildNodes[3].ChildNodes[0].ChildNodes[0];
            int paraSize = paraNode.ChildNodes.Count;
            var paras = new List<string>();
            for(int i = 0; i < paraSize; i++)
            {
                paras.Add(paraNode.ChildNodes[i].ChildNodes[1].ChildNodes[0].Token.Text);
                func = func.Parameter(typeof(int), (string)paras[i]);
            }

            CodeGen code = func;
            var statementsNode = node.ChildNodes[3].ChildNodes[1].ChildNodes[0].ChildNodes[0].ChildNodes[0];
            //int statementSize = statementsNode.ChildNodes.Count;
            foreach(ParseTreeNode statementNode in statementsNode.ChildNodes)
            {

            }
            Operand a = code.Arg(paras[0]);
            Operand b = code.Arg(paras[1]);
            Operand ret = code.Local(typeof(ArrayList), exp.New(typeof(ArrayList)));
            code.Invoke(ret, "Add", a);
            code.Invoke(ret, "Add", b);

            code.Return(ret);
        }

        private string getRetType(ParseTreeNode node)
        {
            if (node.ToString() == "required_type")
            {
                node = node.ChildNodes[0];
                if (node.ToString() == "simple_type")
                {
                    return node.ChildNodes[0].Token.Text;
                }
            }
            return null;
        }

        private Type getType(string str)
        {
            return typeof(int);
        }


        private Operand Expression(ParseTreeNode node) {
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
            argument_list,

            function_definition
        }
    }
}