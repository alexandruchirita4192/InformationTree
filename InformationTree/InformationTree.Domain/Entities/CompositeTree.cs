using System.Collections.Generic;

namespace InformationTree.Domain.Entities
{
    public abstract class CompositeTree<T>
    {
        public CompositeTree()
            : this(null)
        {
        }

        public CompositeTree(List<CompositeTree<T>> children)
        {
            Children = children ?? new List<CompositeTree<T>>();
        }

        public List<CompositeTree<T>> Children { get; private set; }
    }
}