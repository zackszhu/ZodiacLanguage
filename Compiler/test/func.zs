func foo : long {
    param i = long;
    param k = long;

    IO.writeln(i);
    return k + 1;
}

func foo : long {
    param i = long;

    IO.writeln(i);
    return i + 100;
}

IO.writeln(foo(2));
