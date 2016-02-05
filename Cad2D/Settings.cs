using Cad2D;
using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;

public class Settings
{
    public bool [] alarmBits;
    public int clampAmount { get; set; }
    public int clampTopLeft { get; set; }
    public int clampTopRight { get; set; }
    public int clampBottomRight { get; set; }
    public int clampBottomLeft { get; set; }
}

public class PrimarySettings
{
    public string CameraIpAdress { get; set; }
    public int CameraPortNumber { get; set; }
    public string PLCIpAdress { get; set; }
    public int PLCPortNumber { get; set; }
    public int TopLeftOffsetX { get; set; }
    public int TopLeftOffsetY { get; set; }
    public int BottomRightOffsetX { get; set; }
    public int BottomRightOffsetY { get; set; }
    public bool showSpeedMonitorInMainPanel { get; set; }
    public bool showGuideCircles { get; set; }
    public int VarMemoryOffset { get; set; }
    public int ArrayMemoryOffset { get; set; }
}

public static class Extentions
{
    public static void writeToXmlFile(this object obj , string settingFile)
    {
        StreamWriter sw = new StreamWriter(settingFile);

        XmlSerializer s = new XmlSerializer(obj.GetType());
        using (StringWriter writer = new StringWriter())
        {
            s.Serialize(writer, obj);
            sw.Write( writer.ToString() );
            sw.Close();
        }
    }

    public static Settings FromXml()
    {
        string addr = "options.ini";
        if (File.Exists(addr))
        {
            StreamReader sr = new StreamReader(addr);
            XmlSerializer s = new XmlSerializer(typeof(Settings));
            using (StringReader reader = new StringReader(sr.ReadToEnd()))
            {
                object obj = s.Deserialize(reader);
                sr.Close();
                return (Settings)obj;
            }
        }
        else
        {
            Cad2D.Properties.Settings ds = Cad2D.Properties.Settings.Default;
            Settings s = new Settings();
            return s;
        }
    }
    public static PrimarySettings FromXmlPrimary()
    {
        string addr = "_pss.ini";
        if (File.Exists(addr))
        {
            StreamReader sr = new StreamReader(addr);
            XmlSerializer s = new XmlSerializer(typeof(PrimarySettings));
            using (StringReader reader = new StringReader(sr.ReadToEnd()))
            {
                object obj = s.Deserialize(reader);
                sr.Close();
                return (PrimarySettings)obj;
            }
        }
        else
        {
            Cad2D.Properties.Settings ds = Cad2D.Properties.Settings.Default;
            PrimarySettings s = new PrimarySettings()
            {
                CameraIpAdress = ds.CameraIP,
                CameraPortNumber = ds.CameraPort,
                PLCIpAdress = ds.PLCIP,
                PLCPortNumber = ds.PLCPort,
                VarMemoryOffset = ds.VarMemoryOffset,
                ArrayMemoryOffset = ds.ArrayMemoryOffset
            };
            return s;
        }
    }

}
