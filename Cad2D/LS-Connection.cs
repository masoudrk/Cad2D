using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Windows;
using System.Net.NetworkInformation;
using System.Threading;

namespace Cad2D
{
    public enum DataType { BIT, BYTE, WORD, DWORD, CONTINUOUS };
    public class writingPacketInfo
    {
        public writingPacketInfo(DataType dt, int val, int add, ushort or)
        {
            dataType = dt;
            value = val;
            address = add;
            order = or;
        }
        public DataType dataType;
        public int value;
        public int address;
        public ushort order;
    }
    public class readingPacketInfo
    {
        public readingPacketInfo(DataType dt, int add, ushort or)
        {
            dataType = dt;
            address = add;
            order = or;
        }
        public int value;
        public DataType dataType;
        public int address;
        public ushort order;
    }

    public class readingPacketCountinus
    {
        public readingPacketCountinus(byte[] array, ushort or)
        {
            continuousData = array;
            order = or;
        }
        public byte[] continuousData { set; get; }
        public ushort order { set; get; }
    }

    public class LS_Connection
    {
        private Mutex writeToServerMutex = new Mutex();
        private Mutex readToServerMutex = new Mutex();
        private Mutex readEventMutex = new Mutex();
        public static Mutex sendPacketMutex = new Mutex();
        public bool Connected
        {
            get { return connected; }
            set
            {
                connected = value;
                if (value && OnConnect != null)
                    OnConnect(this, null);
                else if (!value && OnDisconnceted != null)
                    OnDisconnceted(this, null);
            }
        }
        public event EventHandler OnDisconnceted;
        public event EventHandler OnConnect;
        public event EventHandler OnWritedSuccessfully;
        public event EventHandler OnReadedSuccessfully;
        public event EventHandler OnReadedContinuous;

        bool connected = true;
        List<writingPacketInfo> writingPacketInfoList;
        List<readingPacketInfo> readingPacketInfoList;
        List<ushort> readingPackeCountinusOrder;
        int portNumber;
        IPAddress Ip;
        TcpClient tcpClient;
        byte[] serverRec;/** the last backet recieved from server**/
        int maxAddress = 8000;
        public static ushort order = 0;
        byte[] packetHeader = { 0x4C, 0x47, 0x49, 0x53, 0x2D, 0x47, 0x4C, 0x4F, 0x46, 0x41, 0x00, 0x00, 0x00, 0x33 };
        public LS_Connection(int ma)
        {
            maxAddress = ma;
            writingPacketInfoList = new List<writingPacketInfo>();
            readingPacketInfoList = new List<readingPacketInfo>();
            readingPackeCountinusOrder = new List<ushort>();
        }

        public bool connect(IPAddress ip, int port)
        {
            Ip = ip;
            portNumber = port;
            if (!PingHost()) return false;
            try
            {
                tcpClient = new TcpClient();
                tcpClient.BeginConnect(Ip, portNumber, new AsyncCallback(onCompleteConnect), tcpClient);
            }
            catch (Exception ex)
            {
                Logger.LogError("_Message : " + ex.Message + "\n\n_Source : " + ex.Source + "\n\n_TargetSite : " + ex.TargetSite + "\n\n _ALL : " + ex.ToString(), LogType.Error, ex);
                Disconnect();
                return false;
            }
            return true;
        }

        public void onCompleteConnect(IAsyncResult iar)
        {
            TcpClient tcpc;
            try
            {
                tcpc = (TcpClient)iar.AsyncState;
                tcpc.EndConnect(iar);
                Connected = true;
                serverRec = new byte[2048];
                tcpc.GetStream().BeginRead(serverRec, 0, serverRec.Length, onCompleteReadFromServer, tcpClient);
            }
            catch (Exception ex)
            {   //when the target machin is 127.0.0.1 it throw exception
                Logger.LogError("_Message : " + ex.Message + "\n\n_Source : " + ex.Source + "\n\n_TargetSite : " + ex.TargetSite + "\n\n _ALL : " + ex.ToString(), LogType.Error, ex);
                Thread.Sleep(1000);
                Disconnect();
            }
        }

        public void sendMessage(byte[] data)
        {
            if (data == null)
                return;

            if (tcpClient == null)
            {
                connect(Ip, portNumber);
                while (tcpClient == null) ;
            }

            try
            {
                if (order < 0xFF)
                    order++;
                else
                    order = 0;
                //sorate ersal vaghti bala bashee lsdsim nemitone sari pars kone vase hamin chandta packet 
                //ba ham merg mishan ke in baes mishe yekish khonde beshe . vase hamin
                //in sleeep ro gozashtam . shayad vase khode plc lazem nabashe
                Thread.Sleep(10);
                tcpClient.GetStream().BeginWrite(data, 0, data.Length, onCompleteWriteToServer, tcpClient);
            }
            catch (Exception ex)
            {
                Logger.LogError("_Message : " + ex.Message + "\n\n_Source : " + ex.Source + "\n\n_TargetSite : " + ex.TargetSite + "\n\n _ALL : " + ex.ToString(), LogType.Error, ex);
                Disconnect();
            }
        }

