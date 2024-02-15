﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Flurl.Http;
using Newtonsoft.Json;
using Thingsboard.Net.Exceptions;
using Thingsboard.Net.Flurl.Utilities;

namespace Thingsboard.Net.Flurl;

public class FlurlTbTelemetryClient : FlurlTbClient<ITbTelemetryClient>, ITbTelemetryClient
{
    /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
    public FlurlTbTelemetryClient(IRequestBuilder builder) : base(builder)
    {
    }

    /// <summary>
    /// Creates or updates the device attributes based on device id and specified attribute scope. The request payload is a JSON object with key-value format of attributes to create or update. For example:
    /// Available for users with 'TENANT_ADMIN' or 'CUSTOMER_USER' authority.
    /// </summary>
    /// <param name="deviceId">A string value representing the device id. For example, '784f394c-42b6-435a-983c-b7beff2784f9'</param>
    /// <param name="scope">A string value representing the attributes scope. For example, 'SERVER_SCOPE'.</param>
    /// <param name="value"> The request payload is a JSON object with key-value format of attributes to create or update.For example:
    /// {
    ///     "stringKey":"value1", 
    ///     "booleanKey":true, 
    ///     "doubleKey":42.0, 
    ///     "longKey":73, 
    ///     "jsonKey": {
    ///         "someNumber": 42,
    ///         "someArray": [1,2,3],
    ///         "someNestedObject": {"key": "value"}
    ///     }
    /// }
    /// </param>
    /// <param name="cancel"></param>
    /// <returns></returns>
    public Task SaveDeviceAttributesAsync(Guid deviceId, TbAttributeScope scope, object value, CancellationToken cancel = default)
    {
        if (value == null) throw new ArgumentNullException(nameof(value));


        var policy = RequestBuilder.GetPolicyBuilder()
            .RetryOnHttpTimeout()
            .RetryOnUnauthorized()
            .FallbackOn(HttpStatusCode.NotFound, () => throw new TbEntityNotFoundException(TbEntityType.DEVICE, deviceId))
            .Build();

        return policy.ExecuteAsync(async builder =>
        {
            await builder.CreateRequest()
                .AppendPathSegment($"api/plugins/telemetry/{deviceId}/{scope}")
                .WithOAuthBearerToken(await builder.GetAccessTokenAsync())
                .PostJsonAsync(value, cancel);
        });
    }

    /// <summary>
    /// Delete device attributes using provided Device Id, scope and a list of keys. Referencing a non-existing Device Id will cause an error
    /// Available for users with 'TENANT_ADMIN' or 'CUSTOMER_USER' authority.
    /// </summary>
    /// <param name="deviceId">A string value representing the device id. For example, '784f394c-42b6-435a-983c-b7beff2784f9'</param>
    /// <param name="scope">A string value representing the attributes scope. For example, 'SERVER_SCOPE'.</param>
    /// <param name="keys">A string value representing the comma-separated list of attributes keys. For example, 'active,inactivityAlarmTime'.</param>
    /// <param name="cancel"></param>
    /// <returns></returns>
    public Task DeleteDeviceAttributesAsync(Guid deviceId, TbAttributeScope scope, string[] keys, CancellationToken cancel = default)
    {
        if (keys == null) throw new ArgumentNullException(nameof(keys));
        if (keys.Length == 0) throw new ArgumentException("Value cannot be an empty collection.", nameof(keys));


        var policy = RequestBuilder.GetPolicyBuilder()
            .RetryOnHttpTimeout()
            .RetryOnUnauthorized()
            .FallbackOn(HttpStatusCode.NotFound, () => throw new TbEntityNotFoundException(TbEntityType.DEVICE, deviceId))
            .Build();

        return policy.ExecuteAsync(async builder =>
        {
            await builder.CreateRequest()
                .AppendPathSegment($"api/plugins/telemetry/{deviceId}/{scope}")
                .WithOAuthBearerToken(await builder.GetAccessTokenAsync())
                .SetQueryParam("keys", keys.JoinWith(","))
                .DeleteAsync(cancel);
        });
    }

