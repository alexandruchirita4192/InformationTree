using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using InformationTree.Domain.Entities;
using InformationTree.Domain.Services;
using InformationTree.Domain.Services.Graphics;
using InformationTree.Forms;
using InformationTree.Tree;
using NLog;

namespace InformationTree.Render.WinForms.Services
{
    public class WinFormsApplication : IApplication
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly ICommandLineParser _commandLineParser;
        private readonly IConfigurationReader _configurationReader;
        private readonly IPopUpConfirmation _popUpConfirmation;
        private readonly ISoundProvider _soundProvider;
        private readonly IGraphicsFileFactory _graphicsFileRecursiveGenerator;
        private readonly ICanvasFormFactory _canvasFormFactory;

        public WinFormsApplication(
            ICommandLineParser commandLineParser,
            IConfigurationReader configurationReader,
            IPopUpConfirmation popUpConfirmation,
            ISoundProvider soundProvider,
            IGraphicsFileFactory graphicsFileRecursiveGenerator,
            ICanvasFormFactory canvasFormFactory)
        {
            _commandLineParser = commandLineParser;
            _configurationReader = configurationReader;
            _popUpConfirmation = popUpConfirmation;
            _soundProvider = soundProvider;
            _graphicsFileRecursiveGenerator = graphicsFileRecursiveGenerator;
            _canvasFormFactory = canvasFormFactory;
        }

        #region extern

        // TODO: Maybe remove these kinds of windows DllImport
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, ExactSpelling = true, SetLastError = true)]
        private static extern void MoveWindow(IntPtr hwnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        #endregion extern

        #region MainForm

        private static MainForm _mainForm;

        public static MainForm MainForm
        {
            get
            {
                if (_mainForm != null)
                    return _mainForm;
                return _mainForm;
            }
            set
            {
                _mainForm = value;
            }
        }

        #endregion MainForm

        private static Timer AutoSaveTimer;

        public static void CenterForm(Form form, Form parentForm)
        {
            if (form == null || parentForm == null)
                return;

            int x = 0, y = 0;
            var locationRectangle = parentForm.DesktopBounds;
            x = locationRectangle.X + locationRectangle.Width / 2 - form.Width / 2;
            y = locationRectangle.Y + locationRectangle.Height / 2 - form.Height / 2;
            MoveWindow(form.Handle, x, y, form.Width, form.Height, false);
        }

        public static void SaveTree()
        {
            if (MainForm != null)
            {
                MainForm.ClearStyleAdded();
                MainForm.SaveTree();
            }
        }

        public static void GlobalExceptionHandling(Exception ex)
        {
            // Log to file
            try
            {
                _logger.Error(ex);
            }
            catch
            {
                // silent crash
            }

            // TODO: Show error message in pop-up using new service
            MessageBox.Show(ex.Message + Environment.NewLine + Environment.NewLine + "Check log file Exception.txt", "Exception occured");
        }

        private static void GlobalUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            GlobalExceptionHandling(e.ExceptionObject as Exception ?? new Exception("Unknown exception"));
        }

        private static void GlobalThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            GlobalExceptionHandling(e.Exception ?? new Exception("Unknown exception"));
        }

        public void Run(string[] args)
        {
            var configuration = _configurationReader.GetConfiguration();
            _commandLineParser.Parse(args, configuration);

            // TODO: Use configuration updated object

            Application.EnableVisualStyles();
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            
            try
            {
                Application.SetCompatibleTextRenderingDefault(false); // this should be called before the first form is created
                Application.ThreadException += GlobalThreadException;
                AppDomain.CurrentDomain.UnhandledException += GlobalUnhandledException;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            if (AutoSaveTimer == null)
                AutoSaveTimer = new Timer();
            AutoSaveTimer.Interval = 1000 * 60;
            AutoSaveTimer.Tick += AutoSaveTimer_Tick;
            AutoSaveTimer.Start();

            Application.ApplicationExit += (o, e) =>
            {
                if (SplashForm.HasInstance())
                    return;

                var isDataChanged = !TreeNodeHelper.TreeUnchanged && TreeNodeHelper.IsSafeToSave;
                if (isDataChanged)
                {
                    var notSafeToChangeCaption = !TreeNodeHelper.IsSafeToSave ? "[HadException]" : string.Empty;
                    var alreadySavedMessage = TreeNodeHelper.TreeSaved ? $"[Tree already saved at {TreeNodeHelper.TreeSavedAt}]" : string.Empty;
                    var unchangedMessage = TreeNodeHelper.TreeUnchanged && !TreeNodeHelper.TreeSaved ? $"[Tree unchanged]" : string.Empty;
                    var title = $"Do you want to save your work? {alreadySavedMessage}{unchangedMessage}{notSafeToChangeCaption}";
                    var message = $"There are unsaved changes in {TreeNodeHelper.FileName}.";

                    var result = _popUpConfirmation.Confirm(message, title);

                    if (result == PopUpResult.Confirm)
                    {
                        TreeNodeHelper.IsSafeToSave = true;
                        TreeNodeHelper.TreeUnchanged = false;
                    }
                    else if (result == PopUpResult.NotConfirm)
                    {
                        TreeNodeHelper.IsSafeToSave = false;
                        TreeNodeHelper.TreeUnchanged = true;
                    }
                }

                SaveTree();
                AutoSaveTimer.Tick -= AutoSaveTimer_Tick;
            };

            Application.Run(MainForm = new MainForm(_soundProvider, _graphicsFileRecursiveGenerator, _canvasFormFactory));
        }

        private static void AutoSaveTimer_Tick(object sender, EventArgs e)
        {
            SaveTree();
        }
    }
}