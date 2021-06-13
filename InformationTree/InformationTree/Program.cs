using System;
using System.Windows.Forms;
using FormsGame.Forms;
using FormsGame.Tree;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using FormsGame.Sound;
using System.IO;

namespace FormsGame
{
    static class Program
    {
        #region extern
        
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, ExactSpelling = true, SetLastError = true)]
        public static extern void MoveWindow(IntPtr hwnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        #endregion extern

        #region MainForm

        public static MainForm _mainForm;
        public static MainForm MainForm
        {
            get
            {
                if (_mainForm != null)
                    return _mainForm;
                _mainForm = new MainForm();
                return _mainForm;
            }
            set
            {
                _mainForm = value;
            }
        }

        #endregion MainForm

        static Timer AutoSaveTimer;

        public static void CenterForm(Form form, Form parentForm)
        {
            if (form == null || parentForm == null)
                return;

            int x = 0, y = 0;
            var locationRectangle = parentForm.DesktopBounds;
            x = locationRectangle.X + (locationRectangle.Width / 2) - (form.Width / 2);
            y = locationRectangle.Y + (locationRectangle.Height / 2) - (form.Height / 2);
            Program.MoveWindow(form.Handle, x, y, form.Width, form.Height, false);
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
                File.WriteAllText("Exception.txt", ex.ToString());
            }
            catch
            {
                // silent crash
            }

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

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            
            Application.EnableVisualStyles();
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

                var notSavingData = TreeNodeHelper.TreeUnchanged || !TreeNodeHelper.IsSafeToSave;
                if (notSavingData)
                {
                    var notSafeToChangeCaption = !TreeNodeHelper.IsSafeToSave ? "[HadException]" : "";
                    var message = "Really close without saving?";
                    var caption = "Do you want to close without saving?" + notSafeToChangeCaption;

                    SoundHelper.PlaySystemSound(4);
                    var result = MessageBox.Show(message, caption, MessageBoxButtons.YesNo);

                    if(result == DialogResult.No)
                    {
                        TreeNodeHelper.IsSafeToSave = true;
                        TreeNodeHelper.TreeUnchanged = false;
                    }
                }
                else
                {
                    var notSafeToChangeCaption = !TreeNodeHelper.IsSafeToSave ? "[HadException]" : "";
                    var caption = "Do you want to close saving data?" + notSafeToChangeCaption;
                    var message = "Really close saving data?";

                    SoundHelper.PlaySystemSound(4);
                    var result = MessageBox.Show(message, caption, MessageBoxButtons.YesNo);
                    if(result == DialogResult.Yes && !TreeNodeHelper.IsSafeToSave)
                        TreeNodeHelper.IsSafeToSave = true;
                    if (result == DialogResult.No)
                    {
                        TreeNodeHelper.IsSafeToSave = false;
                        TreeNodeHelper.TreeUnchanged = true;
                    }
                }

                SaveTree();
                AutoSaveTimer.Tick -= AutoSaveTimer_Tick;
            };

            Application.Run(MainForm);
        }

        private static void AutoSaveTimer_Tick(object sender, EventArgs e)
        {
            AutoSaveTimer.Stop();
            SaveTree();
            AutoSaveTimer.Start();
        }
    }
}