        private void onCompleteWriteToServer(IAsyncResult ar)
        {

            try
            {
                writeToServerMutex.WaitOne();
                TcpClient tcpc;
                tcpc = (TcpClient)ar.AsyncState;
                tcpc.GetStream().EndWrite(ar);
                writeToServerMutex.ReleaseMutex();
            }
            catch (Exception ex)
            {
                writeToServerMutex.ReleaseMutex();
                Logger.LogError("_Message : " + ex.Message + "\n\n_Source : " + ex.Source + "\n\n_TargetSite : " + ex.TargetSite + "\n\n _ALL : " + ex.ToString(), LogType.Error, ex);
                Disconnect();
            }
        }

        private void onCompleteReadFromServer(IAsyncResult ar)
        {
            try
            {
                readToServerMutex.WaitOne();
                TcpClient tcpc;
                int countBytes;
                tcpc = (TcpClient)ar.AsyncState;

                countBytes = tcpc.GetStream().EndRead(ar);

                if (countBytes == 0)
                {
                    Disconnect();
                    readToServerMutex.ReleaseMutex();
                    return;
                }
                List<byte[]> rawPackets = new List<byte[]>();

                int i = 0;
                while (i < countBytes)
                {
                    if (serverRec[i + 0] == 0x4C && serverRec[i + 1] == 0x47 && serverRec[i + 2] == 0x49 && serverRec[i + 3] == 0x53)
                    {
                        if (serverRec[i + 20] == 0x55)
                        {
                            if (serverRec[22 + i] != 0x14)
                            {
                                if (serverRec[30 + i] == 0x2)
                                {
                                    byte[] _array;
                                    _array = new byte[34];
                                    Buffer.BlockCopy(serverRec, i, _array, 0, _array.Length);
                                    rawPackets.Add(_array);
                                    i += _array.Length;
                                }
                                else if (serverRec[30 + i] == 0x4)
                                {
                                    byte[] _array;
                                    _array = new byte[36];
                                    Buffer.BlockCopy(serverRec, i, _array, 0, _array.Length);
                                    rawPackets.Add(_array);
                                    i += _array.Length;
                                }
                                else if (serverRec[30 + i] == 0x1)
                                {
                                    byte[] _array;
                                    _array = new byte[33];
                                    Buffer.BlockCopy(serverRec, i, _array, 0, _array.Length);
                                    rawPackets.Add(_array);
                                    i += _array.Length;
                                }
                            }
                            else
                            {
                                byte[] _array;
                                _array = new byte[32 + serverRec[30 + i]];
                                Buffer.BlockCopy(serverRec, i, _array, 0, _array.Length);
                                rawPackets.Add(_array);
                                i += _array.Length;
                            }
                        }
                        if (serverRec[i + 20] == 0x59)
                        {
                            byte[] _array;
                            _array = new byte[30];
                            Buffer.BlockCopy(serverRec, i, _array, 0, _array.Length);
                            rawPackets.Add(_array);
                            i += _array.Length;
                        }
                    }
                    else
                    {
                        i++;
                    }
                }
                foreach (byte[] rawPacket in rawPackets)
                {
                    managePacket(rawPacket.Length, rawPacket);
                }

                serverRec = new byte[2048];
                readToServerMutex.ReleaseMutex();
                tcpc.GetStream().BeginRead(serverRec, 0, serverRec.Length, onCompleteReadFromServer, tcpClient);
            }
            catch (Exception ex)
            {
                readToServerMutex.ReleaseMutex();
                Logger.LogError("_Message : " + ex.Message + "\n\n_Source : " + ex.Source + "\n\n_TargetSite : " + ex.TargetSite + "\n\n _ALL : " + ex.ToString(), LogType.Error, ex);
                Disconnect();
            }
        }

