@{
type foo{

	var i = long;

    func _init
	{
        i = 100;
	}
}



type bar
{
    var i = long;
    var j = long;

    func _init
    {
        param x = foo;
        i = x.i;
        j = x.i;
    }


    func barbar : long
    {
        return j;
    }

    func barbar : long
    {
    	param x = long;
    	return x;
    }

    oper + : long
    {
    	param x = bar;
    	param y = bar;
    	return x.i + y.i;
    }

}

}@
var a = 1;
if (a == 1) {
    IO.writeln(100);
}
else if (a == -1) {
    IO.writeln(a);
}

@{var f = foo();
var b = bar(f);
var j = long();
 @@var l = list(0,5,1);
var i= 5;
i = b + b;
@@i = b.barbar(66);
IO.writeln(i);
}@
