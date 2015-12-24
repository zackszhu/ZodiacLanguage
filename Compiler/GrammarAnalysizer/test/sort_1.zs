
func le : list 
{
    param arr = list;
    return from item in arr
            where item < arr[0]
            select item;
}

func eq : list 
{
    param arr = list;
    return from item in arr
            where item == arr[0]
            select item;
}

func gt : list 
{
    param arr = list;
    return from item in arr
            where item > arr[0]
            select item;
}


func qsort : list 
{
    param arr = list;
    if (arr.Length() <= 1)  { return arr; }
    IO.writeln("456789");

    var t1 = qsort(le(arr))
            + eq(arr)
            + qsort(gt(arr));
    @@IO.writeln(t1);
    @@IO.writeln(t2);
    @@IO.writeln(t3);

    return  t1;

}

func qsort2 : list
{
    param arr = list;
    if (arr.Length() <= 1)  { return arr; }
    return  qsort2(
            from item in arr
            where item < arr[0]
            select item
            ) +
            ( from item in arr
            where item == arr[0]
            select item )
            + qsort2(
            from item in arr
            where item > arr[0]
            select item);
}
        

@@IO.writeln(qsort([6,2,7,2,1,3,9,30,19,4,100,4,12]));
IO.writeln(qsort2([6,2,7,2,1,3,9,30,19,4,100,4,12]));