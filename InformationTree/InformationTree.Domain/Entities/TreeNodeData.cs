using System;
using System.Collections.Generic;

namespace InformationTree.Domain.Entities
{
    public class TreeNodeData : CompositeTree<TreeNodeData>
    {
        #region Properties

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

        #endregion Properties

        #region Constructors

        public TreeNodeData(string data = null, int addedNumber = 0, DateTime? addedDate = null, DateTime? lastChangeDate = null, int urgency = 0, string link = null, string category = null, bool isStartupAlert = false, decimal percentCompleted = 0m)
        {
            Data = data;
            AddedNumber = addedNumber;
            AddedDate = addedDate;
            Urgency = urgency;
            Link = link;
            Category = category;
            IsStartupAlert = isStartupAlert;
            PercentCompleted = percentCompleted;
        }

        public TreeNodeData(TreeNodeData copy) : this(copy.Data, copy.AddedNumber, copy.AddedDate, copy.LastChangeDate, copy.Urgency, copy.Link, copy.Category, copy.IsStartupAlert, copy.PercentCompleted)
        {
        }

        #endregion Constructors

        #region Methods

        public bool IsEmptyData
        {
            get
            {
                return string.IsNullOrEmpty(Data) &&
                    AddedNumber == 0 &&
                    AddedDate == null &&
                    Urgency == 0 &&
                    string.IsNullOrEmpty(Link) &&
                    string.IsNullOrEmpty(Category) &&
                    !IsStartupAlert &&
                    PercentCompleted == 0m;
            }
        }
        
        public void Copy(TreeNodeData from)
        {
            if (from == null)
                return;
            
            Data = from.Data;
            AddedNumber = from.AddedNumber;
            AddedDate = from.AddedDate;
            LastChangeDate = from.LastChangeDate;
            Urgency = from.Urgency;
            Link = from.Link;
            Category = from.Category;
            IsStartupAlert = from.IsStartupAlert;
            PercentCompleted = from.PercentCompleted;

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

        #endregion Methods
    }
}