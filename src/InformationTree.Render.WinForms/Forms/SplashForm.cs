using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using InformationTree.Domain.Entities.Graphics;
using InformationTree.Domain.Requests;
using InformationTree.Domain.Services.Graphics;
using MediatR;

namespace InformationTree.Forms
{
    public class SplashForm : LoadingForm
    {
        #region Constructor

        private SplashForm(
            IMediator mediator,
            IGraphicsFile graphicsFile,
            int? screenIndex)
            : base(mediator, graphicsFile, screenIndex)
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            SuspendLayout();
            //
            // SplashForm
            //
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            ClientSize = new System.Drawing.Size(631, 448);
            Location = new System.Drawing.Point(0, 0);
            Name = "SplashForm";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion Constructor

        #region Static

        //The type of form to be displayed as the splash screen.
        private static List<SplashForm> splashFormList;

        public static void ShowDefaultSplashScreen(
            IMediator mediator,
            IGraphicsFileFactory graphicsFileFactory)
        {
            InternalShowSplashScreen(mediator, (currentScreenIndex) =>
            {
                var screen = Screen.AllScreens[currentScreenIndex];
                var graphicsFile = graphicsFileFactory.GetDefaultGraphicsFile(screen.Bounds.Height, screen.Bounds.Width);
                return graphicsFile;
            });
        }

        public static void ShowSplashScreen(
            IMediator mediator,
            IGraphicsFile graphicsFile)
        {
            InternalShowSplashScreen(mediator, (currentScreenIndex) => graphicsFile);
        }

        private static void InternalShowSplashScreen(
            IMediator mediator,
            Func<int, IGraphicsFile> graphicsFile)
        {
            // Make sure it is only launched once.
            if (splashFormList == null && graphicsFile != null)
            {
                splashFormList = new List<SplashForm>();
                for (int currentScreenIndex = 0; currentScreenIndex < Screen.AllScreens.Length; currentScreenIndex++)
                {
                    var form = new SplashForm(mediator, graphicsFile(currentScreenIndex), currentScreenIndex);
                    form.Show();
                    splashFormList.Add(form);
                }
            }
        }

        public static bool HasInstance()
        {
            return splashFormList != null && splashFormList.Count > 0 && splashFormList.Any(f => !f.IsDisposed);
        }

        public static void CloseForm(IMediator _mediator)
        {
            if (splashFormList != null)
            {
                foreach (var form in splashFormList)
                    form.Invoke(() => CloseFormInternal(form, _mediator));
                splashFormList.Clear();
                splashFormList = null;
            }
        }

        private async static void CloseFormInternal(SplashForm form, IMediator _mediator)
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
                var formCloseRequest = new FormCloseRequest
                {
                    Form = form
                };
                await _mediator.Send(formCloseRequest);
            }
        }

        #endregion Static
    }
}