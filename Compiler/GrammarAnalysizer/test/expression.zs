type foo{

    var i = long;

    func _init
    {
        i = 100;
    }   
}
type bar <- foo{

    var j = long;

    func _init
    {
        param xxoo = foo;
        j = xxoo.i;
    }   

    func barbar : long {
        return j;
    }
}

var f = foo();
var b = bar(f);
var i = ~111;
var j = b.barbar();


@@ ^^ 没实现