        private void managePacket(int countBytes, byte[] _serverRec)
        {
            if (_serverRec[20] == 0x55)
            {
                if (_serverRec[22] != 0x14)
                {
                    int i = readingPacketInfoList.FindIndex(x => x.order == _serverRec[14]);
                    if (i >= 0)
                    {
                        parseData(readingPacketInfoList.ElementAt(i), _serverRec, countBytes);
                        readingPacketInfoList.RemoveAt(i);
                    }
                }
                else
                {
                    int index = readingPackeCountinusOrder.FindIndex(x => x == _serverRec[14]);
                    if (index >= 0)
                    {
                        byte[] _array;
                        _array = new byte[_serverRec[30]];
                        Buffer.BlockCopy(_serverRec, 32, _array, 0, _array.Length);
                        readingPacketCountinus rpc = new readingPacketCountinus(_array,
                            readingPackeCountinusOrder.ElementAt(index));
                        readingPackeCountinusOrder.RemoveAt(index);
                        if (OnReadedContinuous != null)
                            OnReadedContinuous(rpc, null);
                    }
                }
            }
            if (_serverRec[20] == 0x59)
            {
                if (_serverRec[countBytes - 2] == 1)
                {
                    int i = writingPacketInfoList.FindIndex(x => x.order == _serverRec[14]);
                    if (i >= 0)
                    {
                        if (OnWritedSuccessfully != null)
                            OnWritedSuccessfully(writingPacketInfoList.ElementAt(i), null);
                        writingPacketInfoList.RemoveAt(i);
                    }
                }
            }
        }

        private void parseData(readingPacketInfo p, byte[] data, int dataLength)
        {
            switch (p.dataType)
            {
                case DataType.BIT:
                    p.value = data[dataLength - 1];
                    break;

                case DataType.BYTE:
                    p.value = data[dataLength - 1];
                    break;

                case DataType.WORD:
                    p.value = BitConverter.ToInt16(data, dataLength - 2);
                    break;

                case DataType.DWORD:
                    p.value = BitConverter.ToInt32(data, dataLength - 4);
                    break;
            }
            readEventMutex.WaitOne();
            if (OnReadedSuccessfully != null)
                OnReadedSuccessfully(p, null);
            readEventMutex.ReleaseMutex();
        }

        public void Disconnect()
        {
            if (tcpClient != null)
            {
                tcpClient.Close();
                tcpClient = null;
            }
            Connected = false;
            return;
        }

