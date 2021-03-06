﻿using System.Drawing;
using System.Windows.Forms;
using System.Timers;
using Timer = System.Timers.Timer;
using mPanel.Matrix;

namespace mPanel.Actions.Pong
{
    public partial class PongForm : Form
    {
        private const int FramesPerSecond = 30;

        private MatrixPanel Matrix => ((ContainerForm) MdiParent)?.Matrix;

        private readonly Frame Frame;
        private readonly Timer GameTimer;
        private Ball Ball;
        private Paddle TopPaddle, BottomPaddle;
        private long FrameCount;
        private bool AwaitingStart;

        public PongForm()
        {
            InitializeComponent();

            Frame = new Frame();

            GameTimer = new Timer(1000.0 / FramesPerSecond);
            GameTimer.Elapsed += GameTimer_Elapsed;

            NewGame();
        }

        #region Methods

        private void GameTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Frame.Clear(Color.Black);

            if (AwaitingStart)
            {
                // draw objects in default position
                Ball.Draw();
                TopPaddle.Draw();
                BottomPaddle.Draw();
            }
            else
            {
                // move the paddles
                TopPaddle.Move();
                BottomPaddle.Move();

                // perform ball collision
                BounceBall();

                // move ball periodically
                if (FrameCount % 2 == 0)
                    Ball.Move();

                // check if ball was scored
                if (Ball.Y < 0 || Ball.Y > MatrixPanel.Height - 1)
                    NewGame();

                // draw objects
                Ball.Draw();
                TopPaddle.Draw();
                BottomPaddle.Draw();
            }

            // push frame to matrix
            Matrix.SendFrame(Frame);

            FrameCount++;
        }

        private void NewGame()
        {
            Ball = new Ball(Frame, MatrixPanel.Width / 2, MatrixPanel.Height / 2);
            TopPaddle = new Paddle(Frame, Color.White, 6, 0, 3);
            BottomPaddle = new Paddle(Frame, Color.White, 6, 14, 3);

            AwaitingStart = true;
        }

        private void BounceBall()
        {
            if (Ball.X < 1)
                Ball.Direction.DeltaX = 1;
            else if (Ball.X > MatrixPanel.Width - 2)
                Ball.Direction.DeltaX = -1;

            if (Ball.Y == TopPaddle.Y + 1 && PaddleCollision(TopPaddle, Ball))
            {
                Ball.Direction.DeltaY = 1;

                if (TopPaddle.DeltaX != 0 && TopPaddle.DeltaX != Ball.Direction.DeltaX)
                    Ball.Direction.DeltaX = TopPaddle.DeltaX;

                Ball.Randomize();
            }
            else if (Ball.Y == BottomPaddle.Y - 1 && PaddleCollision(BottomPaddle, Ball))
            {
                Ball.Direction.DeltaY = -1;

                if (BottomPaddle.DeltaX != 0 && BottomPaddle.DeltaX != Ball.Direction.DeltaX)
                    Ball.Direction.DeltaX = BottomPaddle.DeltaX;

                Ball.Randomize();
            }
        }

        private static bool PaddleCollision(Paddle paddle, Ball ball)
        {
            for (var i = paddle.X; i < paddle.X + paddle.Width; i++)
            {
                if (i == ball.X)
                    return true;
            }

            return false;
        }

        #endregion

        #region Form Events

        private void PongForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            GameTimer.Stop();
            Matrix.Clear();
        }

        private void PongForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.D:
                    TopPaddle.DeltaX = -1;
                    break;
                case Keys.F:
                    TopPaddle.DeltaX = 1;
                    break;
                case Keys.J:
                    BottomPaddle.DeltaX = -1;
                    break;
                case Keys.K:
                    BottomPaddle.DeltaX = 1;
                    break;
                case Keys.Space:
                    AwaitingStart = false;
                    break;
            }
        }

        private void PongForm_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.D:
                    if (TopPaddle.DeltaX < 0)
                        TopPaddle.DeltaX = 0;
                    break;
                case Keys.F:
                    if (TopPaddle.DeltaX > 0)
                        TopPaddle.DeltaX = 0;
                    break;
                case Keys.J:
                    if (BottomPaddle.DeltaX < 0)
                        BottomPaddle.DeltaX = 0;
                    break;
                case Keys.K:
                    if (BottomPaddle.DeltaX > 0)
                        BottomPaddle.DeltaX = 0;
                    break;
            }
        }

        private void enableButton_Click(object sender, System.EventArgs e)
        {
            if (GameTimer.Enabled)
            {
                GameTimer.Stop();
                enableButton.Text = "Enable";
            }
            else
            {
                FrameCount = 0;
                GameTimer.Start();
                enableButton.Text = "Disable";
                label1.Focus();
            }
        }

        #endregion
    }
}
