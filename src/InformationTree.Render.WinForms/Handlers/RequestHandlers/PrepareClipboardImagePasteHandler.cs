using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using InformationTree.Domain.Requests;
using InformationTree.Domain.Responses;
using MediatR;

namespace InformationTree.Render.WinForms.Handlers.RequestHandlers
{
    public class PrepareClipboardImagePasteHandler : IRequestHandler<PrepareClipboardImagePasteRequest, BaseResponse>
    {
        public Task<BaseResponse> Handle(PrepareClipboardImagePasteRequest request, CancellationToken cancellationToken)
        {
            if (request.MaxHeight <= 0 || request.MaxWidth <= 0)
                return Task.FromResult<BaseResponse>(null);
            if (!Clipboard.ContainsImage())
                return Task.FromResult<BaseResponse>(null);

            var image = Clipboard.GetImage();
            var maxWidth = request.MaxWidth - 10;
            var maxHeight = request.MaxHeight - 10;

            var newWidth = maxWidth;
            var newHeight = image.Height * newWidth / image.Width;

            if (newHeight > maxHeight)
            {
                // Resize with height instead
                newWidth = image.Width * maxHeight / image.Height;
                newHeight = maxHeight;
            }

            if (newWidth != image.Width || newHeight != image.Height)
            {
                image = image.GetThumbnailImage(newWidth, newHeight, null, IntPtr.Zero);
                Clipboard.SetImage(image);
            }

            return Task.FromResult(new BaseResponse());
        }
    }
}