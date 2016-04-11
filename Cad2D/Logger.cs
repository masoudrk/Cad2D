using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cad2D
{
    public enum LogType
    {
        Error , Warning , Info
    }

    class Logger
    {

        public static void LogError(string msg , LogType lt , Exception ex)
        {
            System.IO.StreamWriter file = new System.IO.StreamWriter(Env.ErrorFolderPath, true);
            file.WriteLine(lt.ToString() + ":");
            string[] lines = msg.Split('\n');
            foreach (var l in lines)
            {
                file.WriteLine(l);
            }
            file.WriteLine("\n_Time :"+ DateTime.Now.ToString(CultureInfo.InvariantCulture));
            // Get stack trace for the exception with source file information
            var st = new StackTrace(ex, true);
            file.WriteLine("\n_StackTrace : "+st.ToString());
            // Get the top stack frame
            var stackFrames = st.GetFrames();
            if (stackFrames != null)
                foreach (var frame in stackFrames)
                {
                    file.WriteLine("\n_Frame : " + frame.ToString());
                    // Get the line number from the stack frame
                    var line = frame.GetFileLineNumber();
                    file.WriteLine("\n_Line : " + line.ToString());
                }
            file.WriteLine("---------------------------------------------------------END----------------------------------------------------------");
            file.Close();
            
        }
    }
}
