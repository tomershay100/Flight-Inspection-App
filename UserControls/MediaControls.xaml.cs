using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace DesktopApp.UserControls
{
    /// <summary>
    /// Interaction logic for MediaControls.xaml
    /// </summary>
    public partial class MediaControls : UserControl, IObservable
    {
        public MediaControls()
        {
            InitializeComponent();
        }

        public event Update Notify;

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex1 = new Regex("[^0-9.]+");
            e.Handled = regex1.IsMatch(e.Text);
        }

        public double clickSubmit(string speedText)
        {
            double d;
            bool flag = double.TryParse(speedText, out d);
            if (flag && d >= 0)
            {
                return d;
            }
            else
            {
                throw new FormatException();
            }
        }

        private void submit_Click(object sender, RoutedEventArgs e)
        {
            if(Notify != null)
            {
                try
                {
                    Notify(this, new MediaEventArgs("submit", clickSubmit(speedText.Text)));
                }
                catch (Exception)
                {
                    MessageBox.Show("Invalid Value", "Alert", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void skip_back_Click(object sender, RoutedEventArgs e)
        {
            if(Notify != null)
            {
                Notify(this, new MediaEventArgs("skip back"));
            }
        }

        private void rewind_Click(object sender, RoutedEventArgs e)
        {
            if(Notify != null)
            {
                Notify(this, new MediaEventArgs("back"));
            }
        }

        private void pause_Click(object sender, RoutedEventArgs e)
        {
            if (Notify != null)
            {
                Notify(this, new MediaEventArgs("pause"));
            }
        }

        private void play_Click(object sender, RoutedEventArgs e)
        {
            if (Notify != null)
            {
                Notify(this, new MediaEventArgs("play"));
            }
        }

        private void stop_Click(object sender, RoutedEventArgs e)
        {
            if (Notify != null)
            {
                Notify(this, new MediaEventArgs("stop"));
            }
        }

        private void forward_Click(object sender, RoutedEventArgs e)
        {
            if (Notify != null)
            {
                Notify(this, new MediaEventArgs("forward"));
            }
        }

        private void end_Click(object sender, RoutedEventArgs e)
        {
            if (Notify != null)
            {
                Notify(this, new MediaEventArgs("skip end"));
            }
        }
    }
}
