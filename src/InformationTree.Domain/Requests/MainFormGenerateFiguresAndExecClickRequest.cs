using InformationTree.Domain.Requests.Base;

namespace InformationTree.Domain.Requests
{
    public class MainFormGenerateFiguresAndExecClickRequest : BaseRequest
    {
        public MainFormGenerateClickRequest MainFormGenerateClickRequest { get; set; }
    }
}