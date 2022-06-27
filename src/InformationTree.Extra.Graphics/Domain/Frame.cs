using InformationTree.Domain.Entities.Graphics;
using InformationTree.Extra.Graphics.Domain;
using D = System.Drawing;

namespace InformationTree.Graphics
{
    public class Frame : IFrame
    {
        #region Properties

        public bool IsActive { get; private set; }
        public Frame? NextFrame { get; private set; }
        public IFigures Figures { get; private set; }

        public Point CenterPoint
        {
            get
            {
                return Figures != null ? Figures.CenterPoint : new Point();
            }
        }

        public Point RealCenterPoint
        {
            get
            {
                return Figures != null ? Figures.RealCenterPoint : new Point();
            }
        }

        #endregion Properties

        #region Constructor

        public Frame() : this(true)
        {
        }

        public Frame(bool isActive) : this(isActive, null)
        {
        }

        public Frame(bool isActive, Frame? nextFrame)
        {
            IsActive = isActive;
            NextFrame = nextFrame;
            Figures = new Figures();
        }

        #endregion Constructor

        #region Methods

        public void NewFrame()
        {
            var c = this;
            while (c.NextFrame != null)
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
            NextFrame = NextFrame.NextFrame;
        }

        public bool ChangeToNextFrame(bool addNewFrameIfNextFrameIsNull = true)
        {
            var c = this;
            while (c.NextFrame != null)
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
            if (NextFrame == null)
                return;

            var c = this;
            while ((c.NextFrame?.NextFrame != null) && (!c.NextFrame.IsActive))
                c = c.NextFrame;

            if (c.NextFrame != null && c.NextFrame.IsActive)
            {
                c.NextFrame.IsActive = false;
                c.IsActive = true;
            }
        }

        public void CycleFramesFromThis()
        {
            if (!ChangeToNextFrame(false))
                InactivateFramesAndReactivateThis();
        }
        
        public void GoToFrame(int position)
        {
            if (position < 0)
                return;

            // Inactivate all frames
            var c = this;
            while (c.NextFrame != null)
            {
                c.IsActive = false;
                c = c.NextFrame;
            }

            // Make the frame at position active (or the last frame active if the position is greater than the number of frames)
            c = this;
            while ((position != 0) && (c.NextFrame != null))
            {
                position--;
                c = c.NextFrame;
            }

            c.IsActive = true;
        }

        public void Show(D.Graphics graphics)
        {
            var frame = GetActiveFrameOrThis();

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
            
            var c = this;
            while (c.NextFrame != null)
            {
                c = c.NextFrame;
                framesList.Add(c);
            }

            return framesList;
        }

        public void CleanAllFrames()
        {
            var c = this;
            do
            {
                c.CleanThisFrame();
                c = c.NextFrame;
            }
            while (c != null);
        }

        public bool HasActiveFrame()
        {
            return GetFirstActiveFrame() != null;
        }

        public IFrame? GetFirstActiveFrame()
        {
            var c = this;
            while (c.NextFrame != null)
            {
                if (c.IsActive)
                    break;
                c = c.NextFrame;
            }

            return c.IsActive ? c : null;
        }

        public IFrame GetActiveFrameOrThis()
        {
            var frame = GetFirstActiveFrame();
            IsActive = frame == null || IsActive;
            return frame ?? this;
        }

        public void InactivateFramesAndReactivateThis()
        {
            var c = this;
            IsActive = true; // activate this
            while (c.NextFrame != null) // inactivate the rest
            {
                c = c.NextFrame;
                c.IsActive = false;
            }
        }

        public void Clean()
        {
            CleanAllFrames();
            NextFrame = null;
        }

        public void TranslateCenter(D.Point oldCenter, D.Point newCenter)
        {
            if (Figures != null)
                Figures.TranslateCenterAll(oldCenter, newCenter);
        }

        public void TranslateCenterAllFrames(D.Point oldCenter, D.Point newCenter)
        {
            var c = this;
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