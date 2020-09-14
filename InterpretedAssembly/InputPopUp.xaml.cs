using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace InterpretedAssembly
{
    /// <summary>
    /// Interaction logic for InputPopUp.xaml
    /// </summary>
    public partial class InputPopUp : Window
    {
        public InputPopUp()
        {
            InitializeComponent();
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            InfoTextBox.Text = Settings.InputPopUpInfo;

        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            Settings.InputPopUpReturn = InputTextBox.Text;
            Settings.LabelInputWait = false;
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(Settings.InputPopUpReturn == "")
            {
                Settings.InputPopUpReturn = "NOLABEL";
            }
        }
    }
}
