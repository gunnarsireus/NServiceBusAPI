using NServiceBus;
using System;

namespace Shared.Responses
{
    [Serializable]
    public class DeleteCompanyResponse : IMessage
  {
        public DeleteCompanyResponse()
        {
            DataId = Guid.NewGuid();
        }

        public Guid DataId { get; set; }
    }
}