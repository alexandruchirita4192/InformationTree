﻿using InformationTree.Extra.Graphics.Computation;
using InformationTree.Extra.Graphics.Services.FileParsing;
using D = System.Drawing;

namespace InformationTree.Forms
{
    public partial class CanvasPopUpForm : Form
    {
        private int op, nr, x, y, r;
        public System.Threading.Timer RunTimer;
        public GraphicsFile GraphicsFile;
        private D.BufferedGraphicsContext context;
        private D.BufferedGraphics grafx;

        public CanvasPopUpForm(GraphicsFile graphicsInitializer)
        {
            InitializeComponent();
            GraphicsFile = graphicsInitializer; //TODO: Fix this better??; old code had this too: "?? new GraphicsFile();"

            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            context = D.BufferedGraphicsManager.Current;
            context.MaximumBuffer = new D.Size(this.Width + 1, this.Height + 1);
            grafx = context.Allocate(this.CreateGraphics(),
                 new D.Rectangle(0, 0, this.Width, this.Height));

            UpdateGraphics(grafx.Graphics);

            op = 0; nr = 0;
            var interval = 1000 / GraphicsComputation.FramesPerSecond;
            RunTimer = new System.Threading.Timer(RunTimer_Tick, null, interval, interval);            
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            if (disposing)
            {
                if (grafx != null)
                    grafx = null;
                if (context != null)
                    context = null;
                RunTimer = null;
                GraphicsFile = null;
            }
            base.Dispose(disposing);
        }

        //static int ticks;
        private void RunTimer_Tick(object? sender)
        {
            if (this.IsDisposed)
            {
                Dispose(false);
                return;
            }

            if (RunTimer != null)
            {
                // Stop timer
                try { RunTimer.Change(Timeout.Infinite, Timeout.Infinite); } catch { /* silent crash */ }
            }
            
            //if (ticks == 20)
            //{
            //    //MessageBox.Show(GraphicsFile.GetDebugText());
            //    ticks = 0;
            //}
            //ticks++;

            if (grafx != null && !IsDisposed)
            {
                UpdateGraphics(grafx.Graphics);
                grafx.Render(D.Graphics.FromHwnd(this.Handle));
            }

            if (RunTimer != null)
            {
                // Start timer
                var interval = 1000 / GraphicsComputation.FramesPerSecond;
                RunTimer.Change(interval, interval);
            }
        }

        private void CanvasPopUpForm_Resize(object sender, EventArgs e)
        {
            context.MaximumBuffer = new D.Size(this.Width + 1, this.Height + 1);
            if (grafx != null)
            {
                grafx.Dispose();
                grafx = null;
            }
            grafx = context.Allocate(this.CreateGraphics(),
                new D.Rectangle(0, 0, this.Width, this.Height));

            // Cause the background to be cleared and redraw.
            UpdateGraphics(grafx.Graphics);
            this.Refresh();
        }

        private void CanvasPopUpForm_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.FormBorderStyle == System.Windows.Forms.FormBorderStyle.Sizable)
                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            else
                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
        }

        private void CanvasPopUpForm_Paint(object sender, PaintEventArgs e)
        {
            ////base.OnPaint(e);
            grafx.Render(e.Graphics);
            //UpdateGraphics(e.Graphics);
        }

        public void UpdateGraphics(D.Graphics g = null)
        {
            if (this.IsDisposed)
                return;

            if (g == null)
                using (var graphics = this.CreateGraphics())
                {
                    GraphicsFile.Show(graphics);
                }
            else
                GraphicsFile.Show(g);
        }

        private void CanvasPopUpForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.D1:
                case Keys.NumPad1:
                    if (op == 0)
                        GraphicsFile.GoToFrame("0");
                    else if ((op > 0) && (op < 10))
                        nr = nr * 10 + 1;
                    break;

                case Keys.D2:
                case Keys.NumPad2:
                    if (op == 0)
                        GraphicsFile.GoToFrame("1");
                    else if ((op > 0) && (op < 10))
                        nr = nr * 10 + 1;
                    break;

                case Keys.D3:
                case Keys.NumPad3:
                    if (op == 0)
                        GraphicsFile.GoToFrame("2");
                    else if ((op > 0) && (op < 10))
                        nr = nr * 10 + 2;
                    break;

                case Keys.D4:
                case Keys.NumPad4:
                    if (op == 0)
                        GraphicsFile.GoToFrame("3");
                    else if ((op > 0) && (op < 10))
                        nr = nr * 10 + 3;
                    break;

                case Keys.D5:
                case Keys.NumPad5:
                    if (op == 0)
                        GraphicsFile.GoToFrame("4");
                    else if ((op > 0) && (op < 10))
                        nr = nr * 10 + 4;
                    break;

                case Keys.D6:
                case Keys.NumPad6:
                    if (op == 0)
                        GraphicsFile.GoToFrame("5");
                    else if ((op > 0) && (op < 10))
                        nr = nr * 10 + 5;
                    break;

                case Keys.D7:
                case Keys.NumPad7:
                    if (op == 0)
                        GraphicsFile.GoToFrame("6");
                    else if ((op > 0) && (op < 10))
                        nr = nr * 10 + 6;
                    break;

                case Keys.D8:
                case Keys.NumPad8:
                    if (op == 0)
                        GraphicsFile.GoToFrame("7");
                    else if ((op > 0) && (op < 10))
                        nr = nr * 10 + 7;
                    break;

                case Keys.D9:
                case Keys.NumPad9:
                    if (op == 0)
                        GraphicsFile.GoToFrame("8");
                    else if ((op > 0) && (op < 10))
                        nr = nr * 10 + 8;
                    break;

                case Keys.D0:
                case Keys.NumPad0:
                    if (op == 0)
                        GraphicsFile.GoToFrame("9");
                    else if ((op > 0) && (op < 10))
                        nr = nr * 10 + 9;
                    break;

                case Keys.F: // figure
                    op = 1; nr = 0;
                    break;
                //case Keys.R: //rotate
                //    op = 2; nr = 0;
                //    break;
                //case Keys.O: //rotate around
                //    op = 3; nr = 0;
                //    break;
                //case Keys.C: //color
                //    op = 4;nr = 0;
                //    break;
                //case Keys.P: //points
                //    op = 5;nr = 0;
                //    break;
                case Keys.Enter:
                    if (op == 1)
                    {
                        x = nr;
                        nr = 0;
                        op++;
                    }
                    else if (op == 2)
                    {
                        y = nr;
                        nr = 0;
                        op++;
                    }
                    else if (op == 3)
                    {
                        r = nr;
                        nr = 0;
                    }
                    break;

                case Keys.Left:
                    GraphicsFile.ChangeToPreviousFrame();
                    break;

                case Keys.Right:
                    GraphicsFile.ChangeToNextFrame("true");
                    break;

                case Keys.Q:
                case Keys.X:
                case Keys.Escape:
                    Close();
                    break;

                case Keys.D:

                    break;
            }
        }
    }
}