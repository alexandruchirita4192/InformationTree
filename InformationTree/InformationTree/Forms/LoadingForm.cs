using System;
using System.Drawing;
using System.Reflection;
using System.Timers;
using System.Windows.Forms;

namespace InformationTree.Forms
{
    public partial class LoadingForm : Form
    {
        #region Properties

        //public GraphicsFile GraphicsFile { get; private set; } // TODO: Maybe remove this property that is feature-dependent?
        public System.Timers.Timer Timer { get; protected set; }

        #endregion Properties

        #region Constructors

        public LoadingForm() : this(null, /*null,*/ null) // TODO: use the commented parameter if possible/necessary, or remove the comment
        {
        }


        // TODO: Maybe find a way to remove constructor or GraphicsFile parameter?? (for now the parameter is removed)
        public LoadingForm(int? screenIndex, /*GraphicsFile graphicsFile = null,*/ System.Timers.Timer timer = null)
        {
            InitializeComponent();

            var screen = screenIndex.HasValue && screenIndex.Value < Screen.AllScreens.Length
                ? Screen.AllScreens[screenIndex.Value]
                : Screen.FromControl(this);
            var rcScreen = screen.WorkingArea;
            var rcForm = new Rectangle(0, 0, Width, Height);

            StartPosition = FormStartPosition.Manual;
            Bounds = screen.Bounds;
            Location = new Point((rcScreen.Left + rcScreen.Right) / 2 - (rcForm.Width / 2), rcScreen.Top);

            //GraphicsFile = graphicsFile != null ? graphicsFile : GetDefaultGraphicsFile(screen);
            Timer = timer ?? GetDefaultTimer();
            if (Timer != null)
            {
                Timer.Elapsed += T_Elapsed; // timer disposed in Dispose(bool)
                Timer.Start();
            }
        }

        private System.Timers.Timer GetDefaultTimer()
        {
            var ret = new System.Timers.Timer(100);
            return ret;
        }

        #endregion Constructors

        #region Methods

        // TODO: maybe extract this to the graphics project?
        //private GraphicsFile GetDefaultGraphicsFile(Screen screen)
        //{
        //    if (screen == null)
        //        return null;

        //    var ret = new GraphicsFile();
        //    var y = screen.Bounds.Height / 2;
        //    var x = screen.Bounds.Width / 2;
        //    var maxScreen = screen.Bounds.Height > screen.Bounds.Width ? screen.Bounds.Height : screen.Bounds.Width;
        //    var max = (maxScreen / 2.0) * 1.41421356237;
        //    var lines = new List<string>();

        //    #region Old code (kept because might be preety)

        //    //lines.Add("ComputeXComputeY 4 200 200 50 0 0 -1 0");
        //    //lines.Add("AddFrame");
        //    //lines.Add("ComputeXComputeY 4 200 200 50 0 6 0 0");
        //    //lines.Add("AddFrame");
        //    //lines.Add("ComputeXComputeY 4 200 200 50 0 6 1 0");
        //    //lines.Add("AddFrame");
        //    //lines.Add("ComputeXComputeY 4 200 200 50 0 6 2 0");
        //    //lines.Add("AddFrame");
        //    //lines.Add("ComputeXComputeY 5 200 200 50 0 6 1 0");
        //    //lines.Add("AddFrame");
        //    //lines.Add("ComputeXComputeY 6 200 200 50 0 6 1 0");
        //    //lines.Add("AllCenter 200 200 " + x.ToString() + " " + y.ToString());
        //    //lines.Add("Cycle 200");

        //    #endregion Old code (kept because might be preety)

        //    lines.Add("ComputeXComputeY 4 200 200 45 0 0 -1 0");

        //    for (int i = 50; i <= max; i += 4)
        //    {
        //        lines.Add("AddFrame");
        //        lines.Add("ComputeXComputeY 4 200 200 " + i.ToString() + " 0 0 -1 0");
        //    }
        //    lines.Add("AllCenter 200 200 " + x.ToString() + " " + y.ToString());
        //    lines.Add("Cycle 1");
        //    ret.ParseLines(lines.ToArray());

        //    return ret;
        //}

        #endregion Methods

        #region Handlers

        private void T_Elapsed(object sender, ElapsedEventArgs e)
        {
            //pbFileLoad.PerformStep();
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
            // TODO: Use an interface after GraphicsFile has an interface in the Domain project
            //GraphicsFile.Show(e.Graphics);
        }

        #endregion Handlers
    }
}