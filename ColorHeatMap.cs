using System;
using System.Collections.Generic;
using Avalonia.Media;

namespace AvaloniaPerlinNoise
{
    public static class ColorHeatMap
    {
        public static Color GetColorForValue(double val, double maxVal)
        {
            double valPerc = val / maxVal;// value%
            double colorPerc = 1d / (mAllColors.Count - 1);// % of each block of color. the last is the "100% Color"
            double blockOfColor = valPerc / colorPerc;// the integer part repersents how many block to skip
            int blockIdx = (int)Math.Truncate(blockOfColor);// Idx of 
            double valPercResidual = valPerc - (blockIdx * colorPerc);//remove the part represented of block 
            double percOfColor = valPercResidual / colorPerc;// % of color of this block that will be filled

            Color cTarget = mAllColors[blockIdx];
            Color cNext = cNext = mAllColors[blockIdx + 1];

            var deltaR = cNext.R - cTarget.R;
            var deltaG = cNext.G - cTarget.G;
            var deltaB = cNext.B - cTarget.B;

            var R = cTarget.R + (deltaR * percOfColor);
            var G = cTarget.G + (deltaG * percOfColor);
            var B = cTarget.B + (deltaB * percOfColor);

            return Color.FromRgb((byte)R, (byte)G, (byte)B);
        }
        static List<Color> mAllColors = new List<Color>()
        {
                Color.FromRgb(0, 0, 0) ,//Black
                Color.FromRgb(0, 0, 0xFF) ,//Blue
                Color.FromRgb(0, 0xFF, 0xFF) ,//Cyan
                Color.FromRgb(0, 0xFF, 0) ,//Green
                Color.FromRgb(0xFF, 0xFF, 0) ,//Yellow
                Color.FromRgb(0xFF, 0, 0) ,//Red
                Color.FromRgb(0xFF, 0xFF, 0xFF) // White
        };
    }
}