    /// <summary>
    /// Creates or updates the entity attributes based on Entity Id and the specified attribute scope. List of possible attribute scopes depends on the entity type:
    ///     SERVER_SCOPE - supported for all entity types;
    ///     CLIENT_SCOPE - supported for devices;
    ///     SHARED_SCOPE - supported for devices.
    /// Referencing a non-existing entity Id or invalid entity type will cause an error.
    /// Available for users with 'TENANT_ADMIN' or 'CUSTOMER_USER' authority.
    /// </summary>
    /// <param name="entityType">A string value representing the entity type. For example, 'DEVICE'</param>
    /// <param name="entityId">A string value representing the entity id. For example, '784f394c-42b6-435a-983c-b7beff2784f9'</param>
    /// <param name="scope">A string value representing the attributes scope. For example, 'SERVER_SCOPE'.</param>
    /// <param name="value"> The request payload is a JSON object with key-value format of attributes to create or update.For example:
    /// {
    ///     "stringKey":"value1", 
    ///     "booleanKey":true, 
    ///     "doubleKey":42.0, 
    ///     "longKey":73, 
    ///     "jsonKey": {
    ///         "someNumber": 42,
    ///         "someArray": [1,2,3],
    ///         "someNestedObject": {"key": "value"}
    ///     }
    /// }
    /// </param>
    /// <param name="cancel"></param>
    /// <returns></returns>
    public Task SaveEntityAttributesAsync(TbEntityType entityType, Guid entityId, TbAttributeScope scope, object value, CancellationToken cancel = default)
    {
        if (value == null) throw new ArgumentNullException(nameof(value));


        var policy = RequestBuilder.GetPolicyBuilder()
            .RetryOnHttpTimeout()
            .RetryOnUnauthorized()
            .FallbackOn(HttpStatusCode.NotFound, () => throw new TbEntityNotFoundException(entityType, entityId))
            .Build();

        return policy.ExecuteAsync(async builder =>
        {
            await builder.CreateRequest()
                .AppendPathSegment($"api/plugins/telemetry/{entityType}/{entityId}/attributes/{scope}")
                .WithOAuthBearerToken(await builder.GetAccessTokenAsync())
                .PostJsonAsync(value, cancel);
        });
    }

    /// <summary>
    /// Delete entity attributes using provided Entity Id, scope and a list of keys. Referencing a non-existing entity Id or invalid entity type will cause an error.
    /// Available for users with 'TENANT_ADMIN' or 'CUSTOMER_USER' authority.
    /// </summary>
    /// <param name="entityType">A string value representing the entity type. For example, 'DEVICE'</param>
    /// <param name="entityId">A string value representing the entity id. For example, '784f394c-42b6-435a-983c-b7beff2784f9'</param>
    /// <param name="scope">A string value representing the attributes scope. For example, 'SERVER_SCOPE'.</param>
    /// <param name="keys">A string value representing the comma-separated list of attributes keys. For example, 'active,inactivityAlarmTime'.</param>
    /// <param name="cancel"></param>
    /// <returns></returns>
    public Task DeleteEntityAttributesAsync(TbEntityType entityType, Guid entityId, TbAttributeScope scope, string[] keys, CancellationToken cancel = default)
    {
        if (keys == null) throw new ArgumentNullException(nameof(keys));
        if (keys.Length == 0) throw new ArgumentException("Value cannot be an empty collection.", nameof(keys));


        var policy = RequestBuilder.GetPolicyBuilder()
            .RetryOnHttpTimeout()
            .RetryOnUnauthorized()
            .FallbackOn(HttpStatusCode.NotFound, () => throw new TbEntityNotFoundException(entityType, entityId))
            .Build();

        return policy.ExecuteAsync(async builder =>
        {
            await builder.CreateRequest()
                .AppendPathSegment($"api/plugins/telemetry/{entityType}/{entityId}/{scope}")
                .WithOAuthBearerToken(await builder.GetAccessTokenAsync())
                .SetQueryParam("keys", keys.JoinWith(","))
                .DeleteAsync(cancel);
        });
    }

    /// <summary>
    /// Returns a set of unique attribute key names for the selected entity and attributes scope:
    ///     SERVER_SCOPE - supported for all entity types;
    ///     CLIENT_SCOPE - supported for devices;
    ///     SHARED_SCOPE - supported for devices.
    /// Available for users with 'TENANT_ADMIN' or 'CUSTOMER_USER' authority.
    /// </summary>
    /// <param name="entityType">A string value representing the entity type. For example, 'DEVICE'</param>
    /// <param name="entityId">A string value representing the entity id. For example, '784f394c-42b6-435a-983c-b7beff2784f9'</param>
    /// <param name="cancel"></param>
    /// <returns></returns>
    public Task<string[]> GetAttributeKeysAsync(TbEntityType entityType, Guid entityId, CancellationToken cancel = default)
    {
        var policy = RequestBuilder.GetPolicyBuilder<string[]>()
            .RetryOnHttpTimeout()
            .RetryOnUnauthorized()
            .FallbackOn(HttpStatusCode.NotFound, () => throw new TbEntityNotFoundException(entityType, entityId))
            .Build();

        return policy.ExecuteAsync(async builder =>
        {
            return await builder.CreateRequest()
                .AppendPathSegment($"api/plugins/telemetry/{entityType}/{entityId}/keys/attributes")
                .WithOAuthBearerToken(await builder.GetAccessTokenAsync())
                .GetJsonAsync<string[]>(cancel);
        });
    }

