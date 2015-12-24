# MyLang: the Z Language

## 类型系统

### 基本类型

整型：支持范围为-2147483648~2147483647，为长整形，默认值为0。举例:42, -32768等。

浮点型：支持范围为-1.79769e+308~-2.22507e-308，2.22507e-308~1.79769e+308，为64位浮点型，默认值为0.0。举例：3.1415926535，-11939.8等。

字符：支持ASCII编码范围为0~256范围的字符，默认ASCII编码为0。举例：'a', "z"等。（单双引号皆可）

布尔型：支持True和False，不支持用1和0代替，默认值为False。

列表：支持列表类型，默认值为[]。列表中每一项的类型需相同，当列表中每一项都是字符时，该列表就是字符串，即字符串属于一种特殊的列表。举例：[1, 1, 2, 3, 5]，["hello"]等。

函数：函数是一种类型，但是不支持匿名函数功能。

### 表达式

支持多种运算操作符、布尔操作符和位运算操作符。

```
1 + 1 @ 加法，得2
[1, 2] + [3, 4] @ 加法，得[1, 2, 3, 4]
2 - 1 @ 减法，得1
6 * 7 @ 乘法，得42
[1] * 5 @ 乘法，得[1, 1, 1, 1, 1]
81 / 9 @ 除法，得9
45 % 7 @ 取模，得3
2 ^^ 3 @ 幂次，得8
```

```
1 == 1 @ 判等运算，得True
1 != 1 @ 判不等运算，得False
1 > 2 @ 判大于运算，得False
1 < 2 @ 判小于运算，得True
1 >= 2 @ 判大于等于运算，得False
1 <= 2 @ 判小于等于运算，得False
True && False @ 与运算，得False
True || False @ 或运算，得True
!True @ 非运算，得False
```

```
1 & 2 @ 按位与运算，得0
1 | 2 @ 按位或运算，得3
1 ^ 1 @ 按位抑或运算，得0
~0 @ 按位取反运算，得-1
1 << 2 @ 左移运算，得4
2 >> 1 @ 右移运算，得1
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
14 | `=` | 从左到右

### 变量定义

变量定义时使用`var`作为关键字，在定义时需要声明变量类型，以`;`结束语句。

```
var number = long(42); @ number = 42
var float_number = real(3.14); @ float_number = 3.14
var ch = char('a'); @ ch = 'a'
@same as var ch = char("a");
var flag = bool(True); @ flag = True(or False)

