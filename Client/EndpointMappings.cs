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
        routing.RouteToEndpoint(typeof(GetCarsRequest), "NServiceBusCore.Server");
        routing.RouteToEndpoint(typeof(UpdateCarRequest), "NServiceBusCore.Server");
        routing.RouteToEndpoint(typeof(GetCompaniesRequest), "NServiceBusCore.Server");
        routing.RouteToEndpoint(typeof(GetCompanyRequest), "NServiceBusCore.Server");
        routing.RouteToEndpoint(typeof(CreateCarRequest), "NServiceBusCore.Server");
        routing.RouteToEndpoint(typeof(GetCarRequest), "NServiceBusCore.Server");
        routing.RouteToEndpoint(typeof(DeleteCarRequest), "NServiceBusCore.Server");
        routing.RouteToEndpoint(typeof(CreateCompanyRequest), "NServiceBusCore.Server");
        routing.RouteToEndpoint(typeof(DeleteCompanyRequest), "NServiceBusCore.Server");
        routing.RouteToEndpoint(typeof(UpdateCompanyRequest), "NServiceBusCore.Server");
        routing.RouteToEndpoint(typeof(GetCarsResponse), "NServiceBusCore.Server");
        routing.RouteToEndpoint(typeof(UpdateCarResponse), "NServiceBusCore.Server");
        routing.RouteToEndpoint(typeof(GetCompaniesResponse), "NServiceBusCore.Server");
        routing.RouteToEndpoint(typeof(GetCompanyResponse), "NServiceBusCore.Server");
        routing.RouteToEndpoint(typeof(CreateCarResponse), "NServiceBusCore.Server");
        routing.RouteToEndpoint(typeof(GetCarResponse), "NServiceBusCore.Server");
        routing.RouteToEndpoint(typeof(DeleteCarResponse), "NServiceBusCore.Server");
        routing.RouteToEndpoint(typeof(CreateCompanyResponse), "NServiceBusCore.Server");
        routing.RouteToEndpoint(typeof(DeleteCompanyResponse), "NServiceBusCore.Server");
        routing.RouteToEndpoint(typeof(UpdateCompanyResponse), "NServiceBusCore.Server");
      };
    }
  }
}