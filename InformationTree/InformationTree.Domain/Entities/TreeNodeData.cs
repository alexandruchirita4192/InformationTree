using System;
using InformationTree.Domain.Extensions;

namespace InformationTree.Domain.Entities
{
    public class TreeNodeData : CompositeTree<TreeNodeData>
    {
        #region Properties

        public string Text { get; set; }

        [Obsolete("This might be modeled as another object (RTF object capable of generating RTF, adding tables, images or other)")]
        public string Data { get; set; }
        public int AddedNumber { get; set; }
        public int Urgency { get; set; }
        public string Link { get; set; }
        public string Category { get; set; }
        public bool IsStartupAlert { get; set; }
        public decimal PercentCompleted { get; set; }

        public DateTime? AddedDate { get; set; }

        #region LastChangeDate

        private DateTime? lastChangeDate;

        public DateTime? LastChangeDate
        {
            get
            {
                return lastChangeDate ?? AddedDate;
            }
            set
            {
                lastChangeDate = value;
            }
        }

        #endregion LastChangeDate

        public TreeNodeFont NodeFont { get; set; }
        public string Name { get; set; }
        public string BackColorName { get; set; }
        public string ForeColorName { get; set; }
        public string ToolTipText { get; set; }

        public bool IsEmptyData
        {
            get
            {
                return Name.IsEmpty() &&
                    Data.IsEmpty() &&
                    ToolTipText.IsEmpty() &&
                    AddedNumber == 0 &&
                    AddedDate == null &&
                    Urgency == 0 &&
                    Link.IsEmpty() &&
                    Category.IsEmpty() &&
                    IsStartupAlert == false &&
                    PercentCompleted == 0m &&
                    (NodeFont?.IsEmpty ?? true) &&
                    BackColorName.IsEmpty() &&
                    ForeColorName.IsEmpty();
            }
        }

        #endregion Properties

        #region Constructors

        public TreeNodeData()
        {
            Text = string.Empty;
            Name = string.Empty;
            ToolTipText = string.Empty;
        }

        public TreeNodeData(TreeNodeData copy) : this()
        {
            Copy(copy);
        }

        #endregion Constructors

        #region Methods

        public void Copy(TreeNodeData from)
        {
            if (from == null)
                return;

            Name = from.Name;
            Text = from.Text;
            Data = from.Data;
            AddedNumber = from.AddedNumber;
            AddedDate = from.AddedDate;
            LastChangeDate = from.LastChangeDate;
            Urgency = from.Urgency;
            Link = from.Link;
            Category = from.Category;
            IsStartupAlert = from.IsStartupAlert;
            PercentCompleted = from.PercentCompleted;
            NodeFont = from.NodeFont?.Clone();
            BackColorName = from.BackColorName;
            ForeColorName = from.ForeColorName;

            Children.Clear();

            if (from.Children != null)
            {
                foreach (var child in from.Children)
                {
                    var newChild = new TreeNodeData();
                    newChild.Copy(child);
                    Children.Add(newChild);
                }
            }
        }

        public TreeNodeData Clone()
        {
            var clone = new TreeNodeData();
            clone.Copy(this);
            return clone;
        }

        #endregion Methods
    }
}