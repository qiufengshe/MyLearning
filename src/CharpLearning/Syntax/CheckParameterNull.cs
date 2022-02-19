using System;

namespace CharpLearning.Syntax
{
    public class CheckParameterNull
    {
        public string Address { get; }
        public CheckParameterNull(string address!!) //在构造方法,使用新语法检查参数是否
        {
            Address = address;
        }

        public bool Test1(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            //等同于上方 if(name == null) { throw new ArgumentNullException("name");)
            //这种方式可以用在检查属性是否为空
            ArgumentNullException.ThrowIfNull(nameof(name));

            return true;
        }

        public bool Test2(string name!!)  //新语法, 在参数名加!! 
        {
            return true;
        }
    }
}
