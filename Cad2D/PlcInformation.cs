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
        public short value { set; get; }
    }
    public struct _ManualOrAuto
    {
        public int wordNumber { set; get; }
        public short value { set; get; }
    }
    public struct _ShutDown
    {
        public int wordNumber { set; get; }
        public short value { set; get; }
    }
    public struct _Positions
    {
        public int wordNumberX { set; get; }
        public int wordNumberY { set; get; }
        public short valueX { set; get; }
        public short vslueY { set; get; }
    }
    public class PlcInformation
    {
        public PlcInformation ()
        {
            PackestId = new List<ushort>();
        }
        public List<ushort> PackestId { set; get; }
        public _Positions positions = new _Positions();
        public _Alert alert = new _Alert();
        public _ShutDown shutdown = new _ShutDown();
        public _ManualOrAuto manualOrAuto = new _ManualOrAuto();

        internal void parse(byte[] continuousData)
        {
            
        }
    }
}
