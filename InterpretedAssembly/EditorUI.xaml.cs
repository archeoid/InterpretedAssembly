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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Xml;
using System.IO;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Highlighting;
namespace InterpretedAssembly
{
    public class Settings
    {
        public static string InputPopUpInfo = "";
        public static string InputPopUpReturn = "";
        public static bool LabelInputWait = false;
        public static int Delay = 100;
        public static bool Running = false;
        public static bool CanReset = false;
        public static bool ProgramChanged = true;
        public static string IterationType = "Delay";
        public static int RunFunction = 1;
        public static string DefaultProgram = "MOV 1 R1\nMOV 1 R2\n:LOOP\nMOV R1 R3\nADD R2 R3\nPRT R3\nMOV R2 R1\nMOV R3 R2\nLES R3 255\n+JMP LOOP\n-JMP END\n:END";
        public static Dictionary<string, int> RunFunctionMap = new Dictionary<string, int>()
        {
            { "Disabled", 0},
            { "Run", 1},
            { "Reset", 2},
            { "Stop", 3},
            { "Step", 4}
        };
        public static int SecondFunction = 1;
        public static Dictionary<string, int> SecondFunctionMap = new Dictionary<string, int>()
        {
            { "Disabled", 0},
            { "Options", 1},
            { "Reset", 2},
        };
        public static string BreakLabel = "";
        public static SolidColorBrush BorderBrush = new SolidColorBrush(Color.FromRgb(112, 112, 112));
        public static SolidColorBrush DisabledBrush = new SolidColorBrush(Color.FromRgb(25, 25, 25));
        public static SolidColorBrush EnabledBrush = new SolidColorBrush(Colors.Transparent);

    }
    public class DataObject
    {
        public string Name { get; set; }
        public int Val { get; set; }
    }
    public partial class EditorUI : Window
    {
        public static EditorUI Instance { get; private set; }
        public static LineHighlightRenderer LineHighlighter { get; private set; }
        private readonly BackgroundWorker Iterator = new BackgroundWorker();
        private readonly BackgroundWorker GetInput = new BackgroundWorker();
        private readonly BackgroundWorker RunFull = new BackgroundWorker();
        private readonly BackgroundWorker RunFullUpdate = new BackgroundWorker();
        static public BackgroundWorker PixelGrid = new BackgroundWorker();

