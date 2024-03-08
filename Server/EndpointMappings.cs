using NServiceBus;
using System;

namespace Server
{
    public class EndpointMappings
    {
        internal static Action<RoutingSettings<SqlServerTransport>> MessageEndpointMappings()
        {
            return routing =>
            {
            };
        }
    }
}