type Base 
{
    var x = long;
    func _init
    {
        x = 1;
    }
    family func familyPrint
    {
        IO.writeln("Base family print:" + x);
    }
    func selfPrint
    {
        IO.writeln("Base self print:" + x);
    }
}

type Child <- Base
{
    var y = long;
    var z = real;
    func _init
    {
        y = 2;
        z = 3.5;
    }
    family func familyPrint
    {
        x = x + 1;
        IO.writeln("Child family print:" + x);
    }

    func selfPrint
    {
        IO.writeln("Child self print:" + y);
    }


}

var a = Child();
a.familyPrint();
a.selfPrint();
a=>Base .familyPrint();
a=>Base .selfPrint(); 