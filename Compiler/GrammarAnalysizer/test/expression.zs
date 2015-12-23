

type s
{
	var k = long;
	var l = long;
	func ok : long
	{
    	param x = long;
    	param y = long;
    	k = x + y;
    	l = x - y ;
    	return k;
	}

	oper + : long
	{
		param x = s;
		param y = s;

		return x.k + y.k;
	}	

}





var ss = s();

var j = 44 + 99 / 66  * 55 + ss.ok(33, 2);
var i = ss + ss;

@@ ^^ 没实现
