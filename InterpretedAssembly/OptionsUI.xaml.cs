using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Text.RegularExpressions;

namespace InterpretedAssembly
{
    /// <summary>
    /// Interaction logic for OptionsUI.xaml
    /// </summary>
    public partial class OptionsUI : Window
    {
        public OptionsUI()
        {
            InitializeComponent();
        }

        private void DelayInput_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            DelayInput.Value = (int)DelayInput.Value;
            if(DelayInput.Value < 0)
                DelayInput.Value = 0;
            DelayValueTextbox.Text = DelayInput.Value.ToString() + " Milliseconds";
            Settings.Delay = (int)DelayInput.Value;
        }

        private void OptionsUI_Window_Initialized(object sender, EventArgs e)
        {
            DelayInput.Value = Settings.Delay;
            DelayValueTextbox.Text = DelayInput.Value.ToString() + " Milliseconds";
            SizeBox.Text = PixelGridUI.GridSize.ToString();
            if (Settings.Running)
            {
                SizeBox.IsEnabled = false;
                SetButton.IsEnabled = false;
            }

        }

        private void InfoButton_Click(object sender, RoutedEventArgs e)
        {
            InfoUI NewUI = new InfoUI();
            NewUI.Show();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            EditorUI.PixelGrid.RunWorkerAsync();

        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            EditorUI.Instance.EditorTextBox.Text = Settings.DefaultProgram;
        }

        private void ParseSizeBox()
        {
            int ParsedInt = PixelGridUI.GridSize;
            Int32.TryParse(Regex.Replace(SizeBox.Text, "[^0-9]", ""), out ParsedInt);
            if (ParsedInt == 0)
                ParsedInt = 40;
            if (ParsedInt > 400)
                ParsedInt = 400;
            if (ParsedInt < 2)
                ParsedInt = 2;
            while ((400.0 / ParsedInt) != (int)(400 / ParsedInt))
            {
                if (ParsedInt > 40)
                {
                    ParsedInt++;
                }
                if (ParsedInt < 40)
                {
                    ParsedInt--;
                }
            }
            SizeBox.Text = ParsedInt.ToString();
            Properties.Settings.Default.GridSize = ParsedInt;
            Properties.Settings.Default.Save();
            if (!PixelGridUI.FormOpen)
            {
                PixelGridUI.GridSize = ParsedInt;
            }
        }
        private void SizeBox_LostFocus(object sender, RoutedEventArgs e)
        {
            ParseSizeBox();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (!PixelGridUI.FormOpen)
            {
                EditorUI.PixelGrid.RunWorkerAsync();
            }
        }

        private void SetButton_Click(object sender, RoutedEventArgs e)
        {
            ParseSizeBox();
        }
    }
}
