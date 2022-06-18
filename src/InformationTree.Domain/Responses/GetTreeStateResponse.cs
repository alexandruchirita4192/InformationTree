using System;
using InformationTree.Domain.EventArguments;

namespace InformationTree.Domain.Responses;

public class GetTreeStateResponse : BaseResponse
{
    public bool IsSafeToSave { get; set; }
    public int TreeNodeCounter { get; set; }
    public bool TreeSaved { get; set; }
    public DateTime TreeSavedAt { get; set; }
    public bool ReadOnlyState { get; set; }
    public string FileName { get; set; }

    #region TreeUnchanged

    public bool TreeUnchanged { get; set; }

    public event EventHandler<ValueChangedEventArgs<bool>> TreeUnchangedValueChanged;

    #endregion TreeUnchanged
}