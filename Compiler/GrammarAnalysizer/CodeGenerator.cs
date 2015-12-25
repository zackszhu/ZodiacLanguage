using Irony.Parsing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using GrammarAnalysizer;
using TriAxis.RunSharp;
using TryAxis.RunSharp;
using System.Linq;

namespace Zodiac {
    class ZOperand {
        public ZOperand(Operand oper, string t , string n = null) {
            Operand = oper;
            Type = t;
            Name = n;
        }

        public Operand Operand { get; }
        public string Type { get; }
        public string Name;
    }


    internal class CodeGenerator {
        private string name;
        private Stack<Dictionary<string, ZOperand>> varTable;
        private Dictionary<string, Type> typeTable;
        private Stack<TypeGen> typeStack;
        private Stack<CodeGen> funcStack;
        private AssemblyGen ag;
        private StaticFactory st;
        private ITypeMapper tm;
        private ExpressionFactory exp;
        private TypeGen defaultClass;
        private CodeGen mainMethod;

        //error info
        private int lineNumber;
        private int columnNumber;
        private bool GeneratedOK;

        private string selectFunc = "_Select_";
        private int SelectFuncNumber = 0;

        public CodeGenerator() {
            varTable = new Stack<Dictionary<string, ZOperand>>();
            typeTable = new Dictionary<string, Type>();
            typeStack = new Stack<TypeGen>();
            funcStack = new Stack<CodeGen>();

            name = "ZodiacConsole";
            var exeDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (exeDir == null) return;
            var exeFilePath = Path.Combine(exeDir, name + ".exe");
            Directory.CreateDirectory(exeDir);
            ag = new AssemblyGen(name, new CompilerOptions() { OutputPath = exeFilePath });
            st = ag.StaticFactory;
            exp = ag.ExpressionFactory;
            tm = ag.TypeMapper;
        }
        public void InitRequiredType() {
            //typeMemberTable["long"] = new Dictionary<string, Type> { ["ToString"] = typeof(string) };

            typeTable.Add("long", typeof(int));
            typeTable.Add("real", typeof(double));
            typeTable.Add("string", typeof(string)); // only for internal use
            typeTable.Add("bool", typeof(bool));
            typeTable.Add("list", typeof(list));
            typeTable.Add("char", typeof(char));
            typeTable.Add("IO", typeof(IO));
        }
        public void Generate(ParseTree parseTree) {
            if (parseTree == null) return;
            GeneratedOK = true;
            defaultClass = ag.Public.Class("Default");
            typeTable.Add("Default", defaultClass);
            mainMethod = defaultClass.Public.Static.Method(typeof(void), "Main");

            //generator stack
            typeStack.Push(defaultClass);
            funcStack.Push(mainMethod);

            //InitIO();
            InitRequiredType();
            PushScope();
            var ioOperand = mainMethod.Local(exp.New(typeTable["IO"]));

            ParseNode(parseTree.Root);
            
            if (GeneratedOK) {
                ag.Save();
                AppDomain.CurrentDomain.ExecuteAssembly(name + ".exe");
            }
        }
        private void ParseNode(ParseTreeNode node) {
            if (node == null) return;

            BNF bnf = GetBNF(node);

            switch (bnf) {
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
                ParseNode(child);
        }
        private void ProgramHeading(ParseTreeNode node) {
            if (node == null) return;
            BNF bnf = GetBNF(node);
            switch (bnf) {
                default:
                    break;
            }
            foreach (var child in node.ChildNodes)
                ProgramHeading(child);
        }
        private void ScopeBody(ParseTreeNode node) {
            if (node == null) return;
            BNF bnf = GetBNF(node);
            try {
                switch (bnf) {
                    case BNF.simple_statement:
                        SimpleStatement(node);
                        return;
                    case BNF.structed_statement:
                        StructedStatement(node);
                        return;
                    case BNF.definition:
                        FuncTypeDefinition(node);
                        return;
                    case BNF.ret_statement:
                        RetStatement(node);
                        return;
                    case BNF.break_statement:
                        BreakStatement();
                        return;
                    case BNF.continue_statement:
                        ContinueStatement();
                        return;
                    default:
                        foreach (var child in node.ChildNodes)
                            ScopeBody(child);
                        break;
                }
          }
          catch (Exception e) {
                GeneratedOK = false;
                Console.WriteLine("("+(lineNumber+1)+","+columnNumber+"):\t"+ e.Message);
           }
        }
        private void ContinueStatement() {
            funcStack.Peek().Continue();
        }
        private void BreakStatement() {
            funcStack.Peek().Break();
        }
        private void FuncTypeDefinition(ParseTreeNode node) {
            if (node == null) return;
            BNF bnf = GetBNF(node);
            switch (bnf) {
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
        private void SimpleStatement(ParseTreeNode node) {
            if (node == null) return;
            BNF bnf = GetBNF(node);
            switch (bnf) {
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
        private void StructedStatement(ParseTreeNode node) {
            if (node == null) return;
            BNF bnf = GetBNF(node);
            switch (bnf) {
                case BNF.if_statement:
                    IfStatement(node);
                    break;
                case BNF.for_statement:
                    ForStatement(node);
                    break;
                case BNF.while_statement:
                    WhileStatement(node);
                    break;
                default:
                    foreach (var childNode in node.ChildNodes) {
                        StructedStatement(childNode);
                    }
                    break;
            }
        }
        private void WhileStatement(ParseTreeNode node) {
            if (node == null) return;
            var ownerFunc = funcStack.Peek();
            ownerFunc.While(Expression(node.ChildNodes[1]).Operand);
            ScopeBody(node.ChildNodes[2]);
            ownerFunc.End();
        }
        private void IfStatement(ParseTreeNode node) {
            if (node == null) return;
            var ownerFunc = funcStack.Peek();
            ownerFunc.If(Expression(node.ChildNodes[1]).Operand);
            ScopeBody(node.ChildNodes[2]);
            ownerFunc.Else();
            ScopeBody(node.ChildNodes[3]);
            ownerFunc.End();
        }
        private void ForStatement(ParseTreeNode node) {
            if (node == null) return;
            PushScope();
            var ownerFunc = funcStack.Peek();
            var enumerable = MemberAccess(node.ChildNodes[3]);
            if (enumerable.Type != "list") {
                throw new Exception("Not enumerable");
            }
            var iterator = ownerFunc.ForEach(typeof (int), enumerable.Operand);
            AddVarToVarTable(GetTokenText(node.ChildNodes[1]), new ZOperand(iterator, "long") );
            ScopeBody(node.ChildNodes[4]);
            ownerFunc.End();
            PopScope();
        }
        private void RetStatement(ParseTreeNode node) {
            if (node == null) return;
            BNF bnf = GetBNF(node);
            switch (bnf) {
                case BNF.escape_statement:
                    EscapeStatement(node);
                    return;
                case BNF.return_statement:
                    ReturnStatement(node);
                    return;
                default:
                    break;
            }
            foreach (var child in node.ChildNodes)
                RetStatement(child);
        }
        private void ReturnStatement(ParseTreeNode node) {
            if (node == null) return;
            BNF bnf = GetBNF(node);
            var ownerFunc = funcStack.Peek();
            var returnValue = Expression(node.ChildNodes[1]);
            ownerFunc.Return(returnValue.Operand);
        }
        private void EscapeStatement(ParseTreeNode node) {
            throw new NotImplementedException();
        }
        private void VariableDefinition(ParseTreeNode node) {
            var ownerFunc = funcStack.Peek();

            var idtList = node.ChildNodes[1].ChildNodes;
            var expList = node.ChildNodes[3].ChildNodes;
            var idtIter = idtList.GetEnumerator();
            var expIter = expList.GetEnumerator();
            
            if (idtList.Count == expList.Count) {
                while (idtIter.MoveNext() && expIter.MoveNext()) {
                    var expressionNode = expIter.Current as ParseTreeNode;
                    var variableName = GetTokenText(idtIter.Current as ParseTreeNode);
                    var tmp = Expression(expressionNode);
                    var variable = ownerFunc.Local(typeTable[tmp.Type], tmp.Operand);
                    AddVarToVarTable(variableName, new ZOperand(variable, tmp.Type));
                }
            }
            else {
                throw new Exception("Mulit Ret not Implemention");
            }
            return;
        }
        private void FunctionDefinition(ParseTreeNode node, bool isVirtual = false, bool isDerived = false) {
            var functype = GetTokenText(node.ChildNodes[1].ChildNodes[0].ChildNodes[0]);

            var funcIdt = GetTokenText(node.ChildNodes[1].ChildNodes[0].ChildNodes[1]);//function_identifier
            if (functype == "oper" ) {
                OperatorDefinition(node, funcIdt ,isVirtual);
            }
            else if (funcIdt != "_init") {
                NormalFunctionDefinition(node, funcIdt, isVirtual, isDerived);
            }
            else {
                ConstructorFunctionDefinition(node, isVirtual);
            }
        }
        private void OperatorDefinition(ParseTreeNode node, string funcIdt, bool isVirtual) {
            //  the owner of the fucntion
            TypeGen ownerType = typeStack.Peek();
            //  function header
            bool isStatic = node.ChildNodes[0].ChildNodes.Count != 0 || ownerType == defaultClass;
            //  ret Type
            Type funcRetType;
            //  parse return type
            var retNode = node.ChildNodes[2];
            if (retNode.ChildNodes.Count == 0) {
                throw new Exception("Operator Function" + funcIdt + " can't find a return type");
            }
            else {
                if (retNode.ChildNodes.Count > 2) throw new Exception("Operator Function" + funcIdt +" with too many return types");
                retNode = retNode.ChildNodes[0].ChildNodes[0];
                string retType = getTypeString(retNode);
                funcRetType = getType(retType);
            }
            
            //  parse parameter
            var paraBlockNode = node.ChildNodes[3].ChildNodes[0];
            var paraSize = 0;
            var paraNames = new List<string>();
            var paraTypes = new List<string>();
            if (paraBlockNode.ChildNodes.Count != 0) {
                var parasNode = paraBlockNode.ChildNodes[0];
                paraSize = parasNode.ChildNodes.Count;
                for (int i = 0; i < paraSize; i++) {
                    paraNames.Add(GetTokenText(parasNode.ChildNodes[i].ChildNodes[1].ChildNodes[0]));
                    var typeStr = getTypeString(parasNode.ChildNodes[i].ChildNodes[3]);
                    paraTypes.Add(typeStr);
                }
            }
            
            //    create operator func -
            MethodGen func;
            if (paraNames.Count == 1){
                var operType = getOverloadUnaryOperator(funcIdt);
                func = ownerType.Operator(operType, funcRetType, getType(paraTypes[0]), paraNames[0]);
            }
            else if (paraNames.Count == 2) {
                var operType = getOverlaodDualOperator(funcIdt);
                func = ownerType.Operator(operType, funcRetType, getType(paraTypes[0]), paraNames[0], getType(paraTypes[1]), paraNames[1]);
            }
            else throw new Exception("Operator function get too many arguemnts");

            // parse function body

            CodeGen code = func.GetCode();

            funcStack.Push(code);
            PushScope();
            for (var i = 0; i < paraSize; i++) {
                var para = code.Arg(paraNames[i]);
                AddVarToVarTable(paraNames[i], new ZOperand(para, paraTypes[i]));
            }
            var statementsNode = node.ChildNodes[3].ChildNodes[1].ChildNodes[0].ChildNodes[0].ChildNodes[0];
            //int statementSize = statementsNode.ChildNodes.Count;
            ScopeBody(statementsNode);
            PopScope();
            funcStack.Pop();
        }
        private void NormalFunctionDefinition(ParseTreeNode node, string funcIdt, bool isVirtual = false, bool isDerived = false) {
            //------------the owner of the fucntion
            TypeGen ownerType = typeStack.Peek();
            //------------function header
            bool isStatic = node.ChildNodes[0].ChildNodes.Count != 0 || ownerType == defaultClass;
            //------------ret Type
            Type funcRetType;
            var retNode = node.ChildNodes[2];
            if (retNode.ChildNodes.Count == 0) {
                funcRetType = typeof(void);
            }
            else {
                retNode = retNode.ChildNodes[0].ChildNodes[0];
                string retType = getTypeString(retNode);
                funcRetType = getType(retType);
            }
            
            // create func
            var funcOpt = (Convert.ToInt32(isVirtual) << 1) + Convert.ToInt32(isStatic);
            MethodGen func;
            switch (funcOpt) {
                case 0:
                    func = ownerType.Public.Method(funcRetType, funcIdt);
                    break;
                case 1:
                    func = ownerType.Public.Static.Method(funcRetType, funcIdt);
                    break;
                case 2:
                    if (isDerived)
                        func = ownerType.Public.Override.Method(funcRetType, funcIdt);
                    else
                        func = ownerType.Public.Virtual.Method(funcRetType, funcIdt);
                    break;
                case 3:
                    func = ownerType.Public.Virtual.Static.Method(funcRetType, funcIdt);
                    break;
                default:
                    throw new Exception("Function option error(which seems to be impossible)");
            }

            //-------------parse parameter
            var paraBlockNode = node.ChildNodes[3].ChildNodes[0];
            var paraSize = 0;
            var paraNames = new List<string>();
            var paraTypes = new List<string>();
            if (paraBlockNode.ChildNodes.Count != 0) {
                var parasNode = paraBlockNode.ChildNodes[0];
                paraSize = parasNode.ChildNodes.Count;
                for (int i = 0; i < paraSize; i++) {
                    paraNames.Add(GetTokenText(parasNode.ChildNodes[i].ChildNodes[1].ChildNodes[0]));
                    var typeStr = getTypeString(parasNode.ChildNodes[i].ChildNodes[3]);
                    paraTypes.Add(typeStr);
                    func = func.Parameter(getType(typeStr), paraNames[i]);
                }
            }
            
            // parse function body
            CodeGen code = func.GetCode();
            funcStack.Push(code);
            PushScope();
            for (var i = 0; i < paraSize; i++) {
                var para = code.Arg(paraNames[i]);
                AddVarToVarTable(paraNames[i], new ZOperand(para, paraTypes[i]));
            }
            var statementsNode = node.ChildNodes[3].ChildNodes[1].ChildNodes[0].ChildNodes[0].ChildNodes[0];
            ScopeBody(statementsNode);
            PopScope();
            funcStack.Pop();
        }
        private void ConstructorFunctionDefinition(ParseTreeNode node, bool isVirtual) {
            TypeGen ownerType = typeStack.Peek();
            ConstructorGen ctr = ownerType.Public.Constructor();

            var paraBlockNode = node.ChildNodes[3].ChildNodes[0];
            var paraSize = 0;
            var paraNames = new List<string>();
            var paraTypes = new List<string>();
            if (paraBlockNode.ChildNodes.Count != 0)            {
                var parasNode = paraBlockNode.ChildNodes[0];
                paraSize = parasNode.ChildNodes.Count;
                for (int i = 0; i < paraSize; i++) {
                    paraNames.Add(GetTokenText(parasNode.ChildNodes[i].ChildNodes[1].ChildNodes[0]));
                    var typeStr = getTypeString(parasNode.ChildNodes[i].ChildNodes[3]);
                    paraTypes.Add(typeStr);
                    ctr = ctr.Parameter(getType(typeStr), paraNames[i]);
                }
            }

            CodeGen code = ctr.GetCode();
            funcStack.Push(code);
            PushScope();
            for (var i = 0; i < paraSize; i++) {
                var para = code.Arg(paraNames[i]);
                AddVarToVarTable(paraNames[i], new ZOperand(para, paraTypes[i]));
            }
            var statementsNode = node.ChildNodes[3].ChildNodes[1].ChildNodes[0].ChildNodes[0].ChildNodes[0];
            ScopeBody(statementsNode);
            PopScope();
            funcStack.Pop();
        }
        private string getTypeString(ParseTreeNode node) {
            if (node.ToString() != "required_type") return null;
            node = node.ChildNodes[0];
            return node.ToString() == "simple_type" ? GetTokenText(node.ChildNodes[0]) : GetTokenText(node);
        }
        private string getTypeString(Type type) {
            if (type == typeof(int))
                return "long";
            if (type == typeof(bool))
                return "bool";
            if (type == typeof(float))
                return "real";
            if (type == typeof(char))
                return "char";
            if (type == typeof(string))
                return "string";
            return type.Name;
        }
        private Type getType(string typeStr) => typeTable[typeStr];
        
        private void AssignmentStatement(ParseTreeNode node) {
            CodeGen ownerFunc = funcStack.Peek();
            //for single assignment
            int count1 = node.ChildNodes[0].ChildNodes.Count;
            int count2 = node.ChildNodes[2].ChildNodes.Count;
            if (count1 != count2)
                throw new Exception("assignment count is not equal");
            for (int i=0; i< count1; i++) {
                ZOperand leftValue = null, rightValue = null;
                leftValue = MemberAccess(node.ChildNodes[0].ChildNodes[i]);
                rightValue = Expression(node.ChildNodes[2].ChildNodes[i]);
                ownerFunc.Assign(leftValue.Operand, rightValue.Operand);
            }
        }
        private void TypeDefinition(ParseTreeNode node) {
            // type init
            var isDerived = node.ChildNodes[2].ChildNodes.Count > 0;
            var typeName = GetTokenText(node.ChildNodes[1]);
            TypeGen thisType;
            if (isDerived) {
                var baseName = GetTokenText(node.ChildNodes[2].ChildNodes[1]);
                if (!typeTable.ContainsKey(baseName)) throw new NotImplementedException();
                var baseType = typeTable[baseName];
                thisType = ag.Public.Class(typeName, baseType);
            }
            else {
                thisType = ag.Public.Class(typeName);
            }
            typeTable.Add(typeName, thisType);
            typeStack.Push(thisType);
            PushScope();
            
            var structured = node.ChildNodes[3];
            foreach (var member in structured.ChildNodes) {
                switch (member.ChildNodes[0].Term.Name) {
                    case "member_variable":
                        TypeMemVarDef(member.ChildNodes[0]);
                        break;
                    case "member_function":
                        TypeMemFuncDef(member.ChildNodes[0], isDerived);
                        break;
                }
            }
            PopScope();
            typeStack.Pop();
        }
        private void TypeMemFuncDef(ParseTreeNode node, bool isDerived) {
            var isVirtual = node.ChildNodes[0].ChildNodes.Count == 1;
            FunctionDefinition(node.ChildNodes[1], isVirtual, isDerived);
            //throw new NotImplementedException();
        }
        private void TypeMemVarDef(ParseTreeNode node) {
            var typeStr = getTypeString(node.ChildNodes[3]);
            var varType = getType(typeStr);
            var ownerType = typeStack.Peek();
            var fieldIdt = GetTokenText(node.ChildNodes[1]);
            var member = ownerType.Public.Field(varType, fieldIdt);
            AddVarToVarTable(fieldIdt, new ZOperand(member, typeStr));
        }
        private void AccessStatement(ParseTreeNode node) {
            MemberAccess(node,true);
        }
        private Operator getOverloadUnaryOperator(string oper) {
            switch (oper) {
                case "+":
                    return Operator.Plus;
                case "-":
                    return Operator.Minus;
                case "!":
                    return Operator.LogicalNot;
                case "~":
                    return Operator.Not;
                default:
                    throw new Exception("invalid unary operator " + oper);
            }
        }

        private Operator getOverlaodDualOperator(string oper) {
            switch (oper) {
                case "+":
                    return Operator.Add;
                case "-":
                    return Operator.Subtract;
                case "*":
                    return Operator.Multiply;
                case "/":
                    return Operator.Divide;
                case "%":
                    return Operator.Modulus;
                case ">>":
                    return Operator.RightShift;
                case "<<":
                    return Operator.LeftShift;
                case "&":
                    return Operator.And;
                case "|":
                    return Operator.Or;
                case "^":
                    return Operator.Xor;
                case "&&":   //attention
                case "||":
                    throw new Exception("operator " + oper +" is not support for overload");
                case ">":
                    return Operator.GreaterThan;
                case "<":
                    return Operator.LessThan;
                case ">=":
                    return Operator.GreaterThanOrEqual;
                case "<=":
                    return Operator.LessThanOrEqual;
                case "==":
                    return Operator.Equality;
                case "!=":
                    return Operator.Inequality;
                default:
                    throw new Exception("invalid operator " + oper);
            }
        }
        private Operand Compute(Operand leftValue, string oper, Operand rightValue = null) {
            if (rightValue as object == null) {
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
                        throw new Exception("invalid unary operator "+ oper);
                }
            }
            else {
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
                    case "&":
                        return leftValue.BitwiseOr(rightValue);
                    case "|":
                        return leftValue.BitwiseOr(rightValue);
                    case "^":
                        return leftValue.Xor(rightValue);
                    case "&&":
                        return leftValue.LogicalAnd(rightValue);
                    case "||":
                        return leftValue.LogicalOr(rightValue);
                    case ">":
                        return leftValue.Gt(rightValue);
                    case "<":
                        return leftValue.Lt(rightValue);
                    case ">=":
                        return leftValue.Ge(rightValue);
                    case "<=":
                        return leftValue.Le(rightValue);
                    case "==":
                        return leftValue.Eq(rightValue);
                    case "!=":
                        return leftValue.Ne(rightValue);
                    case "^^":
                        throw new Exception("operator " + oper + "not implemented");
                    default:
                        throw new Exception("invalid operator " +oper);        
                }
            }
        }
        private ZOperand Compute(ZOperand left, string oper, ZOperand right = null) {
              //very important
            Operand resultValue = null;
            resultValue = right == null ? Compute(left.Operand, oper, null) : Compute(left.Operand, oper, right.Operand);

            if (resultValue == null) throw new Exception("invaild expression");
            else {
                return new ZOperand(resultValue, getTypeString(resultValue.GetReturnType(tm)));
            }
        }
        private ZOperand Expression(ParseTreeNode node) {
            CodeGen ownerFunc = funcStack.Peek();
            if (node == null) return null;
            BNF bnf = GetBNF(node);

            string oper;
            switch (bnf) {
                case BNF.number: {
                        string text = GetTokenText(node);
                        var numberType = AssistFunction.GetNumberType(text);

                        return numberType == typeof(int)  
                            ? new ZOperand(ownerFunc.Local(typeof(int), int.Parse(GetTokenText(node))), "long")
                            : new ZOperand(ownerFunc.Local(typeof(double), double.Parse(GetTokenText(node))), "real");
                    }
                case BNF.charLiteral: {
                        char ch = node.Token.Value.ToString()[0];
                        return new ZOperand(ownerFunc.Local(typeof(char), ch ),"char");
                    }
                case BNF.True:
                    return new ZOperand(ownerFunc.Local(typeof(bool), true), "bool");
                case BNF.False:
                    return new ZOperand(ownerFunc.Local(typeof(bool), false), "bool");
                case BNF.list_expression:
                    return ListExpression(node);
                case BNF.member_access:
                    return MemberAccess(node);
                case BNF.unary_expression:
                    oper = node.ChildNodes[0].ChildNodes[0].Token.Terminal.ToString();
                    return Compute(Expression(node.ChildNodes[1]), oper);
                default:
                    if (node.ChildNodes.Count == 3) {  //primmary_expression
                        oper = node.ChildNodes[1].Token.Terminal.ToString();
                        return Compute(Expression(node.ChildNodes[0]), oper, Expression(node.ChildNodes[2]));
                    }
                    else
                        return Expression(node.ChildNodes[0]);
            }
        }
        private ZOperand ListExpression(ParseTreeNode node) {
            CodeGen ownerFunc = funcStack.Peek();
            node = node.ChildNodes[0];
            if (node == null) throw new Exception("invalid list expression");
            BNF bnf = GetBNF(node);
            
            switch (bnf) {
                case BNF.list_normal_expression:
                    return ListNormalExpression(node);
                case BNF.list_select_expression:
                    return listSelectExpression(node);
                case BNF.list_string_expression:
                    return ListStringExpression(node);
                default:
                    throw new Exception("invalid list expression");    
            }      
        }
        private ZOperand MemberAccess(ParseTreeNode node, bool isAccess = false) {
            CodeGen ownerFunc = funcStack.Peek();
            var mainAccessNode = node.ChildNodes[0];//what for member_access_with_segment?
            if (mainAccessNode.ToString() == "member_access") {
                if (node.ChildNodes.Count == 1) return MemberAccess(mainAccessNode, isAccess);
                var segment = node.ChildNodes[1].ChildNodes[0];
                BnfTerm term = segment.Term;
                BNF bnf = GetBNF(segment);
                ParseTreeNode member;
                ZOperand mainAccess;
                Operand ret;
                Type type;
                switch (bnf) {
                    case BNF.argument_list_par:
                        mainAccess = FunctionAccess(mainAccessNode);
                        member = segment.ChildNodes[0];
                        //func()
                        if (member.ChildNodes.Count == 0) {
                            if (mainAccess.Operand as Object == null) {
                                //var i = foo();
                                if (typeTable.ContainsKey(mainAccess.Name)) {
                                    switch (mainAccess.Name) {
                                        case "long":
                                            type = typeof(int);
                                            ret = ownerFunc.Local(type,0);
                                            break;
                                        case "char":
                                            type = typeof(char);
                                            ret = ownerFunc.Local(type,'\0');
                                            break;
                                        case "bool":
                                            type = typeof(bool);
                                            ret = ownerFunc.Local(type,false);
                                            break;
                                        case "real":
                                            type = typeof(float);
                                            ret = ownerFunc.Local(type,(float)0.0);
                                            break;
                                        default:
                                            type = typeTable[mainAccess.Name];
                                            ret = ownerFunc.Local(exp.New(type));
                                            break;
                                    }
                                }
                                // func();
                                else if (isAccess) {
                                    try {
                                        ownerFunc.Invoke(typeTable[mainAccess.Type], mainAccess.Name);
                                    }
                                    catch (Exception e) {
                                        ownerFunc.Invoke(ownerFunc.This(), mainAccess.Name);
                                    }
                                    return null;
                                }
                                //i = func()
                                else {
                                    try {
                                        ret = st.Invoke(typeTable[mainAccess.Type], mainAccess.Name);
                                    }
                                    catch (Exception e) {
                                        ret = ownerFunc.This().Invoke(mainAccess.Name);
                                    }
                                    type = ret.GetReturnType(tm);
                                }
                            }
                            else {
                                if (isAccess) {
                                    ownerFunc.Invoke(mainAccess.Operand, mainAccess.Name);
                                    return null;
                                }
                                else {
                                    ret = mainAccess.Operand.Invoke(mainAccess.Name, tm);
                                    type = ret.GetReturnType(tm);
                                }
                            }
                        }
                        //func(paras)
                        else {
                            var paraSize = member.ChildNodes[0].ChildNodes.Count;
                            Operand[] paras = new Operand[paraSize];
                            for (var i = 0; i < paraSize; i++) {
                                paras[i] = Expression(member.ChildNodes[0].ChildNodes[i]).Operand;
                            }
                            if (mainAccess.Operand as Object == null) {
                                if (typeTable.ContainsKey(mainAccess.Name)) {
                                    type = typeTable[mainAccess.Name];
                                    ret = ownerFunc.Local(exp.New(type, paras));
                                }
                                else if (isAccess) {
                                    try {
                                        ownerFunc.Invoke(typeTable[mainAccess.Type], mainAccess.Name, paras);
                                    }
                                    catch (Exception e) {
                                        ownerFunc.Invoke(ownerFunc.This(), mainAccess.Name, paras);
                                    }
                                    return null;
                                }
                                else {
                                    try {
                                        ret = st.Invoke(typeTable[mainAccess.Type], mainAccess.Name, paras);
                                    }
                                    catch (Exception e) {
                                        ret = ownerFunc.This().Invoke(mainAccess.Name, paras);
                                    }
                                    type = ret.GetReturnType(tm);
                                }
                            }
                            else {
                                if (isAccess) {
                                    ownerFunc.Invoke(mainAccess.Operand, mainAccess.Name, paras);
                                    return null;
                                }
                                else {
                                    ret = mainAccess.Operand.Invoke(mainAccess.Name, tm, paras);
                                    type = ret.GetReturnType(tm);
                                }
                            }
                        }
                        return new ZOperand(ret, getTypeString(type));
                    case BNF.array_indexer:
                        mainAccess = MemberAccess(mainAccessNode,false);
                        member = segment.ChildNodes[0];
                        var index = Expression(member);
                        return new ZOperand(mainAccess.Operand[tm, index.Operand], "long");
                    case BNF.dot:
                        mainAccess = MemberAccess(mainAccessNode,false);
                        member = node.ChildNodes[1].ChildNodes[1];
                        string fieldStr = GetTokenText(member);
                        var field = mainAccess.Operand.Field(fieldStr, tm);
                        return new ZOperand(field, field.GetReturnType(tm).Name);
                    case BNF.cast:
                        mainAccess = MemberAccess(mainAccessNode, false);
                        member = node.ChildNodes[1].ChildNodes[1];
                        var dstTypeStr = GetTokenText(member);
                        ret = mainAccess.Operand.Cast(typeTable[dstTypeStr]);
                        return new ZOperand(ret, ret.GetReturnType(tm).Name);
                }
            }
            else {
                var idtExt = GetTokenText(mainAccessNode.ChildNodes[0]);
                if (node.ChildNodes.Count != 1) {
                    var segment = node.ChildNodes[1].ChildNodes[0];
                }
                else {
                    return GetVar(idtExt);
                }
            }
            return MemberAccess(mainAccessNode);
        }
        private ZOperand FunctionAccess(ParseTreeNode node) {
            CodeGen ownerFunc = funcStack.Peek();
            ZOperand mainAccess;
            var mainAccessNode = node.ChildNodes[0];//what for member_access_with_segment?
            if (mainAccessNode.ToString() == "member_access") {
                if (node.ChildNodes.Count != 1) {
                    var segment = node.ChildNodes[1].ChildNodes[0];
                    BnfTerm term = segment.Term;
                    BNF bnf = GetBNF(segment);
                    ParseTreeNode member;
                    switch (bnf) {
                        case BNF.dot:
                            mainAccess = MemberAccess(mainAccessNode);
                            member = node.ChildNodes[1].ChildNodes[1];
                            var var = GetTokenText(member);
                            mainAccess.Name = var;
                            return mainAccess;
                        default:
                            throw new Exception("FunctionAccess meet a invalid syntax");
                    }
                }
            }
            else {
                var idtExt = GetTokenText(mainAccessNode.ChildNodes[0]);
                if (node.ChildNodes.Count != 1) {
                    throw new Exception("identifier is multi");
                }
                else {
                    return new ZOperand(null, typeStack.Peek().Name, idtExt);
                }
            }
            return FunctionAccess(mainAccessNode);
        }
        private BNF GetBNF(ParseTreeNode node) {
            try {
                if (node.Token != null) {
                    lineNumber = node.Token.Location.Line;
                    columnNumber = node.Token.Location.Column;
                    return (BNF)Enum.Parse(typeof(BNF), node.Token.Terminal.ToString());
                }
                else return (BNF)Enum.Parse(typeof(BNF), node.Term.ToString());
            }
            catch (Exception e) {
                return BNF.program;
            }
        } 
        private string GetTokenText(ParseTreeNode node) {
            if (node.Token != null) {
                lineNumber = node.Token.Location.Line;
                columnNumber = node.Token.Location.Column;
                return node.Token.Text;
            }
            else if(node.ToString() == "required_type") {
                return GetTokenText(node.ChildNodes[0]);
            }
            else if (node.ToString() == "simple_type") {
                return GetTokenText(node.ChildNodes[0]);
            }
            else if (node.ToString() == "list_type") {
                return GetTokenText(node.ChildNodes[0]);
            }
            else return null;
        }
        private void AddVarToVarTable(string varName, ZOperand zOperand) {
            if (varTable.Peek().ContainsKey(varName)) throw new Exception("variable " + varName + " can't have duplicate definition");
            varTable.Peek()[varName] = zOperand;
        }
        private void PushScope() {
            varTable.Push(new Dictionary<string, ZOperand>());
        }
        private void PopScope(string [] varNames = null) {
            Dictionary<string, ZOperand> curScope = varTable.Peek();
            var escapeVars = new List<ZOperand>();
            if(varNames != null) {
                foreach (var varName in varNames) {
                    if (curScope.ContainsKey(varName))
                        escapeVars.Add(curScope[varName]);
                    else
                        throw new Exception("the Escaping Var: " + varName + " is not in the current Scope");
                }
                varTable.Pop();
                varTable.Peek().Concat(curScope);
            }
            varTable.Pop();
        } 
        private ZOperand GetVar(string varName) {
            if (typeTable.ContainsKey(varName)) {
                return new ZOperand(null, typeTable[varName].Name);
            }
            foreach(var scope in varTable) {
                if (scope.ContainsKey(varName)) {
                    return scope[varName];
                }
            }
            var ownerFunc = funcStack.Peek();
            var field = ownerFunc.This().Field(varName);
            return new ZOperand(field, field.GetReturnType(tm).Name);
            throw new Exception("Var: " + varName + " can not be found!");
        } 
        private ZOperand ListNormalExpression(ParseTreeNode node) {
            var ownerFunc = funcStack.Peek();
            var result = ownerFunc.Local(exp.New(typeof(list)));
            //expression_list add one by one
            var expression_list = node.ChildNodes[0];
            if (expression_list == null) throw new Exception("invalid list normal Expression");
            foreach (var expression_node in expression_list.ChildNodes) {
                var temp = Expression(expression_node).Operand; 
                if (temp.GetReturnType(tm) != typeof(int))
                    throw new Exception("type of list element must be long");
                ownerFunc.Invoke(result,"Append",temp);
            }
            return new ZOperand(result,"list");
        }
        private string CreateSelectFunction(ParseTreeNode node) {
            var ownerType = typeStack.Peek();
            var funcName = selectFunc + SelectFuncNumber;
            SelectFuncNumber++;
            CodeGen ownerFunc = ownerType.Public.Static.Method(typeof(list),funcName).Parameter(typeof(list),"arr");
            funcStack.Push(ownerFunc);
            PushScope();
            string itemName = GetTokenText(node.ChildNodes[1]);
          
            var result = ownerFunc.Local(exp.New(typeof(list)));

            var item = ownerFunc.ForEach(typeof(int),ownerFunc.Arg("arr") );
            AddVarToVarTable(itemName, new ZOperand(item, getTypeString(item.GetReturnType(tm))));

            ownerFunc.If(Expression(node.ChildNodes[5]).Operand);
            ownerFunc.Invoke(result, "Append", item);
            ownerFunc.End(); //end for if 

            ownerFunc.End(); //end for end

            ownerFunc.Return(result);
            //var item = 
            PopScope();
            funcStack.Pop();
            return funcName;
        }
        private ZOperand listSelectExpression(ParseTreeNode node) {
            var ownerType = typeStack.Peek();
            var ownerFunc = funcStack.Peek();
            var funcName = CreateSelectFunction(node);
            var from = Expression(node.ChildNodes[3]).Operand;
            var result = ownerFunc.Local(typeof(list), st.Invoke(ownerType, funcName, from));
            return new ZOperand(result, "list");
        }
        private ZOperand ListStringExpression(ParseTreeNode node) {
            var ownerFunc = funcStack.Peek();
            string str = node.ChildNodes[0].Token.Value.ToString();
            if (str == null) throw new Exception("invalid string expression");
            var result = ownerFunc.Local(typeof(string),str);
            return new ZOperand(result,"string");
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
            cast,
            function_definition,
            definition,
            ret_statement,
            escape_statement,
            return_statement,
            if_statement,
            list_expression,
            list_normal_expression,
            list_select_expression,
            list_string_expression,
            for_statement,
            break_statement,
            continue_statement,
            while_statement,
            charLiteral,
            True,
            False
        }
    }
}