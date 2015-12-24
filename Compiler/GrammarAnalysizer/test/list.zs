var i = [1,2,3,4,5];
IO.writeln(i);
var j = from item in i where item < 3 select item
IO.writeln(j);
var s = "456789"
IO.writeln(s); 