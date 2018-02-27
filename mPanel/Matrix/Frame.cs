﻿using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace mPanel.Matrix
{
    public class Frame : IDisposable
    {
        private static int PixelDataLength => MatrixPanel.PixelDataLength;

        public Bitmap Bitmap { get; }
        public Graphics Graphics { get; }
        public int Width => MatrixPanel.Width;
        public int Height => MatrixPanel.Height;

        public Frame()
        {
            Bitmap = new Bitmap(Width, Height, PixelFormat.Format24bppRgb);
            Graphics = Graphics.FromImage(Bitmap);
        }

        public Frame(Bitmap bitmap)
        {
            Bitmap = bitmap;
            Graphics = Graphics.FromImage(Bitmap);
        }

        public void Clear(Color color)
        {
            Graphics.Clear(color);
        }

        public void SetPixel(Point pixel, Color color)
        {
            Bitmap.SetPixel(pixel.X, pixel.Y, color);
        }

        public byte[] GetBytes()
        {
            var data = Bitmap.LockBits(new Rectangle(0, 0, Bitmap.Width, Bitmap.Height), ImageLockMode.ReadOnly, Bitmap.PixelFormat);

            byte[] bytes;

            try
            {
                bytes = new byte[data.Width * data.Height * PixelDataLength];

                for (var y = 0; y < data.Height; y++)
                {
                    var ptr = (IntPtr) ((long) data.Scan0 + y * data.Stride);

                    var row0 = y * data.Width * PixelDataLength;

                    for (var x = row0; x < row0 + data.Width * PixelDataLength; x += PixelDataLength)
                    {
                        Marshal.Copy(ptr, bytes, x + 2, 1);
                        Marshal.Copy(IntPtr.Add(ptr, 1), bytes, x + 1, 1);
                        Marshal.Copy(IntPtr.Add(ptr, 2), bytes, x, 1);

                        ptr = IntPtr.Add(ptr, PixelDataLength);
                    }
                }
            }
            finally
            {
                Bitmap.UnlockBits(data);
            }

            return bytes;
        }

        public void Dispose()
        {
            Bitmap?.Dispose();
            Graphics?.Dispose();
        }
    }
}
