for i in list(0, 5) {
    if (i < 2) {
        continue;
    }
    if (i >= 4) {
        break;
    }
    IO.writeln(i);
    @@ 2, 3 is supposed to be printed
}
