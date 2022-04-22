using System.Drawing;
using System.Reflection;
using System.Timers;
using System.Windows.Forms;
using InformationTree.Domain.Entities.Graphics;

namespace InformationTree.Forms
{
    public partial class LoadingForm : Form
    {
        #region Properties

        public IGraphicsFile GraphicsFile { get; private set; }
        public System.Timers.Timer Timer { get; protected set; }

        #endregion Properties

        #region Constructors

        public LoadingForm(IGraphicsFile graphicsFile, int? screenIndex, System.Timers.Timer timer = null)
            : base()
        {
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

        private void T_Elapsed(object sender, ElapsedEventArgs e)
        {
            pbFileLoad.PerformStep(); // TODO: TEST: Check if the progress bar changes
            pbLoadingGraphics.Refresh(); // paint?
        }

        public ProgressBar LoadProgressBar
        {
            get
            {
                return pbFileLoad;
            }
        }

        private void pbIcon_Paint(object sender, PaintEventArgs e)
        {
            var currentAssembly = Assembly.GetEntryAssembly();
            if (currentAssembly == null)
                return;

            var currentAssemblyLocation = currentAssembly.Location;
            if (string.IsNullOrEmpty(currentAssemblyLocation))
                return;

            var extractedIconFromCurrentAssembly = Icon.ExtractAssociatedIcon(currentAssemblyLocation);
            if (extractedIconFromCurrentAssembly == null)
                return;

            e.Graphics.DrawIcon(extractedIconFromCurrentAssembly, 0, 0);
        }

        // TODO: maybe use this method based on graphics feature enabled/disabled
        private void pbLoadingGraphics_Paint(object sender, PaintEventArgs e)
        {
            GraphicsFile.Show(e.Graphics);
        }

        #endregion Handlers
    }
}