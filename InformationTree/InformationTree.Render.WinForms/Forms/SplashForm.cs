using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using InformationTree.Domain.Entities.Graphics;
using InformationTree.Domain.Services.Graphics;

namespace InformationTree.Forms
{
    public class SplashForm : LoadingForm
    {
        #region Constructor

        private SplashForm(IGraphicsFile graphicsFile, int? screenIndex)
            : base(graphicsFile, screenIndex)
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            //
            // SplashForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.ClientSize = new System.Drawing.Size(631, 448);
            this.Location = new System.Drawing.Point(0, 0);
            this.Name = "SplashForm";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion Constructor

        #region Static

        private delegate void CloseDelegate();

        //The type of form to be displayed as the splash screen.
        private static List<SplashForm> splashFormList;

        public static void ShowDefaultSplashScreen(IGraphicsFileFactory graphicsFileRecursiveGenerator)
        {
            InternalShowSplashScreen((currentScreenIndex) =>
            {
                var screen = Screen.AllScreens[currentScreenIndex];
                var graphicsFile = graphicsFileRecursiveGenerator.GetDefaultGraphicsFile(screen.Bounds.Height, screen.Bounds.Width);
                return graphicsFile;
            });
        }

        public static void ShowSplashScreen(IGraphicsFile graphicsFile)
        {
            InternalShowSplashScreen((currentScreenIndex) => graphicsFile);
        }

        private static void InternalShowSplashScreen(Func<int, IGraphicsFile> graphicsFile)
        {
            // Make sure it is only launched once.
            if (splashFormList == null && graphicsFile != null)
            {
                splashFormList = new List<SplashForm>();
                for (int currentScreenIndex = 0; currentScreenIndex < Screen.AllScreens.Length; currentScreenIndex++)
                {
                    var form = new SplashForm(graphicsFile(currentScreenIndex), currentScreenIndex);
                    form.Show();
                    splashFormList.Add(form);
                }
            }
        }

        public static bool HasInstance()
        {
            return splashFormList != null && splashFormList.Count > 0 && splashFormList.Any(f => !f.IsDisposed);
        }

        public static void CloseForm()
        {
            if (splashFormList != null)
            {
                foreach (var form in splashFormList)
                    form.Invoke(new CloseDelegate(() => CloseFormInternal(form)));
                splashFormList.Clear();
                splashFormList = null;
            }
        }

        private static void CloseFormInternal(SplashForm form)
        {
            if (form != null)
            {
                if (form.Timer != null)
                {
                    if (form.Timer.Enabled)
                        form.Timer.Stop();
                    form.Timer.Dispose();
                    form.Timer = null;
                }
                form.Close();
            }
        }

        #endregion Static
    }
}