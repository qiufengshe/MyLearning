namespace CharpLearning.Syntax
{
    /// <summary>
    /// 增强的模式匹配 和is一起使用
    /// c# 9 加入,其实也是语法糖
    /// </summary>
    public class LearnPatternMatch
    {
        public bool IsAlpha(char c)
        {
            return c is >= 'a' and <= 'z' or >= 'A' and <= 'Z';

            /*
			 反编译结果:
bool flag;
bool flag1;
if (c >= 'a')
{
    if (c <= 'z')
    {
        flag = true;
        flag1 = flag;
        return flag1;
    }
}
else if (c >= 'A')
{
    if (c <= 'Z')
    {
        flag = true;
        flag1 = flag;
        return flag1;
    }
}
flag = false;
flag1 = flag;
return flag1;
			 */
        }

        //public bool IsAlpha(char c)
        //{
        //    if (c >= 'a')
        //    {
        //        if (c > 'z')
        //        {
        //            goto IL_1D;
        //        }
        //    }
        //    else
        //    {
        //        if (c < 'A')
        //        {
        //            goto IL_1D;
        //        }
        //        if (c > 'Z')
        //        {
        //            goto IL_1D;
        //        }
        //    }
        //    return true;
        //IL_1D:
        //    return false;
        //}

        //ILSpy 反编译
        ////public bool IsAlpha(char c)
        ////{
        ////    switch (c)
        ////    {
        ////        case 'A':
        ////        case 'B':
        ////        case 'C':
        ////        case 'D':
        ////        case 'E':
        ////        case 'F':
        ////        case 'G':
        ////        case 'H':
        ////        case 'I':
        ////        case 'J':
        ////        case 'K':
        ////        case 'L':
        ////        case 'M':
        ////        case 'N':
        ////        case 'O':
        ////        case 'P':
        ////        case 'Q':
        ////        case 'R':
        ////        case 'S':
        ////        case 'T':
        ////        case 'U':
        ////        case 'V':
        ////        case 'W':
        ////        case 'X':
        ////        case 'Y':
        ////        case 'Z':
        ////        case 'a':
        ////        case 'b':
        ////        case 'c':
        ////        case 'd':
        ////        case 'e':
        ////        case 'f':
        ////        case 'g':
        ////        case 'h':
        ////        case 'i':
        ////        case 'j':
        ////        case 'k':
        ////        case 'l':
        ////        case 'm':
        ////        case 'n':
        ////        case 'o':
        ////        case 'p':
        ////        case 'q':
        ////        case 'r':
        ////        case 's':
        ////        case 't':
        ////        case 'u':
        ////        case 'v':
        ////        case 'w':
        ////        case 'x':
        ////        case 'y':
        ////        case 'z':
        ////            return true;
        ////        default:
        ////            return false;
        ////    }
        ////}
        public int PatternMatch8(int val) => val switch
        {
            1 => 10,
            2 => 10 * 2,
            3 => 10 * 3,
            _ => 10 * 10,  //_代表弃元 在这里代替default
        };

        /// <summary>
        /// 变量交换
        /// </summary>
        public void SwapVal()
        {
            int a = 10, b = 20;
            System.Console.WriteLine($"a={a} b={b}");
            (a, b) = (b, a); //在c# 8.0,支持这样语法
            System.Console.WriteLine($"a={a} b={b}");
        }

        /// <summary>
        /// 属性模式
        /// </summary>
        /// <param name="people"></param>
        /// <returns></returns>
        public decimal PatternMatch8(People people) => people switch
        {
            { Name: "tom" } => people.Salary * 1.1m,
            { Name: "jack" } => people.Salary * 1.2m,
            _ => 0m
        };


        /// <summary>
        /// 元祖模式
        /// </summary>
        /// <param name="people"></param>
        /// <returns></returns>
        public decimal PatternMatch8(int a, int b) => (a, b) switch
        {
            (1, 10) => 1.1m,
            (2, 20) => 1.0m,
            _ => 0m
        };




    }
}
