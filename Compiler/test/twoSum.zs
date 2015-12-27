IO.writeln("Input the length of the array");
var n = IO.readln();
IO.writeln("Input the array");
var l = list();
for i in list(0, n) {
    l.Append(IO.readln());
}
IO.writeln("Input the target");
var t = IO.readln();
var b = False;
IO.writeln("Answer:");
for x in list(0, n) {
    for y in list(x+1, n) {
        if (l[x] + l[y] == t) {
            b = True;
            IO.writeln(x+1);
            IO.writeln(y+1);
            break;
        }
    }
    if (b) {
        break;
    }
}
