using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using InformationTree.Domain.Requests;
using InformationTree.Domain.Responses;
using MediatR;

namespace InformationTree.Render.WinForms.Handlers.RequestHandlers
{
    public class PopUpEditFormDataKeyUpHandler : IRequestHandler<PopUpEditFormDataKeyUpRequest, BaseResponse>
    {
        public Task<BaseResponse> Handle(PopUpEditFormDataKeyUpRequest request, CancellationToken cancellationToken)
        {
            if (request.DataRicherTextBox is not RicherTextBox.Controls.RicherTextBox tbData)
                return Task.FromResult<BaseResponse>(null);

            if (request.KeyData == (int)(Keys.B | Keys.Control))
                tbData.ToggleBold();
            //else if (request.KeyData == (Keys.I | Keys.Control))
            //    tbData.ToggleItalic();
            else if (request.KeyData == (int)(Keys.U | Keys.Control))
                tbData.ToggleUnderline();
            else if (request.KeyData == (int)(Keys.S | Keys.Control))
                tbData.ToggleStrikeOut();
            //else if (request.KeyData == (Keys.Left | Keys.Control))
            //    tbData.SetAlign(HorizontalAlignment.Left);
            //else if (request.KeyData == (Keys.Up | Keys.Control))
            //    tbData.SetAlign(HorizontalAlignment.Center);
            //else if (request.KeyData == (Keys.Right | Keys.Control))
            //    tbData.SetAlign(HorizontalAlignment.Right);
            else if (request.KeyData == (int)(Keys.O | Keys.Control))
                tbData.OpenFile();
            else if (request.KeyData == (int)(Keys.S | Keys.Control))
                tbData.SaveFile();
            else if (request.KeyData == (int)(Keys.P | Keys.Control))
                tbData.InsertPicture();

            return Task.FromResult(new BaseResponse());
        }
    }
}