using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using Cad2D.Pages;


namespace SiriusMicrotech.core.UI
{

    public class KeypadTextBox : TextBox
    {
        protected Popup popKpd;
        protected Grid kpd;
        public void hideKeypad()
        {
            if(popKpd != null)
                popKpd.IsOpen = false;
        }
    }

    public partial class TouchNumericBox : KeypadTextBox
    {
        
        public TouchNumericBox()
            : base()
        {
            /*
            try
            {
                DefaultStyleKeyProperty.OverrideMetadata(typeof(TouchNumericBox), new FrameworkPropertyMetadata(typeof(TouchNumericBox)));
                
            }
            catch { }*/
        }


        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            Page_Settings.hideKeypad();
            if (popKpd == null)     // Check if we already have a keypad created
            {
                popKpd = new Popup();
                popKpd.Placement = PlacementMode.Bottom;
                popKpd.PopupAnimation = PopupAnimation.Scroll;
                kpd = new Grid();

                for (int i = 0; i < 5; i++)
                {
                    kpd.ColumnDefinitions.Add(new ColumnDefinition());
                    kpd.RowDefinitions.Add(new RowDefinition());
                }
                int btn = 1;
                populateGridPos(0, 0, "بستن", 3);

                for (int row = 1; row < 4; row++)
                {
                    for (int col = 0; col < 3; col++)
                        populateGridPos(row, col, Convert.ToString(btn++));
                }
                populateGridPos(5, 0, "0");
                populateGridPos(5, 1, ".");
                populateGridPos(5, 2, "CLR");
                

                Border b = new Border();
                b.BorderBrush = (Brush)(new BrushConverter().ConvertFrom("#FF293955"));
                b.BorderThickness = new Thickness(2);
                b.Child = kpd;
                popKpd.Child = b;
                popKpd.IsOpen = false;

                popKpd.PlacementTarget = this;
                popKpd.Placement = PlacementMode.Bottom;
                popKpd.HorizontalOffset = -10;
            }
            popKpd.IsOpen = true;
            base.OnMouseDown(e);
        }


        private void populateGridPos(int row, int col, string btn, int noPos)
        {
            var button = new Button();
            button.Style = (Style)FindResource("TouchKey");
            button.Content = btn;
            button.Width = 40 * noPos;
            button.Click += new RoutedEventHandler(button_Click);
            Grid.SetRow(button, row);
            Grid.SetColumn(button, col);
            Grid.SetColumnSpan(button, noPos);
            this.kpd.Children.Add(button);
        }
        private void populateGridPos(int row, int col, string btn)
        {
            var button = new Button();
            button.Style = (Style)FindResource("TouchKey");
            button.Content = btn;
            button.Width = 40;
            button.Click += new RoutedEventHandler(button_Click);
            Grid.SetRow(button, row);
            Grid.SetColumn(button, col);
            this.kpd.Children.Add(button);
        }

