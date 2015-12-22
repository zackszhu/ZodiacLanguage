using Irony.Parsing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using GrammarAnalysizer;
using TriAxis.RunSharp;
using TryAxis.RunSharp;

namespace Zodiac {
    class ZOperand
    {
        public ZOperand(ContextualOperand oper, string t , string n = null)
        {
            operand = oper;
            type = t;
            name = n;
        }

        public ContextualOperand operand;
        public string type;
        public string name;
    }


    internal class CodeGenerator {
        private string name;
        private Dictionary<string, ZOperand> varTable;
        private Dictionary<string, Dictionary<string, Type>> typeTable;
        private Stack<TypeGen> typeStack;
        private Stack<CodeGen> funcStack;
        private AssemblyGen ag;
        private StaticFactory st;
        private ITypeMapper tm;
        private ExpressionFactory exp;
        private TypeGen defaultClass;
        private CodeGen mainMethod;
        //private TypeGen IOClass;

        private int lineNumber;
        private int columnNumber;

        public CodeGenerator() {
            varTable = new Dictionary<string, ZOperand>();
            typeTable = new Dictionary<string, Dictionary<string, Type>>();
            typeStack = new Stack<TypeGen>();
            funcStack = new Stack<CodeGen>();

            name = "ZodiacConsole";
            var exeDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (exeDir != null)
            {
                var exeFilePath = Path.Combine(exeDir, name + ".exe");
                Directory.CreateDirectory(exeDir);
                //Console.WriteLine(exeFilePath);
                ag = new AssemblyGen(name, new CompilerOptions() { OutputPath = exeFilePath });
                st = ag.StaticFactory;
                exp = ag.ExpressionFactory;
                tm = ag.TypeMapper;
            }
        }
        /*
        public void InitIO() {
            IOClass = ag.Public.Class("IO");
            typeTable["IO"] = new Dictionary<string, Type>();
            typeTable["IO"]["write"] = typeof(void);

            CodeGen writeStrMethod = IOClass.Public.Method(typeof(void), "write")
                .Parameter(typeof(string), "arg");
            {
                var arg = writeStrMethod.Arg("arg");
                writeStrMethod.WriteLine(arg);
            }

            CodeGen writeIntMethod = IOClass.Public.Method(typeof(void), "write")
             .Parameter(typeof(int), "arg");
            {
                var arg = writeIntMethod.Arg("arg");
                writeIntMethod.WriteLine(arg);
            }

            CodeGen writeCharMethod = IOClass.Public.Method(typeof(void), "write")
             .Parameter(typeof(char), "arg");
            {
                var arg = writeCharMethod.Arg("arg");
                writeCharMethod.WriteLine(arg);
            }
        }
        */

        public void InitRequiredType()
        {
            typeTable["long"] = new Dictionary<string, Type>();
            typeTable["long"]["ToString"] = typeof(string);
        }

        private void InitTypeMethod() {
            // take long as a method
        }

        public void Generate(ParseTree parseTree) {
            if (parseTree == null) return;

            defaultClass = ag.Public.Class("Default");
            typeTable["Default"] = new Dictionary<string, Type>();
            mainMethod = defaultClass.Public.Static.Method(typeof(void), "Main");

            //generator stack
            typeStack.Push(defaultClass);
            funcStack.Push(mainMethod);

           // InitIO();
            InitRequiredType();
            AddParseNodeRec(parseTree.Root);

            ContextualOperand a = mainMethod.Local(exp.New(typeof (ArrayList)));
            a.Invoke("Add", 2);


            //varTable["a"] = new ZOperand(a,"List<int>");

            //var c = (a[0] as ContextualOperand).GetReturnType(tm);
            //mainMethod.Invoke(typeof(IO), "WriteLine", varTable["a"].operand);
            //mainMethod.Invoke(typeof(IO), "WriteLine", c);



            var t = varTable["i"].operand.GetReturnType(tm);
            var s = varTable["j"].operand.GetReturnType(tm);
            mainMethod.Invoke(typeof(IO), "WriteLine", t);
            mainMethod.Invoke(typeof(IO), "WriteLine", s);

            mainMethod.Invoke(typeof(IO), "WriteLine", varTable["i"].operand);
            mainMethod.Invoke(typeof(IO), "WriteLine", varTable["j"].operand);
            ag.Save();
            AppDomain.CurrentDomain.ExecuteAssembly(name + ".exe");
        }