        public bool readFromPlc(DataType dt, int address, ref readingPacketInfo rpi)
        {
            sendPacketMutex.WaitOne();
            if (dt == DataType.CONTINUOUS)
            { sendPacketMutex.ReleaseMutex(); return false; }
            if (dt == DataType.BIT)
            {
                if (address < 0 || address > 16 * maxAddress)
                { sendPacketMutex.ReleaseMutex(); return false; }
                byte[] ins = makeInstruction(DataType.BIT, 0, address, 0);
                byte[] packetInfo = new byte[6];
                byte[] intByte = BitConverter.GetBytes(order);
                packetInfo[0] = intByte[0];
                packetInfo[1] = intByte[1];
                intByte = null;
                intByte = BitConverter.GetBytes((ushort)ins.Length);
                packetInfo[2] = intByte[0];
                packetInfo[3] = intByte[1];
                packetInfo[4] = 0x00;
                packetInfo[5] = calculateCheckSum(packetInfo);

                byte[] rv = new byte[packetHeader.Length + packetInfo.Length + ins.Length];
                System.Buffer.BlockCopy(packetHeader, 0, rv, 0, packetHeader.Length);
                System.Buffer.BlockCopy(packetInfo, 0, rv, packetHeader.Length, packetInfo.Length);
                System.Buffer.BlockCopy(ins, 0, rv, packetHeader.Length + packetInfo.Length, ins.Length);
                rpi = new readingPacketInfo(dt, address, (byte)order);
                readingPacketInfoList.Add(rpi);
                sendMessage(rv);
            }
            else
            if (dt == DataType.BYTE)
            {
                if (address < 0 || address > 2 * maxAddress)
                { sendPacketMutex.ReleaseMutex(); return false; }
                byte[] ins = makeInstruction(DataType.BYTE, 0, address, 0);
                byte[] packetInfo = new byte[6];
                byte[] intByte = BitConverter.GetBytes(order);
                packetInfo[0] = intByte[0];
                packetInfo[1] = intByte[1];
                intByte = null;
                intByte = BitConverter.GetBytes((ushort)ins.Length);
                packetInfo[2] = intByte[0];
                packetInfo[3] = intByte[1];
                packetInfo[4] = 0x00;
                packetInfo[5] = calculateCheckSum(packetInfo);

                byte[] rv = new byte[packetHeader.Length + packetInfo.Length + ins.Length];
                System.Buffer.BlockCopy(packetHeader, 0, rv, 0, packetHeader.Length);
                System.Buffer.BlockCopy(packetInfo, 0, rv, packetHeader.Length, packetInfo.Length);
                System.Buffer.BlockCopy(ins, 0, rv, packetHeader.Length + packetInfo.Length, ins.Length);
                rpi = new readingPacketInfo(dt, address, (byte)order);
                readingPacketInfoList.Add(rpi);
                sendMessage(rv);

            }
            else
            if (dt == DataType.WORD)
            {
                if (address < 0 || address > maxAddress)
                { sendPacketMutex.ReleaseMutex(); return false; }
                byte[] ins = makeInstruction(DataType.WORD, 0, address, 0);
                byte[] packetInfo = new byte[6];
                byte[] intByte = BitConverter.GetBytes(order);
                packetInfo[0] = intByte[0];
                packetInfo[1] = intByte[1];
                intByte = null;
                intByte = BitConverter.GetBytes((ushort)ins.Length);
                packetInfo[2] = intByte[0];
                packetInfo[3] = intByte[1];
                packetInfo[4] = 0x00;
                packetInfo[5] = calculateCheckSum(packetInfo);

                byte[] rv = new byte[packetHeader.Length + packetInfo.Length + ins.Length];
                System.Buffer.BlockCopy(packetHeader, 0, rv, 0, packetHeader.Length);
                System.Buffer.BlockCopy(packetInfo, 0, rv, packetHeader.Length, packetInfo.Length);
                System.Buffer.BlockCopy(ins, 0, rv, packetHeader.Length + packetInfo.Length, ins.Length);
                rpi = new readingPacketInfo(dt, address, (byte)order);
                readingPacketInfoList.Add(rpi);
                sendMessage(rv);

            }
            else
            if (dt == DataType.DWORD)
            {
                if (address < 0 || address > maxAddress / 2)
                { sendPacketMutex.ReleaseMutex(); return false; }
                byte[] ins = makeInstruction(DataType.DWORD, 0, address, 0);
                byte[] packetInfo = new byte[6];
                byte[] intByte = BitConverter.GetBytes(order);
                packetInfo[0] = intByte[0];
                packetInfo[1] = intByte[1];
                intByte = null;
                intByte = BitConverter.GetBytes((ushort)ins.Length);
                packetInfo[2] = intByte[0];
                packetInfo[3] = intByte[1];
                packetInfo[4] = 0x00;
                packetInfo[5] = calculateCheckSum(packetInfo);

                byte[] rv = new byte[packetHeader.Length + packetInfo.Length + ins.Length];
                System.Buffer.BlockCopy(packetHeader, 0, rv, 0, packetHeader.Length);
                System.Buffer.BlockCopy(packetInfo, 0, rv, packetHeader.Length, packetInfo.Length);
                System.Buffer.BlockCopy(ins, 0, rv, packetHeader.Length + packetInfo.Length, ins.Length);
                rpi = new readingPacketInfo(dt, address, (byte)order);
                readingPacketInfoList.Add(rpi);
                sendMessage(rv);
            }
            sendPacketMutex.ReleaseMutex();
            return true;

        }

        public bool readFromPlcContinoues(int address, int address_end, ref List<ushort> PackestId)
        {
            sendPacketMutex.WaitOne();
            if (address < 0 || address > maxAddress * 2)
            { sendPacketMutex.ReleaseMutex(); return false; }
            if (address > address_end)
            { sendPacketMutex.ReleaseMutex(); return false; }

            byte[] ins = makeInstruction(DataType.CONTINUOUS, Math.Abs(address_end - address), address, 0);
            byte[] packetInfo = new byte[6];
            byte[] intByte = BitConverter.GetBytes(order);
            packetInfo[0] = intByte[0];
            packetInfo[1] = intByte[1];
            intByte = null;
            intByte = BitConverter.GetBytes((ushort)ins.Length);
            packetInfo[2] = intByte[0];
            packetInfo[3] = intByte[1];
            packetInfo[4] = 0x00;
            packetInfo[5] = calculateCheckSum(packetInfo);

            byte[] rv = new byte[packetHeader.Length + packetInfo.Length + ins.Length];
            System.Buffer.BlockCopy(packetHeader, 0, rv, 0, packetHeader.Length);
            System.Buffer.BlockCopy(packetInfo, 0, rv, packetHeader.Length, packetInfo.Length);
            System.Buffer.BlockCopy(ins, 0, rv, packetHeader.Length + packetInfo.Length, ins.Length);
            PackestId.Add(order);
            readingPackeCountinusOrder.Add(order);
            sendMessage(rv);

            sendPacketMutex.ReleaseMutex(); return true;
        }

