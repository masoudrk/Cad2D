using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cad2D
{
  
    public class PlcInformation
    {
        public PlcInformation()
        {
            setAddressValues();
        }

        public void setAddressValues()
        {
            verticalSobCount = new Packet<ushort>(11);//c
            horizontalSobCount = new Packet<ushort>(12);//c
            edgeSobCount = new Packet<ushort>(13);//c
            sobPhase = new Packet<ushort>(10);//c
            edgeEndStart = new Packet<ushort>(210);//f
            verticalSob = new Packet<ushort>(211);//f
            horizontalSob = new Packet<ushort>(212);//f
            edgeSob = new Packet<ushort>(213);//f

            positionX = new Packet<ushort>(22);
            positionY = new Packet<ushort>(23);
            alert = new Packet<ushort>(2);
            manualOrAuto = new Packet<ushort>(3);
            machinPhase = new Packet<ushort>(4);
            water = new Packet<ushort>(202);
            waterTOff = new Packet<ushort>(262);
            waterTOn = new Packet<ushort>(263);
        }
        public Packet<ushort> verticalSobCount;
        public Packet<ushort> horizontalSobCount;
        public Packet<ushort> edgeSobCount;
        public Packet<ushort> sobPhase;

        public Packet<ushort> edgeEndStart;
        public Packet<ushort> verticalSob;
        public Packet<ushort> horizontalSob;
        public Packet<ushort> edgeSob;

        public Packet<ushort> positionX;
        public Packet<ushort> positionY;
        public Packet<ushort> alert ;
        public Packet<ushort> shutdown;
        public Packet<ushort> manualOrAuto;
        public Packet<ushort> water;
        public Packet<ushort> waterTOff;
        public Packet<ushort> waterTOn;
        public Packet<ushort> machinPhase;

        public void writeWater(int data)
        {
            if (CanvasCad2D.lsConnection.Connected)
            {
                CanvasCad2D.lsConnection.writeToPlc(water.dataType, data, water.valueAddress, ref water.writingPacket);
            }
        }
        public void writeWaterTOff(int data)
        {
            if (CanvasCad2D.lsConnection.Connected)
            {
                CanvasCad2D.lsConnection.writeToPlc(waterTOff.dataType, data, waterTOff.valueAddress, ref waterTOff.writingPacket);
            }
        }
        public void writeWaterTOn(int data)
        {
            if (CanvasCad2D.lsConnection.Connected)
            {
                CanvasCad2D.lsConnection.writeToPlc(waterTOn.dataType, data, waterTOn.valueAddress, ref waterTOn.writingPacket);
            }
        }

        public void writeEdgeStartEnd(int data)
        {
            if (CanvasCad2D.lsConnection.Connected)
            {
                CanvasCad2D.lsConnection.writeToPlc(edgeEndStart.dataType, data, edgeEndStart.valueAddress, ref edgeEndStart.writingPacket);
            }
        }

        public void getFirstValues()
        {
            if (CanvasCad2D.lsConnection.Connected)
            {
                CanvasCad2D.lsConnection.readFromPlc(water.dataType, water.valueAddress,
                       ref water.readingPacket);
                CanvasCad2D.lsConnection.readFromPlc(waterTOff.dataType, waterTOff.valueAddress,
                    ref waterTOff.readingPacket);
                CanvasCad2D.lsConnection.readFromPlc(waterTOn.dataType, waterTOn.valueAddress,
                    ref waterTOn.readingPacket);
                CanvasCad2D.lsConnection.readFromPlc(edgeEndStart.dataType, edgeEndStart.valueAddress,
                    ref edgeEndStart.readingPacket);
                CanvasCad2D.lsConnection.readFromPlc(verticalSob.dataType, verticalSob.valueAddress,
                    ref verticalSob.readingPacket);
                CanvasCad2D.lsConnection.readFromPlc(horizontalSob.dataType, horizontalSob.valueAddress,
                    ref horizontalSob.readingPacket);
                CanvasCad2D.lsConnection.readFromPlc(edgeSob.dataType, edgeSob.valueAddress,
                    ref edgeSob.readingPacket);
            }
        }

        public
            void getAllValues()
        {
            if (CanvasCad2D.lsConnection.Connected)
            {
                CanvasCad2D.lsConnection.readFromPlc(positionX.dataType, positionX.valueAddress,
                    ref positionX.readingPacket);
                CanvasCad2D.lsConnection.readFromPlc(positionY.dataType, positionY.valueAddress,
                    ref positionY.readingPacket);
                CanvasCad2D.lsConnection.readFromPlc(alert.dataType, alert.valueAddress,
                    ref alert.readingPacket);
                CanvasCad2D.lsConnection.readFromPlc(manualOrAuto.dataType, manualOrAuto.valueAddress,
                    ref manualOrAuto.readingPacket);
                CanvasCad2D.lsConnection.readFromPlc(machinPhase.dataType, machinPhase.valueAddress,
                    ref machinPhase.readingPacket);

                CanvasCad2D.lsConnection.readFromPlc(verticalSobCount.dataType, verticalSobCount.valueAddress,
                    ref verticalSobCount.readingPacket);
                CanvasCad2D.lsConnection.readFromPlc(horizontalSobCount.dataType, horizontalSobCount.valueAddress,
                    ref horizontalSobCount.readingPacket);
                CanvasCad2D.lsConnection.readFromPlc(edgeSobCount.dataType, edgeSobCount.valueAddress,
                    ref edgeSobCount.readingPacket);
                CanvasCad2D.lsConnection.readFromPlc(sobPhase.dataType, sobPhase.valueAddress,
                    ref sobPhase.readingPacket);
            }
        }
    }
}