    /// <summary>
    /// Returns a set of unique attribute key names for the selected entity and attributes scope:
    ///     SERVER_SCOPE - supported for all entity types;
    ///     CLIENT_SCOPE - supported for devices;
    ///     SHARED_SCOPE - supported for devices.
    /// Available for users with 'TENANT_ADMIN' or 'CUSTOMER_USER' authority.
    /// </summary>
    /// <param name="entityType">A string value representing the entity type. For example, 'DEVICE'</param>
    /// <param name="entityId">A string value representing the entity id. For example, '784f394c-42b6-435a-983c-b7beff2784f9'</param>
    /// <param name="scope">A string value representing the attributes scope. For example, 'SERVER_SCOPE'.</param>
    /// <param name="cancel"></param>
    /// <returns></returns>
    public Task<string[]> GetAttributeKeysByScopeAsync(TbEntityType entityType, Guid entityId, TbAttributeScope scope, CancellationToken cancel = default)
    {
        var policy = RequestBuilder.GetPolicyBuilder<string[]>()
            .RetryOnHttpTimeout()
            .RetryOnUnauthorized()
            .FallbackOn(HttpStatusCode.NotFound, () => throw new TbEntityNotFoundException(entityType, entityId))
            .Build();

        return policy.ExecuteAsync(async builder =>
        {
            return await builder.CreateRequest()
                .AppendPathSegment($"api/plugins/telemetry/{entityType}/{entityId}/keys/attributes/{scope}")
                .WithOAuthBearerToken(await builder.GetAccessTokenAsync())
                .GetJsonAsync<string[]>(cancel);
        });
    }

    /// <summary>
    /// Returns a set of unique time-series key names for the selected entity.
    /// Available for users with 'TENANT_ADMIN' or 'CUSTOMER_USER' authority.
    /// </summary>
    /// <param name="entityType">A string value representing the entity type. For example, 'DEVICE'</param>
    /// <param name="entityId">A string value representing the entity id. For example, '784f394c-42b6-435a-983c-b7beff2784f9'</param>
    /// <param name="cancel"></param>
    /// <returns></returns>
    public Task<string[]> GetTimeSeriesKeysAsync(TbEntityType entityType, Guid entityId, CancellationToken cancel = default)
    {
        var policy = RequestBuilder.GetPolicyBuilder<string[]>()
            .RetryOnHttpTimeout()
            .RetryOnUnauthorized()
            .FallbackOn(HttpStatusCode.NotFound, () => throw new TbEntityNotFoundException(entityType, entityId))
            .Build();

        return policy.ExecuteAsync(async builder =>
        {
            return await builder.CreateRequest()
                .AppendPathSegment($"api/plugins/telemetry/{entityType}/{entityId}/keys/timeseries")
                .WithOAuthBearerToken(await builder.GetAccessTokenAsync())
                .GetJsonAsync<string[]>(cancel);
        });
    }

    /// <summary>
    /// Creates or updates the entity time-series data based on the Entity Id and request payload.The request payload is a JSON document with three possible formats:
    /// {"temperature": 26}
    /// Single JSON object with timestamp:
    /// {"ts":1634712287000,"values":{"temperature":26, "humidity":87}}
    /// JSON array with timestamps:
    /// [ {"ts":1634712287000,"values":{"temperature":26, "humidity":87}}, {"ts":1634712588000,"values":{"temperature":25, "humidity":88}}]
    /// 
    /// Available for users with 'TENANT_ADMIN' or 'CUSTOMER_USER' authority.
    /// </summary>
    /// <param name="entityType">A string value representing the entity type. For example, 'DEVICE'</param>
    /// <param name="entityId">A string value representing the entity id. For example, '784f394c-42b6-435a-983c-b7beff2784f9</param>
    /// <param name="telemetry">Ref summary</param>
    /// <param name="cancel"></param>
    /// <returns></returns>
    public Task SaveEntityTimeSeriesAsync(TbEntityType entityType, Guid entityId, object telemetry, CancellationToken cancel = default)
    {
        if (telemetry == null) throw new ArgumentNullException(nameof(telemetry));

        // Use PascalCase for JSON to keep the name of the properties
        var telemetryJson = JsonConvert.SerializeObject(telemetry);

        var policy = RequestBuilder.GetPolicyBuilder()
            .RetryOnHttpTimeout()
            .RetryOnUnauthorized()
            .FallbackOn(HttpStatusCode.NotFound, () => throw new TbEntityNotFoundException(entityType, entityId))
            .Build();

        return policy.ExecuteAsync(async builder =>
        {
            await builder.CreateRequest()
                .AppendPathSegment($"api/plugins/telemetry/{entityType}/{entityId}/timeseries/ANY")
                .WithOAuthBearerToken(await builder.GetAccessTokenAsync())
                .PostStringAsync(telemetryJson, cancel);
        });
    }

