type Complex {
    var r = long;
    var i = long;

    func _init {
        param rr = long;
        param ii = long;
        r = rr;
        i = ii;
    }

    oper + : long {
        param a = Complex;
        param b = Complex;

        return a.r + b.r;
    }
}


var cA = Complex(1, 1);
var cB = Complex(2, 2);
IO.writeln(cA + cB);
