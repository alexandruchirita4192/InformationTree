using System.Collections.Generic;

namespace InformationTree.Domain.Entities
{
    public abstract class CompositeTree<T>
    {
        public CompositeTree()
            : this(null)
        {
        }

        public CompositeTree(List<T> children)
        {
            Children = children ?? new List<T>();
        }

        public List<T> Children { get; private set; }
    }
}