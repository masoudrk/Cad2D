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
        public _Encoder Encoder { get; set; }

        public Packet<ushort> DiskOutOfStnFstAXY { set; get; }
        public Packet<ushort> DiskOutOfStnEndAXY { set; get; }
        public Packet<ushort> DiskOutOfStnFstAXX { set; get; }
        public Packet<ushort> DiskOutOfStnEndAXX { set; get; }
        public Packet<uint> ClampAmount { set; get; }
        public Packet<ushort> HashyeBack { set; get; }
        public Packet<ushort> HashyeFront { set; get; }
        public Packet<ushort> HashyeEdge { set; get; }

        

        public PlcUtilitisAndOptions()
        {
            setNewValues();
        }
        public void setNewValues()
        {
            PrimarySettings ps = Extentions.FromXmlPrimary();
            Velocity = new _velocity((ushort)ps.Velocity);//955
            Encoder = new _Encoder((ushort)ps.XEncoderMem, (ushort)ps.YEncoderMem);//980,984

            DiskOutOfStnFstAXY = new Packet<ushort>(233);
            DiskOutOfStnEndAXY = new Packet<ushort>(234);
            DiskOutOfStnFstAXX = new Packet<ushort>(235);
            DiskOutOfStnEndAXX = new Packet<ushort>(236);
            HashyeBack = new Packet<ushort>(258);
            HashyeFront = new Packet<ushort>(257);
            HashyeEdge = new Packet<ushort>(259);
            ClampAmount = new Packet<uint>(135);
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
        public void getAllClapAndBridge()
        {
            if (CanvasCad2D.lsConnection.Connected)
            {
                CanvasCad2D.lsConnection.readFromPlc(DiskOutOfStnFstAXY.dataType, DiskOutOfStnFstAXY.valueAddress,
                    ref DiskOutOfStnFstAXY.readingPacket);
                CanvasCad2D.lsConnection.readFromPlc(DiskOutOfStnEndAXY.dataType, DiskOutOfStnEndAXY.valueAddress,
                    ref DiskOutOfStnEndAXY.readingPacket);
                CanvasCad2D.lsConnection.readFromPlc(DiskOutOfStnFstAXX.dataType, DiskOutOfStnFstAXX.valueAddress,
                    ref DiskOutOfStnFstAXX.readingPacket);
                CanvasCad2D.lsConnection.readFromPlc(DiskOutOfStnEndAXX.dataType, DiskOutOfStnEndAXX.valueAddress,
                    ref DiskOutOfStnEndAXX.readingPacket);
                CanvasCad2D.lsConnection.readFromPlc(HashyeFront.dataType, HashyeFront.valueAddress,
                    ref HashyeFront.readingPacket);
                CanvasCad2D.lsConnection.readFromPlc(HashyeBack.dataType, HashyeBack.valueAddress,
                    ref HashyeBack.readingPacket);
                CanvasCad2D.lsConnection.readFromPlc(HashyeEdge.dataType, HashyeEdge.valueAddress,
                    ref HashyeEdge.readingPacket);
                CanvasCad2D.lsConnection.readFromPlc(ClampAmount.dataType, ClampAmount.valueAddress,
                    ref ClampAmount.readingPacket);
            }
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
