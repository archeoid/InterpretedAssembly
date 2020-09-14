using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows.Documents;
namespace InterpretedAssembly
{
    public class Interpret
    {
        static private Dictionary<string, int> ReturnCodes = new Dictionary<string, int>()
                {
                { "All OK", 0 },
                { "General Error", -1 },
                { "Hit Label", 2 },
                { "Logic Stack: -", 3 },
                { "Logic Stack: +", 4 },
                { "Label Line", 5 },
                { "Comment Line", 6 }
        };
        public class Break
        {

            static public string BreakLabel = "";
            static public bool BreakOnLabel = false;
            static public bool ErrorBreak = false;
            static public int BreakInstructions = 1000000000;
            static public int MaxBreakInstructions = 1000000000;
        }
        public class Program
        {
            static public bool RunFull = false;
            static public string[] Lines = new string[] { };
            static public int MaxLines = 0;
            static public Dictionary<string, int> LabelMap = new Dictionary<string, int>();
            static public string LabelString = "";
        }
        public class Simulation
        {
            static public Random RandomGen = new Random();
            static public List<bool> LogicStack = new List<bool>();
            static public Queue<int> ReturnJumpLine = new Queue<int>();
            static public int CurrentLine = 0;
            static public int[] Registers = new int[100];
            static public int RegisterChanged = -1;
        }
        public class Output
        {
            static public string RunFullOutput = "";
            static public string RunFullErrorOutput = "";
            static public string ErrorMSG = "";
        }
        public class UI
        {

