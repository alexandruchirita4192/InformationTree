using System.ComponentModel;
using InformationTree.Domain.Entities;
using InformationTree.Domain.Requests.Base;

namespace InformationTree.Domain.Requests;

public class ShowTreeFilteredByRangeRequest : BaseRequest
{
    public int Min { get; set; }
    public int Max { get; set; }
    public CopyNodeFilterType FilterType { get; set; }
    public Component TreeView { get; set; }
    public Component ShowAllButton { get; set; }
    public Component TaskGroupBox { get; set; }
    public Component StyleChangeGroupBox { get; set; }
    public Component TimeSpentGroupBox { get; set; }
}