        private void AddParseNodeRec(ParseTreeNode node)
        {
            if (node == null) return;

            BNF bnf = GetBNF(node.Token == null ? node.Term.Name : node.Token.Terminal.ToString());

            switch (bnf)
            {
                case BNF.program_heading:
                    ProgramHeading(node);
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

        private void ProgramHeading(ParseTreeNode node)
        {
            if (node == null) return;
            BNF bnf = GetBNF(node.Token == null ? node.Term.Name : node.Token.Terminal.ToString());
            switch (bnf)
            {

                default:
                    break;
            }

            foreach (var child in node.ChildNodes)
                ProgramHeading(child);
        }

        private void ScopeBody(ParseTreeNode node)
        {
            if (node == null) return;
            BNF bnf = GetBNF(node.Token == null ? node.Term.Name : node.Token.Terminal.ToString());
            switch (bnf)
            {
                case BNF.simple_statement:
                    SimpleStatement(node);
                    return;
                case BNF.structed_statement:
                    StructedStatement(node);
                    return;
                case BNF.definition:
                    FuncTypeDefinition(node);
                    return;
                case BNF.return_statement:
                    ReturnStatement(node);
                    return;
                default:
                    break;
            }

            foreach (var child in node.ChildNodes)
                ScopeBody(child);
        }

        private void FuncTypeDefinition(ParseTreeNode node)
        {
            if (node == null) return;
            BNF bnf = GetBNF(node.Token == null ? node.Term.Name : node.Token.Terminal.ToString());
            switch (bnf)
            {
                case BNF.function_definition:
                    FunctionDefinition(node);
                    return;
                case BNF.type_definition:
                    TypeDefinition(node);
                    return;

                default:
                    break;
            }

            foreach (var child in node.ChildNodes)
                FunctionDefinition(child);
        }


        private void SimpleStatement(ParseTreeNode node)
        {
            if (node == null) return;
            BNF bnf = GetBNF(node.Token == null ? node.Term.Name : node.Token.Terminal.ToString());
            switch (bnf)
            {
                case BNF.variable_definition:
                    VariableDefinition(node);
                    return;
                case BNF.assignment_statement:
                    AssignmentStatement(node);
                    return;
                case BNF.access_statement:
                    AccessStatement(node);
                    return;

                default:
                    break;
            }

            foreach (var child in node.ChildNodes)
                SimpleStatement(child);
        }

        private void StructedStatement(ParseTreeNode node)
        {
            throw new NotImplementedException();
        }

        private void ReturnStatement(ParseTreeNode node)
        {
            if (node == null) return;
            BNF bnf = GetBNF(node.Token == null ? node.Term.Name : node.Token.Terminal.ToString());

            var ownerFunc = funcStack.Peek();

            var returnValue = Expression(node.ChildNodes[1]);

            ownerFunc.Return(returnValue.operand);
            
        }

        private void VariableDefinition(ParseTreeNode node ) {
            CodeGen ownerFunc = funcStack.Peek();

            var nameList = new List<string>();
            
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
                //for multiRturn
                ContextualOperand ret = mainMethod.Local(typeof(int));
                mainMethod.Assign(ret, ag.StaticFactory.Invoke(defaultClass,"getAB",(varTable["i"] as ZOperand).operand , (varTable["j"] as ZOperand).operand));
                ContextualOperand a = mainMethod.Local(typeof(int));
                Operand b = mainMethod.Local(typeof(int));
                
                //b = a.Ref();
                
                mainMethod.Assign(a, ret[0].Cast(typeof(int)));
                mainMethod.Assign(b, ret[1].Cast(typeof(int)));

            }

            //expression_list
            // node identifierList =
            return;
            //
        }

        private void FunctionDefinition(ParseTreeNode node)
        {
            //------------the owner of the fucntion
            TypeGen ownerType = typeStack.Peek();
            //------------function header
            var isStatic = false;
            if (node.ChildNodes[0].ChildNodes.Count != 0 || ownerType == defaultClass) isStatic = true;//static
            var funcIdt = node.ChildNodes[1].ChildNodes[0].ChildNodes[1].Token.Text;//function_identifier
            //------------ret Type
            var retNode = node.ChildNodes[2].ChildNodes[0].ChildNodes[0];
            string retType = getTypeString(retNode);
            //var retTypeList = new ArrayList();
            //int retSize = retNode.ChildNodes.Count;
            Type funcRetType = getType(retType);

            typeTable[ownerType.Name][funcIdt] = funcRetType;

            MethodGen func;
            if (isStatic)
                func = ownerType.Public.Static.Method(funcRetType, funcIdt);
            else
                func = ownerType.Public.Method(funcRetType, funcIdt);


            var paraBlockNode = node.ChildNodes[3].ChildNodes[0];
            int paraSize = 0;
            var paraNames = new List<string>();
            var paras = new List<Operand>();
            if (paraBlockNode.ChildNodes.Count != 0)
            {
                var parasNode = paraBlockNode.ChildNodes[0];
                paraSize = parasNode.ChildNodes.Count;
                for (int i = 0; i < paraSize; i++)
                {
                    paraNames.Add(parasNode.ChildNodes[i].ChildNodes[1].ChildNodes[0].Token.Text);
                    var typeStr = getTypeString(parasNode.ChildNodes[i].ChildNodes[3]);
                    func = func.Parameter(getType(typeStr), paraNames[i]);
                }

            }

            CodeGen code = func.GetCode();

            funcStack.Push(code);

            for(int i = 0; i < paraSize; i++)
            {
                paras.Add(code.Arg(paraNames[i]));
            }
            var statementsNode = node.ChildNodes[3].ChildNodes[1].ChildNodes[0].ChildNodes[0].ChildNodes[0];
            //int statementSize = statementsNode.ChildNodes.Count;
            ScopeBody(statementsNode);
            funcStack.Pop();
            /*foreach(ParseTreeNode statementNode in statementsNode.ChildNodes)
            {

            }*/


        }
        public string getTypeString(ParseTreeNode node)
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
        private string getTypeString(Type type) //TODO => type.Name ?? null; 
        {
            if (type == typeof(int))
                return "long";
            else if (type == typeof(bool))
                return "bool";
            else if (type == typeof(float))
                return "real";
            else if (type == typeof(bool))
                return "bool";
            return null;
        }
        private Type getType(string typeStr)
        {
            if (typeStr == "long")
                return typeof(int);
            else if (typeStr == "bool")
                return typeof(bool);
            else if (typeStr == "real")
                return typeof(float);
            else if (typeStr == "list")
                return typeof(ArrayList);
            return typeof(int);
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
            ZOperand ret = MemberAccess(node);
        }

        
        private ContextualOperand Compute(ContextualOperand leftValue, string oper, ContextualOperand rightValue = null)
        {
            if (rightValue as object == null)
            {
                if (oper == "+")
                    return +leftValue;
                else if (oper == "-")
                    return -leftValue;
                else if (oper == "!")
                    return !leftValue;
                else if (oper == "~")
                    return ~leftValue;
                else throw new Exception("invalid unary operator");
            }
            else
            {
                if (oper == "+")
                    return leftValue + rightValue;
                else if (oper == "-")
                    return leftValue - rightValue;
                else if (oper == "*")
                    return leftValue * rightValue;
                else if (oper == "/")
                    return leftValue / rightValue;
                else if (oper == "%")
                    return leftValue % rightValue;
                else if (oper == ">>")
                    return leftValue.RightShift(rightValue);
                else if (oper == "<<")
                    return leftValue.LeftShift(rightValue);
                else if (oper == "==")
                    return leftValue == rightValue;
                else if (oper == "!=")
                    return leftValue != rightValue;
                else if (oper == "&")
                    return leftValue & rightValue;
                else if (oper == "^")
                    return leftValue ^ rightValue;
                else if (oper == "&&")
                    return leftValue && rightValue;
                else if (oper == "||")
                    return leftValue || rightValue;

            }
            return null;
        }

        private ZOperand Compute(ZOperand left, string oper, ZOperand right = null )
        {


            ContextualOperand resultValue = null;
            resultValue = right == null ? Compute(left.operand, oper, null) : Compute(left.operand, oper, right.operand);

            if (resultValue == null) throw new Exception("invaild expression");
            else return new ZOperand(resultValue, getTypeString(resultValue.GetReturnType(tm)));

        }

        private ZOperand Expression(ParseTreeNode node)
        {
            CodeGen ownerFunc = funcStack.Peek();
            if (node == null ) return null;
            BNF bnf = GetBNF(node.Token == null ? node.Term.Name : node.Token.Terminal.ToString());

            switch (bnf) {
                case BNF.number:
                    return new ZOperand(ownerFunc.Local(typeof(int), int.Parse(node.Token.Text)), "long");

                case BNF.member_access:
                    return MemberAccess(node);
                case BNF.unary_expression:
                
                    string oper = node.ChildNodes[0].ChildNodes[0].Token.Terminal.ToString();
                    return Compute(Expression(node.ChildNodes[1]), oper);
               
               
                    
                default:
                    
                    if (node.ChildNodes.Count == 3)  //primmary_expression
                    {
                        // left = ;

                        string oper2 = node.ChildNodes[1].Token.Terminal.ToString();
                        //throw new NotImplementedException();
                        return Compute(Expression(node.ChildNodes[0]), oper2, Expression(node.ChildNodes[2]));

                    }
                    else
                        return Expression(node.ChildNodes[0]);
                    //return mainMethod.AssignAdd(Expression(node.ChildNodes[0]), Expression(node.ChildNodes[2])) );
            }
           
        }

        private ZOperand MemberAccess(ParseTreeNode node)
        {
            CodeGen ownerFunc = funcStack.Peek();
            ZOperand mainAccess;
            var mainAccessNode = node.ChildNodes[0];//what for member_access_with_segment?
            if(mainAccessNode.ToString() == "member_access")
            {
                if (node.ChildNodes.Count != 1)
                {
                    var segment = node.ChildNodes[1].ChildNodes[0];
                    BnfTerm term = segment.Term;
                    BNF bnf = GetBNF(segment.Token == null ? segment.Term.Name : segment.Token.Terminal.ToString());
                    ParseTreeNode member;
                    switch (bnf) {
                        case BNF.argument_list_par:
                            mainAccess = FunctionAccess(mainAccessNode);
                            member = segment.ChildNodes[0];
                            if(member.ChildNodes.Count == 0)
                            {
                                ContextualOperand ret;
                                Type type;
                                if (mainAccess.operand as Object == null)
                                {
                                    ret = st.Invoke(tm.MapType(defaultClass),mainAccess.name);
                                    type = typeTable[defaultClass.Name][mainAccess.name];
                                    return new ZOperand(ret, getTypeString(type));
                                }
                                ret = mainAccess.operand.Invoke(mainAccess.name);
                                type = typeTable[mainAccess.type][mainAccess.name];
                                return new ZOperand(ret, getTypeString(type));
                            }
                            else
                            {
                                int paraSize = member.ChildNodes.Count;
                                List<Operand> paras = new List<Operand>();
                                for (int i = 0; i < paraSize; i++)
                                {
                                    paras.Add(Expression(member.ChildNodes[i]).operand);
                                }
                                var ret = mainAccess.operand.Invoke(mainAccess.name, paras.ToArray());

                            }
                            break;
                        case BNF.array_indexer:
                            mainAccess = MemberAccess(mainAccessNode);
                            member = segment.ChildNodes[0];
                            var index = Expression(member);
                            return new ZOperand(((ContextualOperand)mainAccess.operand)[index.operand], "char");
                        case BNF.dot:
                            mainAccess = MemberAccess(mainAccessNode);
                            member = node.ChildNodes[1].ChildNodes[1];
                            string var = member.Token.Text;
                            break;
                    }
                    segment.ToString();
                }
            }
            else
            {
                var idt_ext = mainAccessNode.ChildNodes[0].Token.Text;
                if (node.ChildNodes.Count != 1)
            {
                    var segment = node.ChildNodes[1].ChildNodes[0];
            }
            else
            {
                    return varTable[idt_ext];
                }
            }    
            return MemberAccess(mainAccessNode);
            }
        private ZOperand FunctionAccess(ParseTreeNode node)
        {
            CodeGen ownerFunc = funcStack.Peek();
            ZOperand mainAccess;
            var mainAccessNode = node.ChildNodes[0];//what for member_access_with_segment?
            if (mainAccessNode.ToString() == "member_access")
            {
                if (node.ChildNodes.Count != 1)
                {
                    var segment = node.ChildNodes[1].ChildNodes[0];
                    BnfTerm term = segment.Term;
                    BNF bnf = GetBNF(segment.Token == null ? segment.Term.Name : segment.Token.Terminal.ToString());
                    ParseTreeNode member;
                    switch (bnf)
                    {
                        case BNF.argument_list_par:
                            //NOT SUPPORT
                            break;
                        case BNF.array_indexer:
                            //NOT SUPPORT
                            break;
                        case BNF.dot:
                            mainAccess = MemberAccess(mainAccessNode);
                            member = node.ChildNodes[1].ChildNodes[1];
                            string var = member.Token.Text;
                            mainAccess.name = var;
                            return mainAccess;
                        default:
                            throw new Exception("FunctionAccess");
                    }
                }
            }
            else
            {
                var idt_ext = mainAccessNode.ChildNodes[0].Token.Text;
                if (node.ChildNodes.Count != 1)
            {
                    //really meaningful?
                    var segment = node.ChildNodes[1].ChildNodes[0];
            }
            else
            {
                    return new ZOperand(null, null, idt_ext);
            }
            }

            return FunctionAccess(mainAccessNode);
        }

       /* private BNF GetNodeType(string )
        {

        }
        */

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
            unary_expression,
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
            array_indexer,
            dot,
            function_definition,
            definition,
            return_statement,
        }
    }
}