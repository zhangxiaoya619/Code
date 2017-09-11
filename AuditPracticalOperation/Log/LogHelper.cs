using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Log
{
    public static class LogHelper
    {
        private const string LOG_FILE_NAME = "log.txt";

        private static readonly string logFileName;

        private static object lockThis = new object();

        public static void LogError(Exception ex)
        {
            lock (lockThis)
                using (StreamWriter sw = File.AppendText(logFileName))
                    sw.WriteLine(string.Format("{0} : {1}\n调用堆寨 ： {2}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), ex.Message, ex.StackTrace));
        }

        static LogHelper()
        {
            logFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LOG_FILE_NAME);

            if (!File.Exists(logFileName))
                File.Create(logFileName).Close();
        }
    }
}
