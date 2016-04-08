using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cad2D
{
    public class PlcUtilitisAndOptions
    {
        public Packet<ushort> DiskDiameter { set; get; }
        public Packet<ushort> ParkPosAXX { set; get; }
        public Packet<ushort> ParkPosAXY { set; get; }
        public Packet<ushort> MinDif { set; get; }
        public Packet<ushort> AXXFeedDist { set; get; }
        public Packet<ushort> AXYFeedDist { set; get; }
        public Packet<ushort> Hashye { set; get; }
        public Packet<ushort> ManSpdAXX { set; get; }
        public Packet<ushort> ManSpdAXY { set; get; }
        public _velocity Velocity { set; get; }
        public _BridgeOptions BridgeOptions { set; get; }
        public _ClampOptions ClampOptions { set; get; }
        public _Encoder Encoder { get; set; }
        public PlcUtilitisAndOptions()
        {
            setNewValues();
        }
        public void setNewValues()
        {
            PrimarySettings ps = Extentions.FromXmlPrimary();
            ClampOptions = new _ClampOptions((ushort)ps.ClampMem);//970
            BridgeOptions = new _BridgeOptions((ushort)ps.BridgeOptionMem);//960
            Velocity = new _velocity((ushort)ps.Velocity);//955
            Encoder = new _Encoder((ushort)ps.XEncoderMem, (ushort)ps.YEncoderMem);//980,984

            DiskDiameter = new Packet<ushort>(232);
            ParkPosAXX = new Packet<ushort>(237);
            ParkPosAXY = new Packet<ushort>(238);
            MinDif = new Packet<ushort>(239);
            AXXFeedDist = new Packet<ushort>(240);
            AXYFeedDist = new Packet<ushort>(241);
            Hashye = new Packet<ushort>(242);
            ManSpdAXX = new Packet<ushort>(250);
            ManSpdAXY = new Packet<ushort>(256);

        }
        public class _Encoder
        {
            public _Encoder(ushort address1 , ushort address2)
            {//980 , 984
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

                EncoderXPals.valueAddress = address1++;
                EncoderXMult.valueAddress = address1++;
                EncoderXDiv.valueAddress = address1++;
                EncoderXPos.valueAddress = address1++;


                EncoderYPals.valueAddress = address2++;
                EncoderYMult.valueAddress = address2++;
                EncoderYDiv.valueAddress = address2++;
                EncoderYPos.valueAddress = address2++;
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
            public _ClampOptions(ushort address)
            {//970
                PackestId = new List<ushort>();
                clampValue = new clamp();
                upClamp = new clamp();
                downClamp = new clamp();
                frontClamp = new clamp();
                behindClamp = new clamp();

                clampValue.valueAddress = address++;
                upClamp.valueAddress = address++;
                downClamp.valueAddress = address++;
                frontClamp.valueAddress = address++;
                behindClamp.valueAddress = address++;
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
            public _BridgeOptions (int address)
            {
                PackestId = new List<ushort>();
                stoneOffsetUp = new stoneOffset();
                stoneOffsetRight = new stoneOffset();
                stoneOffsetDown = new stoneOffset();
                stoneOffsetLeft = new stoneOffset();
                //shoud assinge addreses
                stoneOffsetUp.valueAddress = address++;
                stoneOffsetRight.valueAddress = address++;
                stoneOffsetDown.valueAddress = address++;
                stoneOffsetLeft.valueAddress = address++;
                //shoud assinge addreses
                stoneOffsetUp.delayAddress = address++;
                stoneOffsetRight.delayAddress = address++;
                stoneOffsetDown.delayAddress = address++;
                stoneOffsetLeft.delayAddress = address++;
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
            public _velocity(int address)
            {
                PackestId = new List<ushort>();
                velocityXAddress = address++;
                velocityYAddress = address++;
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
