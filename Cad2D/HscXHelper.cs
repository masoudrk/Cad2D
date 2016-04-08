using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cad2D
{
    public class HscXHelper
    {
        public Packet<bool>   testModeAXX { set; get; }
        public Packet<ushort> curPosMMAXX { set; get; }
        public Packet<ushort> spdReqAXX { set; get; }
        public Packet<ushort> accAXX { set; get; }
        public Packet<ushort> decModeAXX { set; get; }
        public Packet<ushort> manMaxTarPosAXX { set; get; }
        public Packet<ushort> manMinTarPosAXX { set; get; }
        public Packet<ushort> bwdKeshAXX { set; get; }
        public Packet<ushort> fwdKeshAXX { set; get; }
        public Packet<ushort> bwdMaxDispAXX { set; get; }
        public Packet<ushort> fwdMaxDispAXX { set; get; }
        public Packet<bool> jogCalcAutoAXX { set; get; }
        public Packet<ushort>[] jogArrayAXX { set; get; }
        public Packet<bool> decAutoAXX { set; get; }
        public Packet<ushort> decStopAXX { set; get; }
        public Packet<ushort>[] decArrayAXX { set; get; }

        public HscXHelper()
        {
            testModeAXX = new Packet<bool>(3203);
            curPosMMAXX = new Packet<ushort>(22);
            spdReqAXX = new Packet<ushort>(254);
            accAXX = new Packet<ushort>(247);
            decModeAXX = new Packet<ushort>(248);
            manMaxTarPosAXX = new Packet<ushort>(243);
            manMinTarPosAXX = new Packet<ushort>(244);
            bwdKeshAXX = new Packet<ushort>(28);
            fwdKeshAXX = new Packet<ushort>(26);
            bwdMaxDispAXX = new Packet<ushort>(29);
            fwdMaxDispAXX = new Packet<ushort>(27);
            jogCalcAutoAXX = new Packet<bool>(3201);
            decAutoAXX = new Packet<bool>(3204);
            decStopAXX = new Packet<ushort>(249);
            jogArrayAXX = new Packet<ushort>[40];
            decArrayAXX = new Packet<ushort>[40];
            createPackets(decArrayAXX, 550);
            createPackets(jogArrayAXX, 350);
        }

        void createPackets(Packet<ushort>[] array, int address)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = new Packet<ushort>(address + i);
            }
        }

        void parsJogArrayAXX(byte[] continuousData)
        {
            parse(continuousData, jogArrayAXX);
        }

        void parsDecArrayAXX(byte[] continuousData)
        {
            parse(continuousData, decArrayAXX);
        }

        void parse(byte[] continuousData, Packet<ushort>[] array)
        {
            for (int i = 0; i < array.Length; i++)
                array[i].value = (ushort) (continuousData[i*2 + 1]*256 + continuousData[i*2]);

        }
    }
}
