func reverse : long
{
	param num = long;

	var result = 0;
	while ( !(num == 0) )
	{
		var t = num % 10;
		num = num / 10;
		result = result*10 + t;
	}
	return result;
}

var num = IO.readln();
IO.writeln(reverse(num));


