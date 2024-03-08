using NServiceBus;
using Shared.Models;
using System;

namespace Shared.Requests
{
    [Serializable]
    public class UpdateCompanyRequest : IMessage
  {
        public UpdateCompanyRequest(Company company)
        {
            DataId = Guid.NewGuid();
            Company = company;
        }

        public Guid DataId { get; set; }
        public Company Company { get; set; }
    }
}
