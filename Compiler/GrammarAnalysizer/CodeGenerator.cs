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
    public enum Ztype { zlong = 0, zbool, zreal, zlist, zType }
    class ZOperand
    {
        
        public ZOperand(Operand oper, Ztype z) {
            operand = oper;
            ztype = z;
        }

        public Operand operand;
        public Ztype ztype;
    }


    internal class CodeGenerator {
        private string name;
        private Hashtable varTable;  // < string, >
        private Stack<TypeGen> typeStack;
        private Stack<CodeGen> funcStack;
        private AssemblyGen ag;
        private StaticFactory st;
        private ExpressionFactory exp;
        private TypeGen defaultClass;
        private CodeGen mainMethod;
        private TypeGen IOClass;
        private Operand IOvar;


        public CodeGenerator() {
            varTable = new Hashtable();
            typeStack = new Stack<TypeGen>();
            funcStack = new Stack<CodeGen>();

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

            IOvar = mainMethod.Local(exp.New(IOClass));
        }

        private void initTypeMethod() {
            // take long as a method
        }

        public void Generate(ParseTree parseTree) {
            if (parseTree == null) return;

           

            defaultClass = ag.Public.Class("Default");
            mainMethod = defaultClass.Public.Static.Method(typeof(void), "Main");
            
            //generator stack
            typeStack.Push(defaultClass);
            funcStack.Push(mainMethod);

            initIO();


            AddParseNodeRec(parseTree.Root);

           

            mainMethod.Invoke(IOvar, "write", (Operand)varTable["a"]);
            mainMethod.Invoke(IOvar, "write", (Operand)varTable["b"]);

            ag.Save();
            AppDomain.CurrentDomain.ExecuteAssembly(name + ".exe");
        }



        private void AddParseNodeRec(ParseTreeNode node)
        {
            if (node == null) return;
            //BnfTerm term = node.Term;
            BNF bnf = GetBNF(node.Token == null ? node.Term.Name : node.Token.Terminal.ToString());
            switch (bnf)
            {
                case BNF.program_heading:
                    //VariableDefinition(node);
                    return;
                case BNF.scope_body:
                    ScopeBody(node);
                    return;
               
                default:
                    break;
            }

            foreach (var child in node.ChildNodes)
                AddParseNodeRec(child);
        }



        private void ScopeBody(ParseTreeNode node)
        {
            if (node == null) return;
            BNF bnf = GetBNF(node.Token == null ? node.Term.Name : node.Token.Terminal.ToString());
            switch (bnf)
            {
                case BNF.simple_statement:
                    simple_statement(node);
                    return;
                case BNF.structed_statement:
                    StructedStatement(node);
                    return;

                default:
                    break;
            }

            foreach (var child in node.ChildNodes)
                ScopeBody(child);
        }

        private void simple_statement(ParseTreeNode node)
        {
            if (node == null) return;
            BNF bnf = GetBNF(node.Token == null ? node.Term.Name : node.Token.Terminal.ToString());
            switch (bnf)
            {
                case BNF.variable_definition:
                    VariableDefinition(node);
                    return;
                case BNF.function_definition:
                    FunctionDefinition(node);
                    return;
                case BNF.assignment_statement:
                    AssignmentStatement(node);
                    return;
                case BNF.type_definition:
                    TypeDefinition(node);
                    return;
                case BNF.access_statement:
                    AccessStatement(node);
                    return;
                default:
                    break;
            }

            foreach (var child in node.ChildNodes)
                simple_statement(child);

        }


        private void VariableDefinition(ParseTreeNode node ) {
            //ParseTreeNodeList childList = node.ChildNodes;
            //get variable name

            CodeGen ownerFunc = funcStack.Peek();

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
                mainMethod.Assign(ret, ag.StaticFactory.Invoke(defaultClass,"getAB",(varTable["i"] as ZOperand).operand , (varTable["j"] as ZOperand).operand));
                ContextualOperand a = mainMethod.Local(typeof(int));
                Operand b = mainMethod.Local(typeof(int));

                //b = a.Ref();
                

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

        private void FunctionDefinition(ParseTreeNode node)
        {
            TypeGen masterClass = typeStack.Peek();

            var isStatic = false;
            if (node.ChildNodes[0].ChildNodes.Count != 0) isStatic = true;//static
            var funcIdt = node.ChildNodes[1].ChildNodes[0].ChildNodes[1].Token.Text;//function_identifier
            var retNode = node.ChildNodes[2];

            var para1 = node.ChildNodes[3].ChildNodes[0].ChildNodes[0].ChildNodes[0].ChildNodes[1].ChildNodes[0].Token.Text;
            var para2 = node.ChildNodes[3].ChildNodes[0].ChildNodes[0].ChildNodes[1].ChildNodes[1].ChildNodes[0].Token.Text;
            
            CodeGen func;
            if (isStatic)
                func = masterClass.Public.Static.Method(typeof(ArrayList), funcIdt).Parameter(typeof(int), para1).Parameter(typeof(int), para2);
            else
                func = masterClass.Public.Method(typeof(ArrayList), funcIdt).Parameter(typeof(int), para1).Parameter(typeof(int), para2);

            Operand a = func.Arg(para1);

            

            
  
            Operand b = func.Arg(para2);
            Operand ret = func.Local(typeof(ArrayList), exp.New(typeof(ArrayList)));
            func.Invoke(ret, "Add", a);
            func.Invoke(ret, "Add", b);
            

            func.Return(ret);
        }

        private void AssignmentStatement(ParseTreeNode node)
        {
            throw new NotImplementedException();
        }

        private void TypeDefinition(ParseTreeNode node)
        {
            throw new NotImplementedException();
        }

        private void AccessStatement(ParseTreeNode node)
        {
            throw new NotImplementedException();
        }

        private void StructedStatement(ParseTreeNode node)
        {
            throw new NotImplementedException();
        }

        private ZOperand Expression(ParseTreeNode node)
        {
            CodeGen ownerFunc = funcStack.Peek();
            if (node == null ) return null;
            BNF bnf = GetBNF(node.Token == null ? node.Term.Name : node.Token.Terminal.ToString());
            switch (bnf) {
                case BNF.number:
                   
                    return new ZOperand(ownerFunc.Local(typeof(int), int.Parse(node.Token.Text)),Ztype.zlong);

                case BNF.member_access:
                    return MemberAccess(node);

                default:
                    //@TODO
                    if (node.ChildNodes.Count == 3)
                    {
                        return new ZOperand(Expression(node.ChildNodes[0]).operand.Add(Expression(node.ChildNodes[2]).operand),Ztype.zlong);

                    }
                    //return mainMethod.AssignAdd(Expression(node.ChildNodes[0]), Expression(node.ChildNodes[2])) );
                    break;
            }
            return Expression(node.ChildNodes[0]);
        }

        private ZOperand MemberAccess(ParseTreeNode node) {
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
            structed_statement,
            variable_definition_statement,
            variable_definition,
            type_definition,
            assignment_statement,
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