var lst = list(1, 3); @ lst = [1, 2]
var lst_2 = list[2,4](1); @ 列表的下标为2到4，长度为3，并且填充为1。
var lst_3 = list[2, 4](1, 4)；
@{
    列表的下标为2到4，长度为3，并且填充值为1, 2, 3。
    如果填充列表小于列表长度，其余项填充0。
    如果填充列表大于列表长度，多余部分截去。
@}
var str = ["Hello"];
```

如果想使用类型的默认值，可以这样写：

```
var number = long; @ number = 0
var float_number = real; @ float_number = 0.0
var ch = char; @ ch = '\0'
var flag = bool; @ flag = False
var lst = list; @ lst = []
```

### 函数定义

函数定义时采用`func`关键字，在函数体内分为三部分，参数设定块、函数执行块和返回值块。

在参数设定块中，采用`param`关键字来定义函数的形参，需要规定类型。如果有默认值，需要放至设定块的尾部。

在返回值块中，采用`return`关键字来确定返回值，可以返回多值，用`,`分隔。如果没有返回值，则直接`return;`

举例如下：

```
func Foo{
    @ 参数设定块
    param flag = bool;
    param i, j = long, long;
    param k = long(1);

    @ 函数执行块
    do_something();

    @ 返回值块
    return ret_1, ret_2;
}
```

### 函数调用

在上面的代码片段中`do_something()`就是一个简单的代码调用的例子，它能够调用调用事先定义好的函数。

如果函数中有形参，那么把传的参数写在括号中，以`,`分隔。

举例如下：
```
do_something(foo, bar);
```

### 变量赋值

我们可以采用`=`关键字将等号右边的对象赋值给等号左边的对象，同时Z语言也支持多重赋值，将等号右边的对象一次赋值到等号左边的对象中。

举例如下：

```
a = b; @ 把b的对象赋值给a
a, b = c, d; @ 把c的对象赋值给a，把d的对象赋值给b
a, b = b, a; @ 交换a和b的对象
```

如果函数的返回值有多个，那么等号左边需要有两个变量来接收。

举例如下：

```
maxN, minN = getMaxMin();
```

对于列表类型和用户自定义类型，除了对象的赋值（浅拷贝）之外，还有值的赋值（深拷贝），采用`:=`关键字。

举例如下：
```
var lst_1 = list(1, 2);
var lst_2 = lst_1;
var lst_3 := lst_1;
lst_1[0] = 3; @ 此时lst_2[0] = 3，而lst_3[0] = 1
```

### 用户自定义类型

在这部分，用一个实例来说明这部分的语法：

```
type Foo <- Bar { @ Foo继承Bar类型，可以采用多重继承，用','隔开
    var i = long; @ 定义类型的成员变量，所有成员变量都是外部可见的。

    func _init { @ 定义构造器
        param ii = long;
        do_something();
        @ 构造器没有返回值
    }

    func functionA { @ 定义成员函数
        do_something();
        return ret;
    }

    family func functionB { @ 定义虚函数
        do_something();
        return;
    }

    static func functionC { @ 定义静态函数
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
    @ 此时，在该作用域内，a = 2, b = 1
}
```

## 注释

在程序中，采用`@`关键字来定义注释，如果涉及多行注释，可采用注释作用域的形式。

举例如下：

```
@ 这是一条单行注释
@{
    这是一条
    多行注释
@}
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
for i in list(1, 3) { @ i从[1, 2]的列表中以此迭代
    do_something();
}
```

#### `while`语句

在`while`语句中，当括号中的条件语句成立时，不断循环，举例如下：

```
while (True) { @ 此处将不断循环
    do_something();
}
```

#### 循环控制语句

如果我们在循环体中需要中断循环或者提前进入下一个循环节，可以使用`break`和`continue`。

### 列表内联查询语句(List Inline Query, LIQ)

在生成列表时，为了简化代码，增强列表的功能，Z语言提供了一种内联的查询语句。采用了`select`, `from`, `where`语句来实现，举例如下：

```
var even_numbers = [
    from item in list(1, 100)
    where item % 2 == 0
    select item
];
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
var boo = Bar(foo); @ 被转换的对象，前提是有_init(Foo)函数被编写
```

基本类型中已经内置了一些显式类型转换功能，如下：

```
long => real: real(1) = 1.0
long => char: char(48) = '0'
long => bool: bool(1) = True
long => list: list(1) = [1]
real => long: long(1.6) = 1
real => list: list(3.14) = [3.14]
char => list: list("a") = ["a"]
bool => long: long(True) = 1
```

在继承关系中，如果想使用多态特性，需要显式的先辈类型转换和显式的后代类型转换，前者使用`ance()`函数，后者使用`desc()`函数，举例如下：

```
type A {
    var a = long;
    func _init {
        a = 1;
    }
    family func print {
        return a
    }
    func _init {
        param b = B;
        a = b.a;
    }
}

type B <- A {
    func _init {
        A._init();
    }
    func print {
        return a + 1;
    }
}

var screen = IO;
var lst = list[0, 1](A);
var a = A;
var b = B;
lst[0] =  A;
lst[1] = B; @ error: 列表仅包含A类型
lst[1] = A(B);
screen.write(lst[1].print()); @ 输出是1
lst[1] = ance(B);
screen.write(desc(lst[1]).print()); @ 输出是2
```

从上面的代码片段中，可见`ance()`可以将子类型暂时性地套上父类型的外套，再通过`desc()`恢复原本的对象。

事实上，`ance()`并不仅仅能够代替成父类型，它是通过类型推导，寻找该类型所有的祖先中符合规则的类型，从而进行包装。而`desc()`则是完全恢复成原来的类型。

## 输入输出语句

在Z语言中，事先默认定义了一种IO类型，用来进行输入输出操作。该类型有多种构造函数，提供多种输入输出功能：

```
var screen = IO; @ 对屏幕的输入输出对象
var infile = IO(["infile.log"], "r"); @ 对infile.log文件的读取对象
var outfile = IO(["outfile.log"], "w"); @ 对outfile.log文件的写入对象
var appfile = IO(["appfile.log"], "a"); @ 对appfile.log文件的尾部附加对象
var modfile = IO(["modfile.log"], "m"); @ 对modfile.log文件的可读可写对象

var number = long;
screen.read(number); @ 从对象中读取变量值
screen.write(number); @ 向对象中写入变量值
```

当同时需要读入多个不同类型的值时，可以采用连续调用的方式，举例如下：

```
var screen = IO;
screen.write(["hell"]).write("o");
```

## 标准库

在Z语言中，可以采用`have`关键字来导入外部文件，这些外部文件一般以`.d`作为拓展名。
如果想采用标准库，可以使用`have stdlib.d`来导入。

### 函数

TODO

## 样例程序

见同时上传的其他文件。
