using NServiceBus;
using Shared.Models;
using System;

namespace Shared.Responses
{
    [Serializable]
    public class GetCompanyResponse : IMessage
  {
        public GetCompanyResponse()
        {
            DataId = Guid.NewGuid();
        }

        public Guid DataId { get; set; }
        public Company Company { get; set; }
    }
}