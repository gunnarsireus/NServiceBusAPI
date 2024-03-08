using NServiceBus;
using System;

namespace Shared.Responses
{
    [Serializable]
    public class DeleteCarResponse : IMessage
  {
        public DeleteCarResponse()
        {
            DataId = Guid.NewGuid();
        }

        public Guid DataId { get; set; }
    }
}