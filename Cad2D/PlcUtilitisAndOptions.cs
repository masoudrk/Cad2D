using System;
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
        public _Encoder Encoder { get; set; }
        public PlcUtilitisAndOptions()
        {
            ClampOptions = new _ClampOptions();
            BridgeOptions = new _BridgeOptions();
            Velocity = new _velocity();
            Encoder = new _Encoder();
        }

        public class _Encoder
        {

            public _Encoder()
            {
                PackestIdX = new List<ushort>();
                PackestIdY = new List<ushort>();
                EncoderXPals = new EncoderCell();
                EncoderXMult = new EncoderCell();
                EncoderXDiv = new EncoderCell();
                EncoderXPos = new EncoderCell();
                EncoderYPals = new EncoderCell();
                EncoderYMult = new EncoderCell();
                EncoderYDiv = new EncoderCell();
                EncoderYPos = new EncoderCell();

                EncoderXPals.valueAddress = 980;
                EncoderXMult.valueAddress = 981;
                EncoderXDiv.valueAddress = 982;
                EncoderXPos.valueAddress = 983;


                EncoderYPals.valueAddress = 984;
                EncoderYMult.valueAddress = 985;
                EncoderYDiv.valueAddress = 986;
                EncoderYPos.valueAddress = 987;
            }
            public List<ushort> PackestIdY;
            public List<ushort> PackestIdX;

            public EncoderCell EncoderXPals { set; get; }
            public EncoderCell EncoderXMult { set; get; }
            public EncoderCell EncoderXDiv { set; get; }
            public EncoderCell EncoderXPos { set; get; }
            public EncoderCell EncoderYPals { set; get; }
            public EncoderCell EncoderYMult { set; get; }
            public EncoderCell EncoderYDiv { set; get; }
            public EncoderCell EncoderYPos { set; get; }
            public class EncoderCell
            {
                public writingPacketInfo writingPacket;
                public readingPacketInfo readingPacket;
                public ushort value { set; get; }
                public int valueAddress { set; get; }
            }

            public void updateEncoderXValues(byte [] dataArray)
            {
                EncoderXPals.value = (ushort)(dataArray[1] * 256 + dataArray[0]);
                EncoderXMult.value = (ushort)(dataArray[3] * 256 + dataArray[2]);
                EncoderXDiv.value = (ushort)(dataArray[5] * 256 + dataArray[4]);
                EncoderXPos.value = (ushort)(dataArray[7] * 256 + dataArray[6]);
            }
            public void updateEncoderYValues(byte[] dataArray)
            {
                EncoderYPals.value = (ushort)(dataArray[1] * 256 + dataArray[0]);
                EncoderYMult.value = (ushort)(dataArray[3] * 256 + dataArray[2]);
                EncoderYDiv.value = (ushort)(dataArray[5] * 256 + dataArray[4]);
                EncoderYPos.value = (ushort)(dataArray[7] * 256 + dataArray[6]);
            }
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
            public clamp clampValue { set; get; }
            public clamp upClamp { set; get; }
            public clamp downClamp { set; get; }
            public clamp frontClamp { set; get; }
            public clamp behindClamp { set; get; }
            public class clamp
            {
                public writingPacketInfo writingPacket;
                public ushort value { set; get; }
                public int valueAddress { set; get; }
            }

            public void updateValues(byte[] dataArray)
            {
                clampValue.value = (ushort)(dataArray[1] * 256 + dataArray[0]);

                upClamp.value = (ushort)(dataArray[3] * 256 + dataArray[2]);
                downClamp.value = (ushort)(dataArray[5] * 256 + dataArray[4]);

                frontClamp.value = (ushort)(dataArray[7] * 256 + dataArray[6]);
                behindClamp.value = (ushort)(dataArray[9] * 256 + dataArray[8]);

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
            public List<ushort> PackestId;
            public stoneOffset stoneOffsetUp { set; get; }
            public stoneOffset stoneOffsetRight { set; get; }
            public stoneOffset stoneOffsetDown { set; get; }
            public stoneOffset stoneOffsetLeft { set; get; }
            public class stoneOffset
            {
                public writingPacketInfo writingPacketValue;
                public writingPacketInfo writingPacketDelay;
                public ushort value { set; get; }
                public ushort delay { set; get; }
                public int valueAddress { set; get; }
                public int delayAddress { set; get; }
            }

            public void updateValues(byte [] dataArray)
            {
                stoneOffsetUp.value = (ushort)(dataArray[1] * 256 + dataArray[0]);
                stoneOffsetUp.delay = (ushort)(dataArray[3] * 256 + dataArray[2]);

                stoneOffsetRight.value = (ushort)(dataArray[5] * 256 + dataArray[4]);
                stoneOffsetRight.delay = (ushort)(dataArray[7] * 256 + dataArray[6]);

                stoneOffsetDown.value = (ushort)(dataArray[9] * 256  + dataArray [8]);
                stoneOffsetDown.delay = (ushort)(dataArray[11] * 256 + dataArray[10]);

                stoneOffsetLeft.value = (ushort)(dataArray[13] * 256 + dataArray[12]);
                stoneOffsetLeft.delay = (ushort)(dataArray[15] * 256 + dataArray[14]);

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
            public ushort velocityX { set; get; }
            public int velocityXAddress { set; get; }
            public ushort velocityY { set; get; }
            public int velocityYAddress { set; get; }

            public void updateValues(byte []byteData)
            {
                velocityX = (ushort)(byteData[1] * 256 + byteData[0]);
                velocityY = (ushort)(byteData[3] * 256 + byteData[2]);
            }
        }
    }
}