        void button_Click(object sender, RoutedEventArgs e)
        {
            if (this.IsReadOnly) return;
            Button btn = (Button)e.OriginalSource;
            string s = btn.Content.ToString();

            if (s.Equals("بستن"))
                popKpd.IsOpen = false;

            else if (s == "CLR")
            {
                this.Text = "";
            }
            else
            {
                this.Text += s;
            }
        }

    }
    public partial class TouchAlphaNumericBox : KeypadTextBox
    {
        bool spPressed = false;

        public TouchAlphaNumericBox()
            : base()
        {
            /*
            try
            {
                DefaultStyleKeyProperty.OverrideMetadata(typeof(TouchAlphaNumericBox), new FrameworkPropertyMetadata(typeof(TouchAlphaNumericBox)));
            }
            catch { }*/
        }
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            if (popKpd == null)     // Check if we already have a keypad created
            {
                popKpd = new Popup();
                popKpd.Placement = PlacementMode.Bottom;
                popKpd.PopupAnimation = PopupAnimation.Slide;
                kpd = new Grid();

                for (int i = 0; i < 7; i++)
                {
                    kpd.ColumnDefinitions.Add(new ColumnDefinition());
                    kpd.RowDefinitions.Add(new RowDefinition());
                }
                int r = 1;
                populateGridPos(0, 0, "بستن", 7);
                populateGridPos(r, 0, "A", 1);
                populateGridPos(r, 1, "B", 1);
                populateGridPos(r, 2, "C", 1);
                populateGridPos(r, 3, "D", 1);
                populateGridPos(r, 4, "7", 1);
                populateGridPos(r, 5, "8", 1);
                populateGridPos(r++, 6, "9", 1);

                populateGridPos(r, 0, "E", 1);
                populateGridPos(r, 1, "F", 1);
                populateGridPos(r, 2, "G", 1);
                populateGridPos(r, 3, "H", 1);
                populateGridPos(r, 4, "4", 1);
                populateGridPos(r, 5, "5", 1);
                populateGridPos(r++, 6, "6", 1);

                populateGridPos(r, 0, "I", 1);
                populateGridPos(r, 1, "J", 1);
                populateGridPos(r, 2, "K", 1);
                populateGridPos(r, 3, "L", 1);
                populateGridPos(r, 4, "1", 1);
                populateGridPos(r, 5, "2", 1);
                populateGridPos(r++, 6, "3", 1);

                populateGridPos(r, 0, "M", 1);
                populateGridPos(r, 1, "N", 1);
                populateGridPos(r, 2, "O", 1);
                populateGridPos(r, 3, "P", 1);
                populateGridPos(r, 4, "0", 1);
                populateGridPos(r, 5, ".", 1);
                populateGridPos(r++, 6, "!", 1);

                populateGridPos(r, 0, "Q", 1);
                populateGridPos(r, 1, "R", 1);
                populateGridPos(r, 2, "S", 1);
                populateGridPos(r, 3, "T", 1);
                populateGridPos(r, 4, "@", 1);
                populateGridPos(r, 5, "#", 1);
                populateGridPos(r++, 6, "$", 1);

                populateGridPos(r, 0, "U", 1);
                populateGridPos(r, 1, "V", 1);
                populateGridPos(r, 2, "W", 1);
                populateGridPos(r, 3, "X", 1);
                populateGridPos(r, 4, "Y", 1);
                populateGridPos(r, 5, "Z", 1);
                populateGridPos(r++, 6, "%", 1);


                // Double pos keys
                populateGridPos(r, 0, "CAP", 2);
                populateGridPos(r, 5, "CLR", 2);

                // Three pos key
                populateGridPos(r, 2, "SPACE", 3);

                Border b =new Border();
                b.BorderBrush = (Brush)(new BrushConverter().ConvertFrom("#FF293955"));
                b.BorderThickness = new Thickness(2);

                b.Child = kpd;
                popKpd.Child = b;
                popKpd.IsOpen = false;

                popKpd.PlacementTarget = this;
                popKpd.Placement = PlacementMode.Bottom;
                popKpd.HorizontalOffset = -40;
                popKpd.IsOpen = true;
            }
            popKpd.IsOpen = true;
            base.OnMouseDown(e);
        }

        private void populateGridPos(int row, int col, string btn, int noPos)
        {
            var button = new Button();
            button.Style = (Style)FindResource("TouchKey");
            button.Content = btn;
            button.Width = 40 * noPos;
            button.Click += new RoutedEventHandler(button_Click);
            Grid.SetRow(button, row);
            Grid.SetColumn(button, col);
            Grid.SetColumnSpan(button,noPos);
            this.kpd.Children.Add(button);
        }

        void button_Click(object sender, RoutedEventArgs e)
        {
            if (this.IsReadOnly) return;
            Button btn = (Button)e.OriginalSource;
            string s = btn.Content.ToString();
            if (s == "SPACE") this.Text += " ";
            if (s == "بستن") this.popKpd.IsOpen = false;
            else if (s == "CAP") spPressed ^= true;
            else if (s == "CLR") this.Text = "";
            else
            {
                if (spPressed)
                {
                    this.Text += s.ToUpper();
                }
                else
                {
                    this.Text += s.ToLower();
                }
            }
        }

    }
}
