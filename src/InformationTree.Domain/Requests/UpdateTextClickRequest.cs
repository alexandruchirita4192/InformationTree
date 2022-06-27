using System.ComponentModel;
using Newtonsoft.Json;
using InformationTree.Domain.Entities;
using InformationTree.Domain.Requests.Base;

namespace InformationTree.Domain.Requests;

public class UpdateTextClickRequest : BaseRequest
{
    public TaskListAfterSelectRequest AfterSelectRequest { get; set; }
}