using System.Collections.Generic;
using System.IO;

namespace CharpLearning.Syntax
{
    public class NewInstance
    {
        public void NewTest()
        {
            //第一种实例化对象
            FileStream stream = new FileStream("11.txt", FileMode.Create);
            //处理
            stream.Dispose();

            //第二种实例化对象
            //在.Net 3.5(官方文档)支持var可推断类型,这个语法糖还是很受大家,当然有有一部分人不喜欢这个语法糖
            using (var fileStream = new FileStream("123.txt", FileMode.Create))
            {

            }


            //第三种实例化对象
            //在C# 9.0的时候,增加了新的语法
            //左侧声明具体类型,右侧new 可以省略具体类型
            using (FileStream fileStream1 = new("456.txt", FileMode.Create))
            {

            }


            List<int> list = new(8);  //左侧有具体类型,右侧可以简化

            //如果左侧只是变量,右侧使用简化的new也是没问题的,编译器编译时替换成具体类型,但主要不利于人阅读代码
            list = new(16);
        }
    }
}
