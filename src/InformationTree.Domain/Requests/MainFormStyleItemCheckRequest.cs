using System;
using System.ComponentModel;
using InformationTree.Domain.Requests.Base;
using Newtonsoft.Json;

namespace InformationTree.Domain.Requests
{
    public class MainFormStyleItemCheckRequest : BaseRequest
    {
        public EventArgs ItemCheckEventArgs { get; set; }

        public TaskListAfterSelectRequest AfterSelectRequest { get; set; }
    }
}