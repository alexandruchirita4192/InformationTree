﻿using System.ComponentModel;
using InformationTree.Domain.Entities;
using InformationTree.Domain.Requests.Base;

namespace InformationTree.Domain.Requests
{
    public class PgpEncryptDecryptDataRequest : BaseRequest
    {
        public string InputDataText { get; set; }
        public string InputDataRtf { get; set; }
        public Component FormToCenterTo { get; set; }
        public PgpActionType ActionType { get; set; }
        public bool FromFile { get; set; }
        public bool DataIsPgpEncrypted { get; set; }
    }
}