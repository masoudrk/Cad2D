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
            positionX = new Packet<ushort>(22);
            positionY = new Packet<ushort>(23);
            alert = new Packet<ushort>(2);
            manualOrAuto = new Packet<ushort>(3);
            machinPhase = new Packet<ushort>(4);
            water = new Packet<ushort>(202);
            waterTOff = new Packet<ushort>(262);
            waterTOn = new Packet<ushort>(263);
        }
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
            }
        }
    }
}
