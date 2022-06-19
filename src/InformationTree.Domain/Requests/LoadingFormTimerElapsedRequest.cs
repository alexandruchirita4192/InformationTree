using System.ComponentModel;
using InformationTree.Domain.Requests.Base;

namespace InformationTree.Domain.Requests
{
    public class LoadingFormTimerElapsedRequest : BaseRequest
    {
        public Component FileLoadProgressBar { get; set; }
        public Component LoadingGraphicsPictureBox { get; set; }
    }
}