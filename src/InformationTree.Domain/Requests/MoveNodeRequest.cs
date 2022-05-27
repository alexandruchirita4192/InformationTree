﻿using System.ComponentModel;
using InformationTree.Domain.Requests.Base;

namespace InformationTree.Domain.Requests
{
    /// <summary>
    /// Move node request requires only the <see cref="TreeView"/> because the last 2 selected nodes are saved in <see cref="ITreeNodeSelectionCachingService"/>
    /// </summary>
    public class MoveNodeRequest : BaseRequest
    {
        public Component TreeView { get; set; }
    }
}