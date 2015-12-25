# MyLang: the Zodiac Language

本语言是一个面向对象的编程语言，使用C#进行编程，使用gplex和gppg前端工具生成parser，然后使用llvm作为后端，最终生成机器码。

## 关键字表
Keywords|
-|-|-|-|-|-|-
`long` | `real` | `char` | `bool` | `list` | `func` | `oper`
`(` | `)` | `[` | `]` | `.` | `!` | `~`
`-` | `+` | `*` | `/` | `%` | `<<`
`>>` | `<` | `>` | `<=` | `>=` | `==` | `!=`
`&` | `^` | `｜` | `&&` | `｜｜` | `=` | `:=`
`'` | `"` | `@@` | `var` | `param` | `return` | `escape`
`;` | `,` | `type` | `<-` | `family` | `static` | `True`
`False` | `if` | `else` | `for` | `in` | `while` | `break`
`continue` | `from` | `where` | `select` | `ance` | `desc` | `IO`
`have` | `Null` | '=>'


## 类型系统

### 基本类型

整型：支持范围为-2147483648~2147483647，为长整形，默认值为0。举例:42, -32768等。

浮点型：支持范围为-1.79769e+308~-2.22507e-308，2.22507e-308~1.79769e+308，为64位浮点型，默认值为0.0。举例：3.1415926535，-11939.8等。

字符：支持ASCII编码范围为0~256范围的字符，默认ASCII编码为0。举例：'a', "z"等。（单双引号皆可）

布尔型：支持True和False，不支持用1和0代替，默认值为False。

列表：支持列表类型，默认值为[]。列表中每一项的类型必须是long，举例：[1, 1, 2, 3, 5]等。

### 表达式

支持多种运算操作符、布尔操作符和位运算操作符。

```
1 + 1 @@ 加法，得2
[1, 2] + [3, 4] @@ 加法，得[1, 2, 3, 4]
2 - 1 @@ 减法，得1
6 * 7 @@ 乘法，得42
81 / 9 @@ 除法，得9
45 % 7 @@ 取模，得3
```

```
1 == 1 @@ 判等运算，得True
1 != 1 @@ 判不等运算，得False
1 > 2 @@ 判大于运算，得False
1 < 2 @@ 判小于运算，得True
1 >= 2 @@ 判大于等于运算，得False
1 <= 2 @@ 判小于等于运算，得False
True && False @@ 与运算，得False
True || False @@ 或运算，得True
!True @@ 非运算，得False
```

```
1 & 2 @@ 按位与运算，得0
1 | 2 @@ 按位或运算，得3
1 ^ 1 @@ 按位抑或运算，得0
~0 @@ 按位取反运算，得-1
1 << 2 @@ 左移运算，得4
2 >> 1 @@ 右移运算，得1
```

各运算符的优先级如下（从高到低）：

优先级|符号|结合性
-|-|-
1 | `()`, `[]`, `.` | 从左到右
2 | `!`, `~`, `-(负号)`, `+（正号）` | 从右到左
3 | `^^` | 从右到左
4 | `*`, `/`, `%` | 从左到右
5 | `+`, `-` | 从左到右
6 | `<<`, `>>` | 从左到右
7 | `<`, `>`, `<=`, `>=` | 从左到右
8 | `==`, `!=` | 从左到右
9 | `&` | 从左到右
10 | `^` | 从左到右
11 | `｜` | 从左到右
12 | `&&` | 从左到右
13 | `｜｜` | 从左到右
14 | `=`, `:=` | 从左到右

### 变量定义

变量定义时使用`var`作为关键字，在定义时可以声明变量类型，或者直接赋值立即数，以`;`结束语句。

```
var number1 = 42; var number2 = long(42); @@ number = 42 
var float_number = real(3.14); @@ float_number = 3.14
var ch = char('a'); @@ ch = 'a'
@same as var ch = char("a");
var flag = True; @@ flag = True(or False)

var lst = list(1, 3); @@ lst = [1, 2]
var lst_2 = list(2,4,1); @@ 列表的下标为2到3，长度为2，并且填充为1。
var lst_3 = list(2,4,1, 4)；
@{
    列表的下标为2到3，长度为2，并且填充值为1, 2。
    如果填充列表小于列表长度，其余项填充0。
    如果填充列表大于列表长度，多余部分截去。
}@
```

