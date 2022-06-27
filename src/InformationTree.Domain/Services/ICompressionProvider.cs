namespace InformationTree.Domain.Services
{
    public interface ICompressionProvider
    {
        string Compress(string data);

        string Decompress(string data);
    }
}