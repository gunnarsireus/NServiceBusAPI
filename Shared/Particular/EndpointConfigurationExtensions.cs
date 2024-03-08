using NServiceBus;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Shared.Particular
{
    public static class EndpointCommonConfiguration
    {
        public static void ApplyEndpointConfiguration(
            this EndpointConfiguration endpointConfiguration,
            string connectionString,
            string endpointName,
            Action<RoutingSettings<SqlServerTransport>>? messageEndpointMappings = null)
        {
            var transport = new SqlServerTransport(connectionString)
            {
                //DefaultSchema = "dbo",
                TransportTransactionMode = TransportTransactionMode.SendsAtomicWithReceive,
                //Subscriptions =
                //{
                //    CacheInvalidationPeriod = TimeSpan.FromMinutes(1),
                //    SubscriptionTableName = new SubscriptionTableName(table: "Subscriptions", schema: "dbo")
                //}
            };

            // transport.SchemaAndCatalog.UseSchemaForQueue("error", "dbo");
            // transport.SchemaAndCatalog.UseSchemaForQueue("audit", "dbo");
            // transport.SchemaAndCatalog.UseSchemaForQueue(endpointName, "dbo");

            var routing = endpointConfiguration.UseTransport(transport);

            // SqlHelper.CreateSchema(connectionString, endpointConfiguration.).GetAwaiter().GetResult();

            endpointConfiguration.MakeInstanceUniquelyAddressable("1");
            // Message serialization
            endpointConfiguration.UseSerialization<SystemJsonSerializer>();
            endpointConfiguration.EnableCallbacks();

            // Installers are useful in development. Consider disabling in production.
            // https://docs.particular.net/nservicebus/operations/installers
            endpointConfiguration.EnableInstallers();

            endpointConfiguration.Conventions()
                .DefiningCommandsAs(t => t.Namespace != null && t.Namespace.EndsWith("Requests"))
                .DefiningEventsAs(t => t.Namespace != null && t.Namespace.EndsWith("Events"))
                .DefiningMessagesAs(t => t.Namespace != null && t.Namespace.EndsWith("Responses"));

            endpointConfiguration.AuditProcessedMessagesTo("audit");
            endpointConfiguration.SendFailedMessagesTo("error");
            messageEndpointMappings?.Invoke(routing);

            endpointConfiguration.DefineCriticalErrorAction(OnCriticalError);
        }

        static async Task OnCriticalError(ICriticalErrorContext context, CancellationToken cancellationToken)
        {
            // TODO: decide if stopping the endpoint and exiting the process is the best response to a critical error
            // https://docs.particular.net/nservicebus/hosting/critical-errors
            // and consider setting up service recovery
            // https://docs.particular.net/nservicebus/hosting/windows-service#installation-restart-recovery
            try
            {
                await context.Stop(cancellationToken);
            }
            finally
            {
                FailFast($"Critical error, shutting down: {context.Error}", context.Exception);
            }
        }

        static void FailFast(string message, Exception exception)
        {
            try
            {
                // TODO: decide what kind of last resort logging is necessary
                // TODO: when using an external logging framework it is important to flush any pending entries prior to calling FailFast
                // https://docs.particular.net/nservicebus/hosting/critical-errors#when-to-override-the-default-critical-error-action
            }
            finally
            {
                Environment.FailFast(message, exception);
            }
        }
    }
}