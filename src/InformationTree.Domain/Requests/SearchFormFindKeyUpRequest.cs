using System.ComponentModel;
using Newtonsoft.Json;
using InformationTree.Domain.Requests.Base;

namespace InformationTree.Domain.Requests;

public class SearchFormFindKeyUpRequest : BaseRequest
{
    [JsonIgnore]
    public Component Form { get; set; }

    public int KeyData { get; set; }

    [JsonIgnore]
    public Component FindTextBox { get; set; }
}