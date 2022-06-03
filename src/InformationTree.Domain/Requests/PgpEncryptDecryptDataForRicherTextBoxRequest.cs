using System.ComponentModel;
using InformationTree.Domain.Entities;
using InformationTree.Domain.Requests.Base;

namespace InformationTree.Domain.Requests
{
    public class PgpEncryptDecryptDataForRicherTextBoxRequest : BaseRequest
    {
        public Component DataRicherTextBox { get; set; }
        public Component EncryptionLabel { get; set; }
        public Component FormToCenterTo { get; set; }
        public PgpActionType ActionType { get; set; }
        public bool FromFile { get; set; }
        public bool DataIsPgpEncrypted { get; set; }
    }
}