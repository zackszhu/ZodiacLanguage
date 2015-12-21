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
    class ZOperand
    {

        public ZOperand(Operand oper, string t, string n = null) {
            operand = oper;
            type = t;
            name = n;
        }

        public Operand operand;
        public string type;
        public string name;
    }


    internal class CodeGenerator {
        private string name;
        private Dictionary<string, ZOperand> varTable;
        private Dictionary<string, Dictionary<string, Type>> typeTable;
        private AssemblyGen ag;
        private StaticFactory st;
        private ITypeMapper tm;
        private ExpressionFactory exp;
        private TypeGen defaultClass;
        private CodeGen mainMethod;
        private TypeGen IOClass;
        private Operand IOvar;


        public CodeGenerator() {
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
            tm = ag.TypeMapper;


            varTable = new Dictionary<string, ZOperand>();
            typeTable = new Dictionary<string, Dictionary<string, Type>>();
        }

        public void InitIO() {
            IOClass = ag.Public.Class("IO");
            typeTable["IO"] = new Dictionary<string, Type>();
            typeTable["IO"]["write"] = typeof(void);
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
        public void InitRequiredType()
        {
            typeTable["long"] = new Dictionary<string, Type>();
            typeTable["long"]["ToString"] = typeof(string);
        }

        private void initTypeMethod() {
            // take long as a method
        }

        public void Generate(ParseTree parseTree) {
            if (parseTree == null) return;

            defaultClass = ag.Public.Class("Default");
            typeTable["Default"] = new Dictionary<string, Type>();
            mainMethod = defaultClass.Public.Static.Method(typeof(void), "Main");
            InitIO();
            InitRequiredType();
            AddParseNodeRec(parseTree.Root);


            IOvar = mainMethod.Local(exp.New(IOClass));
            mainMethod.Invoke(IOvar, "write", varTable["i"].operand);
            mainMethod.Invoke(IOvar, "write", varTable["j"].operand);

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
                case BNF.member_access:
                    MemberAccess(node);
                    return;
                default:
                    break;
            }
            foreach (var child in node.ChildNodes)
                AddParseNodeRec(child);
        }
        private void VariableDefinition(ParseTreeNode node , CodeGen ownerScope = null) {
            //ParseTreeNodeList childList = node.ChildNodes;
            //get variable name
            if (ownerScope == null) ownerScope = mainMethod;

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
        private void FunctionDefinition(ParseTreeNode node, TypeGen ownerType = null)
        {
            if (ownerType == null) ownerType = defaultClass;
            var isStatic = false;
            if (node.ChildNodes[0].ChildNodes.Count != 0) isStatic = true;//static
            var funcIdt = node.ChildNodes[1].ChildNodes[0].ChildNodes[1].Token.Text;//function_identifier

            
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


            var paraNode = node.ChildNodes[3].ChildNodes[0].ChildNodes[0];
            int paraSize = paraNode.ChildNodes.Count;
            var paras = new List<string>();
            for(int i = 0; i < paraSize; i++)
            {
                paras.Add(paraNode.ChildNodes[i].ChildNodes[1].ChildNodes[0].Token.Text);
                var typeStr = getTypeString(paraNode.ChildNodes[i].ChildNodes[1].ChildNodes[3]);
                func = func.Parameter(getType(typeStr), paras[i]);
            }

            CodeGen code = func.GetCode();
            var statementsNode = node.ChildNodes[3].ChildNodes[1].ChildNodes[0].ChildNodes[0].ChildNodes[0];
            //int statementSize = statementsNode.ChildNodes.Count;
            foreach(ParseTreeNode statementNode in statementsNode.ChildNodes)
            {

            }
            Operand a = code.Arg(paras[0]);
            Operand b = code.Arg(paras[1]);
            Operand ret = code.Local(typeof(int), a);

            code.Return(ret);
        }          
        private string getTypeString(ParseTreeNode node)
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
        private string getTypeString(Type type)
        {
            if (type == typeof(int))
                return "long";
            else if (type == typeof(bool))
                return "real";
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
            else if(typeStr == "list")
                return typeof(ArrayList);
            return typeof(int);
        }
        private ZOperand Expression(ParseTreeNode node ,CodeGen ownerScope = null )
        {
            if (ownerScope == null) ownerScope = mainMethod;
            if (node == null ) return null;
            BNF bnf = GetBNF(node.Token == null ? node.Term.Name : node.Token.Terminal.ToString());
            switch (bnf) {
                case BNF.number:
                   
                    return new ZOperand(ownerScope.Local(typeof(int), int.Parse(node.Token.Text)), "long");

                case BNF.member_access:
                    return MemberAccess(node);

                default:
                    //@TODO
                    if (node.ChildNodes.Count == 3)
                    {
                        return new ZOperand(Expression(node.ChildNodes[0]).operand.Add(Expression(node.ChildNodes[2]).operand), "long");

                    }
                    //return mainMethod.AssignAdd(Expression(node.ChildNodes[0]), Expression(node.ChildNodes[2])) );
                    break;
            }
            return Expression(node.ChildNodes[0]);
        }

        private ZOperand MemberAccess(ParseTreeNode node, CodeGen ownerScope = null)
        {
            if (ownerScope == null) ownerScope = mainMethod;
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
                            mainAccess = FunctionAccess(mainAccessNode, ownerScope);
                            member = segment.ChildNodes[0];
                            if(member.ChildNodes.Count == 0)
                            {
                                if(mainAccess.operand == null)
                                {
                                    //TODO
                                }
                                Operand ret = mainAccess.operand.Invoke(mainAccess.name, tm);
                                Type type = typeTable[mainAccess.type][mainAccess.name];
                                Operand a =ownerScope.Local(type, ret);
                                return new ZOperand(a, getTypeString(type));
                            }
                            else
                            {
                                int paraSize = member.ChildNodes.Count;
                                List<Operand> paras = new List<Operand>();
                                for (int i = 0; i < paraSize; i++)
                                {
                                    paras.Add(Expression(member.ChildNodes[i], ownerScope).operand);
                                }
                                Operand ret = mainAccess.operand.Invoke(mainAccess.name, tm, paras.ToArray());
                                
                            }
                            break;
                        case BNF.array_indexer:
                            mainAccess = MemberAccess(mainAccessNode, ownerScope);
                            member = segment.ChildNodes[0];
                            var index = Expression(member);
                            break;
                        case BNF.dot:
                            mainAccess = MemberAccess(mainAccessNode, ownerScope);
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

        private ZOperand FunctionAccess(ParseTreeNode node, CodeGen ownerScope = null)
        {
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
                            mainAccess = MemberAccess(mainAccessNode, ownerScope);
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
            array_indexer,
            dot,
            function_definition
        }
    }
}