        public bool writeToPlc(DataType dt, int value, int address, ref writingPacketInfo wpi)
        {
            sendPacketMutex.WaitOne();
            if (dt == DataType.CONTINUOUS)
            { sendPacketMutex.ReleaseMutex(); return false; }
            if (dt == DataType.BIT)
            {
                if (value < 0 || value > 1 || address < 0 || address > 16 * maxAddress)
                { sendPacketMutex.ReleaseMutex(); return false; }
                byte[] ins = makeInstruction(DataType.BIT, value, address, 1);
                byte[] packetInfo = new byte[6];
                byte[] intByte = BitConverter.GetBytes(order);
                packetInfo[0] = intByte[0];
                packetInfo[1] = intByte[1];
                intByte = null;
                intByte = BitConverter.GetBytes((ushort)ins.Length);
                packetInfo[2] = intByte[0];
                packetInfo[3] = intByte[1];
                packetInfo[4] = 0x00;
                packetInfo[5] = calculateCheckSum(packetInfo);

                byte[] rv = new byte[packetHeader.Length + packetInfo.Length + ins.Length];
                System.Buffer.BlockCopy(packetHeader, 0, rv, 0, packetHeader.Length);
                System.Buffer.BlockCopy(packetInfo, 0, rv, packetHeader.Length, packetInfo.Length);
                System.Buffer.BlockCopy(ins, 0, rv, packetHeader.Length + packetInfo.Length, ins.Length);
                //packet info for sending packet
                wpi = new writingPacketInfo(dt, value, address, (byte)order);
                writingPacketInfoList.Add(wpi);
                sendMessage(rv);
            }
            else
            if (dt == DataType.BYTE)
            {
                if (value < 0 || value > 0xFF || address < 0 || address > 2 * maxAddress)
                { sendPacketMutex.ReleaseMutex(); return false; }
                byte[] ins = makeInstruction(DataType.BYTE, value, address, 1);
                byte[] packetInfo = new byte[6];
                byte[] intByte = BitConverter.GetBytes(order);
                packetInfo[0] = intByte[0];
                packetInfo[1] = intByte[1];
                intByte = null;
                intByte = BitConverter.GetBytes((ushort)ins.Length);
                packetInfo[2] = intByte[0];
                packetInfo[3] = intByte[1];
                packetInfo[4] = 0x00;
                packetInfo[5] = calculateCheckSum(packetInfo);

                byte[] rv = new byte[packetHeader.Length + packetInfo.Length + ins.Length];
                System.Buffer.BlockCopy(packetHeader, 0, rv, 0, packetHeader.Length);
                System.Buffer.BlockCopy(packetInfo, 0, rv, packetHeader.Length, packetInfo.Length);
                System.Buffer.BlockCopy(ins, 0, rv, packetHeader.Length + packetInfo.Length, ins.Length);
                //packet info for sending packet
                wpi = new writingPacketInfo(dt, value, address, (byte)order);
                writingPacketInfoList.Add(wpi);
                sendMessage(rv);
            }
            else
            if (dt == DataType.WORD)
            {
                if (value < 0 || value > 0xFFFF || address < 0 || address > maxAddress)
                { sendPacketMutex.ReleaseMutex(); return false; }
                byte[] ins = makeInstruction(DataType.WORD, value, address, 1);
                byte[] packetInfo = new byte[6];
                byte[] intByte = BitConverter.GetBytes(order);
                packetInfo[0] = intByte[0];
                packetInfo[1] = intByte[1];
                intByte = null;
                intByte = BitConverter.GetBytes((ushort)ins.Length);
                packetInfo[2] = intByte[0];
                packetInfo[3] = intByte[1];
                packetInfo[4] = 0x00;
                packetInfo[5] = calculateCheckSum(packetInfo);

                byte[] rv = new byte[packetHeader.Length + packetInfo.Length + ins.Length];
                System.Buffer.BlockCopy(packetHeader, 0, rv, 0, packetHeader.Length);
                System.Buffer.BlockCopy(packetInfo, 0, rv, packetHeader.Length, packetInfo.Length);
                System.Buffer.BlockCopy(ins, 0, rv, packetHeader.Length + packetInfo.Length, ins.Length);
                //packet info for sending packet
                wpi = new writingPacketInfo(dt, value, address, (byte)order);
                writingPacketInfoList.Add(wpi);
                sendMessage(rv);

            }
            else
            if (dt == DataType.DWORD)
            {
                if (address < 0 || address > maxAddress / 2)
                { sendPacketMutex.ReleaseMutex(); return false; }
                byte[] ins = makeInstruction(DataType.DWORD, value, address, 1);
                byte[] packetInfo = new byte[6];
                byte[] intByte = BitConverter.GetBytes(order);
                packetInfo[0] = intByte[0];
                packetInfo[1] = intByte[1];
                intByte = null;
                intByte = BitConverter.GetBytes((ushort)ins.Length);
                packetInfo[2] = intByte[0];
                packetInfo[3] = intByte[1];
                packetInfo[4] = 0x00;
                packetInfo[5] = calculateCheckSum(packetInfo);

                byte[] rv = new byte[packetHeader.Length + packetInfo.Length + ins.Length];
                Buffer.BlockCopy(packetHeader, 0, rv, 0, packetHeader.Length);
                System.Buffer.BlockCopy(packetInfo, 0, rv, packetHeader.Length, packetInfo.Length);
                System.Buffer.BlockCopy(ins, 0, rv, packetHeader.Length + packetInfo.Length, ins.Length);
                //packet info for sending packet
                wpi = new writingPacketInfo(dt, value, address, (byte)order);
                writingPacketInfoList.Add(wpi);
                sendMessage(rv);
            }
            sendPacketMutex.ReleaseMutex(); return true;
        }



