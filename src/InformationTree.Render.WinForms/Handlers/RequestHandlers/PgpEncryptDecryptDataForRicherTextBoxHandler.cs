using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using InformationTree.Domain.Extensions;
using InformationTree.Domain.Requests;
using InformationTree.Domain.Responses;
using MediatR;
using NLog;

namespace InformationTree.Render.WinForms.Handlers.RequestHandlers
{
    public class PgpEncryptDecryptDataForRicherTextBoxHandler : IRequestHandler<PgpEncryptDecryptDataForRicherTextBoxRequest, BaseResponse>
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IMediator _mediator;

        public PgpEncryptDecryptDataForRicherTextBoxHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<BaseResponse> Handle(PgpEncryptDecryptDataForRicherTextBoxRequest request, CancellationToken cancellationToken)
        {
            if (request.DataRicherTextBox is not RicherTextBox.Controls.RicherTextBox tbData)
                return null;
            if (request.EncryptionLabel is not Label lblEncryption)
                lblEncryption = null; // Allow requests without an encryption label, this is information only
            if (request.FormToCenterTo is not Form formToCenterTo)
                return null;

            var baseRequest = new PgpEncryptDecryptDataRequest
            {
                InputDataText = tbData.Text,
                InputDataRtf = tbData.Rtf,
                FormToCenterTo = formToCenterTo,
                ActionType = request.ActionType,
                FromFile = request.FromFile,
                DataIsPgpEncrypted = request.DataIsPgpEncrypted
            };
            if (await _mediator.Send(baseRequest, cancellationToken) is not PgpEncryptDecryptDataResponse baseResponse)
                return null;

            if (lblEncryption != null)
                lblEncryption.Text = baseResponse.EncryptionInfo;
            if (baseResponse.ResultText.IsNotEmpty())
                tbData.Text = baseResponse.ResultText;
            if (baseResponse.ResultRtf.IsNotEmpty())
                tbData.Rtf = baseResponse.ResultRtf;

            if (baseResponse.ResultText.IsNotEmpty()
            && baseResponse.ResultRtf.IsNotEmpty())
                _logger.Debug("Both result text '{resultText}' and result rtf '{resultRtf}' are filled. Rtf result takes priority.",
                    baseResponse.ResultText,
                    baseResponse.ResultRtf);

            var response = new PgpEncryptDecryptDataForRicherTextBoxResponse
            {
                DataIsNull = tbData.Text != baseResponse.ResultText && baseResponse.ResultText.IsNotEmpty()
            };
            return response;
        }
    }
}