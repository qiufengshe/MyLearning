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
    }
}
