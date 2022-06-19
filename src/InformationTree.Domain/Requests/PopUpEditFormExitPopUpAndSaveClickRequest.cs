using System.ComponentModel;
using InformationTree.Domain.Requests.Base;
using Newtonsoft.Json;

namespace InformationTree.Domain.Requests;

public class PopUpEditFormExitPopUpAndSaveClickRequest : BaseRequest
{
    [JsonIgnore]
    public Component ExitPopUpAndSaveTextBox { get; set; }

    [JsonIgnore]
    public Component Form { get; set; }

    [JsonIgnore]
    public object Sender { get; set; }
}