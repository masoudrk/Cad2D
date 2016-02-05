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



namespace SiriusMicrotech.core.UI
{
    /// <summary>
    ///  This is control is derived from WPF TextBox control. Eventhough WPF allows extensive customization of
    ///  controls using Styles and Control Templates, when we need extra functionality, we fall back to the 
    ///  time tested inheritance mechanism. This derived control called TouchNumericBox enables the user to 
    ///  use a on screen popup keypad to enter numerical data in to the text box. This control will be very 
    ///  useful in situations where the only input interface is a Touch Screen. A popup keypad is prefered here
    ///  over the OS supply Onscreen keyboard for comfortable sized keys and a strictly numeric input without a
    ///  need for test of data for valid numeric input.
    /// </summary>
    /// <function>
    /// When this control is used in the code, a regular TextBox with the style defined in TouchStyle.xaml file,
    /// is displayed. If the user clicks (touches) on the TextBox, a numeric keypad is dropped down as a popup. 
    /// When the entry is done, the user clicks (touches) the TextBox again to hide the keypad. This popup keypad 
    /// is assembled in code on the first time the user clicks (touches) on the TextBox. Subsequently, this popup
    /// keypad is used everytime the text box is used.
    /// </function>

    public class KeypadTextBox : TextBox
    {
        protected Popup popKpd;
        protected Grid kpd;
        public void hideKeypad()
        {
            popKpd.IsOpen = false;
        }
    }

    public partial class TouchNumericBox : KeypadTextBox
    {
        
        public TouchNumericBox()
            : base()
        {

            try
            {
                DefaultStyleKeyProperty.OverrideMetadata(typeof(TouchNumericBox), new FrameworkPropertyMetadata(typeof(TouchNumericBox)));
                
            }
            catch { }
        }


        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            if (popKpd == null)     // Check if we already have a keypad created
            {
                popKpd = new Popup();
                popKpd.Placement = PlacementMode.Bottom;
                popKpd.PopupAnimation = PopupAnimation.Scroll;
                kpd = new Grid();

                for (int i = 0; i < 4; i++)
                {
                    kpd.ColumnDefinitions.Add(new ColumnDefinition());
                    kpd.RowDefinitions.Add(new RowDefinition());
                }
                int btn = 1;
                for (int row = 2; row >= 0; row--)
                {
                    for (int col = 0; col < 3; col++)
                        populateGridPos(row, col, Convert.ToString(btn++));
                }
                populateGridPos(3, 0, "0");
                populateGridPos(3, 1, ".");
                populateGridPos(3, 2, "CLR");

                Border b = new Border();
                b.BorderBrush = (Brush)(new BrushConverter().ConvertFrom("#FF293955"));
                b.BorderThickness = new Thickness(2);

                b.Child = kpd;
                popKpd.Child = b;
                popKpd.IsOpen = false;

                popKpd.PlacementTarget = this;
                popKpd.Placement = PlacementMode.Bottom;
                popKpd.HorizontalOffset = -10;
                popKpd.IsOpen = true;
            }
            base.OnMouseDown(e);
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
            if (s == "CLR")
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

            try
            {
                DefaultStyleKeyProperty.OverrideMetadata(typeof(TouchAlphaNumericBox), new FrameworkPropertyMetadata(typeof(TouchAlphaNumericBox)));
            }
            catch { }
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
                populateGridPos(0, 0, "A", 1);
                populateGridPos(0, 1, "B", 1);
                populateGridPos(0, 2, "C", 1);
                populateGridPos(0, 3, "D", 1);
                populateGridPos(0, 4, "7", 1);
                populateGridPos(0, 5, "8", 1);
                populateGridPos(0, 6, "9", 1);

                populateGridPos(1, 0, "E", 1);
                populateGridPos(1, 1, "F", 1);
                populateGridPos(1, 2, "G", 1);
                populateGridPos(1, 3, "H", 1);
                populateGridPos(1, 4, "4", 1);
                populateGridPos(1, 5, "5", 1);
                populateGridPos(1, 6, "6", 1);

                populateGridPos(2, 0, "I", 1);
                populateGridPos(2, 1, "J", 1);
                populateGridPos(2, 2, "K", 1);
                populateGridPos(2, 3, "L", 1);
                populateGridPos(2, 4, "1", 1);
                populateGridPos(2, 5, "2", 1);
                populateGridPos(2, 6, "3", 1);

                populateGridPos(3, 0, "M", 1);
                populateGridPos(3, 1, "N", 1);
                populateGridPos(3, 2, "O", 1);
                populateGridPos(3, 3, "P", 1);
                populateGridPos(3, 4, "0", 1);
                populateGridPos(3, 5, ".", 1);
                populateGridPos(3, 6, "!", 1);

                populateGridPos(4, 0, "Q", 1);
                populateGridPos(4, 1, "R", 1);
                populateGridPos(4, 2, "S", 1);
                populateGridPos(4, 3, "T", 1);
                populateGridPos(4, 4, "@", 1);
                populateGridPos(4, 5, "#", 1);
                populateGridPos(4, 6, "$", 1);

                populateGridPos(5, 0, "U", 1);
                populateGridPos(5, 1, "V", 1);
                populateGridPos(5, 2, "W", 1);
                populateGridPos(5, 3, "X", 1);
                populateGridPos(5, 4, "Y", 1);
                populateGridPos(5, 5, "Z", 1);
                populateGridPos(5, 6, "%", 1);


                // Double pos keys
                populateGridPos(6, 0, "CAP", 2);
                populateGridPos(6, 5, "CLR", 2);

                // Three pos key
                populateGridPos(6, 2, "SPACE", 3);

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
