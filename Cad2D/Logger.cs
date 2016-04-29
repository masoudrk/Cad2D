using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cad2D
{
    public enum LogType
    {
        Error , Warning , Info
    }

    class Logger
    {
        static Mutex writeMutex = new Mutex();
        public static void LogError(string msg , LogType lt , Exception ex)
        {
            writeMutex.WaitOne();
            System.IO.StreamWriter file = new System.IO.StreamWriter(Env.ErrorFolderPath, true);
            file.WriteLine(lt.ToString() + ":");
            string[] lines = msg.Split('\n');
            foreach (var l in lines)
            {
                file.WriteLine(l);
            }
            file.WriteLine("\n_Time :"+ DateTime.Now.ToString(CultureInfo.InvariantCulture));
            if(ex != null)
            {
                var st = new StackTrace(ex, true);
                var stackFrames = st.GetFrames();
                if (stackFrames != null)
                    foreach (var frame in stackFrames)
                    {
                        file.WriteLine("\n_Frame : " + frame.ToString());
                        // Get the line number from the stack frame
                        var line = frame.GetFileLineNumber();
                        file.WriteLine("\n_Line : " + line.ToString());
                    }
            }
            file.WriteLine("---------------------------------------------------------END----------------------------------------------------------");
            file.Close();
            writeMutex.ReleaseMutex();
        }
    }
}
