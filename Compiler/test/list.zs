
var i = [1,2,5,2,3,4,2,1];
IO.writeln(i);
var j = from item in i where item % 2 ==0 select item;
IO.writeln(j);
var s = "456789";
IO.writeln(s); 
