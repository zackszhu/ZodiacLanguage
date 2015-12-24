
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


var f = foo();
var b = bar(f);
var i= 5;
i = b + b;
i = b.barbar(66);
IO.writeln(i);