    /// <summary>
    /// Creates or updates the entity time-series data based on the Entity Id and request payload.The request payload is a JSON document with three possible formats:
    /// {"temperature": 26}
    /// Single JSON object with timestamp:
    /// {"ts":1634712287000,"values":{"temperature":26, "humidity":87}}
    /// JSON array with timestamps:
    /// [ {"ts":1634712287000,"values":{"temperature":26, "humidity":87}}, {"ts":1634712588000,"values":{"temperature":25, "humidity":88}}]
    /// 
    /// Available for users with 'TENANT_ADMIN' or 'CUSTOMER_USER' authority.
    /// </summary>
    /// <param name="entityType">A string value representing the entity type. For example, 'DEVICE'</param>
    /// <param name="entityId">A string value representing the entity id. For example, '784f394c-42b6-435a-983c-b7beff2784f9</param>
    /// <param name="ttl">A long value representing TTL (Time to Live) parameter.</param>
    /// <param name="telemetry">Ref summary</param>
    /// <param name="cancel"></param>
    /// <returns></returns>
    public Task SaveEntityTimeSeriesWithTtlAsync(TbEntityType entityType, Guid entityId, long ttl, object telemetry, CancellationToken cancel = default)
    {
        if (telemetry == null) throw new ArgumentNullException(nameof(telemetry));

        // Use PascalCase for JSON to keep the name of the properties
        var telemetryJson = JsonConvert.SerializeObject(telemetry);

        var policy = RequestBuilder.GetPolicyBuilder()
            .RetryOnHttpTimeout()
            .RetryOnUnauthorized()
            .FallbackOn(HttpStatusCode.NotFound, () => throw new TbEntityNotFoundException(entityType, entityId))
            .Build();

        return policy.ExecuteAsync(async builder =>
        {
            await builder.CreateRequest()
                .AppendPathSegment($"api/plugins/telemetry/{entityType}/{entityId}/timeseries/ANY/{ttl}")
                .WithOAuthBearerToken(await builder.GetAccessTokenAsync())
                .PostStringAsync(telemetryJson, cancel);
        });
    }

    /// <summary>
    /// Delete ALL time-series for selected entity based on entity id, entity type and keys. Use 'rewriteLatestIfDeleted' to rewrite latest value (stored in separate table for performance) after deletion of the time range.
    /// Available for users with 'TENANT_ADMIN' or 'CUSTOMER_USER' authority.
    /// </summary>
    /// <param name="entityType">A string value representing the entity type. For example, 'DEVICE'</param>
    /// <param name="entityId">A string value representing the entity id. For example, '784f394c-42b6-435a-983c-b7beff2784f9'</param>
    /// <param name="keys">A string list of telemetry keys. If keys are not selected, the result will return all latest timeseries. For example, 'temperature,humidity'.</param>
    /// <param name="rewriteLatestIfDeleted">If the parameter is set to true, the latest telemetry will be rewritten in case that current latest value was removed, otherwise, in case that parameter is set to false the new latest value will not set.</param>
    /// <param name="deleteLatest">If the parameter is set to true, the latest telemetry can be removed, otherwise, in case that parameter is set to false the latest value will not removed.</param>
    /// <param name="cancel"></param>
    /// <returns></returns>
    public Task DeleteEntityTimeSeriesAsync(TbEntityType entityType, Guid entityId, string[] keys, bool? rewriteLatestIfDeleted = null, CancellationToken cancel = default)
    {
        return DeleteEntityTimeSeriesCoreAsync(entityType, entityId, keys, true, null, null, rewriteLatestIfDeleted, null, cancel);
    }

