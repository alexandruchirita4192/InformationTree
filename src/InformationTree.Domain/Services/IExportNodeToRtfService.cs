using InformationTree.Domain.Entities;

namespace InformationTree.Domain.Services
{
    public interface IExportNodeToRtfService
    {
        string GetRtfExport(TreeNodeData node);
    }
}