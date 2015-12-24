
func qsort : list
{
    param arr = list;
    if (arr.Length() <= 1)  { return arr; }
    return  qsort(
            from item in arr
            where item < arr[0]
            select item
            ) +
            ( from item in arr
            where item == arr[0]
            select item )
            + qsort(
            from item in arr
            where item > arr[0]
            select item);
}
        
IO.writeln(qsort([6,2,7,2,1,3,9,30,19,4,100,4,12]));