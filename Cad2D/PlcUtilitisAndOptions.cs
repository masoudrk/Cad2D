﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cad2D
{
    public class PlcUtilitisAndOptions
    {
        public _velocity Velocity { set; get; }
        public _BridgeOptions BridgeOptions { set; get; }
        public _ClampOptions ClampOptions { set; get; }

        public PlcUtilitisAndOptions()
        {
            ClampOptions = new _ClampOptions();
            BridgeOptions = new _BridgeOptions();
            Velocity = new _velocity();
        }

        public class _ClampOptions
        {
            public _ClampOptions()
            {
                PackestId = new List<ushort>();
                clampValue = new clamp();
                upClamp = new clamp();
                downClamp = new clamp();
                frontClamp = new clamp();
                behindClamp = new clamp();

                clampValue.valueAddress = 970;
                upClamp.valueAddress = 971;
                downClamp.valueAddress = 972;
                frontClamp.valueAddress = 973;
                behindClamp.valueAddress = 974;
            }
            public List<ushort> PackestId;
            public writingPacketInfo writingPacket { set; get; }
            public clamp clampValue { set; get; }
            public clamp upClamp { set; get; }
            public clamp downClamp { set; get; }
            public clamp frontClamp { set; get; }
            public clamp behindClamp { set; get; }
            public class clamp
            {
                public writingPacketInfo writingPacket;
                public int value { set; get; }
                public int valueAddress { set; get; }
            }

            public void updateValues(byte[] dataArray)
            {
                clampValue.value = dataArray[1] * 256 + dataArray[0];

                upClamp.value = dataArray[3] * 256 + dataArray[2];
                downClamp.value = dataArray[5] * 256 + dataArray[4];

                frontClamp.value = dataArray[7] * 256 + dataArray[6];
                behindClamp.value = dataArray[9] * 256 + dataArray[8];

            }
        }


        public class _BridgeOptions
        {
            public _BridgeOptions ()
            {
                PackestId = new List<ushort>();
                stoneOffsetUp = new stoneOffset();
                stoneOffsetRight = new stoneOffset();
                stoneOffsetDown = new stoneOffset();
                stoneOffsetLeft = new stoneOffset();
                //shoud assinge addreses
                stoneOffsetUp.valueAddress = 960;
                stoneOffsetRight.valueAddress = 962;
                stoneOffsetDown.valueAddress = 964;
                stoneOffsetLeft.valueAddress = 966;
                //shoud assinge addreses
                stoneOffsetUp.delayAddress = 961;
                stoneOffsetRight.delayAddress = 963;
                stoneOffsetDown.delayAddress = 965;
                stoneOffsetLeft.delayAddress = 967;
            }
            public writingPacketInfo writingPacket { set; get; }
            public List<ushort> PackestId;
            public stoneOffset stoneOffsetUp { set; get; }
            public stoneOffset stoneOffsetRight { set; get; }
            public stoneOffset stoneOffsetDown { set; get; }
            public stoneOffset stoneOffsetLeft { set; get; }
            public class stoneOffset
            {
                public writingPacketInfo writingPacketValue;
                public writingPacketInfo writingPacketDelay;
                public int value { set; get; }
                public int delay { set; get; }
                public int valueAddress { set; get; }
                public int delayAddress { set; get; }
            }

            public void updateValues(byte [] dataArray)
            {
                stoneOffsetUp.value = dataArray[1] * 256 + dataArray[0];
                stoneOffsetUp.delay = dataArray[3] * 256 + dataArray[2];

                stoneOffsetRight.value = dataArray[5] * 256 + dataArray[4];
                stoneOffsetRight.delay = dataArray[7] * 256 + dataArray[6];

                stoneOffsetDown.value = dataArray[9] * 256  + dataArray [8];
                stoneOffsetDown.delay = dataArray[11] * 256 + dataArray[10];

                stoneOffsetLeft.value = dataArray[13] * 256 + dataArray[12];
                stoneOffsetLeft.delay = dataArray[15] * 256 + dataArray[14];

            }
        }

        public class _velocity
        {
            public _velocity()
            {
                PackestId = new List<ushort>();
                velocityX = 50;
                velocityY = 50;
                velocityXAddress = 955;
                velocityYAddress = 956;
            }
            public List<ushort> PackestId;
            public int velocityX { set; get; }
            public int velocityXAddress { set; get; }
            public int velocityY { set; get; }
            public int velocityYAddress { set; get; }

            public void updateValues(byte []byteData)
            {
                velocityX = byteData[1] * 256 + byteData[0];
                velocityY = byteData[3] * 256 + byteData[2];
            }
        }


    }
}
