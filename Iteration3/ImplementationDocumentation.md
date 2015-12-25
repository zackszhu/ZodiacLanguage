# 实现方法

由于在第二次迭代中我们获得了代码的parse tree结构，因此只要对于特定的节点进行特别的函数调用即可。第三次迭代时我们主要用了Runsharp通过C#的反射机制来实现代码生成。

所有的代码都从`AddParseNodeRec`中开始被递归调用，在此之前会有一些全局维护性的数据结构的初始化。

主要维护的有变量表，`Stack<Dictionary<string, ZOperand>>()`，是一个根据作用域而产生的字典，记录的是变量名字和操作符。每个操作符`ZOperand`记录了类型和Runsharp反射时采用的操作符。

其次维护了一个类表，`Dictionary<string, Type>`记录的是变量名字对应的类型，在程序初始化时会把一些内建函数也写入，如下：

```
typeTable.Add("long", typeof(int));
typeTable.Add("real", typeof(double));
typeTable.Add("string", typeof(string)); // only for internal use
typeTable.Add("bool", typeof(bool));
typeTable.Add("list", typeof(list));
typeTable.Add("char", typeof(char));
typeTable.Add("IO", typeof(IO));
```

其中list和IO是我们自己包装的一个类型，详细可以参见list.cs和IO.cs

接着是维护当前所在类的堆栈和当前所在函数的堆栈，用于正确地在相应的类和函数中生成语句而不错乱。

在初始化之后，如C#一样，定义一个程序的入口，就是default类的main函数：

```
defaultClass = ag.Public.Class("Default");
typeTable.Add("Default", defaultClass);
mainMethod = defaultClass.Public.Static.Method(typeof(void), "Main");
```

接着根据不同的parse tree节点，生成不同的代码，一些常用的代码生成API如下：

```
var ownerFunc = funcStack.Peek();
var op = ownerFunc.Local(typeTable[node.typeName], expression); // 局部变量声明
AddVarToVarTable(node.name, new ZOperand(op, typeTable[node.name]));
```

类和函数的声明在上文初始化的时候有提到。

变量赋值和函数调用的API如下：

```
ownerFunc.Assign(target.Operand, value.Operand);// 赋值
ownerFunc.Invoke(funcType, funcName, params...);//void调用
op.Invoke(opType, funcName, params...);// 有返回值调用
```

控制流语句的API如下：

```
ownerFunc.If(condition.Operand);
ScopeBody(childNode);
ownerFunc.Else();
ScopeBody(childNode2);
ownerFunc.End();

ownerFunc.While(condition.Operand);
ScopeBody(childNode);
ownerFunc.End();

ownerFunc.ForEach(typeof(int), listOperand);
ScopeBody(childNode);
ownerFunc.End();
```

其他API可以详见代码。

垃圾回收方面，由于我们根据不同的作用域对应了不同的varTable，因此，当scope改变之后，变量就无法访问到了，然而对于实际的资源，是借助IL来实现的，它在每个函数的最初始部分来确定整个函数需要的局部变量，然后在函数结束的时候统一回收。

具体的IL代码可以通过ildasm.exe来查看，会在演示的时候展示。
