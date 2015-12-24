# MyLang: Z Language

## Declaring a Variable

```
var a = long(1); @ declare a new variable a and assign long type 1 to it
var b = a; @ declare a new variable b and assign a to it
```

For every statement, `;` is required which means the end of the statement.

For every declaration, a initial number must be assigned to it , even it's just used to show the type of the variable, like this `var c = long;`.

## Assigning a Variable

```
a = b; @ assign b to a
```

Z language also support multiple assignments like this:

```
a, b = c, d; @ assign c to a and d to b
a, b = b, a; @ swap the value of a and b
```

If a function returns multiple values then it's like this:

```
maxN, minN = getMaxMin();
```

What is worthy mentioned is that when assigning list, this assignment is just a shadow copy, so are user-defnined types.

If deep copy is required, a new assignment operator is supported `:=`.

## Operators

Common operators are supported:

- Arithmetic Operators

```
    +, -, *, /, %, ^^
```

- Boolean Operators

```
    ==, !=, >, <, >=, <= , &&, ||, !, &, |, ^
```

## Primitive Types

```
var number = long(42); @ number = 42
var float_number = real(3.14); @ float_number = 3.14
var ch = char('a'); @ ch = 'a'
@same as var ch = char("a");
var flag = bool(True); @ flag = True(or False)

var lst = list(1, 3); @ lst = [1, 2]
var lst_2 = list[2,4](1); @ lst_2 = [1, 1, 1] where lst_2[2] = lst_2[3] = lst_2[4] = 1
var str = ["Hello"];
```

Type itself is also a value which means the default value of this type.

```
var number = long; @ number = 0
var float_number = real; @ float_number = 0.0
var ch = char; @ ch = '\0'
var flag = bool; @ flag = False
var lst = list; @ lst = []
```


## User-defined Types

```
type Foo <- Bar { @ type Bar is the parent type of type Foo
    var i = long; @ defining type members which are all public and their default initialized value

    func _init { @ defining constructor
        param _i = long; @ the parameters of a function which will be introduced later
        do_something();
        @ constructor has no return value
    }

    func NAME { @ defining member functions
        do_something();
        return ret;
    }
}
```

What should be mentioned is that all member variables are public and will override the parent class's variable.

There can be only one constructor and  `_` must be wrote before `init`.

More about functions will be introduced later.

## If Statement

```
if (True) {
    do_something();
}
else {
    do_other_thing();
}
```

## Loop Statement

- For Statement

```
for i in list(1, 3) { @ i iterrates from 1 to 2
    do_something();
}
```

- While Statement

```
while (True) {
    do_something();
}
```

## Functions

```
func Foo{
    @ parameters of the function
    param flag = bool;
    param i, j = long, long;

    @ body
    do_something();

    @ returns
    return ret_1, ret_2;
}
```

## List Inline Query(LIQ)

```
var even_numbers = [
    from item in list(1, 100)
    where item % 2 == 0
    select item
];
```

## Comments

```
@ This is a one-line comment.
@ {
    This is a multiple-line comment.
    This is the second line.
}
```

## IO Suppurts

There is a `IO` type that can be used to read and write.

```
var num = long;
IO.read(num);
IO.write(["Hello world! "]).write(num);
```

Files can be opened as:

```
var infile = IO.open(["a.txt"], "r"); @ "r" means read-only, "w" means write-only, "a" means append only, "m" means read&write supported
var number = long;
infile.read(number);
infile.write(["Hello world! "]).write(number);
```

## Example Program

- Hello World

```
IO.writeln(["Hello world!"]);
```

- Quick Sort

```
func qsort{
    param arr = list;

    return qsort([
        from item in arr
        where item < arr[0]
        select item
    ]) + [
        from item in arr
        where item == arr[0]
        select item
    ] + qsort([
        from item in arr
        where item > arr[0]
        select item
    ]);
}
```