    /// <summary>
    /// Delete time-series for selected entity based on entity id, entity type and keys. Use 'startTs' and 'endTs' to specify time-range instead. Use 'rewriteLatestIfDeleted' to rewrite latest value (stored in separate table for performance) after deletion of the time range.
    /// Available for users with 'TENANT_ADMIN' or 'CUSTOMER_USER' authority.
    /// </summary>
    /// <param name="entityType">A string value representing the entity type. For example, 'DEVICE'</param>
    /// <param name="entityId">A string value representing the entity id. For example, '784f394c-42b6-435a-983c-b7beff2784f9'</param>
    /// <param name="keys">A string list of telemetry keys. If keys are not selected, the result will return all latest timeseries. For example, 'temperature,humidity'.</param>
    /// <param name="startTs">A datetime value representing the start timestamp of removal time range in milliseconds.</param>
    /// <param name="endTs">A datetime value representing the end timestamp of removal time range in milliseconds.</param>
    /// <param name="rewriteLatestIfDeleted">If the parameter is set to true, the latest telemetry will be rewritten in case that current latest value was removed, otherwise, in case that parameter is set to false the new latest value will not set.</param>
    /// <param name="cancel"></param>
    /// <returns></returns>
    public Task DeleteEntityTimeSeriesAsync(
        TbEntityType entityType,
        Guid entityId,
        string[] keys,
        DateTime startTs,
        DateTime endTs,
        bool? rewriteLatestIfDeleted = null,
        bool? deleteLatest = null,
        CancellationToken cancel = default)
    {
        return DeleteEntityTimeSeriesCoreAsync(entityType, entityId, keys, false, startTs, endTs, rewriteLatestIfDeleted, deleteLatest, cancel);
    }

    /// <summary>
    /// Delete time-series for selected entity based on entity id, entity type and keys. Use 'deleteAllDataForKeys' to delete all time-series data. Use 'startTs' and 'endTs' to specify time-range instead. Use 'rewriteLatestIfDeleted' to rewrite latest value (stored in separate table for performance) after deletion of the time range.
    /// Available for users with 'TENANT_ADMIN' or 'CUSTOMER_USER' authority.
    /// </summary>
    /// <param name="entityType">A string value representing the entity type. For example, 'DEVICE'</param>
    /// <param name="entityId">A string value representing the entity id. For example, '784f394c-42b6-435a-983c-b7beff2784f9'</param>
    /// <param name="keys">A string list of telemetry keys. If keys are not selected, the result will return all latest timeseries. For example, 'temperature,humidity'.</param>
    /// <param name="deleteAllDataForKeys">A boolean value to specify if should be deleted all data for selected keys or only data that are in the selected time range.</param>
    /// <param name="startTs">A datetime value representing the start timestamp of removal time range in milliseconds.</param>
    /// <param name="endTs">A datetime value representing the end timestamp of removal time range in milliseconds.</param>
    /// <param name="rewriteLatestIfDeleted">If the parameter is set to true, the latest telemetry will be rewritten in case that current latest value was removed, otherwise, in case that parameter is set to false the new latest value will not set.</param>
    /// <param name="cancel"></param>
    /// <returns></returns>
    private Task DeleteEntityTimeSeriesCoreAsync(
        TbEntityType entityType,
        Guid entityId,
        string[] keys,
        bool? deleteAllDataForKeys = null,
        DateTime? startTs = null,
        DateTime? endTs = null,
        bool? rewriteLatestIfDeleted = null,
        bool? deleteLatest = null,
        CancellationToken cancel = default)
    {
        if (keys == null) throw new ArgumentNullException(nameof(keys));
        if (keys.Length == 0) throw new ArgumentException("Value cannot be an empty collection.", nameof(keys));


        var policy = RequestBuilder.GetPolicyBuilder()
            .RetryOnHttpTimeout()
            .RetryOnUnauthorized()
            .FallbackOn(HttpStatusCode.NotFound, () => throw new TbEntityNotFoundException(entityType, entityId))
            .Build();

        return policy.ExecuteAsync(async builder =>
        {
            await builder.CreateRequest()
                .AppendPathSegment($"api/plugins/telemetry/{entityType}/{entityId}/timeseries/delete")
                .WithOAuthBearerToken(await builder.GetAccessTokenAsync())
                .SetQueryParam("keys", keys.JoinWith(","))
                .SetQueryParam("deleteAllDataForKeys", deleteAllDataForKeys?.ToString().ToLower())
                .SetQueryParam("startTs", startTs?.ToJavaScriptTicks())
                .SetQueryParam("endTs", endTs?.ToJavaScriptTicks())
                .SetQueryParam("rewriteLatestIfDeleted", rewriteLatestIfDeleted?.ToString().ToLower())
                .SetQueryParam("deleteLatest", deleteLatest?.ToString().ToLower())
                .DeleteAsync(cancel);
        });
    }

