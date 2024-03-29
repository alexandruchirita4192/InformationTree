﻿using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using InformationTree.Domain.Entities;
using InformationTree.Domain.Requests;
using InformationTree.Domain.Responses;
using InformationTree.Domain.Services;
using InformationTree.Forms;
using MediatR;
using NLog;

namespace InformationTree.Render.WinForms.Services
{
    public class WinFormsApplication : IApplication
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IConfigurationReader _configurationReader;
        private readonly IPopUpService _popUpService;
        private readonly IImportTreeFromXmlService _importTreeFromXmlService;
        private readonly IExportTreeToXmlService _exportTreeToXmlService;
        private readonly IMediator _mediator;
        private readonly ICachingService _cachingService;
        private readonly ITreeNodeToTreeNodeDataAdapter _treeNodeToTreeNodeDataAdapter;
        private readonly ITreeNodeDataToTreeNodeAdapter _treeNodeDataToTreeNodeAdapter;

        public WinFormsApplication(
            IConfigurationReader configurationReader,
            IPopUpService popUpService,
            IImportTreeFromXmlService importTreeFromXmlService,
            IExportTreeToXmlService exportTreeToXmlService,
            IMediator mediator,
            ICachingService cachingService,
            ITreeNodeToTreeNodeDataAdapter treeNodeToTreeNodeDataAdapter,
            ITreeNodeDataToTreeNodeAdapter treeNodeDataToTreeNodeAdapter)
        {
            _configurationReader = configurationReader;
            _popUpService = popUpService;
            _importTreeFromXmlService = importTreeFromXmlService;
            _exportTreeToXmlService = exportTreeToXmlService;
            _mediator = mediator;
            _cachingService = cachingService;
            _treeNodeToTreeNodeDataAdapter = treeNodeToTreeNodeDataAdapter;
            _treeNodeDataToTreeNodeAdapter = treeNodeDataToTreeNodeAdapter;
        }

        #region extern

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, ExactSpelling = true, SetLastError = true)]
        private static extern void MoveWindow(IntPtr hwnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        #endregion extern

        #region MainForm

        public static MainForm MainForm { get; set; }

        #endregion MainForm

        private static Timer AutoSaveTimer;

        public static void CenterForm(Form form, Form parentForm)
        {
            if (form == null || parentForm == null)
                return;

            var locationRectangle = parentForm.DesktopBounds;
            var x = locationRectangle.X + locationRectangle.Width / 2 - form.Width / 2;
            var y = locationRectangle.Y + locationRectangle.Height / 2 - form.Height / 2;
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

        public void GlobalExceptionHandling(Exception ex)
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

            _popUpService.ShowWarning($"{ex.Message}{Environment.NewLine}{Environment.NewLine}Check log file Log.txt", "Error occured");
        }

        private void GlobalUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            GlobalExceptionHandling(e.ExceptionObject as Exception ?? new Exception("Unknown exception"));
        }

        private void GlobalThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            GlobalExceptionHandling(e.Exception ?? new Exception("Unknown exception"));
        }

        public void Run()
        {
            var configuration = _configurationReader.GetConfiguration();

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
                _logger.Error(ex, $"Exception occured while setting up global exception handlers {ex.Message}");
                _popUpService.ShowError($"Exception '{ex.Message}' occured while setting up global exception handlers. Check logs.");
            }

            if (AutoSaveTimer == null)
                AutoSaveTimer = new Timer();
            AutoSaveTimer.Interval = 1000 * 60;
            AutoSaveTimer.Tick += AutoSaveTimer_Tick;
            AutoSaveTimer.Start();

            Application.ApplicationExit += async (o, e) =>
            {
                if (SplashForm.HasInstance())
                    return;

                if (await _mediator.Send(new GetTreeStateRequest()) is not GetTreeStateResponse getTreeStateResponse)
                    return;

                var isDataChanged = !getTreeStateResponse.TreeUnchanged && getTreeStateResponse.IsSafeToSave;
                if (isDataChanged)
                {
                    var notSafeToChangeCaption = !getTreeStateResponse.IsSafeToSave ? "[HadException]" : string.Empty;
                    var alreadySavedMessage = getTreeStateResponse.TreeSaved ? $"[Tree already saved at {getTreeStateResponse.TreeSavedAt}]" : string.Empty;
                    var unchangedMessage = getTreeStateResponse.TreeUnchanged && !getTreeStateResponse.TreeSaved ? $"[Tree unchanged]" : string.Empty;
                    var title = $"Do you want to save your work? {alreadySavedMessage}{unchangedMessage}{notSafeToChangeCaption}";
                    var message = $"There are unsaved changes in {getTreeStateResponse.FileName}.";

                    var result = _popUpService.Confirm(message, title);

                    if (result == PopUpResult.Confirm)
                    {
                        var setTreeStateRequest = new SetTreeStateRequest
                        {
                            IsSafeToSave = true,
                            TreeUnchanged = false
                        };
                        await _mediator.Send(setTreeStateRequest);
                    }
                    else if (result == PopUpResult.NotConfirm)
                    {
                        var setTreeStateRequest = new SetTreeStateRequest
                        {
                            IsSafeToSave = false,
                            TreeUnchanged = true
                        };
                        await _mediator.Send(setTreeStateRequest);
                    }
                }

                SaveTree();
                AutoSaveTimer.Tick -= AutoSaveTimer_Tick;
            };

            Application.Run(MainForm = new MainForm(
                _configurationReader,
                _importTreeFromXmlService,
                _exportTreeToXmlService,
                _mediator,
                _cachingService,
                _treeNodeToTreeNodeDataAdapter,
                _treeNodeDataToTreeNodeAdapter
                ));
        }

        private static void AutoSaveTimer_Tick(object sender, EventArgs e)
        {
            SaveTree();
        }
    }
}