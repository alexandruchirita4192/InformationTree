using InformationTree.Domain.Requests.Base;

namespace InformationTree.Domain.Requests;

public class ShowCanvasPopUpRequest : BaseRequest
{
    public string[] FigureLines { get; set; }
}