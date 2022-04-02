using InformationTree.Domain.Entities;

namespace InformationTree.Domain.Services
{
    public interface ITreeRenderer
    {
        void ShowTree(TreeNodeData rootNode);
    }
}