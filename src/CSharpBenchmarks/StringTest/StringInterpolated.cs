using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpBenchmarks.StringTest
{
public class StringInterpolated
{
	public string Test1()
	{
		int a = 20;
		return $"age={a}";  //这种方式就是内插字符串
	}

	public string Test2()
	{
		int a = 20;
		return string.Format("age={0}", a); //在字符串比较短的时候,可以使用,
	}

	public string Test3()
	{
		int a = 20;
		return "age=" + a;  //不建议使用这种方式,进行字符串拼接
	}
}
}
