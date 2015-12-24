var l = list(0,5);
var ll = list(10,15);
var lll = l + ll;
lll.Append(100000);
IO.writeln(lll[12]);

var a = 100;
if (a == 1) {
    IO.writeln(1);
}
else {
    IO.writeln(a);
}
@{
a = lll[12];
if (a == 100000) {
    IO.writeln(2);
}
}@
