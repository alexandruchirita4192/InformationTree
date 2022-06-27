using System.ComponentModel;
using Newtonsoft.Json;
using InformationTree.Domain.Entities;
using InformationTree.Domain.Requests.Base;

namespace InformationTree.Domain.Requests;

public class PgpEncryptDecryptDataForRicherTextBoxRequest : BaseRequest
{
    [JsonIgnore]
    public Component DataRicherTextBox { get; set; }

    [JsonIgnore]
    public Component EncryptionLabel { get; set; }

    [JsonIgnore]
    public Component FormToCenterTo { get; set; }

    public PgpActionType ActionType { get; set; }
    public bool FromFile { get; set; }
    public bool DataIsPgpEncrypted { get; set; }
}