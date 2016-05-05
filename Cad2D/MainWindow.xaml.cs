using MahApps.Metro.Controls;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.SimpleChildWindow;

namespace Cad2D
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public static MainWindow _window;
        private CanvasCad2D cc2d;
        private MyProgressDialog m;
        public MainWindow()
        {
            InitializeComponent();
            _window = this;
            RegistryKey k = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Auth");
            if (k != null)
            {
                contentControl.Content = new Pages.Page_SignIn(false);
            }
            else
            {
                cc2d = new CanvasCad2D();
                contentControl.Content = cc2d;
            }
            this.Closed +=OnClosed;
            /*
            if (!FileAssociation.IsAssociated(".c2d"))
                FileAssociation.Associate(".c2d", "ClassID.ProgID", "2D Cad design files", 
                    "Resources\file_ext_app.ico", "Cad2D.exe");

            string [] allArgs = Environment.GetCommandLineArgs();

            if(allArgs.Length > 1)
            {
                openDesignFile(allArgs[1]);
            }
            openImageFile(@"C:\Users\-MR-\Desktop\image.jpg");*/
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(MyHandler);
        }

        public void MyHandler(object sender, UnhandledExceptionEventArgs args)
        {
            Exception ex = (Exception)args.ExceptionObject;
            Logger.LogError("_File : MainWindow" + "\n_Message : " + ex.Message + "\n_Source : " + ex.Source + "\n_TargetSite : " + ex.TargetSite + "\n", LogType.Error, ex);
        }

        private void OnClosed(object sender, EventArgs eventArgs)
        {
            cc2d.ShutDown();
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {

        }

        public async void showMsg(string title, string msg)
        {
            await this.ShowChildWindowAsync(new MyDialog(title, msg) { Title = "توجه" });
        }
        public async Task<MyProgressDialog> showProgress()
        {
            m = new MyProgressDialog();
            await this.ShowChildWindowAsync(m);
            return m;
        }

        public void setMValue(int total , int stoneScan, int horizontalPoints, int verticalPoints, int innerPoints)
        {
            m?.setProgressValues(total,stoneScan,horizontalPoints,verticalPoints, innerPoints);
        }

        public async Task<MyStartDirectionDialog> showDirections()
        {
                MyStartDirectionDialog m = new MyStartDirectionDialog();
                m.ClosingFinished += MOnClosed;
                await this.ShowChildWindowAsync(m);
                return m;
        }

        private void MOnClosed(object sender, EventArgs eventArgs)
        {
            try
            {
                MyStartDirectionDialog m = (MyStartDirectionDialog)sender;

            }
            catch (Exception ex)
            {
                Logger.LogError("_File : MainWindow" + "\n_Message : " + ex.Message + "\n_Source : " + ex.Source + "\n_TargetSite : " + ex.TargetSite + "\n", LogType.Error, ex);
            }
        }

        public void showAdminPasswordReq()
        {
            this.ShowChildWindowAsync(new MyPasswordDialog());
        }

        /*
        private void openImageFile(string filePath)
        {
            BitmapImage src = new BitmapImage();
            src.BeginInit();
            src.UriSource = new Uri(filePath, UriKind.Relative);
            src.CacheOption = BitmapCacheOption.OnLoad;
            src.EndInit();

            cc2d.mainImage.Source = src;

            if (src.PixelHeight > src.PixelWidth)
            {
                cc2d.mainImage.VerticalAlignment = VerticalAlignment.Top;
                cc2d.mainImage.HorizontalAlignment = HorizontalAlignment.Center;
                //cc2d.mainImage.Height = cc2d.mainCanvas.Height = src.Height / Metrics.pixelPerUnit;
            }
            else
            {
                cc2d.mainImage.VerticalAlignment = VerticalAlignment.Center;
                cc2d.mainImage.HorizontalAlignment = HorizontalAlignment.Stretch;
                cc2d.mainImage.Width = cc2d.contentControl.ActualWidth;
            }


            cc2d.defaultBitmapHeightPixels = src.PixelHeight;
            cc2d.defaultBitmapWidthPixels = src.PixelWidth;
            cc2d.scale = 1;
            cc2d.imageLoaded = true;
        }
        /*
        public void menubutton_openDesignFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "2D Cad design files|*.c2d";

            open.ShowDialog();

            if (!open.FileName.Equals(""))
            {
                openDesignFile(open.FileName);
            }
        }
        public void menubutton_openImageFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Jpeg|*.jpg|Png|*.png";

            open.ShowDialog();

            if (!open.FileName.Equals(""))
            {
                openImageFile(open.FileName);
            }
        }

        private void openImageFile(string filePath)
        {
            BitmapImage src = new BitmapImage();
            src.BeginInit();
            src.UriSource = new Uri(filePath, UriKind.Relative);
            src.CacheOption = BitmapCacheOption.OnLoad;
            src.EndInit();
            
            cc2d.mainImage.Width = cc2d.mainCanvas.Width = src.Width / Metrics.pixelPerUnit;
            cc2d.mainImage.Height = cc2d.mainCanvas.Height = src.Height / Metrics.pixelPerUnit;

            cc2d.mainImage.Source = src;

            cc2d.defaultBitmapHeightPixels = src.PixelHeight;
            cc2d.defaultBitmapWidthPixels = src.PixelWidth;
            cc2d.scale = 1;
            cc2d.imageLoaded = true;
        }


        private void menuButton_saveDesign(object sender, RoutedEventArgs e)
        {
            SaveFileDialog save = new SaveFileDialog();
            save.Filter = "2D Cad design files|*.c2d";
            save.FileName = "MyDesign";
            save.ShowDialog();

            if (!save.FileName.Equals(""))
            {
                saveDesignFile(save.FileName);
            }
        }

        private void saveDesignFile(string filePath)
        {
            FileStream f = new FileStream(filePath, FileMode.OpenOrCreate);
            StreamWriter sw = new StreamWriter(f);

            string s = Parser.parseDataToString(cc2d.connectedsList);
            sw.WriteLine(s);

            MemoryStream ms = new MemoryStream();
            var encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create((BitmapSource)cc2d.mainImage.Source));
            encoder.Save(ms);

           // f.Write(ms, 500, ms.Length);

            sw.Close();
            f.Close();
        }
        private void openDesignFile(string filePath)
        {
            FileStream f = new FileStream(filePath, FileMode.Open);
            StreamReader sr = new StreamReader(f);
            string line = sr.ReadLine();

            sr.Close();
            f.Close();
        }
        static public string EncodeTo64(string toEncode)
        {
            byte[] toEncodeAsBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(toEncode);
            string returnValue = System.Convert.ToBase64String(toEncodeAsBytes);
            return returnValue;
        }

        static public string DecodeFrom64(string encodedData)
        {
            byte[] encodedDataAsBytes = System.Convert.FromBase64String(encodedData);
            string returnValue = System.Text.ASCIIEncoding.ASCII.GetString(encodedDataAsBytes);
            return returnValue;
        }*/
        private void exitApp(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}
