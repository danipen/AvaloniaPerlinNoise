using System;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;

namespace AvaloniaPerlinNoise
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            mTimer = new DispatcherTimer(
                TimeSpan.FromMilliseconds(50),
                DispatcherPriority.Render,
                OnTimerTick);

            mRenderPanel = new RenderPanel();
            Content = mRenderPanel;

            mTimer.Start();

            //this.Renderer.DrawFps = true;
        }

        private void OnTimerTick(object? sender, EventArgs e)
        {
            mRenderPanel.InvalidateVisual();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        class RenderPanel : UserControl
        {
            public override void Render(DrawingContext context)
            {
                base.Render(context);

                context.DrawRectangle(
                    Brushes.Black,
                    null,
                    new Rect(0, 0, Bounds.Width, Bounds.Height));

                if (mBmp == null)
                    mBmp = new WriteableBitmap(
                        new PixelSize((int)Bounds.Width, (int)Bounds.Height),
                        new Vector(96, 96),
                        PixelFormat.Bgra8888,
                        AlphaFormat.Unpremul);

                using (var fb = mBmp.Lock())
                {
                    var data = new int[fb.Size.Width * fb.Size.Height];

                    mYOffset = mStart;
                    for (int x = 0; x < fb.Size.Width; x++)
                    {
                        mXOffset = mStart;
                        for (int y = 0; y < fb.Size.Height; y++)
                        {
                            double value = PerlinNoise.Noise(mXOffset, mYOffset);;
                            Color c = ColorHeatMap.GetColorForValue(value.Map(-1, 1, 0, 1), 1);
                            data[y * fb.Size.Width + x] = (int) c.ToUint32();
                            mXOffset += 0.01;
                        }

                        mYOffset += 0.01;
                    }

                    Marshal.Copy(data, 0, fb.Address, fb.Size.Width * fb.Size.Height);
                }

                context.DrawImage(mBmp, new Rect(0, 0, Bounds.Width, Bounds.Height));

                mStart += 0.01;
            }

            WriteableBitmap mBmp;
            Random mRandom = new Random();
            double mXOffset;
            double mYOffset;
            double mStart;
        }

        RenderPanel mRenderPanel;
        DispatcherTimer mTimer;
    }

    static class Extensions
    {
        public static double Map(this double value, double fromSource, double toSource, double fromTarget, double toTarget)
        {
            return (value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget) + fromTarget;
        }
    }
}