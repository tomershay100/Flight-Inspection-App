using System;
using System.Collections.Generic;
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

namespace DesktopApp.UserControls
{
    /// <summary>
    /// Interaction logic for GraphsUC.xaml
    /// </summary>
    public partial class GraphsUC : UserControl, IObservable
    {
        public GraphsUC()
        {
            InitializeComponent();
            //this.columnsName.SelectedItem = "aileron";
        }
        public event Update Notify;
        //upload DLL button.
        private void DLLFile_Click(object sender, RoutedEventArgs e)
        {
            Notify(this, new GraphEventArgs("ClickDLL"));
        }
        //the ListBox anomalies selection button
        private void AnomaliesPoints_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                string t = (string)e.AddedItems[0];
                Notify(this, new GraphEventArgs("SelectedAnomaly", t));
            }
            catch (Exception)
            {

            }
        }
    }
}
