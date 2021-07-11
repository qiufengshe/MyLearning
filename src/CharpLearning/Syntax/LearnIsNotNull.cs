using System;

namespace CharpLearning.Syntax
{
    /// <summary>
    /// is not null 语法糖
    /// 在c# 9 加入
    /// </summary>
    public class LearnIsNotNull
    {
        public void IsNotNull(object obj)
        {
            //之前
            if (obj != null)
            {
                Console.WriteLine("obj != null");
            }

            //现在可以用,is not null只是语法糖,在编译时,会替换成"!=null"
            if (obj is not null)
            {
                Console.WriteLine("obj is not null");
            }

            //obj is not null 反编译 
            /*
             if (obj != null)
		     {
			      Console.WriteLine("obj is not null");
		     }
             */
        }
    }
}
