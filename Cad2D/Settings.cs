using Cad2D;
using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;

public class Settings
{
    public class Bridge
    {
        public int value { get; set; }
        public int delay { get; set; }
    }

    public bool [] alarmBits;
    public int clampAmount { get; set; }
    public int clampTopLeft { get; set; }
    public int clampTopRight { get; set; }
    public int clampBottomRight { get; set; }
    public int clampBottomLeft { get; set; }
    public Bridge bridgeTop { get; set; }
    public Bridge bridgeRight { get; set; }
    public Bridge bridgeLeft { get; set; }
    public Bridge bridgeBottom { get; set; }
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
    public bool captureModeWhenStart { set; get; }
    public int VarMemoryOffset { get; set; }
    public int ArrayMemoryOffset { get; set; }
    public int ScanVerticalSlice { set; get; }
    public int EdgeVerticalSlice { set; get; }
    public int ScanHorizontalSlice { set; get; }
    public int EdgeHorizontalSlice { set; get; }
    public int ScanAriaSegment { set; get; }
    public int VerticalBoundrySegment { set; get; }
    public int HorizonalBoundrySegment { set; get; }
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
                ArrayMemoryOffset = ds.ArrayMemoryOffset,
                ScanVerticalSlice = ds.VerticalScan,
                ScanHorizontalSlice = ds.HorizontalScan,
                EdgeHorizontalSlice = ds.HorizontalEdge,
                EdgeVerticalSlice = ds.VerticalEdge,
                ScanAriaSegment = ds.ScanAriaSegment,
                VerticalBoundrySegment = ds.VerticalBoundrySegment,
                HorizonalBoundrySegment = ds.HorizonalBoundrySegment
            };
            return s;
        }
    }

}
