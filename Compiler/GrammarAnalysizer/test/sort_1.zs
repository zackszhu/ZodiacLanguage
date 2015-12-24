func qsort : list {
    param arr = list;
    if (arr.Length() <= 1)  { return arr; }
    IO.writeln(arr);

    var t = from item in arr
            where item < arr[0]
            select item;
    var tt = from item in arr
            where item == arr[0]
            select item;

    var ttt = from item in arr
            where item > arr[0]
            select item;

    IO.writeln(t);
    IO.writeln(tt);
    IO.writeln(t+tt);
    IO.writeln(ttt);
    IO.writeln(t+tt+ttt);
    return t + tt +ttt;
@{
    return qsort(
            from item in arr
            where item < arr[0]
            select item
            ) +
            from item in arr
            where item == arr[0]
            select item
            + qsort(
            from item in arr
            where item > arr[0]
            select item
        );
        }@
        
}

IO.writeln(qsort([5,2,7,2,1,3,9,10,12,4]));