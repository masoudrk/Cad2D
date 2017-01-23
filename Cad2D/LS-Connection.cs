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

    public class writingPacketCountinus
    {
        public writingPacketCountinus( ushort or)
        {
            order = or;
        }
        public ushort order { set; get; }
    }

    public class LS_Connection
    {
        //private Mutex writeToServerMutex = new Mutex();
        //private Mutex readToServerMutex = new Mutex();
        //private Mutex readEventMutex = new Mutex();
        //public static Mutex sendPacketMutex = new Mutex();
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
        public event EventHandler OnWritedContinuous;

        private Queue<PLCRequest> requestQueue;
        private PLCRequest curentRequest = null;
        private Thread sendMesssageThread;
        private ushort answeredPacket;
        bool connected = true;
        List<writingPacketInfo> writingPacketInfoList;
        List<readingPacketInfo> readingPacketInfoList;
        List<ushort> readingPackeCountinusOrder;
        List<ushort> writingPackeCountinusOrder;
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
            writingPackeCountinusOrder  =new List<ushort>();
            requestQueue = new Queue<PLCRequest>();
        }

        public bool connect(IPAddress ip, int port)
        {
            writingPacketInfoList = new List<writingPacketInfo>();
            readingPacketInfoList = new List<readingPacketInfo>();
            readingPackeCountinusOrder = new List<ushort>();
            writingPackeCountinusOrder = new List<ushort>();
            requestQueue = new Queue<PLCRequest>();
            order = 0;
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
                Logger.LogError("_File : LsConnection1" + "\n_Message : " + ex.Message + "\n_Source : " + ex.Source + "\n_TargetSite : " + ex.TargetSite + "\n" , LogType.Error, ex);
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
                sendMesssageThread = new Thread(sendingMessageLoop);
                sendMesssageThread.Start();
                tcpc.GetStream().BeginRead(serverRec, 0, serverRec.Length, onCompleteReadFromServer, tcpClient);
            }
            catch (Exception ex)
            {   //when the target machin is 127.0.0.1 it throw exception
                Logger.LogError("_File : LsConnection2" + "\n_Message : " + ex.Message + "\n_Source : " + ex.Source + "\n_TargetSite : " + ex.TargetSite + "\n", LogType.Error, ex);
                Thread.Sleep(1000);
                Disconnect();
            }
        }

        private void sendingMessageLoop()
        {
            int timeCounter = 0;
            while (true)
            {
                if (requestQueue.Count > 0 && curentRequest == null)
                {
                    curentRequest = requestQueue.Dequeue();
                    if(curentRequest != null)
                    switch (curentRequest.requestType)
                    {
                        case PLCRequest._RequestType.READ_CONTINUOUS:
                            _readFromPlcContinoues(curentRequest.address, curentRequest.address_end,
                                ref curentRequest.PackestId , curentRequest.orderID);
                            break;
                        case PLCRequest._RequestType.WRITE_CONTINUOUS:
                            _writeContinouesToPlc(curentRequest.arrayValue, curentRequest.address,ref curentRequest.PackestId, curentRequest.orderID)
                            ;
                            break;
                        case PLCRequest._RequestType.READ:
                            _readFromPlc(curentRequest.dataType, curentRequest.address, ref curentRequest.rpi, curentRequest.orderID);
                            break;
                        case PLCRequest._RequestType.WRITE_ONE_PACKET:;
                            _writeToPlc(curentRequest.dataType, curentRequest.value, curentRequest.address, ref curentRequest.wpi, curentRequest.orderID)
                            ;
                            break;
                        case PLCRequest._RequestType.WRITE_LIST_PACKET:
                            _writeToPlc(curentRequest.dataType, curentRequest.value, curentRequest.address, ref curentRequest.wpi, curentRequest.orderID)
                            ;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                if (curentRequest != null && answeredPacket == curentRequest.orderID)
                {
                    curentRequest = null;
                    timeCounter = 0;
                }
                if (timeCounter >= 10)
                {
                    if(curentRequest != null)
                        requestQueue.Enqueue(curentRequest);
                    timeCounter = 0;
                    curentRequest = null;
                }
                Thread.Sleep(10);
                timeCounter++;
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
                
                //sorate ersal vaghti bala bashee lsdsim nemitone sari pars kone vase hamin chandta packet 
                //ba ham merg mishan ke in baes mishe yekish khonde beshe . vase hamin
                //in sleeep ro gozashtam . shayad vase khode plc lazem nabashe
                //Thread.Sleep(500);
                if (tcpClient.Connected && Thread.CurrentThread.ThreadState != ThreadState.Aborted)
                    tcpClient.GetStream().BeginWrite(data, 0, data.Length, onCompleteWriteToServer, tcpClient);
                else
                    Disconnect();
            }
            catch (Exception ex)
            {
                Logger.LogError("_File : LsConnection3" + "\n_Message : " + ex.Message + "\n_Source : " + ex.Source + "\n_TargetSite : " + ex.TargetSite + "\n", LogType.Error, ex);
                Disconnect();
            }
        }

        private void onCompleteWriteToServer(IAsyncResult ar)
        {
            try
            {
                /*writeToServerMutex.WaitOne();*/
                TcpClient tcpc;
                tcpc = (TcpClient) ar.AsyncState;
                tcpc.GetStream().EndWrite(ar);
                /*writeToServerMutex.ReleaseMutex();*/
            }
            catch (Exception ex)
            {
                Logger.LogError("_File : LsConnection4" + "\n_Message : " + ex.Message + "\n_Source : " + ex.Source + "\n_TargetSite : " + ex.TargetSite + "\n", LogType.Error, ex);
                Disconnect();
            }
        }

        private void onCompleteReadFromServer(IAsyncResult ar)
        {
            try
            {
                /*readToServerMutex.WaitOne();*/
                TcpClient tcpc;
                int countBytes;
                tcpc = (TcpClient) ar.AsyncState;

                if (tcpc.Connected)
                    countBytes = tcpc.GetStream().EndRead(ar);
                else
                {
                    Disconnect();
                    return;
                }

                if (countBytes == 0)
                {
                    Disconnect();
                    /*readToServerMutex.ReleaseMutex();*/
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
                                    var _array = new byte[34];
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
                /*readToServerMutex.ReleaseMutex();*/
                if (tcpc.Connected)
                    tcpc.GetStream().BeginRead(serverRec, 0, serverRec.Length, onCompleteReadFromServer, tcpClient);
                else
                {
                    Disconnect();
                }
            }
            catch (Exception ex)
            {
                //Object synchronization method was called from an unsynchronized block of code
                Logger.LogError("_File : LsConnection5" + "\n_Message : " + ex.Message + "\n_Source : " + ex.Source + "\n_TargetSite : " + ex.TargetSite + "\n", LogType.Error, ex);
                Disconnect();
            }
        }

        private void managePacket(int countBytes, byte[] _serverRec)
        {
            answeredPacket = _serverRec[14];
            if (_serverRec[20] == 0x55)
            {
                if (_serverRec[22] != 0x14)
                {
                    if (readingPacketInfoList == null)
                    {
                        Disconnect();
                        return;
                    }

                    int i = readingPacketInfoList.FindIndex(x => x.order == _serverRec[14]);
                    if (i >= 0)
                    {
                        parseData(readingPacketInfoList.ElementAt(i), _serverRec, countBytes);
                        readingPacketInfoList.RemoveAt(i);
                    }
                }
                else
                {
                    if (readingPackeCountinusOrder == null)
                    {
                        Disconnect();
                        return;
                    }
                    int index = readingPackeCountinusOrder.FindIndex(x => x == _serverRec[14]);
                    if (index >= 0)
                    {
                        var _array = new byte[_serverRec[30]];
                        try
                        {
                            Buffer.BlockCopy(_serverRec, 32, _array, 0, _array.Length);

                            readingPacketCountinus rpc = new readingPacketCountinus(_array, readingPackeCountinusOrder.ElementAt(index));
                            readingPackeCountinusOrder.RemoveAt(index);
                            OnReadedContinuous?.Invoke(rpc, null);
                        }
                        catch (Exception ex)
                        {
                            Logger.LogError("_File : LsConnection6" + "\n_Message : " + ex.Message + "\n_Source : " + ex.Source + "\n_TargetSite : " + ex.TargetSite + "\n", LogType.Error, ex);
                            
                        }
                    }
                }
            }
            if (_serverRec[20] == 0x59)
            {
                //Logger.LogError("writed packet callback line 384", LogType.Info, null);
                if (_serverRec[22] != 0x14)
                {
                    if (_serverRec[countBytes - 2] == 1)
                    {
                        if (writingPacketInfoList == null)
                        {
                            Disconnect();
                            return;
                        }
                        int i = writingPacketInfoList.FindIndex(x => x.order == _serverRec[14]);
                        if (i >= 0)
                        {
                            if (OnWritedSuccessfully != null)
                                OnWritedSuccessfully(writingPacketInfoList.ElementAt(i), null);
                            writingPacketInfoList.RemoveAt(i);
                        }
                    }
                }
                else
                {
                    if (writingPackeCountinusOrder == null)
                    {
                        Disconnect();
                        return;
                    }
                    int index = writingPackeCountinusOrder.FindIndex(x => x == _serverRec[14]);
                    if (index >= 0)
                    {
                        writingPacketCountinus wpc = new writingPacketCountinus(writingPackeCountinusOrder.ElementAt(index));
                        writingPackeCountinusOrder.RemoveAt(index);
                        if (OnWritedContinuous != null)
                            OnWritedContinuous(wpc, null);
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
            OnReadedSuccessfully?.Invoke(p, null);
        }

        public void Disconnect()
        {
            try
            {
                sendMesssageThread?.Abort();
            }
            catch (Exception e)
            {
                Logger.LogError("cannot abort sendMesssageThread", LogType.Error, e);
            }
            finally
            {
                sendMesssageThread?.Abort();
            }
            if (tcpClient != null)
            {
                tcpClient.Close();
                tcpClient = null;
            }
            Connected = false;

            //writeToServerMutex.Dispose();
            //readToServerMutex.Dispose();
            //readEventMutex.Dispose();
            //sendPacketMutex.Dispose();

            //writeToServerMutex = new Mutex();
            //readToServerMutex = new Mutex();
            //readEventMutex = new Mutex();
            //sendPacketMutex = new Mutex();
        }

        public bool readFromPlc(DataType dt, int address, ref readingPacketInfo rpi)
        {
            rpi = new readingPacketInfo(dt, address, (byte)order);
            /*sendPacketMutex.WaitOne();*/
            if (dt == DataType.CONTINUOUS)
            {
                /*sendPacketMutex.ReleaseMutex(); */
                return false;
            }
            if (dt == DataType.BIT)
            {
                if (address < 0 || address > 16 * maxAddress)
                {
                    /*sendPacketMutex.ReleaseMutex(); */
                    return false;
                }
            }
            else if (dt == DataType.BYTE)
            {
                if (address < 0 || address > 2 * maxAddress)
                {
                    /*sendPacketMutex.ReleaseMutex(); */
                    return false;
                }
            }
            else if (dt == DataType.WORD)
            {
                if (address < 0 || address > maxAddress)
                {
                    /*sendPacketMutex.ReleaseMutex(); */
                    return false;
                }
            }
            else if (dt == DataType.DWORD)
            {
                if (address < 0 || address > maxAddress / 2)
                {
                    /*sendPacketMutex.ReleaseMutex();*/
                    return false;
                }
            }
            requestQueue.Enqueue(new PLCRequest(dt , address, ref rpi));
            if (order < 0xFF)
                order++;
            else
                order = 0;
            return true;
        }

        public bool readFromPlcContinoues(int address, int address_end, ref List<ushort> PacketsId)
        {
            PacketsId.Add(order);
            /*sendPacketMutex.WaitOne();*/
            if (address < 0 || address > maxAddress * 2)
            {
                /*sendPacketMutex.ReleaseMutex();*/
                return false;
            }
            if (address > address_end)
            {
                /*sendPacketMutex.ReleaseMutex();*/
                return false;
            }
            requestQueue.Enqueue(new PLCRequest(address , address_end , ref PacketsId));
            /*sendPacketMutex.ReleaseMutex(); */
            if (order < 0xFF)
                order++;
            else
                order = 0;
            return true;
        }

        public bool writeToPlc(DataType dt, int value, int address, ref writingPacketInfo wpi)
        {
            wpi = new writingPacketInfo(dt, value, address, (byte)order);
            /*sendPacketMutex.WaitOne();*/
            if (dt == DataType.CONTINUOUS)
            {
                /*sendPacketMutex.ReleaseMutex();*/
                return false;
            }
            if (dt == DataType.BIT)
            {
                if (value < 0 || value > 1 || address < 0 || address > 16 * maxAddress)
                {
                    /* sendPacketMutex.ReleaseMutex();*/
                    return false;
                }
            }
            else if (dt == DataType.BYTE)
            {
                if (value < 0 || value > 0xFF || address < 0 || address > 2 * maxAddress)
                {
                    /*sendPacketMutex.ReleaseMutex(); */
                    return false;
                }
            }
            else if (dt == DataType.WORD)
            {
                if (value < 0 || value > 0xFFFF || address < 0 || address > maxAddress)
                {
                    /*sendPacketMutex.ReleaseMutex();*/
                    return false;
                }
            }
            else if (dt == DataType.DWORD)
            {
                if (address < 0 || address > maxAddress / 2)
                {
                    /*sendPacketMutex.ReleaseMutex(); */
                    return false;
                }
            }
            requestQueue.Enqueue( new PLCRequest(dt , value , address , ref wpi));
            /*sendPacketMutex.ReleaseMutex(); */
            if (order < 0xFF)
                order++;
            else
                order = 0;
            return true;
        }

        public bool writeToPlc(DataType dt, int value, int address, ref List<writingPacketInfo> wpiList)
        {

            //Logger.LogError("writeToPlc line 591", LogType.Info, null);
            writingPacketInfo wpi = new writingPacketInfo(dt, value, address, (byte)order);
            wpiList.Add(wpi);
            /*sendPacketMutex.WaitOne();*/
            if (dt == DataType.CONTINUOUS)
            {
                /*sendPacketMutex.ReleaseMutex();*/
                return false;
            }
            if (dt == DataType.BIT)
            {
                if (value < 0 || value > 1 || address < 0 || address > 16 * maxAddress)
                {
                    /* sendPacketMutex.ReleaseMutex(); */
                    return false;
                }
            }
            else if (dt == DataType.BYTE)
            {
                if (value < 0 || value > 0xFF || address < 0 || address > 2 * maxAddress)
                {
                    /*sendPacketMutex.ReleaseMutex(); */
                    return false;
                }
            }
            else if (dt == DataType.WORD)
            {
                if (value < 0 || value > 0xFFFF || address < 0 || address > maxAddress)
                {
                    /*sendPacketMutex.ReleaseMutex();*/
                    return false;
                }
            }
            else if (dt == DataType.DWORD)
            {
                if (address < 0 || address > maxAddress / 2)
                {
                    /*sendPacketMutex.ReleaseMutex(); */
                    return false;
                }
            }
            /*sendPacketMutex.ReleaseMutex();*/

            requestQueue.Enqueue(new PLCRequest(dt, value, address, ref wpi));
            if (order < 0xFF)
                order++;
            else
                order = 0;
            return true;
        }

        public bool writeContinouesToPlc(byte[] value, int address, ref List<ushort> PackestId)
        {
            //Logger.LogError("writeContinouesToPlc line 644", LogType.Info, null);
            PackestId.Add((ushort)order);
            if (value.Length > 1400 || value.Length == 0 || address < 0 || address > 16 * maxAddress)
            {
                /* sendPacketMutex.ReleaseMutex(); */
                return false;
            }
            requestQueue.Enqueue(new PLCRequest(value , address , ref PackestId));
            if (order < 0xFF)
                order++;
            else
                order = 0;
            return true;
        }
        private bool _readFromPlc(DataType dt, int address, ref readingPacketInfo rpi, ushort _order)
        {
            /*sendPacketMutex.WaitOne();*/
            if (dt == DataType.CONTINUOUS)
            {
                /*sendPacketMutex.ReleaseMutex(); */
                return false;
            }
            if (dt == DataType.BIT)
            {
                if (address < 0 || address > 16 * maxAddress)
                {
                    /*sendPacketMutex.ReleaseMutex(); */
                    return false;
                }
                byte[] ins = makeInstruction(DataType.BIT, 0, address, 0);
                byte[] packetInfo = new byte[6];
                byte[] intByte = BitConverter.GetBytes(_order);
                packetInfo[0] = intByte[0];
                packetInfo[1] = intByte[1];
                intByte = BitConverter.GetBytes((ushort)ins.Length);
                packetInfo[2] = intByte[0];
                packetInfo[3] = intByte[1];
                int checkSum = calculateCheckSum(packetInfo);
                intByte = null;
                intByte = BitConverter.GetBytes(checkSum);
                packetInfo[4] = intByte[0];
                packetInfo[5] = intByte[1];

                byte[] rv = new byte[packetHeader.Length + packetInfo.Length + ins.Length];
                try
                {
                    System.Buffer.BlockCopy(packetHeader, 0, rv, 0, packetHeader.Length);
                    System.Buffer.BlockCopy(packetInfo, 0, rv, packetHeader.Length, packetInfo.Length);
                    System.Buffer.BlockCopy(ins, 0, rv, packetHeader.Length + packetInfo.Length, ins.Length);

                }
                catch (Exception ex)
                {
                    Logger.LogError("_File : LsConnection19" + "\n_Message : " + ex.Message + "\n_Source : " + ex.Source + "\n_TargetSite : " + ex.TargetSite + "\n", LogType.Error, ex);
                    
                }
                readingPacketInfoList.Add(rpi);
                sendMessage(rv);
            }
            else if (dt == DataType.BYTE)
            {
                if (address < 0 || address > 2 * maxAddress)
                {
                    /*sendPacketMutex.ReleaseMutex(); */
                    return false;
                }
                byte[] ins = makeInstruction(DataType.BYTE, 0, address, 0);
                byte[] packetInfo = new byte[6];
                byte[] intByte = BitConverter.GetBytes(_order);
                packetInfo[0] = intByte[0];
                packetInfo[1] = intByte[1];
                intByte = null;
                intByte = BitConverter.GetBytes((ushort)ins.Length);
                packetInfo[2] = intByte[0];
                packetInfo[3] = intByte[1];
                int checkSum = calculateCheckSum(packetInfo);
                intByte = null;
                intByte = BitConverter.GetBytes(checkSum);
                packetInfo[4] = intByte[0];
                packetInfo[5] = intByte[1];

                byte[] rv = new byte[packetHeader.Length + packetInfo.Length + ins.Length];
                try
                {
                    Buffer.BlockCopy(packetHeader, 0, rv, 0, packetHeader.Length);
                    Buffer.BlockCopy(packetInfo, 0, rv, packetHeader.Length, packetInfo.Length);
                    Buffer.BlockCopy(ins, 0, rv, packetHeader.Length + packetInfo.Length, ins.Length);

                }
                catch (Exception ex)
                {
                    Logger.LogError("_File : LsConnection18" + "\n_Message : " + ex.Message + "\n_Source : " + ex.Source + "\n_TargetSite : " + ex.TargetSite + "\n", LogType.Error, ex);
                    
                }
                readingPacketInfoList.Add(rpi);
                sendMessage(rv);
            }
            else if (dt == DataType.WORD)
            {
                if (address < 0 || address > maxAddress)
                {
                    /*sendPacketMutex.ReleaseMutex(); */
                    return false;
                }
                byte[] ins = makeInstruction(DataType.WORD, 0, address, 0);
                byte[] packetInfo = new byte[6];
                byte[] intByte = BitConverter.GetBytes(_order);
                packetInfo[0] = intByte[0];
                packetInfo[1] = intByte[1];
                intByte = null;
                intByte = BitConverter.GetBytes((ushort)ins.Length);
                packetInfo[2] = intByte[0];
                packetInfo[3] = intByte[1];
                int checkSum = calculateCheckSum(packetInfo);
                intByte = null;
                intByte = BitConverter.GetBytes(checkSum);
                packetInfo[4] = intByte[0];
                packetInfo[5] = intByte[1];

                byte[] rv = new byte[packetHeader.Length + packetInfo.Length + ins.Length];
                try
                {
                    System.Buffer.BlockCopy(packetHeader, 0, rv, 0, packetHeader.Length);
                    System.Buffer.BlockCopy(packetInfo, 0, rv, packetHeader.Length, packetInfo.Length);
                    System.Buffer.BlockCopy(ins, 0, rv, packetHeader.Length + packetInfo.Length, ins.Length);

                }
                catch (Exception ex)
                {
                    Logger.LogError("_File : LsConnection17" + "\n_Message : " + ex.Message + "\n_Source : " + ex.Source + "\n_TargetSite : " + ex.TargetSite + "\n", LogType.Error, ex);
                    
                }
                readingPacketInfoList.Add(rpi);
                sendMessage(rv);
            }
            else if (dt == DataType.DWORD)
            {
                if (address < 0 || address > maxAddress / 2)
                {
                    /*sendPacketMutex.ReleaseMutex();*/
                    return false;
                }
                byte[] ins = makeInstruction(DataType.DWORD, 0, address, 0);
                byte[] packetInfo = new byte[6];
                byte[] intByte = BitConverter.GetBytes(_order);
                packetInfo[0] = intByte[0];
                packetInfo[1] = intByte[1];
                intByte = null;
                intByte = BitConverter.GetBytes((ushort)ins.Length);
                packetInfo[2] = intByte[0];
                packetInfo[3] = intByte[1];
                int checkSum = calculateCheckSum(packetInfo);
                intByte = null;
                intByte = BitConverter.GetBytes(checkSum);
                packetInfo[4] = intByte[0];
                packetInfo[5] = intByte[1];

                byte[] rv = new byte[packetHeader.Length + packetInfo.Length + ins.Length];
                try
                {
                    System.Buffer.BlockCopy(packetHeader, 0, rv, 0, packetHeader.Length);
                    System.Buffer.BlockCopy(packetInfo, 0, rv, packetHeader.Length, packetInfo.Length);
                    System.Buffer.BlockCopy(ins, 0, rv, packetHeader.Length + packetInfo.Length, ins.Length);

                }
                catch (Exception ex)
                {
                    Logger.LogError("_File : LsConnection16" + "\n_Message : " + ex.Message + "\n_Source : " + ex.Source + "\n_TargetSite : " + ex.TargetSite + "\n", LogType.Error, ex);
                    
                }
                readingPacketInfoList.Add(rpi);
                sendMessage(rv);
            }
            //sendPacketMutex.ReleaseMutex();
            return true;
        }

        private bool _readFromPlcContinoues(int address, int address_end, ref List<ushort> PacketsId, ushort _order)
        {
            /*sendPacketMutex.WaitOne();*/
            if (address < 0 || address > maxAddress * 2)
            {
                /*sendPacketMutex.ReleaseMutex();*/
                return false;
            }
            if (address > address_end)
            {
                /*sendPacketMutex.ReleaseMutex();*/
                return false;
            }

            byte[] ins = makeInstruction(DataType.CONTINUOUS, Math.Abs(address_end - address), address, 0);
            byte[] packetInfo = new byte[6];
            byte[] intByte = BitConverter.GetBytes(_order);
            packetInfo[0] = intByte[0];
            packetInfo[1] = intByte[1];
            intByte = null;
            intByte = BitConverter.GetBytes((ushort)ins.Length);
            packetInfo[2] = intByte[0];
            packetInfo[3] = intByte[1];
            int checkSum = calculateCheckSum(packetInfo);
            intByte = null;
            intByte = BitConverter.GetBytes(checkSum);
            packetInfo[4] = intByte[0];
            packetInfo[5] = intByte[1];

            byte[] rv = new byte[packetHeader.Length + packetInfo.Length + ins.Length];
            try
            {
                System.Buffer.BlockCopy(packetHeader, 0, rv, 0, packetHeader.Length);
                System.Buffer.BlockCopy(packetInfo, 0, rv, packetHeader.Length, packetInfo.Length);
                System.Buffer.BlockCopy(ins, 0, rv, packetHeader.Length + packetInfo.Length, ins.Length);

            }
            catch (Exception ex)
            {
                Logger.LogError("_File : LsConnection15" + "\n_Message : " + ex.Message + "\n_Source : " + ex.Source + "\n_TargetSite : " + ex.TargetSite + "\n", LogType.Error, ex);
                

            }
            readingPackeCountinusOrder.Add(_order);
            sendMessage(rv);

            /*sendPacketMutex.ReleaseMutex(); */
            return true;
        }

        private bool _writeToPlc(DataType dt, int value, int address, ref writingPacketInfo wpi, ushort _order)
        {
            //Logger.LogError("_writeToPlc line 825", LogType.Info, null);
            /*sendPacketMutex.WaitOne();*/
            if (dt == DataType.CONTINUOUS)
            {
                /*sendPacketMutex.ReleaseMutex();*/
                return false;
            }
            if (dt == DataType.BIT)
            {
                if (value < 0 || value > 1 || address < 0 || address > 16 * maxAddress)
                {
                    /* sendPacketMutex.ReleaseMutex();*/
                    return false;
                }
                byte[] ins = makeInstruction(DataType.BIT, value, address, 1);
                byte[] packetInfo = new byte[6];
                byte[] intByte = BitConverter.GetBytes(_order);
                packetInfo[0] = intByte[0];
                packetInfo[1] = intByte[1];
                intByte = null;
                intByte = BitConverter.GetBytes((ushort)ins.Length);
                packetInfo[2] = intByte[0];
                packetInfo[3] = intByte[1];
                int checkSum = calculateCheckSum(packetInfo);
                intByte = null;
                intByte = BitConverter.GetBytes(checkSum);
                packetInfo[4] = intByte[0];
                packetInfo[5] = intByte[1];

                byte[] rv = new byte[packetHeader.Length + packetInfo.Length + ins.Length];
                try
                {
                    System.Buffer.BlockCopy(packetHeader, 0, rv, 0, packetHeader.Length);
                    System.Buffer.BlockCopy(packetInfo, 0, rv, packetHeader.Length, packetInfo.Length);
                    System.Buffer.BlockCopy(ins, 0, rv, packetHeader.Length + packetInfo.Length, ins.Length);

                }
                catch (Exception ex)
                {
                    Logger.LogError("_File : LsConnection14" + "\n_Message : " + ex.Message + "\n_Source : " + ex.Source + "\n_TargetSite : " + ex.TargetSite + "\n", LogType.Error, ex);
                    
                }
                //packet info for sending packet
                writingPacketInfoList.Add(wpi);
                sendMessage(rv);
            }
            else if (dt == DataType.BYTE)
            {
                if (value < 0 || value > 0xFF || address < 0 || address > 2 * maxAddress)
                {
                    /*sendPacketMutex.ReleaseMutex(); */
                    return false;
                }
                byte[] ins = makeInstruction(DataType.BYTE, value, address, 1);
                byte[] packetInfo = new byte[6];
                byte[] intByte = BitConverter.GetBytes(_order);
                packetInfo[0] = intByte[0];
                packetInfo[1] = intByte[1];
                intByte = null;
                intByte = BitConverter.GetBytes((ushort)ins.Length);
                packetInfo[2] = intByte[0];
                packetInfo[3] = intByte[1];
                int checkSum = calculateCheckSum(packetInfo);
                intByte = null;
                intByte = BitConverter.GetBytes(checkSum);
                packetInfo[4] = intByte[0];
                packetInfo[5] = intByte[1];

                byte[] rv = new byte[packetHeader.Length + packetInfo.Length + ins.Length];
                try
                {
                    System.Buffer.BlockCopy(packetHeader, 0, rv, 0, packetHeader.Length);
                    System.Buffer.BlockCopy(packetInfo, 0, rv, packetHeader.Length, packetInfo.Length);
                    System.Buffer.BlockCopy(ins, 0, rv, packetHeader.Length + packetInfo.Length, ins.Length);

                }
                catch (Exception ex)
                {
                    Logger.LogError("_File : LsConnection13" + "\n_Message : " + ex.Message + "\n_Source : " + ex.Source + "\n_TargetSite : " + ex.TargetSite + "\n", LogType.Error, ex);
                    
                }
                //packet info for sending packet
                writingPacketInfoList.Add(wpi);
                sendMessage(rv);
            }
            else if (dt == DataType.WORD)
            {
                if (value < 0 || value > 0xFFFF || address < 0 || address > maxAddress)
                {
                    /*sendPacketMutex.ReleaseMutex();*/
                    return false;
                }
                byte[] ins = makeInstruction(DataType.WORD, value, address, 1);
                byte[] packetInfo = new byte[6];
                byte[] intByte = BitConverter.GetBytes(_order);
                packetInfo[0] = intByte[0];
                packetInfo[1] = intByte[1];
                intByte = null;
                intByte = BitConverter.GetBytes((ushort)ins.Length);
                packetInfo[2] = intByte[0];
                packetInfo[3] = intByte[1];
                int checkSum = calculateCheckSum(packetInfo);
                intByte = null;
                intByte = BitConverter.GetBytes(checkSum);
                packetInfo[4] = intByte[0];
                packetInfo[5] = intByte[1];

                byte[] rv = new byte[packetHeader.Length + packetInfo.Length + ins.Length];
                try
                {
                    System.Buffer.BlockCopy(packetHeader, 0, rv, 0, packetHeader.Length);
                    System.Buffer.BlockCopy(packetInfo, 0, rv, packetHeader.Length, packetInfo.Length);
                    System.Buffer.BlockCopy(ins, 0, rv, packetHeader.Length + packetInfo.Length, ins.Length);

                }
                catch (Exception ex)
                {
                    Logger.LogError("_File : LsConnection12" + "\n_Message : " + ex.Message + "\n_Source : " + ex.Source + "\n_TargetSite : " + ex.TargetSite + "\n", LogType.Error, ex);
                    
                }
                //packet info for sending packet
                writingPacketInfoList.Add(wpi);
                sendMessage(rv);
            }
            else if (dt == DataType.DWORD)
            {
                if (address < 0 || address > maxAddress / 2)
                {
                    /*sendPacketMutex.ReleaseMutex(); */
                    return false;
                }
                byte[] ins = makeInstruction(DataType.DWORD, value, address, 1);
                byte[] packetInfo = new byte[6];
                byte[] intByte = BitConverter.GetBytes(_order);
                packetInfo[0] = intByte[0];
                packetInfo[1] = intByte[1];
                intByte = null;
                intByte = BitConverter.GetBytes((ushort)ins.Length);
                packetInfo[2] = intByte[0];
                packetInfo[3] = intByte[1];
                int checkSum = calculateCheckSum(packetInfo);
                intByte = null;
                intByte = BitConverter.GetBytes(checkSum);
                packetInfo[4] = intByte[0];
                packetInfo[5] = intByte[1];

                byte[] rv = new byte[packetHeader.Length + packetInfo.Length + ins.Length];
                try
                {
                    Buffer.BlockCopy(packetHeader, 0, rv, 0, packetHeader.Length);
                    System.Buffer.BlockCopy(packetInfo, 0, rv, packetHeader.Length, packetInfo.Length);
                    System.Buffer.BlockCopy(ins, 0, rv, packetHeader.Length + packetInfo.Length, ins.Length);
                }
                catch (Exception ex)
                {
                    Logger.LogError("_File : LsConnection11" + "\n_Message : " + ex.Message + "\n_Source : " + ex.Source + "\n_TargetSite : " + ex.TargetSite + "\n", LogType.Error, ex);
                    
                }
                //packet info for sending packet
                writingPacketInfoList.Add(wpi);
                sendMessage(rv);
            }
            /*sendPacketMutex.ReleaseMutex(); */
            return true;
        }
        
        private bool _writeContinouesToPlc(byte[] value, int address, ref List<ushort> PackestId, ushort _order)
        {
            //Logger.LogError("_writeContinouesToPlc line 958", LogType.Info, null);
            if (value.Length > 1400 || value.Length == 0 || address < 0 || address > 16 * maxAddress)
            {
                /* sendPacketMutex.ReleaseMutex(); */
                return false;
            }
            byte[] ins = makeInstruction(DataType.CONTINUOUS, value.Length, address, 1);
            byte[] ins2 = new byte[value.Length + ins.Length];
            try
            {
                System.Buffer.BlockCopy(ins, 0, ins2, 0, ins.Length);
                System.Buffer.BlockCopy(value, 0, ins2, ins.Length, value.Length);
            }
            catch (Exception ex)
            {
                Logger.LogError("_File : LsConnection10" + "\n_Message : " + ex.Message + "\n_Source : " + ex.Source + "\n_TargetSite : " + ex.TargetSite + "\n", LogType.Error, ex);
                
            }
            byte[] packetInfo = new byte[6];
            byte[] intByte = BitConverter.GetBytes(_order);
            packetInfo[0] = intByte[0];
            packetInfo[1] = intByte[1];
            intByte = null;
            intByte = BitConverter.GetBytes((ushort)ins2.Length);
            packetInfo[2] = intByte[0];
            packetInfo[3] = intByte[1];
            int checkSum = calculateCheckSum(packetInfo);
            intByte = null;
            intByte = BitConverter.GetBytes(checkSum);
            packetInfo[4] = intByte[0];
            packetInfo[5] = intByte[1];

            byte[] rv = new byte[packetHeader.Length + packetInfo.Length + ins2.Length];
            try
            {
                System.Buffer.BlockCopy(packetHeader, 0, rv, 0, packetHeader.Length);
                System.Buffer.BlockCopy(packetInfo, 0, rv, packetHeader.Length, packetInfo.Length);
                System.Buffer.BlockCopy(ins2, 0, rv, packetHeader.Length + packetInfo.Length, ins2.Length);

            }
            catch (Exception ex)
            {
                Logger.LogError("_File : LsConnection9" + "\n_Message : " + ex.Message + "\n_Source : " + ex.Source + "\n_TargetSite : " + ex.TargetSite + "\n", LogType.Error, ex);
                
            }
            //packet info for sending packet
            writingPackeCountinusOrder.Add(_order);
            sendMessage(rv);

            return true;
        }

        private int calculateCheckSum(byte[] header)
        {
            int sum = 0;
            for (int i = 0; i < 5; i++)
                sum += header[i];
            sum += 760;
            return sum;
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
            ins_Head[8] = (byte) add.Length;
            ins_Head[9] = 0x00;
            byte[] addre = Encoding.ASCII.GetBytes(add);
            byte[] value_Byte;
            if (dt == DataType.CONTINUOUS)
            {
                value_Byte = new byte[2];
                ushort val = (ushort) value;
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
                        value_Byte[2] = (byte) value;
                        break;

                    case DataType.BYTE:
                        value_Byte = new byte[3];
                        value_Byte[0] = 0x01;
                        value_Byte[1] = 0x00;
                        value_Byte[2] = (byte) value;
                        break;

                    case DataType.WORD:
                        value_Byte = new byte[4];
                        value_Byte[0] = 0x02;
                        value_Byte[1] = 0x00;
                        ushort vavl = (ushort) value;
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
                try
                {
                    System.Buffer.BlockCopy(ins_Head, 0, rv1, 0, ins_Head.Length);
                    System.Buffer.BlockCopy(addre, 0, rv1, ins_Head.Length, addre.Length);
                }
                catch (Exception ex)
                {
                    Logger.LogError("_File : LsConnection7" + "\n_Message : " + ex.Message + "\n_Source : " + ex.Source + "\n_TargetSite : " + ex.TargetSite + "\n", LogType.Error, ex);
                    
                }
                return rv1;
            }

            byte[] rv = new byte[ins_Head.Length + addre.Length + value_Byte.Length];
            try
            {

                System.Buffer.BlockCopy(ins_Head, 0, rv, 0, ins_Head.Length);
                System.Buffer.BlockCopy(addre, 0, rv, ins_Head.Length, addre.Length);
                System.Buffer.BlockCopy(value_Byte, 0, rv, ins_Head.Length + addre.Length, value_Byte.Length);
            }
            catch (Exception ex)
            {
                Logger.LogError("_File : LsConnection8" + "\n_Message : " + ex.Message + "\n_Source : " + ex.Source + "\n_TargetSite : " + ex.TargetSite + "\n", LogType.Error, ex);
                
            }
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

    public class PLCRequest
    {
        public enum _RequestType
        {
            READ_CONTINUOUS,
            WRITE_CONTINUOUS,
            READ,
            WRITE_ONE_PACKET,
            WRITE_LIST_PACKET
        };

        public _RequestType requestType { set; get; }
        public DataType dataType { set; get; }
        public static long requestID = 0;
        public byte[] arrayValue { set; get; }
        public int value { set; get; }
        public int address { set; get; }
        public int address_end { set; get; }
        public List<ushort> PackestId;
        public List<writingPacketInfo> wpiList;
        public writingPacketInfo wpi;
        public readingPacketInfo rpi;
        public ushort orderID = 0;

        public PLCRequest(_RequestType rt)
        {
            requestType = rt;
            requestID++;
        }

        public PLCRequest(byte[] _arrayValue, int _address, ref List<ushort> _PackestId)
        {
            requestType = _RequestType.WRITE_CONTINUOUS;
            requestID++;
            arrayValue = _arrayValue;
            address = _address;
            PackestId = _PackestId;
            orderID = LS_Connection.order;
        }

        public PLCRequest(int _address, int _address_end, ref List<ushort> _PacketsId)
        {
            requestType = _RequestType.READ_CONTINUOUS;
            requestID++;
            address_end = _address_end;
            address = _address;
            PackestId = _PacketsId;
            orderID = LS_Connection.order;
        }

        public PLCRequest(DataType _dt, int _address, ref readingPacketInfo _rpi)
        {
            requestType = _RequestType.READ;
            requestID++;
            dataType = _dt;
            address = _address;
            rpi = _rpi;
            orderID = LS_Connection.order;
        }

        public PLCRequest(DataType _dt, int _value, int _address, ref writingPacketInfo _wpi)
        {
            requestType = _RequestType.WRITE_ONE_PACKET;
            requestID++;
            value = _value;
            address = _address;
            dataType = _dt;
            wpi = _wpi;
            orderID = LS_Connection.order;
        }

        public PLCRequest(DataType _dt, int _value, int _address, ref List<writingPacketInfo> _wpiList)
        {
            requestType = _RequestType.WRITE_ONE_PACKET;
            requestID++;
            value = _value;
            address = _address;
            dataType = _dt;
            wpiList = _wpiList;
            orderID = LS_Connection.order;
        }
    }

    public class Packet<T>
    {
        public Packet()
        {
            if (typeof (T) == typeof (bool))
            {
                dataType = DataType.BIT;
            }
            else if (typeof (T) == typeof (byte))
            {
                dataType = DataType.BYTE;
            }
            else if (typeof (T) == typeof (ushort))
            {
                dataType = DataType.WORD;
            }
            else if (typeof (T) == typeof (uint))
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
            if (typeof (T) == typeof (bool))
            {
                dataType = DataType.BIT;
            }
            else if (typeof (T) == typeof (byte))
            {
                dataType = DataType.BYTE;
            }
            else if (typeof (T) == typeof (ushort))
            {
                dataType = DataType.WORD;
            }
            else if (typeof (T) == typeof (uint))
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
            if (typeof (T) == typeof (bool))
            {
                dataType = DataType.BIT;
            }
            else if (typeof (T) == typeof (byte))
            {
                dataType = DataType.BYTE;
            }
            else if (typeof (T) == typeof (ushort))
            {
                dataType = DataType.WORD;
            }
            else if (typeof (T) == typeof (uint))
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