        public bool writeToPlc(DataType dt, int value, int address, ref List<writingPacketInfo> wpiList)
        {
            sendPacketMutex.WaitOne();
            if (dt == DataType.CONTINUOUS)
            { sendPacketMutex.ReleaseMutex(); return false; }
            if (dt == DataType.BIT)
            {
                if (value < 0 || value > 1 || address < 0 || address > 16 * maxAddress)
                { sendPacketMutex.ReleaseMutex(); return false; }
                byte[] ins = makeInstruction(DataType.BIT, value, address, 1);
                byte[] packetInfo = new byte[6];
                byte[] intByte = BitConverter.GetBytes(order);
                packetInfo[0] = intByte[0];
                packetInfo[1] = intByte[1];
                intByte = null;
                intByte = BitConverter.GetBytes((ushort)ins.Length);
                packetInfo[2] = intByte[0];
                packetInfo[3] = intByte[1];
                packetInfo[4] = 0x00;
                packetInfo[5] = calculateCheckSum(packetInfo);

                byte[] rv = new byte[packetHeader.Length + packetInfo.Length + ins.Length];
                System.Buffer.BlockCopy(packetHeader, 0, rv, 0, packetHeader.Length);
                System.Buffer.BlockCopy(packetInfo, 0, rv, packetHeader.Length, packetInfo.Length);
                System.Buffer.BlockCopy(ins, 0, rv, packetHeader.Length + packetInfo.Length, ins.Length);
                //packet info for sending packet
                writingPacketInfo wpi = new writingPacketInfo(dt, value, address, (byte)order);
                writingPacketInfoList.Add(wpi);
                wpiList.Add(wpi);
                sendMessage(rv);
            }
            else
            if (dt == DataType.BYTE)
            {
                if (value < 0 || value > 0xFF || address < 0 || address > 2 * maxAddress)
                { sendPacketMutex.ReleaseMutex(); return false; }
                byte[] ins = makeInstruction(DataType.BYTE, value, address, 1);
                byte[] packetInfo = new byte[6];
                byte[] intByte = BitConverter.GetBytes(order);
                packetInfo[0] = intByte[0];
                packetInfo[1] = intByte[1];
                intByte = null;
                intByte = BitConverter.GetBytes((ushort)ins.Length);
                packetInfo[2] = intByte[0];
                packetInfo[3] = intByte[1];
                packetInfo[4] = 0x00;
                packetInfo[5] = calculateCheckSum(packetInfo);

                byte[] rv = new byte[packetHeader.Length + packetInfo.Length + ins.Length];
                System.Buffer.BlockCopy(packetHeader, 0, rv, 0, packetHeader.Length);
                System.Buffer.BlockCopy(packetInfo, 0, rv, packetHeader.Length, packetInfo.Length);
                System.Buffer.BlockCopy(ins, 0, rv, packetHeader.Length + packetInfo.Length, ins.Length);
                //packet info for sending packet
                writingPacketInfo wpi = new writingPacketInfo(dt, value, address, (byte)order);
                writingPacketInfoList.Add(wpi);
                wpiList.Add(wpi);
                sendMessage(rv);
            }
            else
            if (dt == DataType.WORD)
            {
                if (value < 0 || value > 0xFFFF || address < 0 || address > maxAddress)
                { sendPacketMutex.ReleaseMutex(); return false; }
                byte[] ins = makeInstruction(DataType.WORD, value, address, 1);
                byte[] packetInfo = new byte[6];
                byte[] intByte = BitConverter.GetBytes(order);
                packetInfo[0] = intByte[0];
                packetInfo[1] = intByte[1];
                intByte = null;
                intByte = BitConverter.GetBytes((ushort)ins.Length);
                packetInfo[2] = intByte[0];
                packetInfo[3] = intByte[1];
                packetInfo[4] = 0x00;
                packetInfo[5] = calculateCheckSum(packetInfo);

                byte[] rv = new byte[packetHeader.Length + packetInfo.Length + ins.Length];
                System.Buffer.BlockCopy(packetHeader, 0, rv, 0, packetHeader.Length);
                System.Buffer.BlockCopy(packetInfo, 0, rv, packetHeader.Length, packetInfo.Length);
                System.Buffer.BlockCopy(ins, 0, rv, packetHeader.Length + packetInfo.Length, ins.Length);
                //packet info for sending packet
                writingPacketInfo wpi = new writingPacketInfo(dt, value, address, (byte)order);
                writingPacketInfoList.Add(wpi);
                wpiList.Add(wpi);
                sendMessage(rv);

            }
            else
            if (dt == DataType.DWORD)
            {
                if (address < 0 || address > maxAddress / 2)
                { sendPacketMutex.ReleaseMutex(); return false; }
                byte[] ins = makeInstruction(DataType.DWORD, value, address, 1);
                byte[] packetInfo = new byte[6];
                byte[] intByte = BitConverter.GetBytes(order);
                packetInfo[0] = intByte[0];
                packetInfo[1] = intByte[1];
                intByte = null;
                intByte = BitConverter.GetBytes((ushort)ins.Length);
                packetInfo[2] = intByte[0];
                packetInfo[3] = intByte[1];
                packetInfo[4] = 0x00;
                packetInfo[5] = calculateCheckSum(packetInfo);

                byte[] rv = new byte[packetHeader.Length + packetInfo.Length + ins.Length];
                System.Buffer.BlockCopy(packetHeader, 0, rv, 0, packetHeader.Length);
                System.Buffer.BlockCopy(packetInfo, 0, rv, packetHeader.Length, packetInfo.Length);
                System.Buffer.BlockCopy(ins, 0, rv, packetHeader.Length + packetInfo.Length, ins.Length);
                //packet info for sending packet
                writingPacketInfo wpi = new writingPacketInfo(dt, value, address, (byte)order);
                writingPacketInfoList.Add(wpi);
                wpiList.Add(wpi);
                sendMessage(rv);
            }
            sendPacketMutex.ReleaseMutex(); return true;
        }
        private byte calculateCheckSum(byte[] header)
        {
            int sum = 0;
            for (int i = 0; i < 5; i++)
                sum += header[i];
            sum += 760;
            byte[] intByte = BitConverter.GetBytes(sum);
            return intByte[0];
        }

