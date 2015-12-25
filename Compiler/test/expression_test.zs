var number = 42;
IO.writeln(number);
number = 1;
IO.writeln(number);

var float_number = 3.14;
IO.writeln(float_number);

var a, b = 3, 4;
IO.writeln(a);
IO.writeln(b);

var ch = "a";
IO.writeln(ch);

var flag = number > 0;
IO.writeln(flag);

var lst = list(1, 10);
IO.writeln(lst);
lst = list(2, 4, 1); @@ 列表的下标为2到3，长度为2，并且填充为1。
IO.writeln(lst);
lst = list(2, 4, 1, 4);
IO.writeln(lst);
lst = list();
IO.writeln(lst);
lst = list(2, 10, 1, 4);
IO.writeln(lst);
IO.writeln(lst[4]);
