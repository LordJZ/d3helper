using System;
using System.IO;
using System.Text;

namespace d3helper
{
    class Logger
    {
        static StreamWriter Writer;

        public static void Init()
        {
            string path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            Writer = new StreamWriter(Path.Combine(path, "bot_log.txt"), true, Encoding.UTF8);
            Writer.AutoFlush = true;
            Writer.WriteLine("{0}: Log started", DateTime.Now);
        }

        public static void Add(string line)
        {
            Writer.WriteLine("{0}: {1}", DateTime.Now, line);
        }

        public static void Close()
        {
            Writer.WriteLine("{0}: Log closed", DateTime.Now);
            Writer.Close();
        }
    }
}
