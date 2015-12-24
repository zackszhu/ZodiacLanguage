
var n = 0;
n = IO.readln();
var l = list();
for i in list(0, n) {
    l.Append(IO.readln());
}

for i in list(0, n) {
    var max = l[i];
    var maxi = i;
    for j in list(i + 1, n) {
        if (l[j] > max) {
            max = l[j];
            maxi = j;
            IO.writeln(max);
        }
    }
    IO.writeln(i);
    l[maxi] = l[i];
    l[i] = max;
    IO.writeln(l);
}

IO.writeln(l);
