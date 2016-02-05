using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cad2D
{
    public struct _Alert
    {
        public int wordNumber { set; get; }
        public ushort value { set; get; }
    }
    public struct _ManualOrAuto
    {
        public int wordNumber { set; get; }
        public ushort value { set; get; }
    }
    public struct _ShutDown
    {
        public int wordNumber { set; get; }
        public ushort value { set; get; }
    }
    public struct _Positions
    {
        public int wordNumberX { set; get; }
        public int wordNumberY { set; get; }
        public ushort valueX { set; get; }
        public ushort valueY { set; get; }
    }
    public class PlcInformation
    {
        public PlcInformation()
        {
            PackestId = new List<ushort>();
        }
        public List<ushort> PackestId;
        public _Positions positions = new _Positions();
        public _Alert alert = new _Alert();
        public _ShutDown shutdown = new _ShutDown();
        public _ManualOrAuto manualOrAuto = new _ManualOrAuto();

        internal void parse(byte[] continuousData)
        {
            alert.value = (ushort)(continuousData[1] * 256 + continuousData[0]);
            shutdown.value = (ushort)(continuousData[3] * 256 + continuousData[2]);
            positions.valueX = (ushort)(continuousData[5] * 256 + continuousData[4]);
            positions.valueY = (ushort)(continuousData[7] * 256 + continuousData[6]);
            manualOrAuto.value = (ushort)(continuousData[9] * 256 + continuousData[8]);
        }
    }
}
