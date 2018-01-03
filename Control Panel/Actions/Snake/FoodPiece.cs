﻿using System;
using System.Drawing;
using Control_Panel.Matrix;

namespace Control_Panel.Actions.Snake
{
    public class FoodPiece
    {
        private static readonly Random Random = new Random();
        private readonly Frame Frame;
        private byte Hue;

        public Color Fill { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public FoodPiece(Frame frame)
        {
            Frame = frame;

            Randomize();
        }

        public void Randomize()
        {
            Hue += 16;
            Fill = ColorUtils.HsvToColor(Hue / 255.0, 1.0, 1.0);
            X = Random.Next(0, MatrixPanel.Width);
            Y = Random.Next(0, MatrixPanel.Height);
        }

        public void Draw()
        {
            using (var fill = new SolidBrush(Fill))
            {
                Frame.Graphics.FillRectangle(fill, X, Y, 1, 1);
            }
        }
    }
}