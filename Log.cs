using System;
using System.IO;
using System.Text;
using System.Timers;

namespace FolderLinker
{
    public class Log
    {
        private int timeWaited = 5000;
        private Timer timer;
        private bool isFileOpen = false;
        private FileStream stream;

        private string logFileName;

        public Log()
        {
            string logPath = Path.Combine(AppContext.BaseDirectory, @"logs");
            string log = Path.Combine(logPath, "log.txt");
            string oldlog = Path.Combine(logPath, "oldlog.txt");
            logFileName = log;
            if (!Directory.Exists(logPath)) 
                Directory.CreateDirectory(logPath);
            if (File.Exists(oldlog)) 
                File.Delete(oldlog);
            if (File.Exists(log))
            {
                byte[] content = File.ReadAllBytes(log);
                FileStream fs = File.Create(oldlog);
                fs.Write(content, 0, content.Length);
                fs.Close();
                fs.Dispose();
                File.Delete(log);
            }
            File.Create(log).Close();
            timer = new Timer(timeWaited);
            timer.Elapsed += Timer_Elapsed;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            stream.Close();
            stream.Dispose();
            isFileOpen = false;
            timer.Stop();
        }

        public void Write(string content, LogType type = LogType.Info)
        {
            if (!isFileOpen)
            {
                stream = new FileStream(logFileName, FileMode.Append);
                isFileOpen = true;
                timer.Start();
            }
            string toWriteStr = $"[{type}]\t" + DateTime.Now.ToString("yyyy/MM/dd-hh:mm:ss.fff") + "\t" + content;
            byte[] toWriteBytes = Encoding.UTF8.GetBytes(toWriteStr);
            stream.Write(toWriteBytes, 0, toWriteBytes.Length);
        }

        public void WriteLine(string content, LogType type = LogType.Info)
        {
            if (content.EndsWith('\n'))
                Write(content, type);
            else Write(content + "\n", type);
        }
    }

    public enum LogType
    {
        Info,
        Error,
        Link,
        Delete,
        Rename
    }
}
