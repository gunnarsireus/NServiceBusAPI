using NServiceBus;
using Shared.Requests;
using Shared.Responses;
using System;

namespace Client
{
  public class EndpointMappings
  {
    internal static Action<RoutingSettings<SqlServerTransport>> MessageEndpointMappings()
    {
      return routing =>
      {
        routing.RouteToEndpoint(typeof(GetCarsRequest), "NServiceBus.Server");
        routing.RouteToEndpoint(typeof(UpdateCarRequest), "NServiceBus.Server");
        routing.RouteToEndpoint(typeof(GetCompaniesRequest), "NServiceBus.Server");
        routing.RouteToEndpoint(typeof(GetCompanyRequest), "NServiceBus.Server");
        routing.RouteToEndpoint(typeof(CreateCarRequest), "NServiceBus.Server");
        routing.RouteToEndpoint(typeof(GetCarRequest), "NServiceBus.Server");
        routing.RouteToEndpoint(typeof(DeleteCarRequest), "NServiceBus.Server");
        routing.RouteToEndpoint(typeof(CreateCompanyRequest), "NServiceBus.Server");
        routing.RouteToEndpoint(typeof(DeleteCompanyRequest), "NServiceBus.Server");
        routing.RouteToEndpoint(typeof(UpdateCompanyRequest), "NServiceBus.Server");
        routing.RouteToEndpoint(typeof(GetCarsResponse), "NServiceBus.Server");
        routing.RouteToEndpoint(typeof(UpdateCarResponse), "NServiceBus.Server");
        routing.RouteToEndpoint(typeof(GetCompaniesResponse), "NServiceBus.Server");
        routing.RouteToEndpoint(typeof(GetCompanyResponse), "NServiceBus.Server");
        routing.RouteToEndpoint(typeof(CreateCarResponse), "NServiceBus.Server");
        routing.RouteToEndpoint(typeof(GetCarResponse), "NServiceBus.Server");
        routing.RouteToEndpoint(typeof(DeleteCarResponse), "NServiceBus.Server");
        routing.RouteToEndpoint(typeof(CreateCompanyResponse), "NServiceBus.Server");
        routing.RouteToEndpoint(typeof(DeleteCompanyResponse), "NServiceBus.Server");
        routing.RouteToEndpoint(typeof(UpdateCompanyResponse), "NServiceBus.Server");
      };
    }
  }
}