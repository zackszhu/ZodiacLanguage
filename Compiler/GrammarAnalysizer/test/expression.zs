type s{

	var i = long;

	func ok : long
	{
    	param ii = long;
    	param j = long;

    	i = ii;
    	return i;
	}	
}





var ss = s();
var i = ~111;
var j = 44 + 99 / 66  * 55 + ss.ok(33, 2);

@@ ^^ 没实现
