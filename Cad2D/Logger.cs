using System;
using System.Collections.Generic;
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

        public static void LogError(string msg , LogType lt)
        {
            System.IO.StreamWriter file = new System.IO.StreamWriter(Env.ErrorFolderPath, true);
            file.WriteLine(lt.ToString() + ":");
            string[] lines = msg.Split('\n');
            foreach (var l in lines)
            {
                file.WriteLine(l);
            }
            file.WriteLine("---------------------------------------------------------END----------------------------------------------------------");
            file.Close();
        }
    }
}
