using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cad2D
{
    class Env
    {
        public static string SettingsFile
        {
            get { return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\MgCad\_opt.ini"; }
        }
        public static string PrimarySettingsFile
        {
            get { return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\MgCad\_pss.ini"; }
        }
        public static string SettingsFolderPath
        {
            get { return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\MgCad\"; }
        }
        public static string ErrorFolderPath
        {
            get { return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\MgCadErrorLog.txt"; }
        }

    }
}
