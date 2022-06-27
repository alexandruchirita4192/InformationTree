using System;
using InformationTree.Domain.Entities.Graphics;
using InformationTree.Domain.Requests.Base;

namespace InformationTree.Domain.Requests
{
    public class LoadingFormLoadingGraphicsPaintRequest : BaseRequest
    {
        public MarshalByRefObject Graphics { get; set; }
        public IGraphicsFile GraphicsFile { get; set; }
    }
}