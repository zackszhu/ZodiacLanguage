type Foo {
    var z = long;
    func a : long{
        return 1;
    }
}

func ok : long
{

    var x = 4;
    return x;
}

var i = ~111;
var j = 44 + 99 / 66  * 55 + ok();

@@ ^^ 没实现