        private byte[] makeInstruction(DataType dt, int value, int address, int _instruction)
        {
            string add = "%M";
            byte[] ins_Head = new byte[10];

            if (_instruction == 0)
            {
                ins_Head[0] = 0x54;
                ins_Head[1] = 0x00;
            }
            else
            {
                ins_Head[0] = 0x58;
                ins_Head[1] = 0x00;
            }

            switch (dt)
            {
                case DataType.BIT:
                    add += "X";
                    ins_Head[2] = 0x00;
                    ins_Head[3] = 0x00;
                    break;

                case DataType.BYTE:
                    add += "B";
                    ins_Head[2] = 0x01;
                    ins_Head[3] = 0x00;
                    break;

                case DataType.WORD:
                    add += "W";
                    ins_Head[2] = 0x02;
                    ins_Head[3] = 0x00;
                    break;

                case DataType.DWORD:
                    add += "D";
                    ins_Head[2] = 0x03;
                    ins_Head[3] = 0x00;
                    break;
                case DataType.CONTINUOUS:
                    add += "B";
                    ins_Head[2] = 0x14;
                    ins_Head[3] = 0x00;
                    break;
            }
            ins_Head[4] = 0x00;
            ins_Head[5] = 0x00;
            ins_Head[6] = 0x01;
            ins_Head[7] = 0x00;
            add += address.ToString();
            ins_Head[8] = (byte)add.Length;
            ins_Head[9] = 0x00;
            byte[] addre = Encoding.ASCII.GetBytes(add);
            byte[] value_Byte;
            if (dt == DataType.CONTINUOUS)
            {

                value_Byte = new byte[2];
                ushort val = (ushort)value;
                byte[] intByt = BitConverter.GetBytes(val);
                value_Byte[0] = intByt[0];
                value_Byte[1] = intByt[1];
            }
            else if (_instruction == 1)
            {
                switch (dt)
                {
                    case DataType.BIT:
                        value_Byte = new byte[3];
                        value_Byte[0] = 0x01;
                        value_Byte[1] = 0x00;
                        value_Byte[2] = (byte)value;
                        break;

                    case DataType.BYTE:
                        value_Byte = new byte[3];
                        value_Byte[0] = 0x01;
                        value_Byte[1] = 0x00;
                        value_Byte[2] = (byte)value;
                        break;

                    case DataType.WORD:
                        value_Byte = new byte[4];
                        value_Byte[0] = 0x02;
                        value_Byte[1] = 0x00;
                        ushort vavl = (ushort)value;
                        byte[] intBytes = BitConverter.GetBytes(vavl);
                        value_Byte[2] = intBytes[0];
                        value_Byte[3] = intBytes[1];

                        break;

                    case DataType.DWORD:
                        value_Byte = new byte[6];
                        value_Byte[0] = 0x04;
                        value_Byte[1] = 0x00;
                        byte[] intByte = BitConverter.GetBytes(value);
                        value_Byte[2] = intByte[0];
                        value_Byte[3] = intByte[1];
                        value_Byte[4] = intByte[2];
                        value_Byte[5] = intByte[3];
                        break;

                    default:
                        value_Byte = new byte[1];
                        break;
                }
            }
            else
            {
                byte[] rv1 = new byte[ins_Head.Length + addre.Length];
                System.Buffer.BlockCopy(ins_Head, 0, rv1, 0, ins_Head.Length);
                System.Buffer.BlockCopy(addre, 0, rv1, ins_Head.Length, addre.Length);
                return rv1;
            }

            byte[] rv = new byte[ins_Head.Length + addre.Length + value_Byte.Length];
            System.Buffer.BlockCopy(ins_Head, 0, rv, 0, ins_Head.Length);
            System.Buffer.BlockCopy(addre, 0, rv, ins_Head.Length, addre.Length);
            System.Buffer.BlockCopy(value_Byte, 0, rv, ins_Head.Length + addre.Length, value_Byte.Length);
            return rv;
        }

