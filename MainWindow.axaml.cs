using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
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
                TimeSpan.FromMilliseconds(8),
                DispatcherPriority.Render,
                OnTimerTick);

            mRenderPanel = new RenderPanel();
            Content = mRenderPanel;

            mTimer.Start();
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

                mXOffset = mStart;
                for (int i = 0; i < Bounds.Width; ++i)
                {
                    double value = PerlinNoise.Noise(mXOffset, mXOffset); /*mRandom.Next(-100, 100) / 100.0;*/
                    double y = value.Map(-1, 1, 20, Bounds.Height - 20);

                    context.DrawLine(
                        new Pen(Brushes.White, 1),
                        new Point(i, y),
                        new Point(i, y + 1));

                    mXOffset += 0.01;
                }

                mStart += 0.01;
            }

            Random mRandom = new Random();
            double mXOffset;
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