using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cad2D
{
    public class HscYHelper
    {
        public Packet<bool>      testModeAXY { set; get; }
        public Packet<ushort>    curPosMMAXY { set; get; }
        public Packet<ushort>    spdReqAXY { set; get; }
        public Packet<ushort>    accAXY { set; get; }
        public Packet<ushort>    decModeAXY { set; get; }
        public Packet<ushort>    manMaxTarPosAXY { set; get; }
        public Packet<ushort>    manMinTarPosAXY { set; get; }
        public Packet<ushort>    bwdKeshAXY     { set; get; }
        public Packet<ushort>    fwdKeshAXY { set; get; }
        public Packet<ushort>    bwdMaxDispAXY { set; get; }
        public Packet<ushort>    fwdMaxDispAXY { set; get; }
        public Packet<bool>      jogCalcAutoAXY { set; get; }
        public Packet<ushort>[]  jogArrayAXY { set; get; }
        public Packet<bool>    decAutoAXY { set; get; }
        public Packet<ushort>    decStopAXY { set; get; }
        public Packet<ushort>[]  decArrayAXY { set; get; }

        public HscYHelper()
        {
            testModeAXY = new Packet<bool>(3202);
            curPosMMAXY = new Packet<ushort>(23);
            spdReqAXY = new Packet<ushort>(255);
            accAXY = new Packet<ushort>(251);
            decModeAXY = new Packet<ushort>(252);
            manMaxTarPosAXY = new Packet<ushort>(245);
            manMinTarPosAXY = new Packet<ushort>(246);
            bwdKeshAXY = new Packet<ushort>(32);
            fwdKeshAXY = new Packet<ushort>(30);
            bwdMaxDispAXY = new Packet<ushort>(33);
            fwdMaxDispAXY = new Packet<ushort>(31);
            jogCalcAutoAXY = new Packet<bool>(3200);
            decAutoAXY = new Packet<bool>(3206);
            decStopAXY = new Packet<ushort>(253);
            jogArrayAXY =  new Packet<ushort>[40];
            decArrayAXY = new Packet<ushort>[40];
            createPackets(decArrayAXY, 500);
            createPackets(jogArrayAXY, 300);
        }

        void createPackets(Packet<ushort>[] array, int address )
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = new Packet<ushort>(address+i);
            }
        }

        void parsJogArrayAXY(byte[] continuousData)
        {
            parse(continuousData, jogArrayAXY);
        }

        void parsDecArrayAXY(byte[] continuousData)
        {
            parse(continuousData, decArrayAXY);
        }

        void parse(byte[] continuousData, Packet<ushort>[] array)
        {
            for (int i = 0; i < array.Length; i++)
                array[i].value = (ushort)(continuousData[i * 2 + 1] * 256 + continuousData[i * 2]);
        }
    }
}