    /// <summary>
    /// Returns all attributes that belong to specified entity. Use optional 'keys' parameter to return specific attributes. Example of the result:
    /// Available for users with 'TENANT_ADMIN' or 'CUSTOMER_USER' authority.
    /// </summary>
    /// <param name="entityType">A string value representing the entity type. For example, 'DEVICE'</param>
    /// <param name="entityId">A string value representing the entity id. For example, '784f394c-42b6-435a-983c-b7beff2784f9</param>
    /// <param name="keys">A string list of attributes keys. For example, 'active,inactivityAlarmTime'</param>
    /// <param name="cancel"></param>
    /// <returns></returns>
    public Task<TbEntityLatestValue[]> GetAttributesAsync(TbEntityType entityType, Guid entityId, string[] keys, CancellationToken cancel = default)
    {
        if (keys == null) throw new ArgumentNullException(nameof(keys));
        if (keys.Length == 0) throw new ArgumentException("Value cannot be an empty collection.", nameof(keys));

        var policy = RequestBuilder.GetPolicyBuilder<TbEntityLatestValue[]>()
            .RetryOnHttpTimeout()
            .RetryOnUnauthorized()
            .FallbackOn(HttpStatusCode.NotFound, () => throw new TbEntityNotFoundException(entityType, entityId))
            .Build();

        return policy.ExecuteAsync(async builder =>
        {
            return await builder.CreateRequest()
                .AppendPathSegment($"api/plugins/telemetry/{entityType}/{entityId}/values/attributes")
                .WithOAuthBearerToken(await builder.GetAccessTokenAsync())
                .SetQueryParam("keys", keys.JoinWith(","))
                .GetJsonAsync<TbEntityLatestValue[]>(cancel);
        });
    }

    /// <summary>
    /// Returns all attributes that belong to specified entity. Use optional 'keys' parameter to return specific attributes. Example of the result:
    /// Available for users with 'TENANT_ADMIN' or 'CUSTOMER_USER' authority.
    /// </summary>
    /// <param name="entityType">A string value representing the entity type. For example, 'DEVICE'</param>
    /// <param name="entityId">A string value representing the entity id. For example, '784f394c-42b6-435a-983c-b7beff2784f9</param>
    /// <param name="scope">A enum value representing the attributes scope. For example, 'SERVER_SCOPE'.</param>
    /// <param name="keys">A string list of attributes keys. For example, 'active,inactivityAlarmTime'</param>
    /// <param name="cancel"></param>
    /// <returns></returns>
    public Task<TbEntityLatestValue[]> GetAttributesByScopeAsync(TbEntityType entityType, Guid entityId, TbAttributeScope scope, string[] keys, CancellationToken cancel = default)
    {
        if (keys == null) throw new ArgumentNullException(nameof(keys));
        if (keys.Length == 0) throw new ArgumentException("Value cannot be an empty collection.", nameof(keys));


        var policy = RequestBuilder.GetPolicyBuilder<TbEntityLatestValue[]>()
            .RetryOnHttpTimeout()
            .RetryOnUnauthorized()
            .FallbackOn(HttpStatusCode.NotFound, () => throw new TbEntityNotFoundException(entityType, entityId))
            .Build();

        return policy.ExecuteAsync(async builder =>
        {
            return await builder.CreateRequest()
                .AppendPathSegment($"api/plugins/telemetry/{entityType}/{entityId}/values/attributes/{scope}")
                .WithOAuthBearerToken(await builder.GetAccessTokenAsync())
                .SetQueryParam("keys", keys.JoinWith(","))
                .GetJsonAsync<TbEntityLatestValue[]>(cancel);
        });
    }