            static public SolidColorBrush WhiteBrush = new SolidColorBrush(Colors.DarkGray);
            static public SolidColorBrush ColorBrush = new SolidColorBrush(Colors.WhiteSmoke);
            static public SolidColorBrush DefaultBrush = new SolidColorBrush(Colors.White);
        }
        private class Instructions : Interpret
        {
            static public void ToManyArguments()
            {
                Output.ErrorMSG = "Line " + (Simulation.CurrentLine + 1) + ": Too many arguments for this instruction";
                Functions.ErrorPrint(Output.ErrorMSG);
                Break.ErrorBreak = true;
            }
            static public int Absolute(string[] input)
            {
                if (input.Count() != 1)
                {
                    ToManyArguments();
                    return ReturnCodes["General Error"];
                }
                int DEST = Functions.ParseArgumentToIndex(input[0]);
                if (Break.ErrorBreak)
                    return -1;
                int DEST_VALUE = Functions.RetriveRegister(DEST);
                if (Break.ErrorBreak)
                    return -1;
                Functions.AssignRegister(DEST, Math.Abs(DEST_VALUE));
                return 0;
            }
            static public int Add(string[] input)
            {
                if (input.Count() != 2)
                {
                    ToManyArguments();
                    return ReturnCodes["General Error"];
                }
                int SRC = Functions.ParseArgumentToValue(input[0]);
                int DEST = Functions.ParseArgumentToIndex(input[1]);
                if (Break.ErrorBreak)
                    return -1;
                int DEST_VALUE = Functions.RetriveRegister(DEST);
                if (Break.ErrorBreak)
                    return -1;
                Functions.AssignRegister(DEST, DEST_VALUE + SRC);
                return 0;
            }
            static public int Subtract(string[] input)
            {
                if (input.Count() != 2)
                {
                    ToManyArguments();
                    return ReturnCodes["General Error"];
                }
                int SRC = Functions.ParseArgumentToValue(input[0]);
                int DEST = Functions.ParseArgumentToIndex(input[1]);
                if (Break.ErrorBreak)
                    return -1;
                int DEST_VALUE = Functions.RetriveRegister(DEST);
                if (Break.ErrorBreak)
                    return -1;
                Functions.AssignRegister(DEST, DEST_VALUE - SRC);
                return 0;
            }
            static public int Multiply(string[] input)
            {
                if (input.Count() != 2)
                {
                    ToManyArguments();
                    return ReturnCodes["General Error"];
                }
                int SRC = Functions.ParseArgumentToValue(input[0]);
                int DEST = Functions.ParseArgumentToIndex(input[1]);
                if (Break.ErrorBreak)
                    return -1;
                int DEST_VALUE = Functions.RetriveRegister(DEST);
                if (Break.ErrorBreak)
                    return -1;
                Functions.AssignRegister(DEST, DEST_VALUE * SRC);
                return 0;
            }
            static public int Divide(string[] input)
            {
                if (input.Count() != 2)
                {
                    ToManyArguments();
                    return ReturnCodes["General Error"];
                }
                int SRC = Functions.ParseArgumentToValue(input[0]);
                int DEST = Functions.ParseArgumentToIndex(input[1]);
                if (Break.ErrorBreak)
                    return -1;
                int DEST_VALUE = Functions.RetriveRegister(DEST);
                if (Break.ErrorBreak)
                    return -1;
                Functions.AssignRegister(DEST, (int)Math.Round((double)(DEST_VALUE / SRC)));
                return 0;
            }
            static public int Remainder(string[] input)
            {
                if (input.Count() != 2)
                {
                    ToManyArguments();
                    return ReturnCodes["General Error"];
                }
                int SRC = Functions.ParseArgumentToValue(input[0]);
                int DEST = Functions.ParseArgumentToIndex(input[1]);
                if (Break.ErrorBreak)
                    return -1;
                int DEST_VALUE = Functions.RetriveRegister(DEST);
                if (Break.ErrorBreak)
                    return -1;
                Functions.AssignRegister(DEST, SRC % DEST_VALUE);
                return 0;
            }
            static public int Increment(string[] input)
            {
                if (input.Count() != 1)
                {
                    ToManyArguments();
                    return ReturnCodes["General Error"];
                }
                int DEST = Functions.ParseArgumentToIndex(input[0]);
                if (Break.ErrorBreak)
                    return -1;
                int DEST_VALUE = Functions.RetriveRegister(DEST);
                if (Break.ErrorBreak)
                    return -1;
                Functions.AssignRegister(DEST, DEST_VALUE + 1);
                return 0;
            }
            static public int Decrement(string[] input)
            {
                if (input.Count() != 1)
                {
                    ToManyArguments();
                    return ReturnCodes["General Error"];
                }
                int DEST = Functions.ParseArgumentToIndex(input[0]);
                if (Break.ErrorBreak)
                    return -1;
                int DEST_VALUE = Functions.RetriveRegister(DEST);
                if (Break.ErrorBreak)
                    return -1;
                Functions.AssignRegister(DEST, DEST_VALUE - 1);
                return 0;
            }
            static public int Random(string[] input)
            {

                if (input.Count() != 3)
                {
                    ToManyArguments();
                    return ReturnCodes["General Error"];
                }
                int MIN = Functions.ParseArgumentToValue(input[0]);
                int MAX = Functions.ParseArgumentToValue(input[1]);
                int REG = Functions.ParseArgumentToIndex(input[2]);
                if (Break.ErrorBreak)
                    return -1;
                Functions.AssignRegister(REG, Simulation.RandomGen.Next(MIN, MAX));
                return 0;
            }
            static public int Sleep(string[] input)
            {
                if (input.Count() != 1)
                {
                    ToManyArguments();
                    return ReturnCodes["General Error"];
                }
                int DEST = Functions.ParseArgumentToIndex(input[0]);
                if (Break.ErrorBreak)
                    return -1;
                int DEST_VALUE = Functions.RetriveRegister(DEST);
                if (Break.ErrorBreak)
                    return -1;
                if (DEST >= 0)
                    Thread.Sleep(DEST_VALUE);
                return 0;
            }
            static public int Move(string[] input)
            {
                if (input.Count() != 2)
                {
                    ToManyArguments();
                    return ReturnCodes["General Error"];
                }
                int SRC = Functions.ParseArgumentToValue(input[0]);
                int DEST = Functions.ParseArgumentToIndex(input[1]);
                if (Break.ErrorBreak)
                    return -1;
                Functions.AssignRegister(DEST, SRC);
                return 0;
            }
            static public int Jump(string[] input)
            {
                if (input.Count() != 1)
                {
                    ToManyArguments();
                    return ReturnCodes["General Error"];
                }

                if (Program.LabelMap.ContainsKey(input[0]))
                    Simulation.CurrentLine = Program.LabelMap[input[0]];
                else
                {
                    Output.ErrorMSG = "Line " + (Simulation.CurrentLine + 1) + ": Label \"" + input[0] + "\" Cannot Be Found. " + Environment.NewLine + "Labels: " + Program.LabelString;
                    Functions.ErrorPrint(Output.ErrorMSG);
                    Break.ErrorBreak = true;
                    return -1;
                }
                return 0;
            }
            static public int ReturnJump(string[] input)
            {
                if (input.Count() == 0)
                {
                    if (Simulation.ReturnJumpLine.Count != 0)
                    {
                        Simulation.CurrentLine = Simulation.ReturnJumpLine.Dequeue();
                        return ReturnCodes["All OK"];
                    }
                    else
                    {
                        Output.ErrorMSG = "Line " + (Simulation.CurrentLine + 1) + ": No Labels on the Stack!";
                        Functions.ErrorPrint(Output.ErrorMSG);
                        Break.ErrorBreak = true;
                        return -1;
                    }
                }
                if (input.Count() == 1)
                {
                    if (Program.LabelMap.ContainsKey(input[0]))
                    {
                        Simulation.ReturnJumpLine.Enqueue(Simulation.CurrentLine);
                        Simulation.CurrentLine = Program.LabelMap[input[0]];
                        return ReturnCodes["All OK"];
                    }
                    else
                    {
                        Output.ErrorMSG = "Line " + (Simulation.CurrentLine + 1) + ": Label \"" + input[0] + "\" Cannot Be Found. " + Environment.NewLine + "Labels: " + Program.LabelString;
                        Functions.ErrorPrint(Output.ErrorMSG);
                        Break.ErrorBreak = true;
                        return -1;
                    }
                }
                ToManyArguments();
                return ReturnCodes["General Error"];
            }
            static public int Print(string[] input)
            {
                if (input.Count() == 1)
                {
                    int value = Functions.ParseArgumentToValue(input[0]);
                    if (!Program.RunFull)
                    {
                        EditorUI.Instance.OutputTextBox.AppendText(value.ToString() + "\n");
                        EditorUI.Instance.OutputTextBox.ScrollToEnd();
                    }
                    else
                    {
                        Output.RunFullOutput += value.ToString() + "\n";
                    }
                }
                else
                {
                    ToManyArguments();
                    return ReturnCodes["General Error"];
                }
                return 0;
            }
            static public int Less(string[] input)
            {
                if (input.Count() == 2)
                {
                    int VAL1 = Functions.ParseArgumentToValue(input[0]);
                    int VAL2 = Functions.ParseArgumentToValue(input[1]);
                    Simulation.LogicStack.Add(VAL1 < VAL2);
                }
                else
                {
                    ToManyArguments();
                    return ReturnCodes["General Error"];
                }
                return 1;
            }
            static public int Greater(string[] input)
            {
                if (input.Count() == 2)
                {
                    int VAL1 = Functions.ParseArgumentToValue(input[0]);
                    int VAL2 = Functions.ParseArgumentToValue(input[1]);
                    Simulation.LogicStack.Add(VAL1 > VAL2);
                }
                else
                {
                    ToManyArguments();
                    return ReturnCodes["General Error"];
                }
                return 1;
            }
            static public int LessEql(string[] input)
            {
                if (input.Count() == 2)
                {
                    int VAL1 = Functions.ParseArgumentToValue(input[0]);
                    int VAL2 = Functions.ParseArgumentToValue(input[1]);
                    Simulation.LogicStack.Add(VAL1 <= VAL2);
                }
                else
                {
                    ToManyArguments();
                    return ReturnCodes["General Error"];
                }
                return 1;
            }
            static public int GreaterEql(string[] input)
            {
                if (input.Count() == 2)
                {
                    int VAL1 = Functions.ParseArgumentToValue(input[0]);
                    int VAL2 = Functions.ParseArgumentToValue(input[1]);
                    Simulation.LogicStack.Add(VAL1 >= VAL2);
                }
                else
                {
                    ToManyArguments();
                    return ReturnCodes["General Error"];
                }
                return 1;
            }
            static public int Equal(string[] input)
            {
                if (input.Count() == 2)
                {
                    int VAL1 = Functions.ParseArgumentToValue(input[0]);
                    int VAL2 = Functions.ParseArgumentToValue(input[1]);
                    Simulation.LogicStack.Add(VAL1 == VAL2);
                }
                else
                {
                    ToManyArguments();
                    return ReturnCodes["General Error"];
                }
                return 1;
            }
            static public int NotEqual(string[] input)
            {
                if (input.Count() == 2)
                {
                    int VAL1 = Functions.ParseArgumentToValue(input[0]);
                    int VAL2 = Functions.ParseArgumentToValue(input[1]);
                    Simulation.LogicStack.Add(VAL1 != VAL2);
                }
                else
                {
                    ToManyArguments();
                    return ReturnCodes["General Error"];
                }
                return 1;
            }
            static public int SetPixel(string[] input)
            {
                if (input.Count() == 3)
                {
                    if (!PixelGridUI.FormOpen && !EditorUI.PixelGrid.IsBusy)
                    {
                        EditorUI.PixelGrid.RunWorkerAsync();
                        while (!PixelGridUI.FormOpen)
                        {
                            Thread.Sleep(0);
                        }
                    }
                    int X = Functions.ParseArgumentToValue(input[0]);
                    int Y = Functions.ParseArgumentToValue(input[1]);
                    int VAL = Functions.ParseArgumentToValue(input[2]);
                    if (X >= 0 && X <= PixelGridUI.GridSize - 1 && Y >= 0 && Y <= PixelGridUI.GridSize - 1)
                    {
                        if (VAL <= 0)
                        {
                            PixelGridUI.SetPixelBW(X, Y, 0);
                        }
                        else
                        {
                            PixelGridUI.SetPixelBW(X, Y, 1);
                        }
                    }
                }
                else
                {
                    ToManyArguments();
                    return ReturnCodes["General Error"];
                }
                return 1;
            }
            static public int SetHSV(string[] input)
            {
                if (input.Count() == 5)
                {
                    if (!PixelGridUI.FormOpen && !EditorUI.PixelGrid.IsBusy)
                    {
                        EditorUI.PixelGrid.RunWorkerAsync();
                        while (!PixelGridUI.FormOpen)
                        {
                            Thread.Sleep(0);
                        }
                    }
                    int X = Functions.ParseArgumentToValue(input[0]);
                    int Y = Functions.ParseArgumentToValue(input[1]);
                    int H = Functions.ParseArgumentToValue(input[2]);
                    int S = Functions.ParseArgumentToValue(input[3]);
                    int V = Functions.ParseArgumentToValue(input[4]);
                    if (X >= 0 && X <= PixelGridUI.GridSize - 1 && Y >= 0 && Y <= PixelGridUI.GridSize - 1)
                    {
                        while (H >= 360)
                        {
                            H -= 360;
                        }
                        while (H < 0)
                        {
                            H += 360;
                        }
                        if (S <= 100 && S >= 0 && V <= 100 && V >= 0)
                        {
                            PixelGridUI.SetPixelHSV(X, Y, H, S, V);
                        }
                        else
                        {
                            Output.ErrorMSG = "Line " + (Simulation.CurrentLine + 1) + ": Out of bounds of Color Space";
                            Functions.ErrorPrint(Output.ErrorMSG);
                            Break.ErrorBreak = true;
                        }
                    }
                }
                else
                {
                    ToManyArguments();
                    return ReturnCodes["General Error"];
                }
                return 1;
            }
            static public int SetRGB(string[] input)
            {
                if (input.Count() == 5)
                {
                    if (!PixelGridUI.FormOpen && !EditorUI.PixelGrid.IsBusy)
                    {
                        EditorUI.PixelGrid.RunWorkerAsync();
                        while (!PixelGridUI.FormOpen)
                        {
                            Thread.Sleep(0);
                        }
                    }
                    int X = Functions.ParseArgumentToValue(input[0]);
                    int Y = Functions.ParseArgumentToValue(input[1]);
                    int R = Functions.ParseArgumentToValue(input[2]);
                    int G = Functions.ParseArgumentToValue(input[3]);
                    int B = Functions.ParseArgumentToValue(input[4]);
                    if (X >= 0 && X <= PixelGridUI.GridSize - 1 && Y >= 0 && Y <= PixelGridUI.GridSize - 1)
                    {
                        if (R <= 255 && R >= 0 &&
                            G <= 255 && G >= 0 &&
                            B <= 255 && B >= 0)
                        {
                            PixelGridUI.SetPixelRGB(X, Y, R, G, B);
                        } else
                        {
                            Output.ErrorMSG = "Line " + (Simulation.CurrentLine + 1) + ": Out of bounds of Color Space";
                            Functions.ErrorPrint(Output.ErrorMSG);
                            Break.ErrorBreak = true;
                        }
                    }
                }
                else
                {
                    ToManyArguments();
                    return ReturnCodes["General Error"];
                }
                return 1;
            }
            static public int SetHue(string[] input)
            {
                if (input.Count() == 3)
                {
                    if (!PixelGridUI.FormOpen && !EditorUI.PixelGrid.IsBusy)
                    {
                        EditorUI.PixelGrid.RunWorkerAsync();
                        while (!PixelGridUI.FormOpen)
                        {
                            Thread.Sleep(0);
                        }
                    }
                    int X = Functions.ParseArgumentToValue(input[0]);
                    int Y = Functions.ParseArgumentToValue(input[1]);
                    int H = Functions.ParseArgumentToValue(input[2]);
                    if (X >= 0 && X <= PixelGridUI.GridSize - 1 && Y >= 0 && Y <= PixelGridUI.GridSize - 1)
                    {
                        while (H >= 360)
                        {
                            H -= 360;
                        }
                        while (H < 0)
                        {
                            H += 360;
                        }
                        PixelGridUI.SetPixelHSV(X, Y, H, 100, 100);
                    }
                }
                else
                {
                    ToManyArguments();
                    return ReturnCodes["General Error"];
                }
                return 1;
            }
            static public int ClearPixel(string[] input)
            {
                if (input.Count() == 0)
                {
                    if (!PixelGridUI.FormOpen)
                    {
                        EditorUI.PixelGrid.RunWorkerAsync();
                    }
                    PixelGridUI.NewGrid();
                }
                else
                {
                    ToManyArguments();
                    return ReturnCodes["General Error"];
                }
                return 1;
            }
            static public int End(string[] input)
            {
                if (input.Count() == 0)
                    Simulation.LogicStack.RemoveAt(Simulation.LogicStack.Count - 1);
                else
                {
                    ToManyArguments();
                    return ReturnCodes["General Error"];
                }
                return 1;
            }
        }
        static private Dictionary<string, Func<string[], int>> InstructionMap = new Dictionary<string, Func<string[], int>>()
        {
            { "ADD", Instructions.Add },
            { "SUB", Instructions.Subtract },
            { "MUL", Instructions.Multiply },
            { "DIV", Instructions.Divide },
            { "REM", Instructions.Remainder },
            { "INC", Instructions.Increment },
            { "DEC", Instructions.Decrement },
            { "ABS", Instructions.Absolute },
            { "RJP", Instructions.ReturnJump },
            { "MOV", Instructions.Move },
            { "SLP", Instructions.Sleep },
            { "RND", Instructions.Random },
            { "JMP", Instructions.Jump },
            { "PRT", Instructions.Print },
            { "LES", Instructions.Less },
            { "GTR", Instructions.Greater },
            { "LSE", Instructions.LessEql },
            { "GTE", Instructions.GreaterEql },
            { "EQL", Instructions.Equal },
            { "NEQ", Instructions.NotEqual },
            { "PXL", Instructions.SetPixel },
            { "RGB", Instructions.SetRGB },
            { "HSV", Instructions.SetHSV },
            { "HUE", Instructions.SetHue },
            { "CLR", Instructions.ClearPixel },
        };
        private class Functions : Interpret
        {
            static public void ErrorPrint(string error)
            {
                if (!Program.RunFull)
                {
                    EditorUI.Instance.ErrorTextBox.Text += Output.ErrorMSG + "\n";
                }
                else
                {
                    Output.RunFullErrorOutput += Output.ErrorMSG + "\n";
                }
                Settings.Running = false;
            }
            static public int ParseArgumentToValue(string input)
            {
                int Value = -1;
                if (input.Length >= 0)
                {
                    if (input[0] == 'R')
                    {
                        Value = ParseRegisterIndex(input);
                        Value = RetriveRegister(Value);
                        return Value;
                    }
                    if (input == "GRD")
                    {
                        return PixelGridUI.GridSize - 1;
                    }
                    return ParseInt(input);
                }
                Break.ErrorBreak = true;
                return Value;
            }
            static public int ParseArgumentToIndex(string input)
            {
                int Value = -1;
                if (input.Length >= 0)
                {
                    if (input[0] == 'R')
                    {
                        Value = ParseRegisterIndex(input);
                        Simulation.RegisterChanged = Value;
                        return Value;
                    }
                    else
                    {
                        Break.ErrorBreak = true;
                        return -1;
                    }
                }
                Break.ErrorBreak = true;
                return Value;
            }

