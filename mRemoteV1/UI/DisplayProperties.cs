﻿using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace mRemoteNG.UI
{
    public class DisplayProperties
    {
        // Dpi of a 'normal' definition screen
        private const int BaselineDpi = 96;

        public SizeF ResolutionScalingFactor { get; } = GetResolutionScalingFactor();

        /// <summary>
        /// Scale the given nominal width value by the <see cref="ResolutionScalingFactor"/>
        /// </summary>
        /// <param name="width"></param>
        public int ScaleWidth(float width)
        {
            return CalculateScaledValue(width, ResolutionScalingFactor.Width);
        }

        /// <summary>
        /// Scale the given nominal height value by the <see cref="ResolutionScalingFactor"/>
        /// </summary>
        /// <param name="height"></param>
        public int ScaleHeight(float height)
        {
            return CalculateScaledValue(height, ResolutionScalingFactor.Width);
        }

        /// <summary>
        /// Scales the given image by <see cref="ResolutionScalingFactor"/>
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <returns>The resized image.</returns>
        /// <remarks>
        /// Code from https://stackoverflow.com/questions/1922040/how-to-resize-an-image-c-sharp
        /// </remarks>
        public Bitmap ScaleImage(Image image)
        {
            var width = ScaleWidth(image.Width);
            var height = ScaleHeight(image.Height);
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        public Bitmap ScaleImage(Icon icon)
        {
            return ScaleImage(icon.ToBitmap());
        }

        /// <summary>
        /// Scale the given nominal height value by the <see cref="ResolutionScalingFactor"/>
        /// </summary>
        /// <param name="width"></param>
        private int CalculateScaledValue(float value, float scalingValue)
        {
            return (int)Math.Round(value * scalingValue);
        }

        private static SizeF GetResolutionScalingFactor()
        {
            using (var g = new Form().CreateGraphics())
            {
                return new SizeF(g.DpiX/BaselineDpi, g.DpiY / BaselineDpi);
            }
        }
    }
}