    /// <summary>
    /// Returns a range of time-series values for specified entity. Use aggregation function ('agg') and aggregation interval ('interval') to enable aggregation of the results on the database / server side. The aggregation is generally more efficient then fetching all records.
    /// Available for users with 'TENANT_ADMIN' or 'CUSTOMER_USER' authority.
    /// </summary>
    /// <param name="entityType">A string value representing the entity type. For example, 'DEVICE'</param>
    /// <param name="entityId">A string value representing the entity id. For example, '784f394c-42b6-435a-983c-b7beff2784f9'</param>
    /// <param name="keys">A string list of telemetry keys.</param>
    /// <param name="startTs">A datetime value representing the start timestamp of the time range in milliseconds, UTC.</param>
    /// <param name="endTs">A datetime value representing the end timestamp of the time range in milliseconds, UTC.</param>
    /// <param name="agg">A enum value representing the aggregation function. If the interval is not specified, 'agg' parameter will use 'NONE' value.</param>
    /// <param name="interval">A long value representing the aggregation interval range in milliseconds.</param>
    /// <param name="orderBy">Sort order. ASC (ASCENDING) or DESC (DESCENDING)</param>
    /// <param name="useStrictDataTypes">Enables/disables conversion of telemetry values to strings. Conversion is enabled by default. Set parameter to 'true' in order to disable the conversion.</param>
    /// <param name="cancel"></param>
    /// <returns></returns>
    public Task<ReadOnlyDictionary<TbEntityField, TbEntityTsValue[]>> GetTimeSeriesAsync(
        TbEntityType entityType,
        Guid entityId,
        string[] keys,
        DateTime startTs,
        DateTime endTs,
        TbTimeSeriesAggregate agg,
        long interval,
        TbSortOrder? orderBy = null,
        bool? useStrictDataTypes = null,
        CancellationToken cancel = default)
    {
        if (agg == TbTimeSeriesAggregate.NONE)
            throw new ArgumentException("Aggregation function can not be NONE if interval is specified.", nameof(agg));

        return GetTimeSeriesCoreAsync(entityType, entityId, keys, startTs, endTs, interval, null, agg, orderBy, useStrictDataTypes, cancel);
    }

    /// <summary>
    /// Returns a range of time-series values for specified entity. Returns not aggregated data by default.
    /// Available for users with 'TENANT_ADMIN' or 'CUSTOMER_USER' authority.
    /// </summary>
    /// <param name="entityType">A string value representing the entity type. For example, 'DEVICE'</param>
    /// <param name="entityId">A string value representing the entity id. For example, '784f394c-42b6-435a-983c-b7beff2784f9'</param>
    /// <param name="keys">A string list of telemetry keys.</param>
    /// <param name="startTs">A datetime value representing the start timestamp of the time range in milliseconds, UTC.</param>
    /// <param name="endTs">A datetime value representing the end timestamp of the time range in milliseconds, UTC.</param>
    /// <param name="limit">An integer value that represents a max number of timeseries data points to fetch. This parameter is used only in the case if 'agg' parameter is set to 'NONE'.</param>
    /// <param name="orderBy">Sort order. ASC (ASCENDING) or DESC (DESCENDING)</param>
    /// <param name="useStrictDataTypes">Enables/disables conversion of telemetry values to strings. Conversion is enabled by default. Set parameter to 'true' in order to disable the conversion.</param>
    /// <param name="cancel"></param>
    /// <returns></returns>
    public Task<ReadOnlyDictionary<TbEntityField, TbEntityTsValue[]>> GetTimeSeriesAsync(
        TbEntityType entityType,
        Guid entityId,
        string[] keys,
        DateTime startTs,
        DateTime endTs,
        int? limit = null,
        TbSortOrder? orderBy = null,
        bool? useStrictDataTypes = null,
        CancellationToken cancel = default)
    {
        return GetTimeSeriesCoreAsync(entityType, entityId, keys, startTs, endTs, null, limit, TbTimeSeriesAggregate.NONE, orderBy, useStrictDataTypes, cancel);
    }