如果想使用类型的默认值，可以这样写：

```
var number = long; @@ number = 0
var float_number = real; @@ float_number = 0.0
var ch = char; @@ ch = '\0'
var flag = bool; @@ flag = False
var lst = list; @@ lst = []
```

### 函数定义

函数定义时采用`func`关键字，如果有返回值类型，在函数名`:`之后说明返回类型（类型唯一）。在函数体内分为两部分，参数设定块和函数执行块。

在参数设定块中，采用`param`关键字来定义函数的形参，需要规定类型。如果有默认值，需要放至设定块的尾部。

在`param`后需要定义函数的传值方式，如果是传值调用，则采用`:=`，如果是传址调用，则采用`=`进行定义。如果有默认值，则必须使用传值定义。

可以将`func`作为参数类型，但是其默认值只能是`Null`。

在函数执行块中，采用`return`关键字来确定返回值。如果没有返回值，则直接`return;`

举例如下：

```
func Foo : long{
    @@ 参数设定块
    param flag = bool;
    param i, j := long, long; @@ 传值
    param k := long(1); @@ 传值

    @@ 函数执行块
    do_something();
    return ret_1;
}
```

### 函数调用

在上面的代码片段中`do_something()`就是一个简单的代码调用的例子，它能够调用调用事先定义好的函数。被调用的函数将加入堆栈，并在函数结束时弹出。

如果函数中有形参，那么把传的参数写在括号中，以`,`分隔。

举例如下：
```
do_something(foo, bar);
```

### 函数重载

如果函数的形参类型不同，则可以进行重载，举例如下：

```
func foo {
    param i = long;
    return i;
}

func foo {
    param i = bool;
    return !i;
}
```

在调用时如果采用`long`类型调用，则调用第一个`foo`函数，如果采用`bool`类型调用，则调用第二个`foo`函数。


#### 操作符重载

如果需要重载的函数是一个操作符，则需要使用`oper`关键字，举例如下：

```
oper + : long {
    param a = Foo;
    param b = Foo;
    return a.bar + b.bar;
}
```

### 变量赋值

我们可以采用`=`关键字将等号右边的对象赋值给等号左边的对象，同时Z语言也支持多重赋值，将等号右边的对象一次赋值到等号左边的对象中。

举例如下：

```
a = b; @@ 把b的对象赋值给a
a, b = c, d; @@ 把c的对象赋值给a，把d的对象赋值给b
a, b = b, a; @@ 交换a和b的对象
```


### 用户自定义类型

在这部分，用一个实例来说明这部分的语法：

```
type Foo <- Bar { @@ Foo继承Bar类型，可以采用多重继承，用','隔开
    var i = long; @@ 定义类型的成员变量，所有成员变量都是外部可见的。

    func _init { @@ 定义构造器
        param ii = long;
        do_something();
        @@ 构造器没有返回值
    }

    func functionA { @@ 定义成员函数
        do_something();
        return ret;
    }

    family func functionB { @@ 定义虚函数
        do_something();
        return;
    }

    static func functionC { @@ 定义静态函数
        do_something();
        return;
    }
}
```

## 作用域

在程序中，采用`{`和`}`进行划分，每个变量的生命周期在一个作用域之间。如果希望某个变量的生命周期逃逸出这个作用域，保留至上一层作用域，则可以使用`escape`关键字。

举例如下:
```
{
    var a = long(1);
    var b = long(1);
    {
        var a = long(2);
        var b = long(2);
        escape a;
    }
    @@ 此时，在该作用域内，a = 2, b = 1
}
```

### 垃圾回收机制

由作用于的规则，我们采用RAII的形式进行资源的分配与回收。在对象定义时就进行构造，并分配资源。在一个作用域结束时，回收由于该作用域产生的新分配资源，对于已`escape`的对象，则不进行回收，改至到下一个定义域再回收资源。

## 注释

在程序中，采用`@@`关键字来定义注释，如果涉及多行注释，可采用注释作用域的形式。

