using Irony.Parsing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using GrammarAnalysizer;
using TriAxis.RunSharp;
using TryAxis.RunSharp;

namespace Zodiac {
    class ZOperand
    {
        public ZOperand(ContextualOperand oper, string t , string n = null)
        {
            Operand = oper;
            Type = t;
            Name = n;
        }

        public ContextualOperand Operand { get; }
        public string Type { get; }
        public string Name;
    }


    internal class CodeGenerator {
        private string name;
        private Dictionary<string, ZOperand> varTable;
        private Dictionary<string, Dictionary<string, Type>> funcTable;
        private Dictionary<string, Type> typeTable;
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
            funcTable = new Dictionary<string, Dictionary<string, Type>>();
            typeTable = new Dictionary<string, Type>();
            typeStack = new Stack<TypeGen>();
            funcStack = new Stack<CodeGen>();

            name = "ZodiacConsole";
            var exeDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (exeDir == null) return;
            var exeFilePath = Path.Combine(exeDir, name + ".exe");
            Directory.CreateDirectory(exeDir);
            //Console.WriteLine(exeFilePath);
            ag = new AssemblyGen(name, new CompilerOptions() { OutputPath = exeFilePath });
            st = ag.StaticFactory;
            exp = ag.ExpressionFactory;
            tm = ag.TypeMapper;
        }
        /*
        public void InitIO() {
            IOClass = ag.Public.Class("IO");
            funcTable["IO"] = new Dictionary<string, Type>();
            funcTable["IO"]["write"] = typeof(void);

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
            funcTable["long"] = new Dictionary<string, Type> {["ToString"] = typeof (string)};

            typeTable.Add("long", typeof(int));
            typeTable.Add("real", typeof(double));
            typeTable.Add("bool", typeof(bool));
            typeTable.Add("list", typeof(MyList));
            typeTable.Add("char", typeof(char));
        }

        private void InitTypeMethod() {
            // take long as a method
        }

        public void Generate(ParseTree parseTree) {
            if (parseTree == null) return;

            defaultClass = ag.Public.Class("Default");
            typeTable.Add("Default", defaultClass);
            funcTable["Default"] = new Dictionary<string, Type>();
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
            //mainMethod.Invoke(typeof(IO), "WriteLine", varTable["a"].Operand);
            //mainMethod.Invoke(typeof(IO), "WriteLine", c);



            var t = varTable["i"].Operand.GetReturnType(tm);
            var s = varTable["j"].Operand.GetReturnType(tm);
            mainMethod.Invoke(typeof(IO), "WriteLine", t);
            mainMethod.Invoke(typeof(IO), "WriteLine", s);

            mainMethod.Invoke(typeof(IO), "WriteLine", varTable["i"].Operand);
            mainMethod.Invoke(typeof(IO), "WriteLine", varTable["j"].Operand);
            ag.Save();
            AppDomain.CurrentDomain.ExecuteAssembly(name + ".exe");
        }



        private void AddParseNodeRec(ParseTreeNode node)
        {
            if (node == null) return;

            BNF bnf = GetBNF(node.Token?.Terminal.ToString() ?? node.Term.Name);

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
            BNF bnf = GetBNF(node.Token?.Terminal.ToString() ?? node.Term.Name);
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
            BNF bnf = GetBNF(node.Token?.Terminal.ToString() ?? node.Term.Name);
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
            BNF bnf = GetBNF(node.Token?.Terminal.ToString() ?? node.Term.Name);
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
                FuncTypeDefinition(child);
        }


        private void SimpleStatement(ParseTreeNode node)
        {
            if (node == null) return;
            BNF bnf = GetBNF(node.Token?.Terminal.ToString() ?? node.Term.Name);
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
            BNF bnf = GetBNF(node.Token?.Terminal.ToString() ?? node.Term.Name);

            var ownerFunc = funcStack.Peek();

            var returnValue = Expression(node.ChildNodes[1]);

            ownerFunc.Return(returnValue.Operand);
            
        }

        private void VariableDefinition(ParseTreeNode node ) {
            var ownerFunc = funcStack.Peek();

            var nameList = new List<string>();
            
            var idtList = node.ChildNodes[1].ChildNodes;
            var expList = node.ChildNodes[3].ChildNodes;
            var idtIter = idtList.GetEnumerator();
            var expIter = expList.GetEnumerator();

            if(idtList.Count == expList.Count)
            {
                while (idtIter.MoveNext() && expIter.MoveNext())
                {
                    var expressionNode = expIter.Current as ParseTreeNode;
                    var variableName = (idtIter.Current as ParseTreeNode)?.Token.Text;
                    varTable.Add(variableName, Expression(expressionNode));
                }
            }
            else
            {
                //for multiRturn
                ContextualOperand ret = mainMethod.Local(typeof(int));
                mainMethod.Assign(ret, ag.StaticFactory.Invoke(defaultClass,"getAB",(varTable["i"] as ZOperand).Operand , (varTable["j"] as ZOperand).Operand));
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
            bool isStatic = node.ChildNodes[0].ChildNodes.Count != 0 || ownerType == defaultClass;
            var funcIdt = node.ChildNodes[1].ChildNodes[0].ChildNodes[1].Token.Text;//function_identifier
            //------------ret Type
            var retNode = node.ChildNodes[2].ChildNodes[0].ChildNodes[0];
            string retType = getTypeString(retNode);
            //var retTypeList = new ArrayList();
            //int retSize = retNode.ChildNodes.Count;
            Type funcRetType = getType(retType);

            funcTable[ownerType.Name][funcIdt] = funcRetType;

            var func = isStatic ? ownerType.Public.Static.Method(funcRetType, funcIdt) : ownerType.Public.Method(funcRetType, funcIdt);

            var paraBlockNode = node.ChildNodes[3].ChildNodes[0];
            var paraSize = 0;
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

            for(var i = 0; i < paraSize; i++)
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
            if (node.ToString() != "required_type") return null;
            node = node.ChildNodes[0];
            return node.ToString() == "simple_type" ? node.ChildNodes[0].Token.Text : null;
        }

        private string getTypeString(Type type) //TODO => Type.Name ?? null; 
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

        private Type getType(string typeStr) => typeTable[typeStr];
//            {
//            switch (typeStr) {
//                case "long":
//                    return typeof(int);
//                case "bool":
//                    return typeof(bool);
//                case "real":
//                    return typeof(float);
//                case "list":
//                    return typeof(ArrayList);
//            }
//            return typeof(int);
//        }

        private void AssignmentStatement(ParseTreeNode node)
        {
            throw new NotImplementedException();
        }
            
        private void TypeDefinition(ParseTreeNode node) {
            // type init
            var isDerived = node.ChildNodes[2].ChildNodes.Count > 0;
            var typeName = node.ChildNodes[1].Token.Text;
            TypeGen thisType;
            if (isDerived) {
                var baseName = node.ChildNodes[2].ChildNodes[1].Token.Text;
                if (!typeTable.ContainsKey(baseName)) throw new NotImplementedException();
                var baseType = typeTable[baseName];
                thisType = ag.Public.Class(typeName, baseType);
            }
            else {
                thisType = ag.Public.Class(typeName);
            }
            typeTable.Add(typeName, thisType);
            typeStack.Push(thisType);

            var structured = node.ChildNodes[3];
            foreach (var member in structured.ChildNodes) {
                switch (member.ChildNodes[0].Term.Name) {
                    case "member_variable":
                        TypeMemVarDef(member.ChildNodes[0]);
                        break;
                    case "member_function":
                        continue;
                }
            }

            typeStack.Pop();

            //throw new NotImplementedException();
        }

        private void TypeMemVarDef(ParseTreeNode node) {
            var varType = getType(getTypeString(node.ChildNodes[3]));
            var currentClass = typeStack.Peek();
            currentClass.Public.Field(varType, node.ChildNodes[1].Token.Text);
        }

        private void AccessStatement(ParseTreeNode node)
        {
            ZOperand ret = MemberAccess(node);
        }

        
        private ContextualOperand Compute(ContextualOperand leftValue, string oper, ContextualOperand rightValue = null)
        {
            if (rightValue as object == null)
            {
                switch (oper) {
                    case "+":
                        return +leftValue;
                    case "-":
                        return -leftValue;
                    case "!":
                        return !leftValue;
                    case "~":
                        return ~leftValue;
                    default:
                        throw new Exception("invalid unary operator");
                }
            }
            else
            {
                switch (oper) {
                    case "+":
                        return leftValue + rightValue;
                    case "-":
                        return leftValue - rightValue;
                    case "*":
                        return leftValue * rightValue;
                    case "/":
                        return leftValue / rightValue;
                    case "%":
                        return leftValue % rightValue;
                    case ">>":
                        return leftValue.RightShift(rightValue);
                    case "<<":
                        return leftValue.LeftShift(rightValue);
                    case "==":
                        return leftValue == rightValue;
                    case "!=":
                        return leftValue != rightValue;
                    case "&":
                        return leftValue & rightValue;
                    case "^":
                        return leftValue ^ rightValue;
                    case "&&":
                        return leftValue && rightValue;
                    case "||":
                        return leftValue || rightValue;
                }

            }
            return null;
        }

        private ZOperand Compute(ZOperand left, string oper, ZOperand right = null )
        {


            ContextualOperand resultValue = null;
            resultValue = right == null ? Compute(left.Operand, oper, null) : Compute(left.Operand, oper, right.Operand);

            if (resultValue == null) throw new Exception("invaild expression");
            else return new ZOperand(resultValue, getTypeString(resultValue.GetReturnType(tm)));

        }

        private ZOperand Expression(ParseTreeNode node)
        {
            CodeGen ownerFunc = funcStack.Peek();
            if (node == null ) return null;
            BNF bnf = GetBNF(node.Token?.Terminal.ToString() ?? node.Term.Name);

            string oper;
            switch (bnf) {
                case BNF.number:
                    // @TODO assist function?
                    return new ZOperand(ownerFunc.Local(typeof(int), int.Parse(node.Token.Text)), "long");

                case BNF.member_access:
                    return MemberAccess(node);
                case BNF.unary_expression:
                
                    oper = node.ChildNodes[0].ChildNodes[0].Token.Terminal.ToString();
                    return Compute(Expression(node.ChildNodes[1]), oper);
               
               
                    
                default:
                    
                    if (node.ChildNodes.Count == 3)  //primmary_expression
                    {
                        // left = ;

                        oper = node.ChildNodes[1].Token.Terminal.ToString();
                        //throw new NotImplementedException();
                        return Compute(Expression(node.ChildNodes[0]), oper, Expression(node.ChildNodes[2]));
                    }
                    else
                        return Expression(node.ChildNodes[0]);
                    //return mainMethod.AssignAdd(Expression(node.ChildNodes[0]), Expression(node.ChildNodes[2])) );
            }
           
        }

        private ZOperand MemberAccess(ParseTreeNode node)
        {
            CodeGen ownerFunc = funcStack.Peek();
            var mainAccessNode = node.ChildNodes[0];//what for member_access_with_segment?
            if(mainAccessNode.ToString() == "member_access")
            {
                if (node.ChildNodes.Count == 1) return MemberAccess(mainAccessNode);
                var segment = node.ChildNodes[1].ChildNodes[0];
                BnfTerm term = segment.Term;
                BNF bnf = GetBNF(segment.Token?.Terminal.ToString() ?? segment.Term.Name);
                ParseTreeNode member;
                ZOperand mainAccess;
                switch (bnf) {
                    case BNF.argument_list_par:
                        mainAccess = FunctionAccess(mainAccessNode);
                        member = segment.ChildNodes[0];
                        if(member.ChildNodes.Count == 0)
                        {
                            ContextualOperand ret;
                            Type type;
                            if (mainAccess.Operand as Object == null)
                            {
                                ret = st.Invoke(tm.MapType(defaultClass),mainAccess.Name);
                                type = funcTable[defaultClass.Name][mainAccess.Name];
                                return new ZOperand(ret, getTypeString(type));
                            }
                            ret = mainAccess.Operand.Invoke(mainAccess.Name);
                            type = funcTable[mainAccess.Type][mainAccess.Name];
                            return new ZOperand(ret, getTypeString(type));
                        }
                        else
                        {
                            var paraSize = member.ChildNodes.Count;
                            var paras = new List<Operand>();
                            for (var i = 0; i < paraSize; i++)
                            {
                                paras.Add(Expression(member.ChildNodes[i]).Operand);
                            }
                            var ret = mainAccess.Operand.Invoke(mainAccess.Name, paras.ToArray());

                        }
                        break;
                    case BNF.array_indexer:
                        mainAccess = MemberAccess(mainAccessNode);
                        member = segment.ChildNodes[0];
                        var index = Expression(member);
                        return new ZOperand(((ContextualOperand)mainAccess.Operand)[index.Operand], "char");
                    case BNF.dot:
                        mainAccess = MemberAccess(mainAccessNode);
                        member = node.ChildNodes[1].ChildNodes[1];
                        string var = member.Token.Text;
                        break;
                }
                // @TODO ret value?
                segment.ToString();
            }
            else
            {
                var idtExt = mainAccessNode.ChildNodes[0].Token.Text;
                if (node.ChildNodes.Count != 1)
            {
                    var segment = node.ChildNodes[1].ChildNodes[0];
            }
            else
            {
                    return varTable[idtExt];
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
                    BNF bnf = GetBNF(segment.Token?.Terminal.ToString() ?? segment.Term.Name);
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
                            var var = member.Token.Text;
                            mainAccess.Name = var;
                            return mainAccess;
                        default:
                            throw new Exception("FunctionAccess");
                    }
                }
            }
            else
            {
                var idtExt = mainAccessNode.ChildNodes[0].Token.Text;
                if (node.ChildNodes.Count != 1)
            {
                    //really meaningful?
                    var segment = node.ChildNodes[1].ChildNodes[0];
            }
            else
            {
                    return new ZOperand(null, null, idtExt);
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