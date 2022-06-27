using System;
using InformationTree.Domain.EventArguments;
using InformationTree.Domain.Extensions;

namespace InformationTree.Domain.Entities
{
    public class TreeState
    {
        public bool IsSafeToSave { get; set; }
        public int TreeNodeCounter { get; set; }
        public bool TreeSaved { get; set; }
        public DateTime TreeSavedAt { get; set; }
        public bool ReadOnlyState { get; set; }

        #region FileName

        private string fileName;

        public string FileName
        {
            get
            {
                return fileName.IsNotEmpty() ? fileName : Constants.DefaultFileName;
            }
            set
            {
                fileName = value;
            }
        }

        #endregion FileName

        #region TreeUnchanged

        private bool _treeUnchanged;

        public bool TreeUnchanged
        {
            get
            {
                return _treeUnchanged;
            }
            set
            {
                if (_treeUnchanged != value)
                {
                    TreeUnchangedValueChanged?.Invoke(this, new ValueChangedEventArgs<bool>(_treeUnchanged, value));
                    _treeUnchanged = value;
                }
            }
        }

        public event EventHandler<ValueChangedEventArgs<bool>> TreeUnchangedValueChanged;

        public void InvokeTreeUnchangedValueChangedEventHandler(object source, ValueChangedEventArgs<bool> e)
        {
            TreeUnchangedValueChanged?.Invoke(source, e);
        }

        #endregion TreeUnchanged
    }
}