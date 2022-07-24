using System.IO;

namespace CharpLearning.Syntax
{
    public class Log
    {
        public static void WriteLog(string fileName, string log)
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), fileName);
            FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);
            try
            {
                sw.BaseStream.Seek(0, SeekOrigin.End);
                sw.WriteLine(log);
                sw.Flush();
            }
            finally
            {
                sw.Close();    //先对StreamWriter进行关闭
                fs.Dispose();  //在对FileStream进行释放
            }
        }

        public static void WriteLog2(string fileName, string log)
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), fileName);
            using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.BaseStream.Seek(0, SeekOrigin.End);
                    sw.WriteLine(log);
                    sw.Flush();
                }
            }
        }
    }
}
