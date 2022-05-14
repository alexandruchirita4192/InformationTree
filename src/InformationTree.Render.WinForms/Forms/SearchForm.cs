using System;
using System.Windows.Forms;
using InformationTree.Domain.Services;

namespace InformationTree.Forms
{
    public partial class SearchForm : Form
    {
        private readonly IPopUpService _popUpService;

        public string TextToFind
        { get { return tbFind.Text?.Replace("\n", "")?.Replace("\r", ""); } set { tbFind.Text = value; } }

        public SearchForm(IPopUpService popUpService, string text = null)
        {
            _popUpService = popUpService;
            
            InitializeComponent();
            TextToFind = text;

            if (text != null)
                tbFind.Select(text.Length, 0);
        }

        private void DoSearch(object sender, EventArgs e)
        {
            if (tbFind.Text.Length > 3)
            {
                _popUpService.ShowMessage($"Searching for a node with text '{TextToFind}'");
                
                Close();
            }
        }

        private void tbFind_KeyUp(object sender, KeyEventArgs e)
        {
            if (object.ReferenceEquals(tbFind, sender) && e.KeyData == Keys.Enter)
                DoSearch(sender, e);

            if (e.KeyData == Keys.Escape)
            {
                tbFind.Text = null;
                this.Close();
            }
        }
    }
}