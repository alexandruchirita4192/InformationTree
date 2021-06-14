using System;
using System.Collections.Generic;
using D = System.Drawing;

namespace InformationTree.Graphics
{
    public class Frame
    {
        #region Properties

        public bool IsActive { get; private set; } //false = inactive; true = active
        public Frame NextFrame { get; private set; }
        public Figures Figures { get; private set; }

        public D.Point CenterPoint
        {
            get
            {
                return Figures != null ? Figures.CenterPoint : new D.Point();
            }
        }
        public D.Point RealCenterPoint
        {
            get
            {
                return Figures != null ? Figures.RealCenterPoint : new D.Point();
            }
        }

        #endregion Properties

        #region Constructor

        public Frame() : this(true) { }
        public Frame(bool isActive) : this(isActive, null) { }
        public Frame(bool isActive, Frame nextFrame) { IsActive = isActive; NextFrame = nextFrame; Figures = new Figures(); }

        #endregion Constructor

        #region Methods

        public void NewFrame()
        {
            Frame c = this;
            while(c.NextFrame != null)
            {
                c.IsActive = false;
                c = c.NextFrame;
            }
            c.IsActive = false;
            c.NextFrame = new Frame(true, null);
        }

        public void DeleteNextFrame()
        {
            if (NextFrame == null)
                return;

            Frame d = this.NextFrame, c = this;
            c.NextFrame = c.NextFrame.NextFrame;
            d = null;
        }

        public bool ChangeToNextFrame(bool addNewFrameIfNextFrameIsNull = true)
        {
            Frame c = this;
            while(c.NextFrame != null)
            {
                if (c.IsActive)
                    break;
                c = c.NextFrame;
            }

            if (c.IsActive)
            {
                if (c.NextFrame == null)
                {
                    if (addNewFrameIfNextFrameIsNull)
                        c.NextFrame = new Frame(true);
                    else
                        return false;
                }
                c.NextFrame.IsActive = true;
                c.IsActive = false;
            }
            return true;
        }

        public void ChangeToPreviousFrame()
        {
            if (this.NextFrame == null)
                return;

            Frame c = this;
            while ((c.NextFrame.NextFrame != null) && (!c.NextFrame.IsActive))
                c = c.NextFrame;

            c.NextFrame.IsActive = false;
            c.IsActive = true;
        }

        public void CycleFramesFromThis()
        {
            if(!ChangeToNextFrame(false))
                InactivateFramesAndReactivateThis();
        }

        public void GoToFrame(int position)
        {
            if (position < 0)
                return;

            Frame c = this;
            //facem toate frame-urile inactive
            while (c.NextFrame != null)
            {
                c.IsActive = false;
                c = c.NextFrame;
            }

            c = this;
            //activam frame-ul pe care il vrem
            while((position != 0) && (c.NextFrame != null))
            {
                position--;
                c = c.NextFrame;
            }

            c.IsActive = true;
        }

        public void Show(D.Graphics graphics)
        {
            Frame frame = GetActiveFrameOrThis();

            graphics.Clear(D.Color.Black);

            if (frame.Figures != null)
                frame.Figures.ShowAll(graphics);
        }

        public void CleanThisFrame()
        {
            if (Figures != null)
                Figures.FigureList.Clear();
        }

        public List<Frame> GetFramesList()
        {
            var framesList = new List<Frame>() { this };

            Frame c = this;
            while (c.NextFrame != null)
            {
                c = c.NextFrame;
                framesList.Add(c);
            }

            return framesList;
        }

        public void CleanAllFrames()
        {
            Frame c = this;
            c.CleanThisFrame();
            while (c.NextFrame != null)
            {
                c = c.NextFrame;
                c.CleanThisFrame();
            }
        }

        public bool HasActiveFrame()
        {
            return GetFirstActiveFrame() != null;
        }


        public Frame GetFirstActiveFrame()
        {
            Frame c = this;
            while (c.NextFrame != null)
            {
                if (c.IsActive)
                    break;
                c = c.NextFrame;
            }

            return c.IsActive ? c : null;
        }

        public Frame GetActiveFrameOrThis()
        {
            var frame = GetFirstActiveFrame();
            this.IsActive = frame == null ? true : this.IsActive; // fix IsActive
            return frame ?? this;
        }
        
        public void InactivateFramesAndReactivateThis()
        {
            Frame c = this;
            this.IsActive = true; // activate this
            while (c.NextFrame != null) // inactivate the rest
            {
                c = c.NextFrame;
                c.IsActive = false;
            }
        }

        public void Clean()
        {
            CleanAllFrames();
            this.NextFrame = null;
        }

        public void TranslateCenter(D.Point oldCenter, D.Point newCenter)
        {
            if (oldCenter == null || newCenter == null)
                return;

            if (Figures != null)
                Figures.TranslateCenterAll(oldCenter, newCenter);
        }

        public void TranslateCenterAllFrames(D.Point oldCenter, D.Point newCenter)
        {
            if (oldCenter == null || newCenter == null)
                return;

            Frame c = this;
            c.TranslateCenter(oldCenter, newCenter);
            while (c.NextFrame != null)
            {
                c = c.NextFrame;
                c.TranslateCenter(oldCenter, newCenter);
            }
        }

        #endregion Methods
    }
}
