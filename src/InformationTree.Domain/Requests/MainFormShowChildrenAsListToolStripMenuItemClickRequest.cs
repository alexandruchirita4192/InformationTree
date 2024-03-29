﻿using System;
using InformationTree.Domain.Requests.Base;
using Newtonsoft.Json;

namespace InformationTree.Domain.Requests
{
    public class MainFormShowChildrenAsListToolStripMenuItemClickRequest : BaseRequest
    {
        [JsonIgnore]
        public MarshalByRefObject SelectedNode { get; set; }
    }
}