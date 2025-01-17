﻿using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Thingsboard.Net.Flurl.Options;
using Thingsboard.Net.Flurl.Utilities;

namespace Thingsboard.Net.Flurl;

/// <summary> The factory for creating a <see cref="ITbClient{TClient}"/> instance. </summary>
public class FlurlTbClientFactory
{
    public FlurlTbClientFactory()
    {
    }

    public FlurlTbClientFactory(ThingsboardNetFlurlOptions options)
    {
        Options = options;
    }

    public FlurlTbClientFactory(ThingsboardNetFlurlOptions options, ILoggerFactory loggerFactory)
    {
        Options = options;
        LoggerFactory = loggerFactory;
    }

    /// <summary> Global factory instance. </summary>
    public static FlurlTbClientFactory Instance { get; } = new();

    /// <summary> The logger factory used to create a logger to logging. </summary>
    public ILoggerFactory? LoggerFactory { get; set; }

    /// <summary> The factory method options </summary>
    public ThingsboardNetFlurlOptions Options { get; set; } = new();

    public ITbAlarmClient CreateAlarmClient() => new FlurlTbAlarmClient(CreateRequestBuilder());

    public ITbAssetClient CreateAssetClient() => new FlurlTbAssetClient(CreateRequestBuilder());

    public ITbAssetProfileClient CreateAssetProfileClient() => new FlurlTbAssetProfileClient(CreateRequestBuilder());

    public ITbAuditLogClient CreateAuditLogClient() => new FlurlTbAuditLogClient(CreateRequestBuilder());

    public ITbAuthClient CreateAuthClient() => new FlurlTbAuthClient(CreateRequestBuilder());

    public ITbCustomerClient CreateCustomerClient() => new FlurlTbCustomerClient(CreateRequestBuilder());

    public ITbDashboardClient CreateDashboardClient() => new FlurlTbDashboardClient(CreateRequestBuilder());

    public ITbDeviceClient CreateDeviceClient() => new FlurlTbDeviceClient(CreateRequestBuilder());

    public ITbDeviceProfileClient CreateDeviceProfileClient() => new FlurlTbDeviceProfileClient(CreateRequestBuilder());

    public ITbEntityQueryClient CreateEntityQueryClient() => new FlurlTbEntityQueryClient(CreateRequestBuilder());

    public ITbEntityRelationClient CreateEntityRelationClient() => new FlurlTbEntityRelationClient(CreateRequestBuilder());

    public ITbLoginClient CreateLoginClient() => new FlurlTbLoginClient(CreateRequestBuilder());

    public ITbQueueClient CreateQueueClient() => new FlurlTbQueueClient(CreateRequestBuilder());

    public IRequestBuilder CreateRequestBuilder()
    {
        return new FlurlRequestBuilder(
            new DefaultOptionsReaderFactory(new ThingsboardNetFlurlOptionsReader(Options)),
            new InMemoryAccessTokenRepository(),
            LoggerFactory ?? NullLoggerFactory.Instance);
    }

    public ITbRpcClient CreateRpcClient() => new FlurlTbRpcClient(CreateRequestBuilder());

    public ITbTelemetryClient CreateTelemetryClient() => new FlurlTbTelemetryClient(CreateRequestBuilder());
}