        public EditorUI()
        {
            InitializeComponent();
            Instance = this;
        }
        public void SetRunToRun()
        {
            Settings.RunFunction = Settings.RunFunctionMap["Run"];
            RunButton.Content = "Run";
            RunButton.BorderBrush = Settings.BorderBrush;
            RunButton.IsEnabled = true;
        }
        public void SetRunToStop()
        {
            Settings.RunFunction = Settings.RunFunctionMap["Stop"];
            RunButton.Content = "Stop";
            RunButton.BorderBrush = new SolidColorBrush(Colors.OrangeRed);
            RunButton.IsEnabled = true;
        }
        public void SetRunToStep()
        {
            Settings.RunFunction = Settings.RunFunctionMap["Step"];
            RunButton.Content = "Step";
            RunButton.BorderBrush = Settings.BorderBrush;
            RunButton.IsEnabled = true;
        }
        public void SetRunToDisabled()
        {
            Settings.RunFunction = Settings.RunFunctionMap["Disabled"];
            RunButton.Content = "Run";
            RunButton.BorderBrush = Settings.BorderBrush;
            RunButton.IsEnabled = false;
        }
        public void SetRunToReset()
        {
            Settings.RunFunction = Settings.RunFunctionMap["Reset"];
            RunButton.Content = "Reset";
            RunButton.BorderBrush = new SolidColorBrush(Colors.Yellow);
            RunButton.IsEnabled = true;
        }
        public void SetOptionsToOptions()
        {
            Settings.SecondFunction = Settings.SecondFunctionMap["Options"];
            SecondButton.Content = "More Options";
            SecondButton.BorderBrush = Settings.BorderBrush;
            RunButton.IsEnabled = true;
        }
        public void SetOptionsToReset()
        {
            Settings.SecondFunction = Settings.SecondFunctionMap["Reset"];
            SecondButton.Content = "Reset";
            SecondButton.BorderBrush = new SolidColorBrush(Colors.Yellow);
            SecondButton.IsEnabled = true;
        }
        public void SetOptionsToDisabled()
        {
            Settings.SecondFunction = Settings.SecondFunctionMap["Disabled"];
            SecondButton.Content = "More Options";
            SecondButton.BorderBrush = Settings.BorderBrush;
            RunButton.IsEnabled = false;
        }
        public void DisableEdit()
        {
            EditorTextBox.IsEnabled = true;
            ForwardOnce.IsEnabled = false;
            IterateDelay.IsEnabled = false;
            ForwardEnd.IsEnabled = false;
        }
        public void EnableEdit()
        {
            EditorTextBox.IsEnabled = true;
            ForwardOnce.IsEnabled = true;
            IterateDelay.IsEnabled = true;
            ForwardEnd.IsEnabled = true;
        }
        public void RunProgram()
        {
            if (!Settings.Running)
            {
                Interpret.ProgramInput(EditorTextBox.Text);
                DisableEdit();
            }
            if (Settings.IterationType == "Delay")
            {
                Settings.CanReset = true;
                Settings.Running = true;
                SetRunToStop();
                Iterator.RunWorkerAsync();
            }
            if (Settings.IterationType == "Once")
            {
                Settings.CanReset = true;
                Settings.Running = true;
                SetRunToStep();
                SetOptionsToReset();
                Settings.RunFunction = Settings.RunFunctionMap["Step"];
                Interpret.IterateOnce();
            }
            if (Settings.IterationType == "End")
            {
                Settings.CanReset = true;
                Settings.Running = true;
                SetRunToStop();
                RunFull.RunWorkerAsync();
                RunFullUpdate.RunWorkerAsync();
                RunButton.Background = new SolidColorBrush();
            }
            if (Settings.IterationType == "Label")
            {
                Interpret.Break.BreakLabel = Settings.BreakLabel;
                Interpret.Break.BreakOnLabel = true;
                Settings.CanReset = true;
                Settings.Running = true;
                SetRunToStop();
                RunFull.RunWorkerAsync();
                RunFullUpdate.RunWorkerAsync();
                RunButton.Background = new SolidColorBrush();
            }
        }
        public bool CanRun()
        {
            bool CanRunBool = true;

            if (Settings.Running)
                CanRunBool = false;

            return CanRunBool;
        }
        public void AttemptStop()
        {
            Settings.Running = false;
            if(Settings.IterationType == "End" || Settings.IterationType == "Label")
            {
                Interpret.Break.ErrorBreak = true;
            }
        }
        public void ResetProgram()
        {
            int test = new int();
            AttemptStop();
            ErrorTextBox.Clear();
            OutputTextBox.Clear();
            Interpret.Reset();
            Settings.CanReset = false;
            SetRunToRun();
            SetOptionsToOptions();
            EnableEdit();
            PixelGridUI.NewGrid();
        }
        private void UIWindow_Initialized(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.LastEdit == "")
            {
                EditorTextBox.AppendText("MOV 1 R1\nMOV 1 R2\n:LOOP\nMOV R1 R3\nADD R2 R3\nPRT R3\nMOV R2 R1\nMOV R3 R2\nLES R3 255\n+JMP LOOP\n-JMP END\n:END");
            } else
            {
                EditorTextBox.AppendText(Properties.Settings.Default.LastEdit);
            }
            PixelGridUI.GridSize = Properties.Settings.Default.GridSize;
            EditorTextBox.ShowLineNumbers = true;
            using (Stream s = new MemoryStream(Properties.Resources.SyntaxRules))
            {
                using (XmlTextReader reader = new XmlTextReader(s))
                {
                    EditorTextBox.SyntaxHighlighting = ICSharpCode.AvalonEdit.Highlighting.Xshd.HighlightingLoader.Load(reader, HighlightingManager.Instance);
                    LineHighlighter = new LineHighlightRenderer();
                    EditorTextBox.TextArea.TextView.BackgroundRenderers.Add(LineHighlighter);
                }
            }
            Iterator.DoWork += IteratorWait;
            Iterator.RunWorkerCompleted += IteratorDone;
            GetInput.DoWork += GetInputWait;
            GetInput.RunWorkerCompleted += GetInputDone;
            RunFull.DoWork += RunFullWait;
            RunFull.RunWorkerCompleted += RunFullDone;
            RunFullUpdate.DoWork += RunFullUpdateWait;
            RunFullUpdate.RunWorkerCompleted += RunFullUpdateDone;
            PixelGrid.DoWork += PixelGridWait;
        }