            static public int ParseInt(string input)
            {
                int value = -1;
                if (Int32.TryParse(input, out value))
                {
                    return value;
                }
                Output.ErrorMSG = "Line " + (Simulation.CurrentLine + 1) + ": Integer could not be parsed from, " + input + " .";
                ErrorPrint(Output.ErrorMSG);
                Break.ErrorBreak = true;
                return -1;
            }
            static public int AssignRegister(int index, int value)
            {
                if (index >= 1 && index <= 100)
                {
                    Simulation.Registers[index - 1] = value;
                    return 0;
                }
                else
                {
                    Output.ErrorMSG = "Line " + (Simulation.CurrentLine + 1) + ": Register index out of range, " + index + " .";
                    ErrorPrint(Output.ErrorMSG);
                    Break.ErrorBreak = true;
                    return -1;
                }
            }
            static public int RetriveRegister(int index)
            {
                return Simulation.Registers[index - 1];
            }
            static public int ParseRegisterIndex(string input)
            {
                int index = -1;
                if (Int32.TryParse(input.Replace("R", ""), out index))
                {
                    if(index > 8)
                    {
                        return index;
                    }
                    return index;
                }
                else
                {
                    Output.ErrorMSG = "Line " + (Simulation.CurrentLine + 1) + ": Could not parse register, " + input + " .";
                    ErrorPrint(Output.ErrorMSG);
                    Break.ErrorBreak = true;
                    return -1;
                }
            }
            static public int ParseRegisterValue(string input)
            {
                return Simulation.Registers[ParseRegisterIndex(input) - 1];
            }
            static public int ParseLabels(string[] Lines)
            {
                Program.LabelString = "[ ";
                for (int c = 0; c < Lines.Count(); c++)
                {
                    if (Lines[c].ElementAtOrDefault(0) == ':')
                    {
                        bool RestAlpha = true;
                        foreach (char chr in Lines[c].Substring(1))
                        {
                            if (!Char.IsLetter(chr))
                            {
                                RestAlpha = false;
                            }
                        }
                        if (RestAlpha)
                        {
                            if (Program.LabelMap.ContainsKey(Lines[c].Substring(1)))
                            {
                                Output.ErrorMSG = "Line " + (c + 1) + ": Label with that name already exists.";
                                ErrorPrint(Output.ErrorMSG);
                                Break.ErrorBreak = true;
                                return -1;
                            }
                            Program.LabelMap.Add(Lines[c].Substring(1), c);
                            Program.LabelString += " " + Lines[c].Substring(1) + ",";
                        }
                    }
                }
                Program.LabelString = Program.LabelString.Remove(Program.LabelString.Length - 1);
                Program.LabelString += " ]";
                return 0;
            }
        }
        static public void UpdateRegisterUI()
        {
            if (Simulation.RegisterChanged != -1)
            {
                EditorUI.Instance.R1Value.Foreground = UI.WhiteBrush;
                EditorUI.Instance.R2Value.Foreground = UI.WhiteBrush;
                EditorUI.Instance.R3Value.Foreground = UI.WhiteBrush;
                EditorUI.Instance.R4Value.Foreground = UI.WhiteBrush;
                EditorUI.Instance.R5Value.Foreground = UI.WhiteBrush;
                EditorUI.Instance.R6Value.Foreground = UI.WhiteBrush;
                EditorUI.Instance.R7Value.Foreground = UI.WhiteBrush;
                EditorUI.Instance.R8Value.Foreground = UI.WhiteBrush;
            }
            if (Simulation.RegisterChanged == 1)
            {
                EditorUI.Instance.R1Value.Foreground = UI.ColorBrush;
            }
            if (Simulation.RegisterChanged == 2)
            {
                EditorUI.Instance.R2Value.Foreground = UI.ColorBrush;
            }
            if (Simulation.RegisterChanged == 3)
            {
                EditorUI.Instance.R3Value.Foreground = UI.ColorBrush;
            }
            if (Simulation.RegisterChanged == 4)
            {
                EditorUI.Instance.R4Value.Foreground = UI.ColorBrush;
            }
            if (Simulation.RegisterChanged == 5)
            {
                EditorUI.Instance.R5Value.Foreground = UI.ColorBrush;
            }
            if (Simulation.RegisterChanged == 6)
            {
                EditorUI.Instance.R6Value.Foreground = UI.ColorBrush;
            }
            if (Simulation.RegisterChanged == 7)
            {
                EditorUI.Instance.R7Value.Foreground = UI.ColorBrush;
            }
            if (Simulation.RegisterChanged == 8)
            {
                EditorUI.Instance.R8Value.Foreground = UI.ColorBrush;
            }
            EditorUI.Instance.R1Value.Content = "R1" + new String(' ', 21 - Simulation.Registers[0].ToString().Length) + Simulation.Registers[0].ToString();
            EditorUI.Instance.R2Value.Content = "R2" + new String(' ', 21 - Simulation.Registers[1].ToString().Length) + Simulation.Registers[1].ToString();
            EditorUI.Instance.R3Value.Content = "R3" + new String(' ', 21 - Simulation.Registers[2].ToString().Length) + Simulation.Registers[2].ToString();
            EditorUI.Instance.R4Value.Content = "R4" + new String(' ', 21 - Simulation.Registers[3].ToString().Length) + Simulation.Registers[3].ToString();
            EditorUI.Instance.R5Value.Content = "R5" + new String(' ', 21 - Simulation.Registers[4].ToString().Length) + Simulation.Registers[4].ToString();
            EditorUI.Instance.R6Value.Content = "R6" + new String(' ', 21 - Simulation.Registers[5].ToString().Length) + Simulation.Registers[5].ToString();
            EditorUI.Instance.R7Value.Content = "R7" + new String(' ', 21 - Simulation.Registers[6].ToString().Length) + Simulation.Registers[6].ToString();
            EditorUI.Instance.R8Value.Content = "R8" + new String(' ', 21 - Simulation.Registers[7].ToString().Length) + Simulation.Registers[7].ToString();
            EditorUI.Instance.LineNumber.Content = "Line" + new String(' ', 19 - Simulation.CurrentLine.ToString().Length) + Simulation.CurrentLine.ToString();
            EditorUI.Instance.InstructionsExcecuted.Content = "INS" + new String(' ', 20 - (Break.MaxBreakInstructions - Break.BreakInstructions).ToString().Length) + (Break.MaxBreakInstructions - Break.BreakInstructions).ToString();
        }
        static public void ProgramInput(string program)
        {
            Program.Lines = program.ToUpper().Split(Environment.NewLine.ToCharArray());
            Program.MaxLines = Program.Lines.Count();
            Functions.ParseLabels(Program.Lines);
        }
        static public void Reset()
        {
            Simulation.LogicStack = new List<bool>();
            Break.BreakInstructions = Break.MaxBreakInstructions;
            Program.MaxLines = 0;
            Simulation.CurrentLine = 0;
            Output.ErrorMSG = "";
            Break.ErrorBreak = false;
            Simulation.Registers = new int[100];
            Program.Lines = new string[] { };
            Program.LabelMap.Clear();
            Functions.UpdateRegisterUI();
            EditorUI.Instance.R1Value.Foreground = UI.DefaultBrush;
            EditorUI.Instance.R2Value.Foreground = UI.DefaultBrush;
            EditorUI.Instance.R3Value.Foreground = UI.DefaultBrush;
            EditorUI.Instance.R4Value.Foreground = UI.DefaultBrush;
            EditorUI.Instance.R5Value.Foreground = UI.DefaultBrush;
            EditorUI.Instance.R6Value.Foreground = UI.DefaultBrush;
            EditorUI.Instance.R7Value.Foreground = UI.DefaultBrush;
            EditorUI.Instance.R8Value.Foreground = UI.DefaultBrush;
            EditorUI.LineHighlighter.stopHighlight();
            EditorUI.Instance.EditorTextBox.TextArea.TextView.Redraw();
        }
        static public void FullRun()
        {
            Program.RunFull = true;
            Output.RunFullOutput = "";
            Output.RunFullErrorOutput = "";
            while (Simulation.CurrentLine != Program.MaxLines && Simulation.CurrentLine >= 0 && !Break.ErrorBreak && Break.BreakInstructions != 0)
            {
                if (RunLine(Program.Lines[Simulation.CurrentLine]) == ReturnCodes["Hit Label"])
                {
                    Output.ErrorMSG = "Hit Target Label";
                    Functions.ErrorPrint(Output.ErrorMSG);
                    break;
                }
                Simulation.CurrentLine++;
                Break.BreakInstructions--;
            }
            if (Break.BreakInstructions == 0)
            {

                Output.ErrorMSG = "Excecuted to Many Instructions! [ " + Break.MaxBreakInstructions.ToString() + " ]";
                Functions.ErrorPrint(Output.ErrorMSG);
                Break.ErrorBreak = true;
            }
            if (Simulation.CurrentLine == Program.MaxLines)
            {
                Output.ErrorMSG = "Line " + (Simulation.CurrentLine + 1) + ": Hit End Line";
                Functions.ErrorPrint(Output.ErrorMSG);
                Break.ErrorBreak = true;
            }
            Program.RunFull = false;
            Break.ErrorBreak = false;
            Break.BreakLabel = "";
            Break.BreakOnLabel = false;
        }
        static public void IterateOnce()
        {
            if (Simulation.CurrentLine != Program.MaxLines && Simulation.CurrentLine >= 0)
            {

                if (RunLine(Program.Lines[Simulation.CurrentLine]) == ReturnCodes["All OK"] && Settings.Delay > 10)
                {
                    EditorUI.LineHighlighter.setLine(Simulation.CurrentLine);
                    EditorUI.Instance.EditorTextBox.TextArea.TextView.Redraw();
                }
                Functions.UpdateRegisterUI();
                Simulation.CurrentLine++;
                Break.BreakInstructions--;
            }
            else
            {
                Settings.Running = false;
            }
            if (Simulation.CurrentLine == Program.MaxLines)
            {
                Output.ErrorMSG = "Line " + (Simulation.CurrentLine + 1) + ": Hit End Line";
                Functions.ErrorPrint(Output.ErrorMSG);
                Break.ErrorBreak = true;
            }
        }


