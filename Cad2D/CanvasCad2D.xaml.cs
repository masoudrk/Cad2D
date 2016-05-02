using Cad2D.Pages;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Hikvision;
using MahApps.Metro.Controls;
using Brushes = System.Windows.Media.Brushes;
using Image = System.Windows.Controls.Image;
using Point = System.Windows.Point;
using System.Windows.Data;
using System.Windows.Media;
using System.Text.RegularExpressions;

namespace Cad2D
{
    public partial class CanvasCad2D : UserControl
    {

        [DllImport("PowrProf.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern bool SetSuspendState(bool hiberate, bool forceCritical, bool disableWakeEvent);

        public enum CadTool
        {
            HAND, CREATE_PATH, ERASER, NON
        }
        public enum State
        {
            NON, GET_START, DRAWING, END, DRAGGING_VERTEX
        }

        public class ConnectedLine
        {
            public Line line { set; get; }
            public Circle circle { set; get; }
        }

        public class Path : LinkedList<ConnectedLine>
        {
            public bool loop;
        }

        public bool mouseEnteredInCircle;
        public bool canLoopPath { set; get; }

        public Circle onTopCircle;
        public CadTool cadTool { set; get; }
        public State state { set; get; }
        public Path connectedsList { set; get; }
        PlcInformation plcInformation;
        public bool imageLoaded;
        public double defaultBitmapHeightPixels, defaultBitmapWidthPixels;
        public double scale;

        public Stack<object> pagesStack;

        private BitmapImage alarmOffBitmap, alarmOnBitmap;
        public static int alarm;
        private int sensitiveAlarms;

        //@milad
        double widthOffsetLength;
        double heightOffsetLength;
        /// <summary>
        /// should add to settings
        int StoneScanVerticalSlice = 140;
        public static PlcUtilitisAndOptions plcUtilitisAndOptions;
        int stoneScanHorizontalSlice = 80;
        int StoneEdgeVerticalSlice = 400;
        int stoneEdgeHorizontalSlice = 400;
        /// </summary>
        Point startPoint = new Point(0, 0);
        Point endPoint = new Point(704, 576);
        public List<LineGeometry> lineGeometryList;
        private List<Circle> guids;
        private List<Line> innerLines;
        Thread plcInfoReaderTimer;
        Circle head = cloneCircle();
        ////////////////
        //lsconnection
        
        public static LS_Connection lsConnection;
        int stoneScanPacketCounter = 0;
        int verticalBoundryCounter = 0;
        int horizonalBoundryCounter = 0;
        private int stoneScanPacketCount;
        private int verticalBoundryCount;
        private int horizonalBoundryCount;
        Page_Tools pageToolsObject;
        Page_Settings pageSettingObject;
        public static HscXHelper hscXHelper;
        public static HscYHelper hscYHelper;
        public static Page_Hsc_X pageHscX;
        public static Page_Hsc_Y pageHscY;
        IPAddress ip;
        int portNumber;
        int maxAddress = 8000;
        int scanAriaSegment = 1000; // startin memory for sending array 
        int verticalBoundrySegment = 2000; // startin memory for sending array 
        int horizonalBoundrySegment = 3000; // startin memory for sending array 

        /// <summary>
        /// ////////shoud add to settingd
        double minVerticalSlice = 10;
        double maxVerticalSlice = 4000;
        double minHorizontalSlice = 5;
        double maxHorizontalSlice = 8000;
        /// </summary>
        /// 

        int[] stoneScan;
        ushort[] stoneHorizontalEdge;
        ushort[] stoneVerticalEdge;
        List<writingPacketInfo> writingPackets;
        string cameraIp;
        private Thread alarmThread;
        private System.Timers.Timer cameraCheckerTimer;
        private System.Timers.Timer clockTimer;
        private int headPosition;

        private Window_DisplaySendData _windowDisplaySendData;

        private Packet<ushort> diskDiameter;
        public CanvasCad2D()
        {
            InitializeComponent();
            pagesStack = new Stack<object>();
            plcUtilitisAndOptions = new PlcUtilitisAndOptions();
            this.Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;

            System.Net.ServicePointManager.Expect100Continue = false;

            head.mouseActionsEnable = false;
            head.radius = 50;
            head.X = 0;
            head.Y = 0;
            head.defaultColor = Brushes.DarkCyan;
            head.Name = "head";
            headPosition = mainCanvas.Children.Add(head);
            state = State.GET_START;
            cadTool = CadTool.NON;

            connectedsList = new Path();

            alarmOnBitmap = new BitmapImage(
                new Uri("pack://application:,,,/Cad2D;component/Resources/alarm_on.png",
                    UriKind.Absolute));
            alarmOffBitmap = new BitmapImage(
                new Uri("pack://application:,,,/Cad2D;component/Resources/alarm_off.png",
                    UriKind.Absolute));

            plcInformation = new PlcInformation();
            hscXHelper =new HscXHelper();
            hscYHelper = new HscYHelper();
            PrimarySettings ps = checkPrimarySettings();
            getSensitiveAlarms();

            alarmThread = new Thread(checkAlarms) { IsBackground = true };
            alarmThread.Start();

            clockTimer = new System.Timers.Timer()
            {
                Interval = 1000
            };
            clockTimer.Elapsed += updateClock;
            clockTimer.Start();

            cameraCheckerTimer = new System.Timers.Timer()
            {
                Interval = 5000
            };
            cameraCheckerTimer.Elapsed += checkCamera;
            cameraCheckerTimer.Start();

            //@milad
            lineGeometryList = new List<LineGeometry>();
            guids = new List<Circle>();
            innerLines = new List<Line>();
            lsConnection = new LS_Connection(maxAddress);

            lsConnection.OnDisconnceted += Ls_connection_OnDisconnceted;
            lsConnection.OnConnect += Ls_connection_OnConnect;
            lsConnection.OnWritedSuccessfully += Ls_connection_OnWritedSuccessfully;
            lsConnection.OnReadedSuccessfully += Ls_connection_OnReadedSuccessfully;
            lsConnection.OnReadedContinuous += Ls_connection_OnReadedContinuous;
            lsConnection.Connected = false;
            stoneScanPacketCount = 0;
            verticalBoundryCount = 0;
            horizonalBoundryCount = 0;
            diskDiameter = new Packet<ushort>(232);
            lsConnection.connect(ip, portNumber);
            writingPackets = new List<writingPacketInfo>();

            plcInfoReaderTimer = new Thread(PlcInfoReaderTimer_Elapsed);
            plcInfoReaderTimer.Start();

            if(ps.captureModeWhenStart)
                CaptureMode();
            //this.ContextMenuClosing +=OnContextMenuClosing;
            initDataGrid(dataGrid);

        }

        //private void OnContextMenuClosing(object sender, ContextMenuEventArgs contextMenuEventArgs)
        //{
        //    alarmThread.Abort();
        //    cameraCheckerTimer.Stop();
        //    cameraCheckerTimer.Close();
        //    cameraCheckerTimer.Dispose();
        //    plcInfoReaderTimer.Abort();
        //    lsConnection.Disconnect();
        //    clockTimer.Stop();
        //    clockTimer.Close();
        //    clockTimer.Dispose();
        //}

        public static void initDataGrid(DataGrid d)
        {
            DataGridTextColumn c1 = new DataGridTextColumn();
            c1.Header = "Index";
            c1.Binding = new Binding("val1");
            c1.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
            d.Columns.Add(c1);
            DataGridTextColumn c2 = new DataGridTextColumn();
            c2.Header = "Min";
            c2.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
            c2.Binding = new Binding("val2");
            d.Columns.Add(c2);
            DataGridTextColumn c3 = new DataGridTextColumn();
            c3.Header = "Max";
            c3.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
            c3.Binding = new Binding("val3");
            d.Columns.Add(c3);
            DataGridTextColumn c4 = new DataGridTextColumn();
            c4.Header = "Min";
            c4.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
            c4.Binding = new Binding("val4");
            d.Columns.Add(c4);
            DataGridTextColumn c5 = new DataGridTextColumn();
            c5.Header = "Max";
            c5.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
            c5.Binding = new Binding("val5");
            d.Columns.Add(c5);
        }
        
        class GridItem
        {
            public double val1 { set; get; }
            public double val2 { set; get; }
            public double val3 { set; get; }
            public double val4 { set; get; }
            public double val5 { set; get; }
        }

        private void PlcInfoReaderTimer_Elapsed()
        {
            if (lsConnection.Connected)
            {
                plcInformation.getAllValues();
            }
            else
            {
                Thread.Sleep(2000);
                if (!lsConnection.Connected)
                {
                    bool ping = lsConnection.PingHost();
                    if (ping)
                    {
                        lsConnection.connect(ip, portNumber);
                    }
                }
            }
            Thread.Sleep(1000);
            PlcInfoReaderTimer_Elapsed();
        }

        private void Dispatcher_ShutdownStarted(object sender, EventArgs e)
        {
            alarmThread.Abort();
            cameraCheckerTimer.Stop();
            cameraCheckerTimer.Close();
            cameraCheckerTimer.Dispose();
            plcInfoReaderTimer.Abort();
            lsConnection.Disconnect();
            clockTimer.Stop();
            clockTimer.Close();
            clockTimer.Dispose();
        }

        public void ShutDown()
        {
            alarmThread.Abort();
            cameraCheckerTimer.Stop();
            cameraCheckerTimer.Close();
            cameraCheckerTimer.Dispose();
            plcInfoReaderTimer.Abort();
            lsConnection.Disconnect();
            clockTimer.Stop();
            clockTimer.Close();
            clockTimer.Dispose();
        }

        #region ls events

        private void checkCamera(object sender, EventArgs e)
        {
            bool c = HikvisionController.cameraConnection(IPAddress.Parse(cameraIp));
            if (!c)
            {
                alarm = alarm | 512;
            }
            else
            {
                alarm = alarm & 3583;
            }
        }


        public void setSlidersValues()
        {
            slider_x.Value = plcUtilitisAndOptions.Velocity.velocityX;
            slider_y.Value = plcUtilitisAndOptions.Velocity.velocityY;
        }
        private void OnGUIActions(Action action)
        {
            Dispatcher.Invoke(action);
        }

        private bool[] createBoolArray(bool[] array)
        {
            bool[] newArray = new bool[16];
            for (int i = 0; i < array.Length; i++)
            {
                newArray[i] = array[array.Length - i - 1];
            }
            return newArray;
        }

        private void shoutDownThePanelPC(ushort shut)
        {
            switch (shut)
            {
                case 1: //shutDown
                    System.Diagnostics.Process.Start("shutdown.exe", "-s -t 0");
                    break;
                case 2: //reset
                    System.Diagnostics.Process.Start("shutdown.exe", "-r -t 0");
                    break;
                case 3: // hibernate : set the 951 mw to 0 the go to case 4
                     
                    if(lsConnection.Connected)
                        lsConnection.writeToPlc(DataType.WORD, 0, 951, ref shutDownPacketId);
                     
                    break;
                case 4: // end hibernate
                    shutDownPacketId = null;
                    SetSuspendState(true, true, true);
                    break;
                default:
                    break;
            }
        }

        private void setDiskDiameter(ushort diameter)
        {
            diameter = (ushort)(((endPoint.X - startPoint.X)/3500)*diameter);
            Circle c = (Circle)mainCanvas.Children[headPosition];
            mainCanvas.Children.Remove(c);
            c.radius = diameter / 2;
            mainCanvas.Children.Insert(headPosition , c);
        }

        private void writeToPlcFinished()
        {
            setEnable(btn_sendToPlc_back, true);
            progressDialog.Result.Close();
        }
        private void Ls_connection_OnConnect(object sender, EventArgs e)
        {
            alarm = alarm & 1048319;

            if (lsConnection.Connected)
            {
                lsConnection.readFromPlc(diskDiameter.dataType, diskDiameter.valueAddress, ref diskDiameter.readingPacket);
            }
            plcInformation.getFirstValues();
        }
        private void Ls_connection_OnDisconnceted(object sender, EventArgs e)
        {
            alarm = alarm | 256;
        }
        #endregion
        private void updateClock(object sender, ElapsedEventArgs e)
        {
            Dispatcher.Invoke(
                new Action(() => label_clock.Content = DateTime.Now.ToString("HH:mm:ss")));
        }

        private void getSensitiveAlarms()
        {
            Settings s = Extentions.FromXml();
            if (s.alarmBits != null)
            {
                string sb = "";

                for (int i = 0; i < s.alarmBits.Length; i++)
                {
                    sb = ((s.alarmBits[i]) ? "1" : "0") + sb;
                }
                sensitiveAlarms = Convert.ToInt32(sb.ToString(), 2);
            }
            else
                sensitiveAlarms = 64;
        }

        public void checkAlarms()
        {
            while (true)
            {
                int mask = alarm & sensitiveAlarms;
                if (mask != 0)
                {
                    Dispatcher.Invoke(new Action(() => image_alarm.Source = alarmOnBitmap));
                    Thread.Sleep(400);
                    Dispatcher.Invoke(new Action(() => image_alarm.Source = alarmOffBitmap));
                    Thread.Sleep(400);
                }
                else
                {
                    Dispatcher.Invoke(new Action(() => image_alarm.Source = alarmOffBitmap));
                    Thread.Sleep(1000);
                }
            }
        }

        #region Draw Actions

        public void endDrawVertex()
        {
            state = State.NON;
            connectedsList.First.Value.circle.connectedLine1 = connectedsList.Last.Value.line;

            connectedsList.Last.Value.line.X2 = connectedsList.First.Value.circle.X;
            connectedsList.Last.Value.line.Y2 = connectedsList.First.Value.circle.Y;

            canLoopPath = false;
            connectedsList.loop = true;

            setEnable(btn_clearPath, true);
            setEnable(btn_CreatePathTool, false);
            setEnable(btn_sendToPlc_back, true);
            calculateAllPoints();
        }

        public void addConnectedLine(Line l, Circle c)
        {
            c.cc2d = this;

            mainCanvas.Children.Insert(2, l);
            mainCanvas.Children.Add(c);
            ConnectedLine cl = new ConnectedLine() { line = l, circle = c };
            c.cl = cl;
            connectedsList.AddLast(cl);
        }
        #endregion

        #region Clone Drawings


        public static Line cloneLine()
        {
            Line l = new Line();
            l.Fill = Brushes.Gray;
            l.Stroke = Brushes.Gray;
            l.StrokeThickness = 1;
            l.HorizontalAlignment = HorizontalAlignment.Center;
            l.VerticalAlignment = VerticalAlignment.Center;
            return l;
        }
        public static Line InnerLine()
        {
            Line l = new Line();
            l.Fill = Brushes.OrangeRed;
            l.Stroke = Brushes.OrangeRed;
            l.StrokeThickness = 1;
            l.HorizontalAlignment = HorizontalAlignment.Center;
            l.VerticalAlignment = VerticalAlignment.Center;
            return l;
        }

        public static Circle cloneCircle()
        {
            Circle e = new Circle();
            e.HorizontalAlignment = HorizontalAlignment.Center;
            e.VerticalAlignment = VerticalAlignment.Center;

            return e;
        }

        #endregion

        #region Cad Tool Toggle Buttons

        private void toggle_PenTool_Checked(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Arrow;
            cadTool = CadTool.CREATE_PATH;
            state = State.GET_START;
            setEnable((Control)sender, false);
            setEnable(btn_clearPath, true);
        }
    
        private void toggle_Tool_Unchecked(object sender, RoutedEventArgs e)
        {
            cadTool = CadTool.NON;
        }

        #endregion

        #region Controls Events

        private void mainCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            #region Create Path Actions
            if (cadTool == CadTool.CREATE_PATH)
            {
                if (e.RightButton == MouseButtonState.Pressed && state == State.DRAWING)
                {
                    if (connectedsList.Count > 0)
                    {
                        mainCanvas.Children.Remove(connectedsList.Last.Value.line);
                        mainCanvas.Children.Remove(connectedsList.Last.Value.circle);
                        connectedsList.RemoveLast();

                        if (connectedsList.Count == 0)
                            state = State.GET_START;
                    }
                    return;
                }
                
                if (connectedsList.Count > 2)
                {
                    setEnable(btn_sendToPlc_back,true);
                }

                if (state == State.NON)
                    return;

                if (canLoopPath)
                {
                    endDrawVertex();
                    return;
                }

                Point clickedPoint = e.GetPosition(mainCanvas);

                if (state == State.GET_START)
                {
                    Line newLine = cloneLine();
                    newLine.X1 = newLine.X2 = clickedPoint.X;
                    newLine.Y1 = newLine.Y2 = clickedPoint.Y;

                    Circle c = cloneCircle();
                    c.X = clickedPoint.X;
                    c.Y = clickedPoint.Y;
                    c.connectedLine2 = newLine;
                    c.isFirstPoint = true;

                    addConnectedLine(newLine, c);

                    state = State.DRAWING;
                }
                else if (state == State.DRAWING)
                {
                    Line lastLine = connectedsList.Last.Value.line;
                    lastLine.X2 = clickedPoint.X;
                    lastLine.Y2 = clickedPoint.Y;

                    Line newLine = cloneLine();
                    newLine.X1= newLine.X2 = clickedPoint.X;
                    newLine.Y1= newLine.Y2 = clickedPoint.Y;

                    Circle c = cloneCircle();
                    c.X = clickedPoint.X;
                    c.Y = clickedPoint.Y;
                    c.connectedLine1 = lastLine;
                    c.connectedLine2 = newLine;

                    addConnectedLine(newLine, c);
                }
            }
            #endregion
        }
        
        /*
        private void mainCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (state == State.DRAWING)
            {
                Point mousePos = e.GetPosition(mainCanvas);
                Line lastLine = connectedsList.Last.Value.line;
                lastLine.X2 = mousePos.X;
                lastLine.Y2 = mousePos.Y;
            }
        }*/

        private Bitmap bSrc;
        private writingPacketInfo shutDownPacketId = null;

        private void button_switchToCamera_Click(object sender, RoutedEventArgs e)
        {
            try
            {
            
                Type t = contentControl.Content.GetType();

                if (t == typeof(CameraPage))
                {
                    contentControl.Transition = TransitionType.RightReplace;
                    CameraPage cp = (CameraPage)contentControl.Content;
                    cp.hv.stopCapturing();
                    Image i = cp.image;

                    if (pagesStack.Count() < 2)
                    {
                        BitmapSource bs = (BitmapSource)i.Source;
                        if (bs != null)
                        {
                            PrimarySettings s = Extentions.FromXmlPrimary();
                            Analyzer a = new Analyzer(s.FELimit,s.ScaleFESize);
                            bSrc = Utils.BitmapFromSource(bs);
                            Bitmap b1 = a.RemoveFisheye(ref bSrc, s.FocalLinPixels);
                            mainImage.Source = Utils.ConvertBitmapToBitmapSource(b1);
                        }
                        clearPath();
                        EditMode();
                        contentControl.Transition = TransitionType.Normal;
                    }
                    else
                    {

                        Page_SetOffsets pso = new Page_SetOffsets();
                        BitmapSource bs = (BitmapSource)i.Source;
                        if (bs != null)
                        {
                            PrimarySettings s = Extentions.FromXmlPrimary();
                            Analyzer a = new Analyzer(s.FELimit, s.ScaleFESize);
                            bSrc = Utils.BitmapFromSource(bs);
                            Bitmap b1 = a.RemoveFisheye(ref bSrc, s.FocalLinPixels);
                            pso.offsetImage.Source = Utils.ConvertBitmapToBitmapSource(b1);
                        }
                        contentControl.Content = pso;
                    }
                }
                else if (t == typeof(Page_SetOffsets))
                {
                    Page_SetOffsets pso = (Page_SetOffsets)contentControl.Content;
                    Point[] pns = pso.points;
                    if (pso._pointCount != 2)
                    {
                        ((MainWindow)Application.Current.MainWindow).showMsg("خطا", "حتما بایستی 2 نقطه بالا چپ و پایین راست میز تعیین شود.");
                        return;
                    }
                    Page_Settings ps = (Page_Settings)pagesStack.Pop();
                    ps.setOffsets(pns);

                    contentControl.Content = ps;

                    ((Image)button_switchToCamera.Content).Source =
                        new BitmapImage(
                            new Uri("pack://application:,,,/Cad2D;component/Resources/camera.png",
                            UriKind.Absolute));
                    btn_sendToPlc_back.Visibility = Visibility.Visible;
                    border_tools2.Visibility = Visibility.Hidden;
                    border_WaterOption.Visibility = Visibility.Hidden;
                }
                else
                {
                    CaptureMode();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("_Message : " + ex.Message + "\n\n_Source : " + ex.Source + "\n\n_TargetSite : " + ex.TargetSite + "\n\n _ALL : " + ex.ToString(), LogType.Error,ex);
            }
        }
        private void btn_sendToPlc_back_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (contentControl.Content.GetType() == typeof(CameraPage) || sender == null)
                {
                    EditMode();
                }
                else
                {
                    if (!connectedsList.loop)
                        endDrawVertex();
                    _sendDataToPlc();
                    setEnable(btn_sendToPlc_back, false);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("_Message : " + ex.Message + "\n\n_Source : " + ex.Source + "\n\n_TargetSite : " + ex.TargetSite + "\n\n _ALL : " + ex.ToString(), LogType.Error, ex);
            }
        }

        private void btn_clear_path_Click(object sender, RoutedEventArgs e)
        {
            clearPath();
        }

        private void clearPath()
        {
            state = State.NON;
            foreach (ConnectedLine c in connectedsList)
            {
                mainCanvas.Children.Remove(c.circle.connectedLine1);
                mainCanvas.Children.Remove(c.circle.connectedLine2);
                mainCanvas.Children.Remove(c.circle);
            }

            ///@milad
            dataGrid.Items.Clear();
            foreach (Circle c in guids)
                mainCanvas.Children.Remove(c);

            foreach (Line line in innerLines)
                mainCanvas.Children.Remove(line);

            innerLines.Clear();
            guids.Clear();
            ///////////////////

            //state = State.GET_START;
            connectedsList.loop = false;
            connectedsList.Clear();

            setEnable(btn_clearPath, false);
            setEnable(btn_sendToPlc_back, false);
            setEnable(btn_CreatePathTool, true);
        }

        private void button_tools_click(object sender, RoutedEventArgs e)
        {
            if (!canPushPage(typeof(Page_Tools)))
                return;

            
            pageToolsObject = new Page_Tools(this);
            pageToolsObject.backPageHandler += backFromPage;
            contentControl.Content = pageToolsObject;

            //btn_help.Visibility = Visibility.Collapsed;
            button_about.Visibility = Visibility.Collapsed;
            button_setting.Visibility = Visibility.Collapsed;
            button_tools.Visibility = Visibility.Collapsed;
            image_alarm.Visibility = Visibility.Collapsed;
        }
        private void button_settings_click(object sender, RoutedEventArgs e)
        {
            Page_SignIn ps = new Page_SignIn(true);
            ps.adminBackEvent += onCurrectEnterAdminPass;
            pagesStack.Push(contentControl.Content);
            contentControl.Content = ps;

            if (!canPushPage(typeof(Page_Settings)))
                return;
            //btn_help.Visibility = Visibility.Collapsed;
            button_about.Visibility = Visibility.Collapsed;
            button_setting.Visibility = Visibility.Collapsed;
            button_tools.Visibility = Visibility.Collapsed;
            image_alarm.Visibility = Visibility.Collapsed;
        }

        public void onCurrectEnterAdminPass(object sender, EventArgs e)
        {
            if (!sender.Equals("95498"))
            {
                ((MainWindow)Application.Current.MainWindow).showMsg("خطا", "رمز ادمین اشتباه است.");

                button_back_ex_click(null, null);
                return;
            }

            pageSettingObject = new Page_Settings(this);
            pageSettingObject.backPageHandler += backFromPage;
            contentControl.Content = pageSettingObject;
        }

        public void backFromPage(object sender, EventArgs e)
        {
            
            if (sender.Equals("OPTIONS"))
            {
                pageSettingObject.encoderReader.Abort();
                pageSettingObject = null;
                Page_Settings.readingFinished = false;
                checkPrimarySettings();
            }
            else if (sender.Equals("TOOLS"))
            {
                getSensitiveAlarms();
                pageToolsObject = null;
            }
            button_back_ex_click(null, null);
        }

        private PrimarySettings checkPrimarySettings()
        {
            PrimarySettings ps = Extentions.FromXmlPrimary();
            if (ps.showSpeedMonitorInMainPanel)
            {
                panel_speeds.Visibility = Visibility.Visible;
            }
            else
            {
                panel_speeds.Visibility = Visibility.Collapsed;
            }

            plcUtilitisAndOptions.setNewValues();
            plcInformation.setAddressValues();
            minVerticalSlice = ps.EdgeVerticalSliceMin;
            maxVerticalSlice = ps.EdgeVerticalSliceMax;
            minHorizontalSlice = ps.EdgeHorizontalSliceMin;
            maxHorizontalSlice = ps.EdgeHorizontalSliceMax;

            StoneScanVerticalSlice = ps.ScanVerticalSlice;
            stoneScanHorizontalSlice = ps.ScanHorizontalSlice;
            StoneEdgeVerticalSlice = ps.EdgeVerticalSlice;
            stoneEdgeHorizontalSlice = ps.EdgeHorizontalSlice;

            startPoint = new Point(ps.TopLeftOffsetX, ps.TopLeftOffsetY);
            endPoint = new Point(ps.BottomRightOffsetX, ps.BottomRightOffsetY);

            scanAriaSegment = ps.ScanAriaSegment; // startin memory for sending array 
            verticalBoundrySegment = ps.VerticalBoundrySegment; // startin memory for sending array 
            horizonalBoundrySegment = ps.HorizonalBoundrySegment; // startin memory for sending array 

            ip = IPAddress.Parse(ps.PLCIpAdress);
            portNumber = ps.PLCPortNumber;
            cameraIp = ps.CameraIpAdress;
            if(lsConnection != null)
                lsConnection.set(ip, portNumber);

            return ps;
        }

        bool is_inSendingx = false;
        bool is_inSendingy = false;
        writingPacketInfo positionxPacketInfo = null;
        writingPacketInfo positionyPacketInfo = null;

        private void slider_x_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!is_inSendingx)
            {
                is_inSendingx = true;
                Thread t = new Thread(sendPositionxToPlc);
                t.Start();
            }
        }


