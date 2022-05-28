using System;
using InformationTree.Domain.EventArguments;
using InformationTree.Domain.Responses;
using MediatR;

namespace InformationTree.Domain.Requests;

public class SetTreeStateRequest : IRequest<BaseResponse>
{
    public bool? IsSafeToSave { get; set; }
    public int? TreeNodeCounter { get; set; }
    public bool? TreeUnchanged { get; set; }
    public bool? TreeSaved { get; set; }
    public DateTime? TreeSavedAt { get; set; }
    public bool? ReadOnlyState { get; set; }
    public FileInformation FileInformation { get; set; }

    #region TreeUnchangedValueChanged

    private bool _treeUnchangedValueChangedEventAdded = false;

    private event EventHandler<ValueChangedEventArgs<bool>> _treeUnchangedValueChanged;

    public event EventHandler<ValueChangedEventArgs<bool>> TreeUnchangedValueChanged
    {
        add
        {
            _treeUnchangedValueChangedEventAdded = true;
            _treeUnchangedValueChanged += value;
        }
        remove
        {
            // Do not allow removing events because they are not used anyway
        }
    }

    public bool TreeUnchangedValueChangeEventAdded => _treeUnchangedValueChangedEventAdded;

    public void InvokeTreeUnchangedValueChangeEvent(object source, ValueChangedEventArgs<bool> value)
    {
        _treeUnchangedValueChanged?.Invoke(source, value);
    }

    #endregion TreeUnchangedValueChanged
}

/// <summary>
/// File information class is created to know when someone wants to set a <see cref="FileName"/> value because you could also set <see cref="FileName"/> null
/// (and null <see cref="FileInformation"/> means not setting the value)
/// </summary>
public class FileInformation
{
    public string FileName { get; set; }
}