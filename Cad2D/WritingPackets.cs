using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cad2D
{
    public class WritingPackets
    {
        public byte[] stoneScan { set; get; }
        public byte[] stoneHorizontalEdge { set; get; }
        public byte[] stoneVerticalEdge { set; get; }
        public byte[] stoneInnerPoints { set; get; }
        public byte[] stonePoints { set; get; }
        public List<ushort> packetID;
        private byte[] stoneScanArray1 { set; get; }
        private byte[] stoneScanArray2 { set; get; }
        private byte[] stoneScanArray3 { set; get; }
        private byte[] stoneVerticalEdge1 { set; get; }
        private byte[] stoneVerticalEdge2 { set; get; }
        private int stoneScanAddressWord;
        private int stoneVerticalEdgeAddressWord;
        private int stoneHorizontalEdgeAddressWord;
        private int stoneInnerPointsAddressWord;
        private int stonePointsAddressWord;
        public WritingPackets(int stoneScanAddress, int stoneVerticalEdgeAddress
            , int stoneHorizontalEdgeAddress, int stoneInnerPointsAddress , int stonePointsAddress)
        {
            stoneScanAddressWord = stoneScanAddress;
            stoneVerticalEdgeAddressWord = stoneVerticalEdgeAddress;
            stoneHorizontalEdgeAddressWord = stoneHorizontalEdgeAddress;
            stoneInnerPointsAddressWord = stoneInnerPointsAddress;
            stonePointsAddressWord = stonePointsAddress;
            packetID  =  new List<ushort>();
        }

        public bool sendAllData()
        {
            if (CanvasCad2D.lsConnection.Connected)
            {
                if (stoneScan != null && stoneHorizontalEdge != null && stoneVerticalEdge != null &&
                    stoneInnerPoints != null && stonePoints != null )
                {
                    if (stoneScan.Length > 1000 && stoneVerticalEdge.Length > 500 && stoneInnerPoints.Length <= 100 && stonePoints.Length <= 100)
                    {
                        try
                        {
                            stoneScanArray1 = new byte[500];
                            stoneScanArray2 = new byte[500];
                            stoneScanArray3 = new byte[stoneScan.Length - 1000];

                            System.Buffer.BlockCopy(stoneScan, 0, stoneScanArray1, 0, stoneScanArray1.Length);
                            System.Buffer.BlockCopy(stoneScan, 500, stoneScanArray2, 0, stoneScanArray2.Length);
                            System.Buffer.BlockCopy(stoneScan, 1000, stoneScanArray3, 0, stoneScanArray3.Length);

                            stoneVerticalEdge1 = new byte[500];
                            stoneVerticalEdge2 = new byte[stoneVerticalEdge.Length - 500];
                            System.Buffer.BlockCopy(stoneVerticalEdge, 0, stoneVerticalEdge1, 0, stoneVerticalEdge1.Length);
                            System.Buffer.BlockCopy(stoneVerticalEdge, 500, stoneVerticalEdge2, 0, stoneVerticalEdge2.Length);

                        }
                        catch (Exception ex)
                        {
                            Logger.LogError("_File : WrithingPacket1" + "\n_Message : " + ex.Message + "\n_Source : " + ex.Source + "\n_TargetSite : " + ex.TargetSite + "\n", LogType.Error, ex);
                            return false;
                        }
                        CanvasCad2D.lsConnection.writeContinouesToPlc(stoneScanArray1, stoneScanAddressWord * 2, ref packetID);
                        //Logger.LogError("writing packets file reached" , LogType.Info, null);
                        CanvasCad2D.lsConnection.writeContinouesToPlc(stoneScanArray2, stoneScanAddressWord * 2 + 500, ref packetID);
                        CanvasCad2D.lsConnection.writeContinouesToPlc(stoneScanArray3, stoneScanAddressWord * 2 + 1000, ref packetID);
                        CanvasCad2D.lsConnection.writeContinouesToPlc(stoneVerticalEdge1, stoneVerticalEdgeAddressWord * 2, ref packetID);
                        CanvasCad2D.lsConnection.writeContinouesToPlc(stoneVerticalEdge2, stoneVerticalEdgeAddressWord * 2 + 500, ref packetID);
                        CanvasCad2D.lsConnection.writeContinouesToPlc(stoneHorizontalEdge, stoneHorizontalEdgeAddressWord * 2, ref packetID);
                        // for clearing old data
                        byte[] stoneInnerPointsN = new byte[100];
                        byte[] stonePointsN = new byte[100];
                        System.Buffer.BlockCopy(stoneInnerPoints, 0, stoneInnerPointsN, 0, stoneInnerPoints.Length);
                        System.Buffer.BlockCopy(stonePoints, 0, stonePointsN, 0, stonePoints.Length);

                        CanvasCad2D.lsConnection.writeContinouesToPlc(stoneInnerPointsN, stoneInnerPointsAddressWord * 2, ref packetID);
                        CanvasCad2D.lsConnection.writeContinouesToPlc(stonePointsN, stonePointsAddressWord * 2, ref packetID);
                        return true;
                    }
                    return false;
                }
                return false;
            }
            return false;
        }
    }
}
