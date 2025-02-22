using System.Drawing;
using System.Timers;
using System.Windows.Forms;
using InformationTree.Domain.Entities.Graphics;
using InformationTree.Domain.Requests;
using MediatR;
using System.ComponentModel;

namespace InformationTree.Forms
{
    public partial class LoadingForm : Form
    {
        private readonly IMediator _mediator;

        #region Properties

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IGraphicsFile GraphicsFile { get; private set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public System.Timers.Timer Timer { get; protected set; }

        #endregion Properties

        #region Constructors

        public LoadingForm(
            IMediator mediator,
            IGraphicsFile graphicsFile,
            int? screenIndex,
            System.Timers.Timer timer = null)
            : base()
        {
            _mediator = mediator;

            InitializeComponent();

            SetProperties(screenIndex);

            GraphicsFile = graphicsFile;

            SetupTimer(timer);
        }

        private void SetupTimer(System.Timers.Timer timer = null)
        {
            Timer = timer ?? GetDefaultTimer();
            if (Timer != null)
            {
                Timer.Elapsed += T_Elapsed; // timer disposed in Dispose(bool)
                Timer.Start();
            }
        }

        private void SetProperties(int? screenIndex)
        {
            var screen = screenIndex.HasValue && screenIndex.Value < Screen.AllScreens.Length
                            ? Screen.AllScreens[screenIndex.Value]
                            : Screen.FromControl(this);
            var rcScreen = screen.WorkingArea;
            var rcForm = new Rectangle(0, 0, Width, Height);

            StartPosition = FormStartPosition.Manual;
            Bounds = screen.Bounds;
            Location = new Point((rcScreen.Left + rcScreen.Right) / 2 - (rcForm.Width / 2), rcScreen.Top);
        }

        private System.Timers.Timer GetDefaultTimer()
        {
            var ret = new System.Timers.Timer(100);
            return ret;
        }

        #endregion Constructors

        #region Handlers

        private async void T_Elapsed(object sender, ElapsedEventArgs e)
        {
            var request = new LoadingFormTimerElapsedRequest
            {
                FileLoadProgressBar = pbFileLoad,
                LoadingGraphicsPictureBox = pbLoadingGraphics,
            };
            await _mediator.Send(request);
        }

        public ProgressBar LoadProgressBar
        {
            get
            {
                return pbFileLoad;
            }
        }

        private async void pbIcon_Paint(object sender, PaintEventArgs e)
        {
            var request = new LoadingFormIconPaintRequest
            {
                Graphics = e.Graphics
            };
            await _mediator.Send(request);
        }

        private async void pbLoadingGraphics_Paint(object sender, PaintEventArgs e)
        {
            var request = new LoadingFormLoadingGraphicsPaintRequest
            {
                Graphics = e.Graphics,
                GraphicsFile = GraphicsFile
            };
            await _mediator.Send(request);
        }

        #endregion Handlers
    }
}