type base 
{
	var x = long;
	func _init
	{
		x = 1;
	}
	family func print
	{
		IO.writeln("base print" + x);
	}
}

type child <- base
{
	var y = long;
	var z = real;
	func _init
	{
		y = 2;
		z = 3.5;
	}
	family func print
	{
		x = x + 1;
		IO.writeln("child print" + x);
	}
}

var a = child();
a.print();
a=>base .print();