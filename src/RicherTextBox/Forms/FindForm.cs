using System;
using System.Windows.Forms;
using InformationTree.Domain.Requests;
using InformationTree.Domain.Services;
using MediatR;

namespace RicherTextBox
{
    public partial class FindForm : Form
    {
        private readonly IPopUpService _popUpService;
        private readonly IMediator _mediator;
        private int lastFound = 0;
        private RichTextBox rtbInstance = null;

        public RichTextBox RtbInstance
        {
            set { rtbInstance = value; }
            get { return rtbInstance; }
        }

        public string InitialText
        {
            set { txtSearchText.Text = value; }
            get { return txtSearchText.Text; }
        }

        public FindForm()
        {
            InitializeComponent();
            TopMost = true;
        }

        public FindForm(
            IPopUpService popUpService,
            IMediator mediator)
            : this()
        {
            _popUpService = popUpService;
            _mediator = mediator;
        }
        
        private void rtbInstance_SelectionChanged(object sender, EventArgs e)
        {
            lastFound = rtbInstance.SelectionStart;
        }

        private async void btnDone_Click(object sender, EventArgs e)
        {
            rtbInstance.SelectionChanged -= rtbInstance_SelectionChanged;
            var request = new FormCloseRequest
            {
                Form = this
            };
            await _mediator.Send(request);
        }

        private void btnFindNext_Click(object sender, EventArgs e)
        {
            RichTextBoxFinds options = RichTextBoxFinds.None;
            if (chkMatchCase.Checked) options |= RichTextBoxFinds.MatchCase;
            if (chkWholeWord.Checked) options |= RichTextBoxFinds.WholeWord;

            int index = rtbInstance.Find(txtSearchText.Text, lastFound, options);
            lastFound += txtSearchText.Text.Length;
            if (index >= 0)
            {
                rtbInstance.Parent.Focus();
            }
            else
            {
                _popUpService.ShowInfo("Search string not found", "Find");
                lastFound = 0;
            }
        }

        private void FindForm_Load(object sender, EventArgs e)
        {
            if (rtbInstance != null)
                rtbInstance.SelectionChanged += new EventHandler(rtbInstance_SelectionChanged);
        }
    }
}