举例如下：

```
@@ 这是一条单行注释
@{
    这是一条
    多行注释
}@
```

## 语句语法

### 条件判断语句

在程序中采用`if`, `else`关键字来组成条件判断语句组，举例如下：

```
if (True) {
    do_something();
}
else {
    do_other_thing();
}
```

### 循环语句

在程序组采用`for`, `while`, `break`, `continue`, `in`关键字来组成条件判断语句组。

#### `for`语句

在`for`语句中，采用循环变量在列表中迭代的方法，进行循环，举例如下：

```
for i in list(1, 3) { @@ i从[1, 2]的列表中以此迭代
    do_something();
}
```

#### `while`语句

在`while`语句中，当括号中的条件语句成立时，不断循环，举例如下：

```
while (True) { @@ 此处将不断循环
    do_something();
}
```

#### 循环控制语句

如果我们在循环体中需要中断循环或者提前进入下一个循环节，可以使用`break`和`continue`。

### 列表内联查询语句(List Inline Query, LIQ)

在生成列表时，为了简化代码，增强列表的功能，Z语言提供了一种内联的查询语句。采用了`select`, `from`, `where`语句来实现，举例如下：

```
var even_numbers = 
    from item in list(1, 100)
    where item % 2 == 0
    select item
;
```
在这段代码中，item从1到99中迭代，当item能够被2整除时，将item加入列表，从而生成一个偶数数组。

由此可见，当`in`后的列表中某项满足`where`后的条件时，将`select`中对该项的操作后的返回值加入列表，从而实现特殊列表的实现。

## 类型推导系统

在程序编译的时候，编译器会进行类型推导。由于Z语言是强类型语言，不允许任何隐式类型转换，因此，一旦类型不匹配，就报错。

对于一个函数，它的`return`语句可以分布在整个函数体中，因此，它返回值的类型由这些语句中所返回的类型共同决定。如果每个返回值的类型不同，就提示错误，只有所有返回值的类型一致，才会使该类型称为函数的返回类型。

对于不同类型，可以采用显式的强制类型转换，类型转换的函数在`_init()`的非默认构造器中，然后通过调用，来实现类型转换，举例如下：

```
var foo = Foo;
var bar = Bar;
var boo = Bar(foo); @@ 被转换的对象，前提是有_init(Foo)函数被编写
```

基本类型中已经内置了一些显式类型转换功能，如下：

```
long => real: 1 => real = 1.0
long => char: 48 => char = '0'
long => bool: 1 => bool = True
real => long: 1.5 => long = 1
```

在继承关系中，也支持继承类型之间的转换，如果想使用多态特性，需要显式地使用转换功能使得父类子类函数相互调用，举例如下：

```type Base 
{
    var x = long;
    func _init
    {
        x = 1;
    }
    family func familyPrint
    {
        IO.writeln("Base family print:" + x);
    }
    func selfPrint
    {
        IO.writeln("Base self print:" + x);
    }
}

type Child <- Base
{
    var y = long;
    var z = real;
    func _init
    {
        y = 2;
        z = 3.5;
    }
    family func familyPrint
    {
        x = x + 1;
        IO.writeln("Child family print:" + x);
    }

    func selfPrint
    {
        IO.writeln("Child self print:" + y);
    }


}

var a = Child();
a.familyPrint();           @@ 输出Child family print:2
a.selfPrint();             @@ 输出Child self print:2
a=>Base .familyPrint();    @@ 输出Child family print:3
a=>Base .selfPrint();      @@ 输出Base self print:3
```

从上面的代码片段中，可见，子类对象可以调用自己的类型，转型到父类对象之后，如果是family函数（虚函数），就会保留子类的函数体，否则则使用原来父类的函数体。

## 输入输出语句

在Z语言中，事先默认定义了一种IO静态类型，可以调用其方法进行输入输出操作。提供以下两种输入输出功能：

```
var number = IO.readln(); @@ 读取一个整数变量值
IO.write(number); @@ 屏幕输出变量值，可以输出标准类型和字符串。
```

## 样例程序

见同时上传的其他文件。
