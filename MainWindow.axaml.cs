using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Imaging;
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

            this.Renderer.DrawFps = true;
        }

        private void OnTimerTick(object? sender, EventArgs e)
        {
            mRenderPanel.InvalidateVisual();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        class RenderPanel : Panel
        {
            public override void Render(DrawingContext context)
            {
                base.Render(context);

                context.DrawRectangle(
                    Brushes.Black,
                    null,
                    new Rect(0, 0, Bounds.Width, Bounds.Height));

                if (mBmp == null)
                    mBmp = new RenderTargetBitmap(
                        new PixelSize((int)Bounds.Width, (int)Bounds.Height),
                        new Vector(96, 96));

                using (var gc = mBmp.CreateDrawingContext(null))
                using(var ctx = new DrawingContext(gc, false))
                {
                    mYOffset = mStart;
                    for (int i = 0; i < Bounds.Width; ++i)
                    {
                        mXOffset = mStart;
                        for (int j = 0; j < Bounds.Height; j++)
                        {
                            double value = PerlinNoise.Noise(mXOffset, mYOffset);
 
                            Color c = mColorHeatMap.GetColorForValue(value.Map(-1, 1, 0, 1), 1);

                            ctx.DrawRectangle(
                                new SolidColorBrush(c),
                                null,
                                new Rect(i, j, 1, 1));

                            mXOffset += 0.01;
                        }

                        mYOffset += 0.01;
                    }
                }

                context.DrawImage(mBmp, new Rect(0, 0, Bounds.Width, Bounds.Height));

                mStart += 0.01;
            }

            RenderTargetBitmap mBmp;
            ColorHeatMap mColorHeatMap = new ColorHeatMap(150);
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