        private void slider_y_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!is_inSendingy)
            {
                is_inSendingy = true;
                Thread t = new Thread(sendPositionYToPlc);
                t.Start();
            }
        }

        private void slider_x_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (label_x != null)
                label_x.Content = string.Format("{0}x", arg0: slider_x.Value.ToString("F0"));
            if(plcUtilitisAndOptions != null)
                plcUtilitisAndOptions.Velocity.velocityX = (ushort)slider_x.Value;
        }

        private void slider_y_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (label_y != null)
                label_y.Content = string.Format("{0}x", arg0: slider_y.Value.ToString("F0"));
            if (plcUtilitisAndOptions != null)
                plcUtilitisAndOptions.Velocity.velocityY = (ushort)slider_y.Value;
        }

        void sendPositionxToPlc()
        {

             
            if (lsConnection != null && lsConnection.Connected)
                OnGUIActions(() => lsConnection.writeToPlc(DataType.WORD, (int)slider_x.Value, plcUtilitisAndOptions.Velocity.velocityXAddress, ref positionxPacketInfo));
             
        }
        void sendPositionYToPlc()
        {
            
            if (lsConnection != null && lsConnection.Connected)
                OnGUIActions(() => lsConnection.writeToPlc(DataType.WORD, (int)slider_y.Value, plcUtilitisAndOptions.Velocity.velocityYAddress, ref positionyPacketInfo));

        }


        private void btn_help_click(object sender, RoutedEventArgs e)
        {
            if (!canPushPage(typeof(Page_Help)))
                return;

            contentControl.Content = new Page_Help();
        }

        private void button_about_click(object sender, RoutedEventArgs e)
        {
            if (!canPushPage(typeof(Page_About)))
                return;

            contentControl.Content = new Page_About();
        }
        private void button_back_ex_click(object sender, RoutedEventArgs e)
        {
            if (contentControl.Content.GetType() == typeof(Page_Settings))
            {
                if(pageSettingObject != null)
                    pageSettingObject.encoderReader.Abort();
                pageSettingObject = null;
                Page_Settings.readingFinished = false;
                checkPrimarySettings();
            }
            if (contentControl.Content.GetType() == typeof(Page_Tools))
            {
                pageToolsObject = null;
            }
            contentControl.Content = pagesStack.Pop();
            if (contentControl.Content.GetType() == typeof(Page_Settings))
            {
                border_tools2.Visibility = Visibility.Collapsed;
                border_WaterOption.Visibility = Visibility.Collapsed;
            }

            if (pagesStack.Count == 0)
            {
                setupMainPanels(true);
                //btn_help.Visibility = Visibility.Visible;
                button_about.Visibility = Visibility.Visible;
                button_setting.Visibility = Visibility.Visible;
                button_tools.Visibility = Visibility.Visible;
                image_alarm.Visibility = Visibility.Visible;
            }
        }

        private void button_alarm_click(object sender, RoutedEventArgs e)
        {
            if (canPushPage(typeof(Page_Alarms)))
            {
                contentControl.Content = new Page_Alarms();
                //btn_help.Visibility = Visibility.Collapsed;
                button_about.Visibility = Visibility.Collapsed;
                button_setting.Visibility = Visibility.Collapsed;
                image_alarm.Visibility = Visibility.Collapsed;
            }
        }

        private bool canPushPage(Type pageType)
        {
            Type t = contentControl.Content.GetType();

            if (t.Equals(pageType))
                return false;

            if (t.Equals(typeof(Border)))
                pagesStack.Push(contentControl.Content);

            setupMainPanels(false);

            return true;
        }

        public void setupMainPanels(bool en)
        {
            border_WaterOption.Visibility = border_tools1.Visibility = border_tools2.Visibility =
                (en) ? Visibility.Visible : Visibility.Collapsed;
            button_back_from_down.Visibility = (!en) ? Visibility.Visible : Visibility.Collapsed;
        }

        public void setEnable(Control c, bool en)
        {
            c.IsEnabled = en;
            c.Opacity = (en) ? 1 : 0.5f;
        }

        #endregion

        #region Capture And Edit Mode
        
        private void CameraNotConnected_Click(object sender, EventArgs e)
        {
           // btn_sendToPlc_back_Click(null, null);
        }

        private void CaptureMode()
        {
            //border_tools.Visibility = Visibility.Collapsed;
            //border_tools1.Visibility = Visibility.Collapsed;
            //border_Directions.Visibility = Visibility.Collapsed;
            //border_monitors.Visibility = Visibility.Collapsed;
            pagesStack.Push(contentControl.Content);
            CameraPage c = new CameraPage();
            c.notConnectedHandler += CameraNotConnected_Click;
            c.start();
            contentControl.Content = c;

            setEnable(btn_sendToPlc_back, true);
            setEnable(button_about, false);
            //setEnable(btn_help, false);
            setEnable(button_tools, false);
            setEnable(button_setting, false);
            setEnable(btn_CreatePathTool, false);
            setEnable(btn_clearPath, false);

            setEnable(btn_sendToPlc_back, true);
            ((Image)btn_sendToPlc_back.Content).Source =
                new BitmapImage(
                    new Uri("pack://application:,,,/Cad2D;component/Resources/back.png",
                    UriKind.Absolute));

            ((Image)button_switchToCamera.Content).Source =
                new BitmapImage(
                    new Uri("pack://application:,,,/Cad2D;component/Resources/tick.png",
                    UriKind.Absolute));
        }


        private void EditMode()
        {
            contentControl.Content = pagesStack.Pop();
            border_tools1.Visibility = Visibility.Visible;
            border_setting.Visibility = Visibility.Visible;
            setEnable(button_about, true);
            setEnable(button_setting, true);
            setEnable(button_tools, true);
            setEnable(btn_sendToPlc_back, connectedsList.loop);
            setEnable(btn_CreatePathTool, !connectedsList.loop);
            setEnable(btn_clearPath, connectedsList.loop);

            ((Image)btn_sendToPlc_back.Content).Source =
                new BitmapImage(
                    new Uri("pack://application:,,,/Cad2D;component/Resources/saw.png",
                    UriKind.Absolute));

            ((Image)button_switchToCamera.Content).Source =
                new BitmapImage(new Uri("pack://application:,,,/Cad2D;component/Resources/camera.png", UriKind.Absolute));
        }
        
        #endregion

        private void calculateAllPoints()
        {
            lineGeometryList.Clear();
            foreach (Circle c in guids)
            {
                mainCanvas.Children.Remove(c);
            }
            guids.Clear();
            foreach (ConnectedLine c in connectedsList)
                lineGeometryList.Add(new LineGeometry(c.line));
            LinkedList<System.Drawing.PointF> points = calculateInnerPoints();
            double[] array1;
            array1 = calCulateVerticalPoints();
            stoneVerticalEdge = calculateStoneVerticalPoints(array1);
            // y + (2 * horizontal slice) = z
            double[] array2;
            array2 = calCulateHorizontalPoints();
            stoneHorizontalEdge = calculateStoneHorizontalPoints(array2);
            // z 
            bool[,] array3;
            array3 = calCulateTheArray();
            stoneScan = calculateStoneScan(array3);

            dataGrid.Items.Clear();
            for (int i = 0; i < StoneEdgeVerticalSlice; i++)
            {
                if (i < stoneEdgeHorizontalSlice)
                    dataGrid.Items.Add(new GridItem() { val1 = i, val2 = setPericision(stoneVerticalEdge[i * 2]), val3 = setPericision(stoneVerticalEdge[i * 2 + 1]), val4 = setPericision(stoneHorizontalEdge[i * 2]), val5 = setPericision(stoneHorizontalEdge[i * 2 + 1]) });
                else
                    dataGrid.Items.Add(new GridItem() { val1 = i, val2 = setPericision(stoneVerticalEdge[i * 2]), val3 = setPericision(stoneVerticalEdge[i * 2 + 1]), val4 = 0, val5 = 0 });
            }
        }

        private LinkedList<System.Drawing.PointF> calculateInnerPoints()
        {
            PrimarySettings pri = Extentions.FromXmlPrimary();
            float offset = (float)(704.0/3500.0)*pri.edgeOffset; 
            LinkedList<System.Drawing.PointF> points = new LinkedList<System.Drawing.PointF>();
            for (int i = 0 ; i < lineGeometryList.Count ; i++)
            {
                LineGeometry line1 = lineGeometryList[i];
                LineGeometry line2 = (i+1) < lineGeometryList.Count ? lineGeometryList[i+1] : lineGeometryList[0];

                double angle = Math.Abs((line2.angleX - line1.angleX) / 2) +
                                        Math.Min(line2.angleX, line1.angleX);

                PointF center = new PointF();
                PointF p1PointF = new PointF();
                PointF p2PointF = new PointF();
                if (line1.X1 == line2.X1 && line1.Y1 == line2.Y1)
                {
                    center = new PointF((float)line1.X1 , (float)line1.Y1);
                    p1PointF = new PointF((float)line1.X2 , (float)line1.Y2);
                    p2PointF =new PointF((float)line2.X2, (float)line2.Y2);
                }
                else if (line1.X1 == line2.X2 && line1.Y1 == line2.Y2)
                {
                    center = new PointF((float)line1.X1, (float)line1.Y1);
                    p1PointF = new PointF((float)line1.X2, (float)line1.Y2);
                    p2PointF = new PointF((float)line2.X1, (float)line2.Y1);
                }
                else if (line1.X2 == line2.X1 && line1.Y2 == line2.Y1)
                {
                    center = new PointF((float)line2.X1, (float)line2.Y1);
                    p1PointF = new PointF((float)line1.X1, (float)line1.Y1);
                    p2PointF = new PointF((float)line2.X2, (float)line2.Y2);
                }
                else if (line1.X2 == line2.X2 && line1.Y2 == line2.Y2)
                {
                    center = new PointF((float)line1.X2, (float)line1.Y2);
                    p1PointF = new PointF((float)line1.X1, (float)line1.Y1);
                    p2PointF = new PointF((float)line2.X1, (float)line2.Y1);
                }

                LineGeometry newLine =new LineGeometry(angle , center.X, center.Y);
                PointF [] point= newLine.calculateTestPonits(4);
                int count = point.Count(t => checkIsInnerPoint(t));
                if (count%2 == 0)
                    newLine = new LineGeometry(angle +(Math.PI/2), center.X, center.Y);

                double angelBetBisectorAndLine = LineGeometry.CalculateAngle3Point(center, p1PointF, p2PointF);
                point = newLine.calculatePonits(offset, (angelBetBisectorAndLine/2));
                if (checkIsInnerPoint(point[0]))
                {
                    Circle c1 = cloneCircle();
                    c1.mouseActionsEnable = false;
                    c1.radius = 2;
                    c1.X = point[0].X;
                    c1.defaultColor = Brushes.MediumPurple;
                    c1.Y = point[0].Y;
                    guids.Add(c1);
                    mainCanvas.Children.Add(c1);
                    points.AddLast(point[0]);
                }
                if (checkIsInnerPoint(point[1]))
                {
                    Circle c = cloneCircle();
                    c.mouseActionsEnable = false;
                    c.radius = 2;
                    c.X = point[1].X;
                    c.defaultColor = Brushes.Yellow;
                    c.Y = point[1].Y;
                    guids.Add(c);
                    mainCanvas.Children.Add(c);
                    points.AddLast(point[1]);
                }
            }
            for (int i = 0; i < points.Count; i++)
            {
                Line newLine = InnerLine();
                PointF p1 = points.ElementAt(i);
                PointF p2 = ((i + 1) < points.Count) ? points.ElementAt(i+1) : points.ElementAt(0);
                newLine.X1 = p1.X;
                newLine.Y1 = p1.Y;
                newLine.X2 = p2.X;
                newLine.Y2 = p2.Y;
                mainCanvas.Children.Add(newLine);
                innerLines.Add(newLine);
            }
            return points;
        }

        private bool checkIsInnerPoint(PointF point )
        {
            int biger = 0;
            int smaller =0;
            foreach (LineGeometry lg in lineGeometryList)
            {
                if (lg.verticalStright && lg.checkVerticalColision(point.X) && (point.Y < lg.maxY || point.Y > lg.minX))
                    return true;

                if (lg.checkVerticalColision(point.X))
                {
                    if (lg.calculateLineY(point.X) < point.Y)
                        biger++;
                    else
                        smaller++;
                }
            }
            if (biger % 2 != 0 && (biger + smaller) % 2 == 0)
                return true;
            return false;
        }

        //private void sendDataToPlc()
        //{
        //    _sendDataToPlc();
        //    //directionDialog = MainWindow._window.showDirections();
        //}

        private void _sendDataToPlc()
        {
            try
            {
                Dispatcher.Invoke(() => progressDialog = MainWindow._window.showProgress());
                Thread t = new Thread(sendingStoneScanToPLC);
                t.Start();
            }
            catch (Exception ex)
            {
                Logger.LogError("_Message : " + ex.Message + "\n\n_Source : " + ex.Source + "\n\n_TargetSite : " + ex.TargetSite + "\n\n _ALL : " + ex.ToString(), LogType.Error, ex);
            }
        }
        public double setPericision(double v)
        {
            return Math.Truncate(v * 1000f) / 1000f;
        }

        private Task<MyProgressDialog> progressDialog;
        private Task<MyStartDirectionDialog> directionDialog;

        private void sendingStoneScanToPLC()
        {
            if (lsConnection.Connected)
            {
                stoneScanPacketCount = stoneScan.Length;
                verticalBoundryCount = stoneVerticalEdge.Length;
                horizonalBoundryCount = stoneHorizontalEdge.Length;
                stoneScanPacketCounter = 0;
                horizonalBoundryCounter = 0;
                verticalBoundryCounter = 0;
                 
                if (lsConnection.Connected)
                    lsConnection.writeToPlc(DataType.WORD, stoneScan[stoneScanPacketCounter], scanAriaSegment + stoneScanPacketCounter, ref writingPackets);
                 
            }
            else
            {
                Dispatcher.Invoke(new Action(() =>
                ((MainWindow)Application.Current.MainWindow).showMsg("خطا", "پی ال سی قطع می باشد . لطفا ابتدا به آن متصل شوید.")));
            }
        }

        // convert bool array of stone scan to simple int array


        //@milad
        #region calculating stone edge and points
        private ushort[] calculateStoneHorizontalPoints(double[] array)
        {
            ushort[] stoneHorizontalPoints = new ushort[array.Length];
            double zarib = (maxHorizontalSlice - minHorizontalSlice) / (endPoint.X - startPoint.X);
            for (int i = 0; i < (array.Length / 2); i++)
            {
                if (array[i * 2] == startPoint.X && array[i * 2 + 1] == startPoint.X)
                {
                    stoneHorizontalPoints[i * 2] = 0;
                    stoneHorizontalPoints[i * 2 + 1] = 0;
                    continue;
                }

                stoneHorizontalPoints[i * 2] = (ushort)(zarib * (array[i * 2] - startPoint.X) + minHorizontalSlice);
                stoneHorizontalPoints[i * 2 + 1] = (ushort)((zarib * (array[i * 2 + 1] - startPoint.X) + minHorizontalSlice));
                if (stoneHorizontalPoints[i * 2 + 1] >= maxHorizontalSlice)
                    stoneHorizontalPoints[i * 2 + 1] = (ushort)(maxHorizontalSlice - 1);
                if (stoneHorizontalPoints[i * 2] >= maxHorizontalSlice)
                    stoneHorizontalPoints[i * 2] = (ushort)(maxHorizontalSlice - 1);
            }
            return stoneHorizontalPoints;
        }

        private ushort[] calculateStoneVerticalPoints(double[] array1)
        {
            ushort[] stoneVerticalPoints = new ushort[array1.Length];
            double zarib = (maxVerticalSlice - minVerticalSlice) / (endPoint.Y - startPoint.Y);
            for (int i = 0; i < (array1.Length / 2); i++)
            {
                if(array1[i * 2] == startPoint.Y && array1[i * 2 + 1] == startPoint.Y)
                {
                    stoneVerticalPoints[i * 2] = 0;
                    stoneVerticalPoints[i * 2 + 1] = 0;
                    continue;
                }
                stoneVerticalPoints[i * 2] = (ushort)(zarib * (array1[i * 2] - startPoint.Y) + minVerticalSlice);
                stoneVerticalPoints[i * 2 + 1] = (ushort)((zarib * (array1[i * 2 + 1] - startPoint.Y) + minVerticalSlice));
                if (stoneVerticalPoints[i * 2 + 1] >= maxVerticalSlice)
                    stoneVerticalPoints[i * 2 + 1] = (ushort)(maxVerticalSlice - 1);
                if (stoneVerticalPoints[i * 2] >= maxVerticalSlice)
                    stoneVerticalPoints[i * 2] = (ushort)(maxVerticalSlice - 1);
            }
            return stoneVerticalPoints;
        }
        private bool[,] calCulateTheArray()
        {
            widthOffsetLength = ((float)(endPoint.X - startPoint.X)) / StoneScanVerticalSlice;
            double WOffset = startPoint.X;
            heightOffsetLength = ((float)(endPoint.Y - startPoint.Y)) / stoneScanHorizontalSlice;
            double HOffset = startPoint.Y;

            bool[,] array = new bool[stoneScanHorizontalSlice, StoneScanVerticalSlice];

            PrimarySettings ps = Extentions.FromXmlPrimary();

            for (int i = 0; i < stoneScanHorizontalSlice; i++)
            {
                for (int j = 0; j < StoneScanVerticalSlice; j++)
                {
                    array[i, j] = false;
                    short biger = 0;
                    short smaller = 0;
                    foreach (LineGeometry lg in lineGeometryList)
                    {
                        if (lg.verticalStright && lg.checkVerticalColision(WOffset) && (HOffset < lg.maxY || HOffset > lg.minX))
                        {
                            array[i, j] = true;
                            break;
                        }

                        if (lg.checkVerticalColision(WOffset))
                        {
                            if (lg.calculateLineY(WOffset) < HOffset)
                                biger++;
                            else
                                smaller++;
                        }
                    }
                    if (array[i, j] == true)
                        continue;
                    if (biger % 2 != 0 && (biger + smaller) % 2 == 0)
                    {
                        array[i, j] = true;
                        // for testing ->
                        if(ps.showGuideCircles)
                        {
                            Circle c = cloneCircle();
                            c.mouseActionsEnable = false;
                            c.radius = 2;
                            c.X = WOffset;
                            c.defaultColor = Brushes.GreenYellow;
                            c.Y = HOffset;
                            guids.Add(c);
                            mainCanvas.Children.Add(c);
                            //end testing   
                        }
                    }

                    WOffset += widthOffsetLength;
                }
                WOffset = startPoint.X;
                HOffset += heightOffsetLength;
            }

            return array;
        }

        private double[] calCulateVerticalPoints()
        {
            double[] array = new double[2 * StoneEdgeVerticalSlice];


            widthOffsetLength = ((float)(endPoint.X - startPoint.X)) / StoneEdgeVerticalSlice;
            double offset = startPoint.X;
            double[] values = new double[2];
            PrimarySettings ps = Extentions.FromXmlPrimary();
            for (int i = 0; i < StoneEdgeVerticalSlice; i++)
            {
                int j = 0;
                values[0] = 0;
                values[1] = 0;
                foreach (LineGeometry l in lineGeometryList)
                {
                    if (l.verticalStright)
                    {
                        if (l.checkVerticalColision(offset, widthOffsetLength))
                        {
                            values = l.strightVerticalValues();
                            j = 2;
                        }
                    }
                    else
                    {
                        if (l.checkVerticalColision(offset))
                        {
                            if (j >= 2)
                            {
                                double d = l.calculateLineY(offset);
                                if (d < values[0])
                                    values[0] = d;
                                else if (d > values[1])
                                    values[1] = d;
                            }
                            else
                            {
                                values[j] = l.calculateLineY(offset);
                                if (j >= 1)
                                {
                                    if (values[0] > values[1])
                                    {
                                        double tmp = values[0];
                                        values[0] = values[1];
                                        values[1] = tmp;
                                    }
                                }
                                j++;
                            }
                        }
                    }
                }

                values[0] = Clamp(values[0], startPoint.Y, endPoint.Y);
                values[1] = Clamp(values[1], startPoint.Y, endPoint.Y);
                if (values[0] != startPoint.Y || values[1] != startPoint.Y)
                {
                    if (ps.showGuideCircles)
                    {
                        //for testing 
                        Circle c = cloneCircle();
                        c.mouseActionsEnable = false;
                        c.radius = 2;
                        c.X = offset;
                        c.Y = values[0];

                        Circle c2 = cloneCircle();
                        c2.mouseActionsEnable = false;
                        c2.radius = 2;
                        c2.X = offset;
                        c2.Y = values[1];
                        guids.Add(c);
                        guids.Add(c2);
                        mainCanvas.Children.Add(c);
                        mainCanvas.Children.Add(c2);
                        //end testing
                    }
                }
                array[2 * i] = values[0];
                array[2 * i + 1] = values[1];

                offset += widthOffsetLength;
            }

            return array;
        }

        private int[] calculateStoneScan(bool[,] array)
        {
            int[] stoneScan = new int[(stoneScanHorizontalSlice / 16) * StoneScanVerticalSlice];

            for (int i = 0; i < StoneScanVerticalSlice; i++)
            {
                for (int k = 0; k < (stoneScanHorizontalSlice / 16); k++)
                {
                    for (int j = 15; j >=0 ; j--)
                    {
                        stoneScan[((stoneScanHorizontalSlice / 16) * i) + k] *= 2;
                        if (array[(16 * k) + j, i])
                            stoneScan[((stoneScanHorizontalSlice / 16) * i) + k]++;
                    }
                }
            }

            return stoneScan;
        }


        private void button_showData_Click(object sender, RoutedEventArgs e)
        {
            if (_windowDisplaySendData == null || ! _windowDisplaySendData.IsVisible)
            {
                _windowDisplaySendData = new Window_DisplaySendData(dataGrid.Items);
                _windowDisplaySendData.Show();
            }
            else
            {
               
                _windowDisplaySendData.Show();
                if (_windowDisplaySendData.WindowState == WindowState.Minimized)
                {
                    _windowDisplaySendData.WindowState = WindowState.Normal;
                }
                _windowDisplaySendData.Activate();
                _windowDisplaySendData.Topmost = true;  // important
                _windowDisplaySendData.Topmost = false; // important
                _windowDisplaySendData.Focus();
            }
        }
        
        private void dataGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            e.Cancel = true;
        }

        private double[] calCulateHorizontalPoints()
        {
            double[] array = new double[2 * stoneEdgeHorizontalSlice];

            heightOffsetLength = ((float)(endPoint.Y - startPoint.Y)) / stoneEdgeHorizontalSlice;
            double offset = startPoint.Y;
            double[] values = new double[2];
            PrimarySettings ps = Extentions.FromXmlPrimary();
            for (int i = 0; i < stoneEdgeHorizontalSlice; i++)
            {
                int j = 0;
                values[0] = 0;
                values[1] = 0;
                foreach (LineGeometry l in lineGeometryList)
                {
                    if (l.HorizonalStright)
                    {
                        if (l.checkHorizentalColision(offset, heightOffsetLength))
                        {
                            values = l.strightHorizentalValues();
                            j = 2;
                        }
                    }
                    else
                    {
                        if (l.checkHorizentalColision(offset))
                        {
                            if (j >= 2)
                            {
                                double d = l.calculateLineX(offset);
                                if (d < values[0])
                                    values[0] = d;
                                else if (d > values[1])
                                    values[1] = d;
                            }
                            else
                            {
                                values[j] = l.calculateLineX(offset);
                                if (j >= 1)
                                {
                                    if (values[0] > values[1])
                                    {
                                        double tmp = values[0];
                                        values[0] = values[1];
                                        values[1] = tmp;
                                    }
                                }
                                j++;
                            }
                        }
                    }
                }


                values[0] = Clamp(values[0], startPoint.X, endPoint.X);
                values[1] = Clamp(values[1], startPoint.X, endPoint.X);

                if (values[0] != startPoint.X || values[1] != startPoint.X)
                {
                    if(ps.showGuideCircles)
                    {
                        Circle c = cloneCircle();
                        c.mouseActionsEnable = false;
                        c.radius = 2;
                        c.X = values[0];
                        c.defaultColor = Brushes.Red;
                        c.Y = offset;

                        Circle c2 = cloneCircle();
                        c2.mouseActionsEnable = false;
                        c2.radius = 2;
                        c2.X = values[1];
                        c2.defaultColor = Brushes.Red;
                        c2.Y = offset;

                        guids.Add(c);
                        guids.Add(c2);
                        mainCanvas.Children.Add(c);
                        mainCanvas.Children.Add(c2);

                    }
                    //for testing 
                    // end testing 
                }


                array[2 * i] = values[0];
                array[2 * i + 1] = values[1];

                offset += heightOffsetLength;
            }

            return array;
        }

        private double Clamp(double var, double valueMin, double valueMax)
        {
            if (var > valueMax)
                var = valueMax;
            if (var < valueMin)
                var = valueMin;
            return var;
        }

        #endregion
        

        private bool sendingInFirstTime = true;
        private Packet<ushort> directionTypePacket;
        
        private void textBox__TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox t = (TextBox) sender;
            Regex regex = new Regex("[^0-9.-]+");

            if (regex.IsMatch(t.Text))
                t.Text = "";
            short x;
            Int16.TryParse(t.Text, out x);
            if (x > 10)
                t.Text = "10";
            if (x <0)
                t.Text = "0";

        }

        private void button_water_Click(object sender, RoutedEventArgs e)
        {
            bool x = (plcInformation.water.value & (1 << 1 - 1)) != 0;
            if (x)
            {
                plcInformation.water.value = (ushort)(plcInformation.water.value & 65534);
            }
            else
            {
                plcInformation.water.value = (ushort)(plcInformation.water.value | 1);
            }
            plcInformation.writeWater(plcInformation.water.value);
        }

        private void button_Water_Timer_Click(object sender, RoutedEventArgs e)
        {
            bool x = (plcInformation.water.value & (1 << 1 - 1)) != 0;
            if (x)
            {
                plcInformation.water.value = (ushort)(plcInformation.water.value | 1);
            }
            else
            {
                plcInformation.water.value = (ushort)(plcInformation.water.value & 65534);
            }
            x = (plcInformation.water.value & (1 << 2 - 1)) != 0;
            if (x)
            {
                plcInformation.water.value = (ushort)(plcInformation.water.value & 65533);
            }
            else
            {
                plcInformation.water.value = (ushort)(plcInformation.water.value | 2);
            }
            plcInformation.writeWater(plcInformation.water.value);
        }



        private void textBox_Timer_On_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                plcInformation.writeWaterTOn(Int32.Parse(textBox_Timer_On.Text));
            }
        }

        private void textBox_Timer_Off_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                plcInformation.writeWaterTOff(Int32.Parse(textBox_Timer_Off.Text));
            }
        }

        private void button_edgeStart_MouseDown(object sender, MouseButtonEventArgs e)
        {
            int data = 0;
            if (plcInformation.edgeEndStart.value == 0)
                data = 1;
            else if (plcInformation.edgeEndStart.value == 1)
                data = 0;
            else if (plcInformation.edgeEndStart.value == 2)
                data = 3;
            else if (plcInformation.edgeEndStart.value == 3)
                data = 2;
            plcInformation.edgeEndStart.value = (ushort)data;
            plcInformation.writeEdgeStartEnd(data);
        }

        private void button_edgeEnd_MouseDown(object sender, MouseButtonEventArgs e)
        {
            int data = 0;
            if (plcInformation.edgeEndStart.value == 0)
                data = 2;
            else if (plcInformation.edgeEndStart.value == 1)
                data = 3;
            else if (plcInformation.edgeEndStart.value == 2)
                data = 0;
            else if (plcInformation.edgeEndStart.value == 3)
                data = 1;
            plcInformation.edgeEndStart.value = (ushort)data;
            plcInformation.writeEdgeStartEnd(data);
        }

        private void textBox_horizontalSob_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
        {
            if (lsConnection == null) return;
            if (lsConnection.Connected)
            {
                if (textBox_horizontalSob.Value != null)
                    lsConnection.writeToPlc(plcInformation.horizontalSob.dataType, (int)textBox_horizontalSob.Value, plcInformation.horizontalSob.valueAddress, ref plcInformation.horizontalSob.writingPacket);
            }
        }

        private void textBox_verticalSob_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
        {
            if (lsConnection == null) return;
            if (lsConnection.Connected)
            {
                if (textBox_verticalSob.Value != null)
                    lsConnection.writeToPlc(plcInformation.verticalSob.dataType, (int)textBox_verticalSob.Value, plcInformation.verticalSob.valueAddress, ref plcInformation.verticalSob.writingPacket);
            }
        }

        private void textBox_edgeSob_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
        {
            if(lsConnection == null) return;
            if (lsConnection.Connected)
            {
                lsConnection.writeToPlc(plcInformation.edgeSob.dataType, (int)textBox_edgeSob.Value, plcInformation.edgeSob.valueAddress, ref plcInformation.edgeSob.writingPacket);
            }
        }
    }
}
