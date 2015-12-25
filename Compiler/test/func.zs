func foo : long {
    param i = long;
    param k = long;

    IO.writeln(i);
    return k + 1;
}

func foo : real {
    param i = long;
    param k = real;

    IO.writeln(i);
    return k + 1.414;
}

IO.writeln(foo(2, 1.1));
