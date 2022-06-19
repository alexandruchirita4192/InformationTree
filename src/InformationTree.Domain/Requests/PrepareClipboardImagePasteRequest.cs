using InformationTree.Domain.Requests.Base;

namespace InformationTree.Domain.Requests
{
    public class PrepareClipboardImagePasteRequest : BaseRequest
    {
        public int MaxWidth { get; set; }
        public int MaxHeight { get; set; }
    }
}