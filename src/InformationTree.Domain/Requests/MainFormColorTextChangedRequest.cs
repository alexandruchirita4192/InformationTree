using System;
using System.ComponentModel;
using InformationTree.Domain.Entities;
using InformationTree.Domain.Requests.Base;
using Newtonsoft.Json;

namespace InformationTree.Domain.Requests
{
    public class MainFormColorTextChangedRequest : BaseRequest
    {
        [JsonIgnore]
        public ColorChangeType ChangeType { get; set; }

        [JsonIgnore]
        public MarshalByRefObject SelectedTreeNode { get; set; }

        [JsonIgnore]
        public Component ColorTextBox { get; set; }
    }
}