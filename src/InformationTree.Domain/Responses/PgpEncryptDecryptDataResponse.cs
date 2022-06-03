namespace InformationTree.Domain.Responses
{
    public class PgpEncryptDecryptDataResponse : BaseResponse
    {
        public string ResultText { get; set; }
        public string ResultRtf { get; set; }
        public string EncryptionInfo { get; set; }
    }
}