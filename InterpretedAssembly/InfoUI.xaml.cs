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
    /// Interaction logic for Info.xaml
    /// </summary>
    public partial class InfoUI : Window
    {
        public InfoUI()
        {
            InitializeComponent();
        }
        private static new Dictionary<string, string> DefaultText = new Dictionary<string, string>();
        private static bool initialized = false;
        private void Window_Initialized(object sender, EventArgs e)
        {
            if (initialized)
                return;
            foreach (System.Windows.Controls.TabControl Control in TabGrid.Children)
            {
                System.Windows.Controls.TabControl TabControl_ = (System.Windows.Controls.TabControl)Control;
                foreach (System.Windows.Controls.TabItem Tab in TabControl_.Items)
                {
                    System.Windows.Controls.TextBox TextBox_ = (System.Windows.Controls.TextBox)Tab.Content;
                    TextBox_.Focusable = false;
                    TextBox_.TextChanged += new TextChangedEventHandler(TextChanged);
                    DefaultText.Add(TextBox_.Name, TextBox_.Text);
                }
            }
            initialized = true;
        }
        private void TextChanged(object sender, TextChangedEventArgs e)
        {
            System.Windows.Controls.TextBox SrcBox = ((System.Windows.Controls.TextBox)(e.Source));
            string DefaultString = "";
            DefaultText.TryGetValue(SrcBox.Name, out DefaultString);
            SrcBox.Text = DefaultString;
        }
    }
}
