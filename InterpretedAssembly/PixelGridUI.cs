using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Reflection;
using SlimDX;
using SlimDX.Direct3D9;
using SlimDX.Direct2D;
using SlimDX.Windows;
namespace InterpretedAssembly
{
    class PixelGridUI
    {
        static public bool FormOpen = false;

        static public int GridSize = 40;
        static public int[,,] Grid = new int[40,40,3];

        static private RenderForm GridForm;
        static private Device RenderDevice;
        static private Sprite SpriteDrawer;
        static public void PixelGridClosed(object sender, FormClosedEventArgs e)
        {
            FormOpen = false;
            Grid = new int[GridSize,GridSize,3];
        }
        static public void NewGrid()
        {
            Grid = new int[GridSize, GridSize, 3];
        }
        static public void SetPixelHSV(int x, int y, int h, double s, double v)
        {
            double[] RGB = { 0.0, 0.0, 0.0 };
            s = s / 100.0;
            v = v / 100.0;
            double H_ = h;
            double S_ = s;
            double V_ = v;
            double P, Q, T, fract = 0;
            if (H_ == 360.0)
                H_ = 0.0;
            else
                H_ /= 60.0;
            fract = H_ - Math.Floor(H_);
            P = V_ * (1.0 - S_);
            Q = V_ * (1.0 - S_ * fract);
            T = V_ * (1.0 - S_ * (1.0 - fract));
            if (0.0 <= H_ && H_ < 1.0)
                RGB = new double[] { v, T, P };
            else if (1.0 <= H_ && H_ < 2.0)
                RGB = new double[] { Q, V_, P };
            else if (2.0 <= h && H_ < 3.0)
                RGB = new double[] { P, V_, T };
            else if (3.0 <= H_ && H_ < 4.0)
                RGB = new double[] { P, Q, V_ };
            else if (4.0 <= H_ && H_ < 5.0)
                RGB = new double[] { T, P, V_ };
            else if (5.0 <= H_ && H_ < 6.0)
                RGB = new double[] { V_, P, Q };
            else
                RGB = new double[] { 0.0, 0.0, 0.0 };
            RGB[0] = RGB[0] * 255;
            RGB[1] = RGB[1] * 255;
            RGB[2] = RGB[2] * 255;
            Grid[x, y, 0] = (int)(RGB[0]);
            Grid[x, y, 1] = (int)(RGB[1]);
            Grid[x, y, 2] = (int)(RGB[2]);
        }
        static public void SetPixelRGB(int x, int y, int r, int g, int b)
        {
            Grid[x, y, 0] = r;
            Grid[x, y, 1] = g;
            Grid[x, y, 2] = b;
        }
        static public void SetPixelBW(int x, int y, int val)
        {
            Grid[x, y, 0] = val * 255;
            Grid[x, y, 1] = val * 255;
            Grid[x, y, 2] = val * 255;
        }
        static public void ClearGrid()
        {
            Array.Clear(Grid, 0, Grid.Length);
        }
        static public void InitPixelGrid()
        {
            
            GridSize = Properties.Settings.Default.GridSize;
            if(Grid.LongLength != GridSize * GridSize * 3)
                Grid = new int[GridSize, GridSize, 3];
            GridForm = new RenderForm("Pixel Grid"); ;
            GridForm.Icon = Properties.Resources.IA;
            int RectSize = 400 / GridSize;
            GridForm.FormClosed += new FormClosedEventHandler(PixelGridClosed);
            GridForm.ClientSize = new Size(400, 400);
            GridForm.MinimumSize = GridForm.Size;
            RenderDevice = null;
            RenderDevice = new Device(new Direct3D(), 0, DeviceType.Hardware, GridForm.Handle, CreateFlags.HardwareVertexProcessing, new PresentParameters()
            {
                BackBufferWidth = GridForm.ClientSize.Width,
                BackBufferHeight = GridForm.ClientSize.Height,
                Windowed = true,
                PresentationInterval = PresentInterval.One,
            });
            SpriteDrawer = new Sprite(RenderDevice);
            Texture Square = Texture.FromMemory(RenderDevice, 
                (byte[])(new ImageConverter()).ConvertTo(Properties.Resources.rectangle, typeof(byte[])),
                RectSize, RectSize, 0, Usage.None, Format.A8R8G8B8, Pool.Default, SlimDX.Direct3D9.Filter.Point, SlimDX.Direct3D9.Filter.Point, 1);
            FormOpen = true;
            MessagePump.Run(GridForm, () =>
            {
                if (MessagePump.IsApplicationIdle)
                {
                    RenderDevice.Clear(ClearFlags.Target, Color.Black, 1.0f, 0);
                    RenderDevice.BeginScene();
                    SpriteDrawer.Begin(SpriteFlags.None);
                    for (int x = 0; x < GridSize; x++)
                    {
                        for (int y = 0; y < GridSize; y++)
                        {
                            if(Grid[x, y, 0] + Grid[x, y, 1] + Grid[x, y, 2] != 0)
                            {
                                SpriteDrawer.Draw(Square, new Vector3(0, 0, 0), new Vector3(x * RectSize, y * RectSize, 0), Color.FromArgb(255, Grid[x, y, 0], Grid[x, y, 1], Grid[x, y, 2]));
                            }
                        }
                    }
                    SpriteDrawer.End();
                    RenderDevice.EndScene();
                    RenderDevice.Present();
                }
            }
            );
        }
    }
}