        static private int RunLine(string input)
        {
            Simulation.RegisterChanged = -1;
            if (input.Length == 0)
                return -1;
           string[] LineWords = input.Split(' ');
            
            if (LineWords[0][0] == '/' && LineWords[0][1] == '/')
            {
                return ReturnCodes["Comment Line"];
            }
            if (LineWords[0][0] == ':')
            {
                if (Break.BreakOnLabel && LineWords[0].Substring(1) == Break.BreakLabel)
                {
                    return ReturnCodes["Hit Label"];
                }
                return ReturnCodes["Label Line"];
            }
            List<bool> LineStack = new List<bool>();
            int i = 0;
            while (i < LineWords[0].Length && (LineWords[0][i] == '+' || LineWords[0][i] == '-'))
            {
                if (LineWords[0][i] == '+')
                    LineStack.Add(true);
                if (LineWords[0][i] == '-')
                    LineStack.Add(false);
                i++;
            }
            if(Simulation.LogicStack.Count > LineStack.Count)
            {
                while (Simulation.LogicStack.Count > LineStack.Count)
                    Simulation.LogicStack.RemoveAt(Simulation.LogicStack.Count - 1);
            }
            if (LineStack.SequenceEqual(Simulation.LogicStack))
            {
                LineWords[0] = LineWords[0].Replace("+", "").Replace("-", "");
                if (LineWords[0].Length == 3)
                {
                    if (InstructionMap.ContainsKey(LineWords[0]))
                    {
                        InstructionMap[LineWords[0]](LineWords.Skip(1).ToArray());
                    }
                }
            }
            return ReturnCodes["All OK"];
        }
    }
}
