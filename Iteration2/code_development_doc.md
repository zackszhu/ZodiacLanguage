


###开发环境
- 环境需求：
   - Windows (>=)7 
   - .NET (>=)4.0 
   - Visual Studio (>=)2010
- 编程语言：C#


###功能介绍
完成了对在《LanguageDesignV1-2.md.html》的所描述语言的词法语法分析，并可以通过图形化界面实行以下操作：
- 浏览语法的定义内容。Nonterminal，termianl，parsing state
- 在test区输入代码
- 对代码进行parse，可以得到token流和parse tree，如果输入代码语法有错，就将报错


###代码实现
####第三方语法生成器

[Irony](https://irony.codeplex.com/) - .NET Language Implementation Kit


选用它是因为它提供了一套可以直接使用C#的重载操作符去直接设计语法逻辑的接口。使得整个语法设计模块的代码可读性非常强，格式与bnf类似。




####模块介绍
整个解决方案分为了两个工程，Compiler（语法生成）和GrammerExplorer（图形化显示）。
- Compiler：语法定义部分，完全由自己编写。主要有两个文件： ZodiacTermianlFactory.cs定义了一些非关键字的Token的词法； Program.cs定义了关键字、符号和语法。完成后将生成一个dll类库，可由GrammerExplorer导入后用于语法解析。


- GrammerExplorer： 可供用户图形化操作，这部分是我们根据Irony主页上提供的一个用于展示语法解析的样例修改的，修改内容只涉及界面，不涉及语法逻辑。导入Complier生成的类库之后就可以进行词法语法解析以及生成。


####代码逻辑
词法部分已经由Irony实现，不需要手写状态机。
语法部分以IR(1)为解析逻辑， 编写的是解析的所根据的表达式法则，内容与bnf类似。如以下代码就能分别定义二进制操作符、二元二进制运算表达式和for循环语句的语法。
```csharp
bin_operator.Rule = "^^" | "*" | "/" | "%" | "+" | "-" | "<<" | ">>" | "<" | ">" | "==" | "&"  | "^" | "|" | "&&" | "||";
bin_op_expression.Rule = expression + bin_operator + expression;
for_statement.Rule = "for" + identifier + "in" + member_access + scope;
```




####项目Git地址

https://github.com/zackszhu/ZodiacLanguage.git