        internal void set(IPAddress _ip, int _portNumber)
        {
            this.Ip = _ip;
            this.portNumber = _portNumber;
        }


        #region ping region

        public bool PingHost()
        {
            IPAddress nameOrAddress = Ip;
            bool pingable = false;
            Ping pinger = new Ping();
            string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
            byte[] buffer = Encoding.ASCII.GetBytes(data);
            int timeOut = 500;
            PingOptions options = new PingOptions(64, true);
            try
            {
                PingReply reply = pinger.Send(nameOrAddress, timeOut, buffer, options);
                pingable = reply.Status == IPStatus.Success;
            }
            catch (PingException)
            {
                Connected = false;
                return false;
            }
            return pingable;
        }
        #endregion
    }

    public class Packet<T>
    {
        public Packet()
        {

            if (typeof(T) == typeof(bool))
            {
                dataType = DataType.BIT;
            }
            else if (typeof(T) == typeof(byte))
            {
                dataType = DataType.BYTE;
            }
            else if (typeof(T) == typeof(ushort))
            {
                dataType = DataType.WORD;
            }
            else if (typeof(T) == typeof(uint))
            {
                dataType = DataType.DWORD;
            }
            else
            {
                dataType = DataType.WORD;
            }
        }
        public Packet(int _address)
        {

            if (typeof(T) == typeof(bool))
            {
                dataType = DataType.BIT;
            }
            else if (typeof(T) == typeof(byte))
            {
                dataType = DataType.BYTE;
            }
            else if (typeof(T) == typeof(ushort))
            {
                dataType = DataType.WORD;
            }
            else if (typeof(T) == typeof(uint))
            {
                dataType = DataType.DWORD;
            }
            else
            {
                dataType = DataType.WORD;
            }

            valueAddress = _address;
        }
        public Packet(int _address, T _value)
        {

            if (typeof(T) == typeof(bool))
            {
                dataType = DataType.BIT;
            }
            else if (typeof(T) == typeof(byte))
            {
                dataType = DataType.BYTE;
            }
            else if (typeof(T) == typeof(ushort))
            {
                dataType = DataType.WORD;
            }
            else if (typeof(T) == typeof(uint))
            {
                dataType = DataType.DWORD;
            }
            else
            {
                dataType = DataType.WORD;
            }
            value = _value;
            valueAddress = _address;
        }
        public writingPacketInfo writingPacket;
        public readingPacketInfo readingPacket;
        public T value { set; get; }
        public int valueAddress { set; get; }
        public DataType dataType { set; get; }
    }
}