        private void RunButton_Click(object sender, RoutedEventArgs e)
        {

            if (Settings.RunFunction == Settings.RunFunctionMap["Run"])
            {
                if (CanRun())
                {
                    RunProgram();
                    return;
                }
            }
            if (Settings.RunFunction == Settings.RunFunctionMap["Step"])
            {
                if(Settings.Running && Settings.IterationType == "Once")
                {
                    Interpret.IterateOnce();
                    return;
                }
            }
            if (Settings.RunFunction == Settings.RunFunctionMap["Stop"])
            {
                AttemptStop();
                return;
            }
            if (Settings.RunFunction == Settings.RunFunctionMap["Reset"])
            {
                ResetProgram();
                return;
            }
        }
        private void RunFullWait(object sender, DoWorkEventArgs e)
        {
            Interpret.FullRun();
        }
        private void RunFullDone(object sender, RunWorkerCompletedEventArgs e)
        {
            Settings.Running = false;
            ErrorTextBox.Text = Interpret.Output.RunFullErrorOutput;
            OutputTextBox.Text = Interpret.Output.RunFullOutput;
            OutputTextBox.ScrollToEnd();
            Interpret.UpdateRegisterUI();
            SetRunToReset();
            SetOptionsToOptions();
        }
        private void PixelGridWait(object sender, DoWorkEventArgs e)
        {
            PixelGridUI.InitPixelGrid();
        }
        private void RunFullUpdateWait(object sender, DoWorkEventArgs e)
        {
            Thread.Sleep(200);
        }
        private void RunFullUpdateDone(object sender, RunWorkerCompletedEventArgs e)
        {
            if (Settings.Running)
            {
                ErrorTextBox.Text = Interpret.Output.RunFullErrorOutput;
                OutputTextBox.Text = Interpret.Output.RunFullOutput;
                OutputTextBox.ScrollToEnd();
                Interpret.UpdateRegisterUI();
                RunFullUpdate.RunWorkerAsync();
            }
        }
        private void IteratorWait(object sender, DoWorkEventArgs e)
        {
            if (Settings.Running)
            {
                Thread.Sleep(Settings.Delay);
            } else
            {
                Thread.Sleep(10);
            }
        }
        private void IteratorDone(object sender, RunWorkerCompletedEventArgs e)
        {
            if (Settings.IterationType == "Delay")
            {
                if (Settings.Running)
                {
                    Interpret.IterateOnce();
                    Iterator.RunWorkerAsync();
                } else
                {
                    SetRunToReset();
                    SetOptionsToOptions();
                }
            }            
        }
        private void GetInputWait(object sender, DoWorkEventArgs e)
        {
            while(Settings.InputPopUpReturn == "")
            {
                Thread.Sleep(100);
            }
        }
        private void GetInputDone(object sender, RunWorkerCompletedEventArgs e)
        {
            Settings.LabelInputWait = false;
            Settings.BreakLabel = Settings.InputPopUpReturn;
            Settings.InputPopUpReturn = "";
            SetRunToRun();
        }
        private void SecondButton_Click(object sender, RoutedEventArgs e)
        {
            if(Settings.SecondFunction == Settings.SecondFunctionMap["Reset"])
            {
                ResetProgram();
                return;
            }
            if (Settings.SecondFunction == Settings.SecondFunctionMap["Options"])
            {
                OptionsUI NewUI = new OptionsUI();
                NewUI.Show();
            }
        }

        private void ForwardOnce_Checked(object sender, RoutedEventArgs e)
        {
            Settings.IterationType = "Once";
        }

        private void IterateDelay_Checked(object sender, RoutedEventArgs e)
        {
            Settings.IterationType = "Delay";
        }

        private void ForwardEnd_Checked(object sender, RoutedEventArgs e)
        {
            Settings.IterationType = "End";
        }

        private void ForwardLabel_Checked(object sender, RoutedEventArgs e)
        {

            Settings.IterationType = "Label";
            Settings.InputPopUpInfo = "Input Label Name:";
            SetRunToDisabled();
            GetInput.RunWorkerAsync();
            InputPopUp NewUI = new InputPopUp();
            NewUI.Show();
        }

        private void UIWindow_Closing(object sender, CancelEventArgs e)
        {
            Properties.Settings.Default.LastEdit = EditorTextBox.Text;
            Properties.Settings.Default.Save();
            Environment.Exit(0);
        }

        private void EditorTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            EditorTextBox.Text = EditorTextBox.Text.ToUpper();
            Properties.Settings.Default.LastEdit = EditorTextBox.Text;
            Properties.Settings.Default.Save();
        }
    }
}