    /// <summary>
    /// Returns a range of time-series values for specified entity. Returns not aggregated data by default. Use aggregation function ('agg') and aggregation interval ('interval') to enable aggregation of the results on the database / server side. The aggregation is generally more efficient then fetching all records.
    /// Available for users with 'TENANT_ADMIN' or 'CUSTOMER_USER' authority.
    /// </summary>
    /// <param name="entityType">A string value representing the entity type. For example, 'DEVICE'</param>
    /// <param name="entityId">A string value representing the entity id. For example, '784f394c-42b6-435a-983c-b7beff2784f9'</param>
    /// <param name="keys">A string list of telemetry keys.</param>
    /// <param name="startTs">A datetime value representing the start timestamp of the time range in milliseconds, UTC.</param>
    /// <param name="endTs">A datetime value representing the end timestamp of the time range in milliseconds, UTC.</param>
    /// <param name="interval">A long value representing the aggregation interval range in milliseconds.</param>
    /// <param name="limit">An integer value that represents a max number of timeseries data points to fetch. This parameter is used only in the case if 'agg' parameter is set to 'NONE'.</param>
    /// <param name="agg">A enum value representing the aggregation function. If the interval is not specified, 'agg' parameter will use 'NONE' value.</param>
    /// <param name="orderBy">Sort order. ASC (ASCENDING) or DESC (DESCENDING)</param>
    /// <param name="useStrictDataTypes">Enables/disables conversion of telemetry values to strings. Conversion is enabled by default. Set parameter to 'true' in order to disable the conversion.</param>
    /// <param name="cancel"></param>
    /// <returns></returns>
    public Task<ReadOnlyDictionary<TbEntityField, TbEntityTsValue[]>> GetTimeSeriesCoreAsync(
        TbEntityType entityType,
        Guid entityId,
        string[] keys,
        DateTime startTs,
        DateTime endTs,
        long? interval = null,
        int? limit = null,
        TbTimeSeriesAggregate? agg = null,
        TbSortOrder? orderBy = null,
        bool? useStrictDataTypes = null,
        CancellationToken cancel = default)
    {
        if (keys == null) throw new ArgumentNullException(nameof(keys));
        if (keys.Length == 0) throw new ArgumentException("Value cannot be an empty collection.", nameof(keys));


        var policy = RequestBuilder.GetPolicyBuilder<ReadOnlyDictionary<TbEntityField, TbEntityTsValue[]>>()
            .RetryOnHttpTimeout()
            .RetryOnUnauthorized()
            .FallbackOn(HttpStatusCode.NotFound, () => throw new TbEntityNotFoundException(entityType, entityId))
            .Build();

        return policy.ExecuteAsync(async builder =>
        {
            var request = builder.CreateRequest()
                .AppendPathSegment($"api/plugins/telemetry/{entityType}/{entityId}/values/timeseries")
                .WithOAuthBearerToken(await builder.GetAccessTokenAsync())
                .SetQueryParam("keys", keys.JoinWith(","))
                .SetQueryParam("startTs", startTs.ToJavaScriptTicks())
                .SetQueryParam("endTs", endTs.ToJavaScriptTicks())
                .SetQueryParam("interval", interval)
                .SetQueryParam("limit", limit)
                .SetQueryParam("agg", agg)
                .SetQueryParam("orderBy", orderBy)
                .SetQueryParam("useStrictDataTypes", useStrictDataTypes);

            var result = await request.GetJsonAsync<Dictionary<string, TbEntityTsValue[]>>(cancel);
            var converted = result.ToDictionary(x => new TbEntityField(x.Key, TbEntityFieldType.TIME_SERIES), x => x.Value);

            return new ReadOnlyDictionary<TbEntityField, TbEntityTsValue[]>(converted);
        });
    }

    /// <summary>
    /// Returns all time-series that belong to specified entity. Use optional 'keys' parameter to return specific time-series. The result is a JSON object. The format of the values depends on the 'useStrictDataTypes' parameter. By default, all time-series values are converted to strings:
    /// </summary>
    /// <param name="entityType">A string value representing the entity type. For example, 'DEVICE'</param>
    /// <param name="entityId">A string value representing the entity id. For example, '784f394c-42b6-435a-983c-b7beff2784f9'</param>
    /// <param name="keys">A string list of telemetry keys. If keys are not selected, the result will return all latest timeseries.</param>
    /// <param name="useStrictDataTypes">Enables/disables conversion of telemetry values to strings. Conversion is enabled by default. Set parameter to 'true' in order to disable the conversion.</param>
    /// <param name="cancel"></param>
    /// <returns></returns>
    public Task<ReadOnlyDictionary<TbEntityField, TbEntityTsValue>> GetLatestTimeSeriesAsync(
        TbEntityType entityType,
        Guid entityId,
        string[]? keys,
        bool? useStrictDataTypes = null,
        CancellationToken cancel = default)
    {
        var policy = RequestBuilder.GetPolicyBuilder<ReadOnlyDictionary<TbEntityField, TbEntityTsValue>>()
            .RetryOnHttpTimeout()
            .RetryOnUnauthorized()
            .FallbackOn(HttpStatusCode.NotFound, () => throw new TbEntityNotFoundException(entityType, entityId))
            .Build();

        return policy.ExecuteAsync(async builder =>
        {
            var request = builder.CreateRequest()
                .AppendPathSegment($"api/plugins/telemetry/{entityType}/{entityId}/values/timeseries")
                .WithOAuthBearerToken(await builder.GetAccessTokenAsync())
                .SetQueryParam("keys", keys?.JoinWith(","))
                .SetQueryParam("useStrictDataTypes", useStrictDataTypes);

            var result = await request.GetJsonAsync<Dictionary<string, TbEntityTsValue[]>>(cancel);
            var converted = result.ToDictionary(x => new TbEntityField(x.Key, TbEntityFieldType.TIME_SERIES), x => x.Value.FirstOrDefault());

            return new ReadOnlyDictionary<TbEntityField, TbEntityTsValue>(converted);
        });
    }
}
