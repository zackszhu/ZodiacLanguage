have stdlib.d

func qsort{
    param arr = list;
    return qsort(
            from item in arr
            where cmp(item, arr[0]) < 0
            select item
            ) + 
            from item in arr
            where cmp(item, arr[0]) == 0
            select item
            + qsort(
            from item in arr
            where cmp(item, arr[0]) > 0
            select item
        );
}

oper < {
    param a = list;
    param b = list;

    if (Len(a) > Len(b)) {
        return -1;
    }
    if (Len(a) == Len(b)) {
        return 0;
    }
    if (Len(a) < Len(b)) {
        return 1;
    }
}

a      = [[1, 2], [1]];
result = qsort(a, myCmp);
@@ screen = IO;
screen.write(result); 