using System.Windows.Forms;
using InformationTree.Domain.Extensions;
using InformationTree.Domain.Requests;
using MediatR;

namespace InformationTree.Forms
{
    public partial class SearchForm : Form
    {
        private readonly IMediator _mediator;

        public string TextToFind
        {
            get
            {
                return tbFind.Text
                    ?.RemoveEndLines();
            }
            set
            {
                tbFind.Text = value;
            }
        }

        public SearchForm(
            IMediator mediator,
            string text = null)
        {
            _mediator = mediator;

            InitializeComponent();
            TextToFind = text;

            if (text != null)
                tbFind.Select(text.Length, 0);
        }

        private async void tbFind_KeyUp(object sender, KeyEventArgs e)
        {
            var request = new SearchFormFindKeyUpRequest
            {
                FindTextBox = tbFind,
                Form = this,
                KeyData = (int)e.KeyData
            };
            await _mediator.Send(request);
        }
    }
}